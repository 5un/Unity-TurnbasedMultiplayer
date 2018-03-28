using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MockInputController : MonoBehaviour {

	public Button mockActionButton;
	public CellGrid cellGrid;

	// Use this for initialization
	void Start () {
		mockActionButton.onClick.AddListener(OnMockActionButtonClicked);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMockActionButtonClicked() {
		// Do sth with the cell grid
		// Debug.Log("Clicked");
		// List<Unit> units = cellGrid.UnitsForCell("22");
		// Debug.Log ("Found: " + units.Count);

		cellGrid.MoveUnit ("22", "6");
	}
}