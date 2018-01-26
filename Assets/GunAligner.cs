using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAligner : MonoBehaviour {

	public Transform psydoparent;
	public float strength = 1;
	public bool x,y,z;

	Vector3 dist; 
	bool ready = false;
	// Use this for initialization
	void Start () {
		StartCoroutine (waitstart ());
	}
	
	// Update is called once per frame
	void Update () {
		if (ready) {
			float xs, ys, zs;
			if (x)
				xs = (psydoparent.position + psydoparent.TransformDirection (dist * transform.localScale.magnitude)).x;
			else
				xs =transform.position.x;
			if (y)
				ys = (psydoparent.position + psydoparent.TransformDirection (dist * transform.localScale.magnitude)).y;
			else
				ys =transform.position.y;
			if (z)
				zs = (psydoparent.position + psydoparent.TransformDirection (dist * transform.localScale.magnitude)).z;
			else
				zs =transform.position.z;

			transform.position = Vector3.Lerp (transform.position, new Vector3(xs,ys,zs), strength * Time.deltaTime);

		}
	}
	IEnumerator waitstart()
	{
		yield return new WaitForSeconds (1);
		Transform oldParent = transform.parent;
		transform.parent = psydoparent;
		dist = transform.localPosition/transform.localScale.magnitude;
		transform.parent = oldParent;
		ready = true;
	}
}
