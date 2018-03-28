using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TicTacToeCell : MonoBehaviour {

	public Button button;
	public GameObject cross1;
	public GameObject cross2;
	public GameObject diamond;
	public String occupiedBy;
	// GameController gameController;
	public Action OnClick;
	public int row;
	public int col;

	public delegate void OnCellClicked(int r, int c);

	public event OnCellClicked onCellClicked;

	// Use this for initialization
	void Start () {
		button = gameObject.GetComponent (typeof(Button)) as Button;
		button.onClick.AddListener (OnButtonClick);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetOccupiedBy(String player) {
		if (player == "0") {
			occupiedBy = player;
			cross1.SetActive (false);
			cross2.SetActive (false);
			diamond.SetActive (true);
		} else if (player == "1") {
			occupiedBy = player;
			cross1.SetActive (true);
			cross2.SetActive (true);
			diamond.SetActive (false);
		} else {
			occupiedBy = player;
			cross1.SetActive (false);
			cross2.SetActive (false);
			diamond.SetActive (false);
		}
	}

	public void SetCellPosition(int r, int c) {
		row = r;
		col = c;
	}

	public void OnButtonClick() {
		onCellClicked(row, col);
		// SetOccupiedBy (gameController.getCurrentPlayer ());
		// gameController.EndTurn ();
	}
}
