using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EndScreenUI : MonoBehaviour {

	public Text conditionLabel;

	public void OnRetToLobbyClicked()
	{
		Debug.Log("a CLIENT wants to return to lobby");
		FindObjectOfType<NetworkLobbyManager> ().ServerReturnToLobby ();
	}
}
