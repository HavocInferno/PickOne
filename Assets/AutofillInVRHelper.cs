using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI; 

public class AutofillInVRHelper : MonoBehaviour {

	public string defaultText;
	public InputField inputField;

	// Use this for initialization
	void Start () {
		if (XRDevice.userPresence == UserPresenceState.Unsupported) {
			this.enabled = false;
		} else if (XRDevice.userPresence == UserPresenceState.Present && inputField.text.Equals("")) {
			Debug.Log("In VR but InputField was empty.");
			inputField.text = defaultText + SystemInfo.deviceName;
		}
	}
}

