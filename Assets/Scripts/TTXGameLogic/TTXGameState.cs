using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TTXGameState: GameState
{
	public int stage = 0;
	public int numRounds = 0;
	public string phase;
	public string whoseTurn = "O";
	public string winner = "";

	public List<string> cells;

	// In case there's a deck of cards
	public List<string> cards;

	public TTXGameState() {
		// TODO: generate cells

		// TODO: put resource makers into designated territories



	}

	public GameParticipant GetParticipantWithId(string id) {
		foreach(GameParticipant gp in participants){
			if (gp.id == id) {
				return gp;
			}
		}
		return null;
	}

}

