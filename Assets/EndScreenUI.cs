using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class EndScreenUI : NetworkBehaviour {

	public Text conditionLabel;
	public Image background;
	public Color bkgndWinColor;
	public Color bkgndLoseColor;

	public void OnRetToLobbyClicked()
	{
		Debug.Log("a CLIENT wants to return to lobby");
		if (isServer)
			LobbyManager.s_Singleton.StopHostClbk ();
		else
			LobbyManager.s_Singleton.StopClientClbk ();
	}

	public void SetEndScreen(bool won) {
		if (won) {
			conditionLabel.text = "Winner Wnr Chkn Dnr";
			background.color = bkgndWinColor;
		} else {
			conditionLabel.text = "Sucks to be a loser";
			background.color = bkgndLoseColor;
		}
	}
}
