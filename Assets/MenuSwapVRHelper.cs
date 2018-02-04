using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI; 

public class MenuSwapVRHelper : MonoBehaviour {

	public Canvas target;

	//have to manually set the recttransform values for world space mode with every switch
	//---recttransform world space custom values here

	// Use this for initialization
	void Start () {
		if (!XRDevice.isPresent) {
			target.renderMode = RenderMode.ScreenSpaceOverlay;
			this.enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if (XRDevice.userPresence == UserPresenceState.Present && !(target.renderMode == RenderMode.WorldSpace)) {
			Debug.Log("VR user detected, switching Lobby to World Space");
			target.renderMode = RenderMode.WorldSpace;
		}

		if (XRDevice.userPresence == UserPresenceState.NotPresent && !(target.renderMode == RenderMode.ScreenSpaceOverlay)) {
			Debug.Log("no VR user, switching Lobby to Screen Space");
			target.renderMode = RenderMode.ScreenSpaceOverlay;
		}
	}
}
