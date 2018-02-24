using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NetworkSessionManager : MonoBehaviour {
	
	public string serverHost = "127.0.0.1";
	public string serverPort = "7350";
	public bool useSSL = false;
	public string serverKey = "defaultkey";
	public int numMatchParticipants = 2;

	//public List<Action> OnSessionConnectedActions;
	//public List<Action> OnMatchJoinedActions;

	public delegate void OnSessionConnectedDelegate(INSession session);
	public delegate void OnMatchJoinedDelegate(INMatch match);

	public OnSessionConnectedDelegate onSessionConnected;
	public OnMatchJoinedDelegate onMatchJoined;

	protected INClient _client;
	protected INSession _session;
	protected INMatch _match;
	protected IList<INUserPresence> _matchParticipants;

	private Queue<IEnumerator> _executionQueue;

	public NetworkSessionManager() {
		_executionQueue = new Queue<IEnumerator>(1024);
		// OnSessionConnectedActions = new List<Action> ();
		// OnMatchJoinedActions = new List<Action> ();

	}

	public void Awake() {
		_client = new NClient.Builder(serverKey)
			.Host(serverHost)
			.Port(uint.Parse(serverPort))
			.SSL(useSSL)
			.Build();
		_client.OnMatchData = OnMatchData;
		RestoreSessionAndConnect();
	}

	public void Update() {
		lock (_executionQueue) {
			for (int i = 0, len = _executionQueue.Count; i < len; i++) {
				StartCoroutine(_executionQueue.Dequeue());
			}
		}
	}

	private void RestoreSessionAndConnect() {
		// Lets check if we can restore a cached session.
		var sessionString = PlayerPrefs.GetString("nk.session");
		if (string.IsNullOrEmpty(sessionString)) {
			return; // We have no session to restore.
		}

		var session = NSession.Restore(sessionString);
		if (session.HasExpired(DateTime.UtcNow)) {
			return; // We can't restore an expired session.
		}

		SessionHandler(session);
	}

	public void LoginOrRegisterWithCustomId(string customId, Action onLoginSucceeded, Action onLoginError) {
		var message = NAuthenticateMessage.Custom(customId);
		_client.Login(message, 
			(INSession session) => 
			{
				Debug.Log("Login Callback");
				SessionHandler(session);
			}, 
			(INError err) => 
			{
				if (err.Code == ErrorCode.UserNotFound) {
					_client.Register(message, SessionHandler, ErrorHandler);

				} else {
					ErrorHandler(err);
				}
			}
		);
	}

	private void SessionHandler(INSession session) {
		_session = session;
		Debug.LogFormat("Session: '{0}'.", session.Token);
		_client.Connect(_session, (bool done) => {
			// We enqueue callbacks which contain code which must be dispatched on
			// the Unity main thread.
//			foreach(Action a in OnSessionConnectedActions) {
//				a();
//			}


			Enqueue(() => {
				Debug.Log("Session connected.");
				// Store session for quick reconnects.
				onSessionConnected(session);
				PlayerPrefs.SetString("nk.session", session.Token);
			});
		});
	}

	private void OnApplicationQuit() {
		if (_session != null) {
			_client.Disconnect();
		}
	}

	private void Enqueue(Action action) {
		lock (_executionQueue) {
			_executionQueue.Enqueue(ActionWrapper(action));
			if (_executionQueue.Count > 1024) {
				Debug.LogWarning("Queued actions not consumed fast enough.");
				_client.Disconnect();
			}
		}
	}

	private IEnumerator ActionWrapper(Action action) {
		action();
		yield return null;
	}

	private static void ErrorHandler(INError err) {
		Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", err.Code, err.Message);
	}

	public void StartMatchMaking(Action onMatchMakeSucceeded, Action onMatchMakeFailed) {
		INMatchmakeTicket matchmake = null;
		IList<INUserPresence> matchParticipants = null;

		// Look for a match for two participants. Yourself and one more.
		var message = NMatchmakeAddMessage.Default(numMatchParticipants);
		_client.Send(message, (INMatchmakeTicket result) => {
			Debug.Log("Added user to matchmaker pool.");

			var cancelTicket = result.Ticket;
			Debug.LogFormat("The cancellation code {0}", cancelTicket);
		}, (INError err) => {
			Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", err.Code, err.Message);
		});

		_client.OnMatchmakeMatched = (INMatchmakeMatched matched) => {
			// a match token is used to join the match.
			Debug.LogFormat("Match token: '{0}'", matched.Token);

			matchParticipants = matched.Presence;
			// a list of users who've been matched as opponents.
			foreach (var presence in matched.Presence) {
				Debug.LogFormat("User id: '{0}'.", presence.UserId);
				Debug.LogFormat("User handle: '{0}'.", presence.Handle);
			}

			// list of all match properties
			foreach (var userProperty in matched.UserProperties) {
				foreach(KeyValuePair<string, object> entry in userProperty.Properties) {
					Debug.LogFormat("Property '{0}' for user '{1}' has value '{2}'.", entry.Key, userProperty.Id, entry.Value);
				}

				foreach(KeyValuePair<string, INMatchmakeFilter> entry in userProperty.Filters) {
					Debug.LogFormat("Filter '{0}' for user '{1}' has value '{2}'.", entry.Key, userProperty.Id, entry.Value.ToString());
				}
			}

			var jm = NMatchJoinMessage.Default(matched.Token);
			_client.Send(jm, (INResultSet<INMatch> matches) => {
				Debug.Log("Successfully joined match.");
				_match = matches.Results[0];
				_matchParticipants = matchParticipants; 
//				foreach(Action a in OnMatchJoinedActions) {
//					a();
//				}

				foreach(INMatch match in matches.Results) {
					Debug.LogFormat("Match id: {0} Presence", match.Id);

					foreach(INUserPresence presence in match.Presence) {
						Debug.LogFormat("User handle: {0} id {1}.", presence.Handle, presence.UserId);

					}
				}

				Enqueue(() => {
					onMatchJoined(_match);
				});
			}, (INError error) => {
				Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", error.Code, error.Message);
			});

			// TODO callback to UI
			onMatchMakeSucceeded();
		};
	}

	public void SendMatchMessage(long opCode, string strMessage) {
		//TODO: only if current match excist
		string id = _match.Id; // an INMatch Id.
		Debug.Log("Sending Match Message to id " + _match.Id);
		byte[] data = Encoding.UTF8.GetBytes(strMessage);
		var message = NMatchDataSendMessage.Default(id, opCode, data);
		_client.Send(message, (bool done) => {
			Debug.Log("Successfully sent data message.");
		}, (INError err) => {
			Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", err.Code, err.Message);
		});
	}

	public virtual void OnMatchData(INMatchData m) {
		var content = Encoding.UTF8.GetString(m.Data);
		switch (m.OpCode) {
		case 101L:
			Debug.Log("A custom opcode.");
			break;
		default:
			Debug.LogFormat ("User handle '{0}' sent '{1}'", m.Presence.Handle, content);
			break;
		};
	}

//	public void AddOnSessionConnectAction(Action action) {
//		OnSessionConnectedActions.Add (action);
//	}
//
//	public void AddOnMatchJoinedAction(Action action) {
//		OnMatchJoinedActions.Add (action);
//	}

}