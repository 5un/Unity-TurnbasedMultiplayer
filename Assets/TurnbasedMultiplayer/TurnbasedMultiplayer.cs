using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nakama;

public class TurnbasedMultiplayer : NetworkSessionManager {

	public bool useMultiplayerSimulation;

	private GameRule gameRule;
	private GameState gameState;
	private bool isHost = true;

	const long OPCODE_CHAT = 100L;
	const long OPCODE_HOST_GAME_STATE = 101L;
	const long OPCODE_GUEST_MOVE = 102L;

	public delegate void OnGameStateUpdated (GameState gameState);
	public OnGameStateUpdated onGameStateUpdated;

	public delegate void OnChatMessageReceived (string from, string message);
	public OnChatMessageReceived onChatMessageReceived;

	private Queue<IEnumerator> _executionQueue;

	public TurnbasedMultiplayer() {
		_executionQueue = new Queue<IEnumerator>(1024);
	}

	// Use this for initialization
	void Start () {
		
		gameRule = new GameRule ();
		onMatchJoined += OnMatchJoined;

		GameTurn turn = JsonUtility.FromJson<GameTurn> ("{\"moves\":[{\"targetRow\":0,\"targetCol\":0}]}");
		Debug.Log (turn);
		Debug.Log (JsonUtility.ToJson (turn));

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
				isHost = true;
			}
			i++;
		}

		Debug.Log ("isHost " + isHost);

		if (isHost) {
			// Establish game state
			gameState = new GameState();
			gameState = gameRule.AssignParticipants (gameState, _matchParticipants);
			SendGameState ();
		}
	}
		
	public override void OnMatchData(INMatchData m) {
		base.OnMatchData (m);
		var content = Encoding.UTF8.GetString(m.Data);
		content = content.Trim ();
		content = content.Substring (1, content.Length - 2);
		content = content.Replace (@"\", "");


		Debug.Log ("OnMatchData");
		Debug.Log (content);
		// TODO: if it's a multiplayer message, do something with it
		// TODO: if you are a host an receives moves from guests, resolve them
		switch (m.OpCode) {
		case OPCODE_CHAT:
			Enqueue (() => {
				onChatMessageReceived (m.Presence.Handle, content);
			});
			break;
		case OPCODE_HOST_GAME_STATE:
			GameState newGameState = JsonUtility.FromJson<GameState> (content);
			//TODO: Update to other game objects in the game
			gameState = newGameState;
			onGameStateUpdated (gameState);
			break;
		case OPCODE_GUEST_MOVE:
			Debug.Log ("Received action from guest");
			if (isHost) {
				Debug.Log("is host, Parsing");
				GameTurn turn = JsonUtility.FromJson<GameTurn> (content);

				Debug.Log("Parsed");
				Debug.Log (JsonUtility.ToJson (turn));

				gameState = gameRule.ResolveMoves (gameState, turn);
				SendGameState ();
				Enqueue (() => {
					onGameStateUpdated (gameState);
				});
			}
			break;
		default:
			Debug.LogFormat ("User handle '{0}' sent '{1}' '21}'", m.Presence.Handle, m.OpCode, content);
			break;
		};
	}

	public bool CanMakeMoves() {
		return gameRule.CanMakeMoves (gameState, _session.Id);
	}

	public void CommitMoves(GameTurn turn) {
		if (isHost) {
			gameState = gameRule.ResolveMoves (gameState, turn);
			SendGameState ();
			onGameStateUpdated (gameState);
		} else {
			string json = JsonUtility.ToJson (turn);
			SendMatchMessage (OPCODE_GUEST_MOVE, json);
		}
	}

	public void SendChatMessage(string messageString) {
		SendMatchMessage (OPCODE_CHAT, messageString);
	}

	// Send the current game state to all the guests
	public void SendGameState() {
		string json = JsonUtility.ToJson (gameState);
		SendMatchMessage (OPCODE_HOST_GAME_STATE, json);
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
