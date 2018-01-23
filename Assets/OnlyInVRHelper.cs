using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OnlyInVRHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (XRDevice.userPresence == UserPresenceState.Unsupported) {
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (XRDevice.userPresence == UserPresenceState.Present && !GetComponent<Canvas> ().enabled) {
			GetComponent<Canvas> ().enabled = true;
		}

		if (XRDevice.userPresence == UserPresenceState.NotPresent && GetComponent<Canvas> ().enabled) {
			GetComponent<Canvas> ().enabled = false;
		}
	}
}
