using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TTXGameParticipant
{
	public string id;
	public string role;

	public List<TTXCard> cardsOnHand;
	public List<Object> resourceMakers;
	public List<string> goals;
	public List<string> signalingTokens;

	public TTXGameParticipant() {

	}
}

