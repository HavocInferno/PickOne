using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISizeHelperForVR : MonoBehaviour {

	public RectTransform uiObj;
	public float vrSize;

	// Use this for initialization
	void Start () {
		if (FindObjectOfType<Master> () && UnityEngine.XR.XRSettings.enabled)
			uiObj.localScale *= vrSize;
	}
}
