using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VRTutorialHelper : MonoBehaviour {

	public GameObject sign;

	// Use this for initialization
	void Start () {
		LoadSetting ();
	}

	public void LoadSetting() {
		GameSettingsContainer gameSettings = new GameSettingsContainer ();

		if (File.Exists (Application.persistentDataPath + "/gamesettings.json")) {
			gameSettings = JsonUtility.FromJson<GameSettingsContainer> (File.ReadAllText (Application.persistentDataPath + "/gamesettings.json"));

			sign.SetActive(gameSettings.showVRTutorial);
		}
	}
}
