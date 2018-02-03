using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VRTutorialHelper : MonoBehaviour {

	bool tutOn;
	public GameObject sign;
	public GameObject riftPart;
	public GameObject vivePart;
	int vrDeviceModel = -1;

	// Use this for initialization
	void Start () {
		LoadSetting ();

		if (!tutOn)
			return;

		if(!UnityEngine.XR.XRSettings.enabled) {
			vivePart.SetActive (false);
			riftPart.SetActive (false);
			return;
		}

		QueryVRDeviceModel ();

		if (vrDeviceModel == 1) {
			vivePart.SetActive (true);
			riftPart.SetActive (false);
		} else if (vrDeviceModel == 2) {
			vivePart.SetActive (false);
			riftPart.SetActive (true);
		} else {
			vivePart.SetActive (false);
			riftPart.SetActive (false);
		}
	}

	public void LoadSetting() {
		GameSettingsContainer gameSettings = new GameSettingsContainer ();

		if (File.Exists (Application.persistentDataPath + "/gamesettings.json")) {
			gameSettings = JsonUtility.FromJson<GameSettingsContainer> (File.ReadAllText (Application.persistentDataPath + "/gamesettings.json"));

			tutOn = gameSettings.showVRTutorial;
			sign.SetActive(tutOn);
		}
	}

	public void QueryVRDeviceModel() {
		Debug.Log ("Querying VR Device Model");
		if (UnityEngine.XR.XRDevice.isPresent) {
			//isVRcapable = true;

			string model = UnityEngine.XR.XRDevice.model != null ?
				UnityEngine.XR.XRDevice.model : "";

			if (model.IndexOf ("Rift") >= 0) {
				vrDeviceModel = 2;
			} else {
				vrDeviceModel = 1;
			}
			Debug.Log ("VR headset found, model " + vrDeviceModel + "; " + model);
		} else {
			vrDeviceModel = -1;
		}
	}
}
