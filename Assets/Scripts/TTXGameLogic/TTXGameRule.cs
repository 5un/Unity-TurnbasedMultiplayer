using Nakama;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TTXGameRule
{

	public GameState initGameState() {
		TTXGameState gs = new TTXGameState();

		// TODO

		return gs;
	}

	// Method to initialize game state

	public GameState AssignParticipants(GameState oldState, IList<INUserPresence> presence) 
	{
		string[] roles = { "0", "1" }; 
		int i = 0;
		List<GameParticipant> gameParticipants = new List<GameParticipant> ();
		foreach (var user in presence) 
		{
			GameParticipant gp = new GameParticipant ();
			gp.id = user.UserId;
			gp.role = roles [i];
			gameParticipants.Add (gp);
			i++;
		}
		oldState.whoseTurn = "0";
		oldState.participants = gameParticipants;
		return oldState;
	}

	public bool CanMakeMoves(GameState gameState, string participantId) 
	{
		GameParticipant gp = gameState.GetParticipantWithId (participantId);
		return gameState.stage == 0 && gp.role == gameState.whoseTurn;
	}

	public GameState ResolveMoves (GameState oldState, GameTurn gameTurn) 
	{
		// TODO: check stage

		// TODO: check phase



		foreach (GameMove move in gameTurn.moves) 
		{
			oldState.cells [move.targetRow * 3 + move.targetCol] = oldState.whoseTurn;
		}

		// Check winner
		string[] roles = { "0", "1" }; 
		List<string> players = new List<string> (roles);

		string winner = TicTacToeLogic.GetWinner (players, oldState.cells);
		if (winner != null) {
			oldState.stage = 1;
			oldState.winner = winner;
		} else {
			if (oldState.whoseTurn == "0") 
			{
				oldState.whoseTurn = "1";
			} 
			else 
			{
				oldState.whoseTurn = "0";
			}
		}

		return oldState;
	}

}

