using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamMover : MonoBehaviour {

	public Transform deathCamT;
	
	// Update is called once per frame
	void Update () {
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, deathCamT.position, Time.deltaTime * 5f);
		Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, deathCamT.rotation, Time.deltaTime * 5f);
	}
}
