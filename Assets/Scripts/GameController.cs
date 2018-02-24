using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public Canvas matchMakerUI;
	public Canvas gameUI;
	public TurnbasedMultiplayer tbm;
	public TicTacToeBoard ticTacToeBoard;

	// Use this for initialization
	void Start () {
		ticTacToeBoard.onCellClicked += OnCellClicked;
		tbm.onGameStateUpdated += OnGameStateUpdated;
		tbm.onMatchJoined += OnMatchJoined;

		matchMakerUI.enabled = true;
		gameUI.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCellClicked(int row, int col) {
		if (tbm.CanMakeMoves ()) {
			GameTurn turn = new GameTurn ();
			GameMove move = new GameMove ();
			move.targetRow = row;
			move.targetCol = col;
			turn.moves.Add (move);
			tbm.CommitMoves (turn);
		} else {
			Debug.Log ("This is not your turn");
		}
	}

	void OnGameStateUpdated(GameState gameState) {
		Debug.Log ("OnGameStateUpdated");
		Debug.Log (JsonUtility.ToJson (gameState));

		ticTacToeBoard.UpdateBoard (gameState.cells);
	}

	void OnMatchJoined(INMatch match) {
		Debug.Log ("OnMatchJoined from GameController");
		//TODO: hide match maker ui and show ttt
		gameUI.enabled = true;
		matchMakerUI.enabled = false;
			
	}


}
