using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {

	public float ejectionSpeed;
	public float rand;
	public float lifeTime;
	// Use this for initialization
	void Start () {
		var rig = GetComponent<Rigidbody> ();
		transform.Rotate (0,0,Random.Range(-rand, rand));
		if (rig != null) {
			rig.velocity = transform.TransformDirection(new Vector3 (ejectionSpeed+Random.Range(-rand, rand), 0,0));
			rig.angularVelocity = new Vector3 (ejectionSpeed+Random.Range(-rand, rand),ejectionSpeed+Random.Range(-rand, rand),ejectionSpeed+Random.Range(-rand, rand));
		}
		Destroy (gameObject, lifeTime);
	}
}
