using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI; 

public class MenuSwapVRHelper : MonoBehaviour {

	public Canvas target;

	public Vector3 rectTPos;
	public Vector3 rectTRot;
	public Vector3 rectTScl;
	public Vector2 rectTDlt;

	//have to manually set the recttransform values for world space mode with every switch
	//---recttransform world space custom values here

	// Use this for initialization
	void Start () {
		//sadly this needs to be in since for no good reason whenever a match is hosted from inside VR, players cant choose classes anymore. Makes no sense...
		if (true) {
			target.renderMode = RenderMode.ScreenSpaceOverlay;
			this.enabled = false;
		}
		
		rectTPos = GetComponent<RectTransform> ().position;
		rectTRot = GetComponent<RectTransform> ().eulerAngles;
		rectTScl = GetComponent<RectTransform> ().localScale;
		rectTDlt = GetComponent<RectTransform> ().sizeDelta;

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

			GetComponent<RectTransform> ().position = rectTPos;
			GetComponent<RectTransform> ().eulerAngles = rectTRot;
			GetComponent<RectTransform> ().localScale = rectTScl;
			GetComponent<RectTransform> ().sizeDelta = rectTDlt;
		}

		if (XRDevice.userPresence == UserPresenceState.NotPresent && !(target.renderMode == RenderMode.ScreenSpaceOverlay)) {
			Debug.Log("no VR user, switching Lobby to Screen Space");
			target.renderMode = RenderMode.ScreenSpaceOverlay;
		}
	}
}
