using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamMover : MonoBehaviour {

	public Transform deathCamT;
	public float movSpeed = 1f;
	
	// Update is called once per frame
	void Update () {
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, deathCamT.position, Time.deltaTime * movSpeed);
		Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, deathCamT.rotation, Time.deltaTime * movSpeed);
	}
}
