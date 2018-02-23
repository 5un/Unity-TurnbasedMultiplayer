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

	const long OPCODE_HOST_GAME_STATE = 101L;
	const long OPCODE_GUEST_MOVE = 102L;

	void Awake() {
		base.Awake ();
		AddOnMatchJoinedAction (OnMatchJoined);
	}

	// Use this for initialization
	void Start () {
		gameRule = new GameRule ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}

	public void EnterSimulationMode() {
		// TODO: build Automated Participants to match the number of desired players
		// Init host logic

	}

	public void OnMatchJoined() {
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
			SendGameState ();
		}
	}
		
	public void OnMatchData(INMatchData m) {
		base.OnMatchData (m);
		var content = Encoding.UTF8.GetString(m.Data);

		// TODO: if it's a multiplayer message, do something with it
		// TODO: if you are a host an receives moves from guests, resolve them
		switch (m.OpCode) {
		case OPCODE_HOST_GAME_STATE:
			GameState newGameState = JsonUtility.FromJson<GameState> (content);
			//TODO: Update to other game objects in the game
			gameState = newGameState;
			break;
		case OPCODE_GUEST_MOVE:
			if (isHost) {
				GameTurn turn = JsonUtility.FromJson<GameTurn> (content);
				gameState = gameRule.ResolveMoves (turn);
				SendGameState ();
			}
			break;
		default:
			Debug.LogFormat("User handle '{0}' sent '{1}'", m.Presence.Handle, content);
		};
	}

	public void CommitMoves(GameTurn turn) {
		string json = JsonUtility.ToJson (turn);
		SendMatchMessage (OPCODE_GUEST_MOVE, json);
	}

	// Send the current game state to all the guests
	public void SendGameState() {
		string json = JsonUtility.ToJson (gameState);
		SendMatchMessage (OPCODE_HOST_GAME_STATE, json);
	}

}
