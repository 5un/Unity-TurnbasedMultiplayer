using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Canvas matchMakerUI;
	public Canvas gameUI;
	public Canvas chatUI;
	public TurnbasedMultiplayer tbm;
	public TicTacToeBoard ticTacToeBoard;
	public Text txtWinner;

	// Use this for initialization
	void Start () {
		ticTacToeBoard.onCellClicked += OnCellClicked;
		tbm.onGameStateUpdated += OnGameStateUpdated;
		tbm.onMatchJoined += OnMatchJoined;

		matchMakerUI.enabled = true;
		gameUI.enabled = false;
		chatUI.enabled = false;
		txtWinner.enabled = false;
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
		if (gameState.stage == 0) {
			txtWinner.enabled = false;
			ticTacToeBoard.UpdateBoard (gameState.cells);
		} else if(gameState.stage == 1) {
			txtWinner.enabled = true;
			txtWinner.text = "The Winner is player " + gameState.winner;
			ticTacToeBoard.UpdateBoard (gameState.cells);
		}

	}

	void OnMatchJoined(INMatch match) {
		Debug.Log ("OnMatchJoined from GameController");
		//TODO: hide match maker ui and show ttt
		gameUI.enabled = true;
		matchMakerUI.enabled = false;
		chatUI.enabled = true;
			
	}


}
