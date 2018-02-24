using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeBoard : MonoBehaviour {

	public List<Cell> cells;
	//public GameObject[] cells;

	public delegate void OnCellClicked(int r, int c);
	public event OnCellClicked onCellClicked;

	// Use this for initialization
	void Start () {
		for (var i = 0; i < 3; i++) {
			for (var j = 0; j < 3; j++) {
				cells[i * 3 + j].SetOccupiedBy ("");
				cells[i * 3 + j].SetCellPosition (i, j);
				cells [i * 3 + j].onCellClicked += (row, col) => {
					onCellClicked (row, col);
				};
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateBoard(List<string> c) {
		for (var i = 0; i < 3; i++) {
			for (var j = 0; j < 3; j++) {
				cells [i * 3 + j].SetOccupiedBy (c [i * 3 + j]);
			}
		}
	}

}
