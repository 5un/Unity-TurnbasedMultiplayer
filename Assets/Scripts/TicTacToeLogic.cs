using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TicTacToeLogic
{
	public static string GetWinner(List<string> players, List<string> cells) {
		bool isWinning = false;
		for (int i = 0; i < 3; i++) {
			foreach (string playerIdToCheck in players) {
				isWinning = true;
				for (int j = 0; j < 3; j++) {
					if (cells[i * 3 + j] != playerIdToCheck) {
						isWinning = false;
					}
				}
				if (isWinning) {
					return playerIdToCheck;
				}
			}

			foreach (string playerIdToCheck in players) {
				isWinning = true;
				for (int j = 0; j < 3; j++) {
					if (cells[j * 3 + i] != playerIdToCheck) {
						isWinning = false;
					}
				}

				if (isWinning) {
					return playerIdToCheck;
				}
			}

		}
		return null;
	}
}

