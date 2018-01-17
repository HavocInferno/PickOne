using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperDisableForNonVR : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(!GetComponentInParent<Crawler>().isVRMasterPlayer) //if (!UnityEngine.XR.XRSettings.enabled)
			gameObject.SetActive (false);
	}
}
