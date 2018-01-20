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

	public GameObject endScreenObj;
	public GameObject deathScreenObj;
	public GameObject abandonScreenObj;

	public GameObject crawlerUI;

	private bool gameEnded;

	public void OnRetToLobbyClicked()
	{
		Debug.Log("a CLIENT wants to return to lobby");
		if (isServer)
			LobbyManager.s_Singleton.GoBackButton (); //.StopHostClbk ();
		else
			LobbyManager.s_Singleton.GoBackButton (); //.StopClientClbk ();
	}

	public void SetEndScreen(bool won) {
		gameEnded = true;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		endScreenObj.SetActive (true);
		SetDeathScreen (false);
		if (won) {
			conditionLabel.text = "You won. Good job! All enemy forces eliminated.";
			background.color = bkgndWinColor;
		} else {
			conditionLabel.text = "Enemy forces overwhelmed you. Better luck next time!";
			background.color = bkgndLoseColor;
		}
	}

	public void SetAbandonedScreen() {
		gameEnded = true;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		abandonScreenObj.SetActive (true);
		crawlerUI.SetActive (false);
	}

	[ClientRpc]
	public void RpcAbandoned()
	{
		Debug.LogError ("Error: WOOPS-0 --- A player left the match");
		SetAbandonedScreen ();
	}

	public void SetDeathScreen(bool state) {
		if (gameEnded)
			return;
		
		deathScreenObj.SetActive (state);
		crawlerUI.SetActive (!state);

		if (state) {
			Camera.main.GetComponent<DungeonCamera> ().enabled = false;
			GetComponent<DeathCamMover> ().enabled = true;
		}
	}
}
