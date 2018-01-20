﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;

public class MainMenu : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject settingsPanel;
	public GameObject infoPanel;

	private GameObject currentPanel;

	static public MainMenu s_Singleton;

	void Awake () {
		s_Singleton = this;
	}

	// Use this for initialization
	void Start () {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;

		if(LobbyManager.s_Singleton)
			LobbyManager.s_Singleton.mainMenuUI = gameObject;

		if (LobbyManager.s_Singleton.straightToLobby) {
			OnClickPlay ();
			LobbyManager.s_Singleton.straightToLobby = false;
		}

		ChangeTo (mainPanel);
	}

	public void OnClickPlay() {
		gameObject.SetActive (false);
	}

	public void OnClickChangePanel(GameObject panel) {
		ChangeTo (panel);
	}

	void ChangeTo(GameObject nextPanel) {
		nextPanel.SetActive (true);
		if (currentPanel)
			currentPanel.SetActive (false);
		currentPanel = nextPanel;
	}

	public void StartDisplayInfo(string infoText) {
		infoPanel.SetActive (true);
		infoPanel.GetComponent<LobbyInfoPanel> ().infoText.text = infoText;
		//infoPanel.GetComponent<LobbyInfoPanel> ().singleButton.gameObject.SetActive (false);
	}

	public void StopDisplayInfo() {
		infoPanel.SetActive (false);
	}
}
