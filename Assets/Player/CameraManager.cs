﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraManager : MonoBehaviour
{

	//level-wide container class for miscellanous necessary camera information/access

	public GameObject nonVRCamera;
	public GameObject vrCamera;


	[MenuItem("PickOneTools/Cams to crawler setup")]
	private static void CrawlerCamsOption()
	{
		FindObjectOfType<CameraManager> ().nonVRCamera.SetActive (true);
		FindObjectOfType<CameraManager> ().vrCamera.SetActive (false);
	}

	[MenuItem("PickOneTools/Cams to VR master setup")]
	private static void MasterCamsOption()
	{
		FindObjectOfType<CameraManager> ().nonVRCamera.SetActive (false);
		FindObjectOfType<CameraManager> ().vrCamera.SetActive (true);
	}
}
