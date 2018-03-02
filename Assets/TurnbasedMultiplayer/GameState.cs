using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameState
{
	public int stage = 0;
	public int numTurns = 0;
	public string whoseTurn = "O";
	public string winner = "";

	public List<string> cells;

	// In case there's a deck of cards
	public List<string> cards;

	public List<GameParticipant> participants;

	public GameState() {
		cells = new List<string> ();
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				cells.Add ("");
			}
		}
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

