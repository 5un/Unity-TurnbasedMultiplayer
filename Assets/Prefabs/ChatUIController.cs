using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUIController : MonoBehaviour {

	public GameObject chatPanel;
	public GameObject messagesContainer;
	public Text prototypeChatText;
	public Button btnShowHideToggle;
	public Button btnSend;
	public InputField txtMessage;
	public bool isShowing = false;

	public TurnbasedMultiplayer turnbasedMultiplayer;

	public List<string> messages;
	private bool inputFieldWasFocused = false;

	// Use this for initialization
	void Start () 
	{
		btnSend.onClick.AddListener(OnBtnSendClicked);
		btnShowHideToggle.onClick.AddListener(OnBtnShowHideToggleClicked);
		turnbasedMultiplayer.onChatMessageReceived += OnNewMessage;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (inputFieldWasFocused && Input.GetKeyDown(KeyCode.Return)) {
			OnInputFieldSubmit();
		}

		inputFieldWasFocused = txtMessage.isFocused;
	}

	public Text AddTextToCanvas(string from, string textString)
	{

		Text text = Instantiate(prototypeChatText) as Text;
		text.transform.SetParent(messagesContainer.transform, false);
		text.text = from + ": " + textString;

		return text;
	}

	public void SendMessage(string inputText) {
		AddTextToCanvas("me", inputText);
		messages.Add (txtMessage.text);
		turnbasedMultiplayer.SendChatMessage (txtMessage.text);
		txtMessage.text = "";
	}

	void OnNewMessage(string from, string message) {
		AddTextToCanvas (from, message);
	}

	void OnBtnSendClicked() 
	{
		SendMessage (txtMessage.text);
	}

	void OnInputFieldSubmit()
	{
		SendMessage (txtMessage.text);
	}

	void OnBtnShowHideToggleClicked() 
	{
		Debug.Log ("show or hide");
		if (isShowing) 
		{
			chatPanel.SetActive (false);
			isShowing = false;
		} 
		else 
		{
			chatPanel.SetActive (true);
			isShowing = true;
		}
	}
}
