using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperDisableForNonVR : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(FindObjectOfType<Master>() && UnityEngine.XR.XRSettings.enabled)
			gameObject.SetActive (true);
		else
			gameObject.SetActive (false);
	}
}
