using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nakama;

public class TBSMultiplayerManager : NetworkSessionManager {

	public bool useMultiplayerSimulation;

	const long OPCODE_CHAT = 100L;
	// const long OPCODE_HOST_GAME_STATE = 101L;
	const long OPCODE_MOVE = 102L;

	public delegate void OnPlayerMoveMade (TBSBaseGameMove move);
	public OnPlayerMoveMade onPlayerMoveMade;
	public CellGrid cellGrid;

	public delegate void OnChatMessageReceived (string from, string message);
	public OnChatMessageReceived onChatMessageReceived;

	private Queue<IEnumerator> _executionQueue;

	public TBSMultiplayerManager() {
		_executionQueue = new Queue<IEnumerator>(1024);
	}

	// Use this for initialization
	void Start () {

		onMatchJoined += OnMatchJoined;
		cellGrid.UnitMoved += OnUnitMoved;
	}

	void Update() {
		base.Update ();
		lock (_executionQueue) {
			for (int i = 0, len = _executionQueue.Count; i < len; i++) {
				StartCoroutine(_executionQueue.Dequeue());
			}
		}
	}

	public void EnterSimulationMode() {
		// TODO: build Automated Participants to match the number of desired players
		// Init host logic

	}

	public void OnMatchJoined(INMatch match) {
		// Establish a host guest relationship
		int i = 0;
		foreach (var participant in _matchParticipants) {
			if (participant.UserId == _session.Id && i == 0) {
				// isHost = true;
				// How to assign factions
				// My player index

			}
			i++;
		}

//		if (isHost) {
//			// Establish game state
//			gameState = new GameState();
//			gameState = gameRule.AssignParticipants (gameState, _matchParticipants);
//			SendGameState ();
//		}
	}

	public override void OnMatchData(INMatchData m) {
		base.OnMatchData (m);
		var content = Encoding.UTF8.GetString(m.Data);
		content = content.Trim ();
		// content = content.Substring (1, content.Length - 2);
		content = content.Replace (@"\", "");

		switch (m.OpCode) {
		case OPCODE_CHAT:
			Enqueue (() => {
				onChatMessageReceived (m.Presence.Handle, content);
			});
			break;
		case OPCODE_MOVE:
			TBSBaseGameMove move = JsonUtility.FromJson<TBSBaseGameMove> (content);
			Enqueue (() => {
				OnPlayerMoveReceived(move, content);
				// onPlayerMoveMade(move);
			});

			break;
		default:
			Debug.LogFormat ("User handle '{0}' sent '{1}' '21}'", m.Presence.Handle, m.OpCode, content);
			break;
		};
	}

	private void OnPlayerMoveReceived(TBSBaseGameMove move, string rawMessage) {
		if (move.type == "unitMove") {

			// TODO: have to know whom is this from.

			TBSUnitGameMove m = JsonUtility.FromJson<TBSUnitGameMove> (rawMessage);
			cellGrid.MoveUnit (m.fromCell, m.toCell);
		}
	}

	// TODO: Depracate
//	public bool CanMakeMoves() {
//		return gameRule.CanMakeMoves (gameState, _session.Id);
//	}

	private void OnUnitMoved(object sender, MovementEventArgs args) {
		TBSUnitGameMove move = new TBSUnitGameMove();
		move.fromCell = args.OriginCell.Location;
		move.toCell = args.DestinationCell.Location;
		CommitMove(move);
	}

	public void CommitMove(TBSBaseGameMove move) {
		string json = JsonUtility.ToJson (move);
		SendMatchMessage (OPCODE_MOVE, json);
	}

	public void SendChatMessage(string messageString) {
		SendMatchMessage (OPCODE_CHAT, messageString);
	}

	private void Enqueue(Action action) {
		lock (_executionQueue) {
			_executionQueue.Enqueue(ActionWrapper(action));
			if (_executionQueue.Count > 1024) {
				Debug.LogWarning("Queued actions not consumed fast enough.");
				_client.Disconnect();
			}
		}
	}

	private IEnumerator ActionWrapper(Action action) {
		action();
		yield return null;
	}



}
