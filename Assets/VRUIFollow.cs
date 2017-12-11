using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRUIFollow : MonoBehaviour {

	public Transform followedUIObj;
	public float dampen = 3f;
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Slerp (
									transform.rotation, 
									Quaternion.LookRotation (new Vector3 (followedUIObj.forward.x, 0, followedUIObj.forward.z)), 
									Time.deltaTime * dampen);
	}
}
