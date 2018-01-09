using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	public Vector3 rotationPerAxis;
	public float speed = 1f;

	// Update is called once per frame
	void Update () {
		transform.Rotate (rotationPerAxis * Time.deltaTime * speed);
	}
}
