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
			LobbyManager.s_Singleton.GoBackButton (); //.StopHostClbk ();
		else
			LobbyManager.s_Singleton.GoBackButton (); //.StopClientClbk ();
	}

	public void SetEndScreen(bool won) {
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		if (won) {
			conditionLabel.text = "You won. Good job! All enemy forces eliminated.";
			background.color = bkgndWinColor;
		} else {
			conditionLabel.text = "Enemy forces overwhelmed you. Better luck next time!";
			background.color = bkgndLoseColor;
		}
	}
}
