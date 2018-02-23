using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakerUIController : MonoBehaviour {

	public string matchingState;
	public NetworkSessionManager networkSessionManager;
	public Button btnLogin;
	public InputField inputCustomId;

	public Text txtMatchMakingStatus;
	public Button btnStartMatchMaking;

	public Canvas pageLogin;
	public Canvas pageMatchMaking;

	private Queue<IEnumerator> _executionQueue;


	public MatchMakerUIController() {
		_executionQueue = new Queue<IEnumerator>(1024);
	}

	// Use this for initialization
	void Start () 
	{
		btnLogin.onClick.AddListener (OnBtnLoginClicked);
		btnStartMatchMaking.onClick.AddListener (OnBtnStartMatchMakingClicked);

		networkSessionManager.AddOnSessionConnectAction(OnSessionConnected);
		networkSessionManager.AddOnMatchJoinedAction (OnMatchJoined);

		Reset ();
	}
	
	private void Update() {
		lock (_executionQueue) {
			for (int i = 0, len = _executionQueue.Count; i < len; i++) {
				StartCoroutine(_executionQueue.Dequeue());
			}
		}
	}

	private void Reset() {
		pageLogin.enabled = true;
		pageMatchMaking.enabled = false;
	}

	public void OnBtnLoginClicked() 
	{
		//TODO: validation
		networkSessionManager.LoginOrRegisterWithCustomId (inputCustomId.text, 
			() => {
				Debug.Log("btn login callback");
			},
			() => {
				Debug.Log("btn login fail callback");
			}
		);
		//TODO: Callback?
	}

	public void OnBtnStartMatchMakingClicked() 
	{
		networkSessionManager.StartMatchMaking (() => {
			Debug.Log("Match making succeeded");
			// do nothing
		}, () => {
			// Failed
			btnStartMatchMaking.enabled = true;
			// var txt = btnStartMatchMaking.GetComponentsInChildren(typeof(Text)) as Text;
			// txt.text = "Retry";
			txtMatchMakingStatus.text = "Failed to find match";
		});
		btnStartMatchMaking.enabled = false;
		txtMatchMakingStatus.text = "Looking for a match...";

	}

	public void OnSessionConnected()
	{
		Debug.Log ("On Session Connected");
		Enqueue (() => 
			{
				Debug.Log ("Main thread");
				pageLogin.enabled = false;
				pageMatchMaking.enabled = true;
			}
		);
	}

	public void OnMatchJoined()
	{
		Enqueue (() => 
			{
				Debug.Log ("Main thread");
				txtMatchMakingStatus.text = "Match Joined";
			}
		);
	}

	private void Enqueue(Action action) {
		lock (_executionQueue) {
			_executionQueue.Enqueue(ActionWrapper(action));
			if (_executionQueue.Count > 1024) {
				Debug.LogWarning("Queued actions not consumed fast enough.");
				// _client.Disconnect();
			}
		}
	}

	private IEnumerator ActionWrapper(Action action) {
		action();
		yield return null;
	}

}
