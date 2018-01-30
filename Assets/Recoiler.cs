using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoiler : MonoBehaviour {

	public Vector3 recoilPos;
	public float recoilRot;
	public float resetSpeed;

	Vector3 originalPos;
	Quaternion originalRot;
	// Use this for initialization
	void Start () {
		originalPos = transform.localPosition;
		originalRot = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = Vector3.Lerp (transform.localPosition, originalPos, Time.deltaTime*resetSpeed);
		transform.localRotation = Quaternion.Slerp (transform.localRotation, originalRot, Time.deltaTime * resetSpeed*2);
	}

	public void recoil()
	{
		transform.localPosition -= recoilPos;
		transform.Rotate (Random.Range(-recoilRot,recoilRot),Random.Range(-recoilRot,recoilRot),Random.Range(-recoilRot,recoilRot),Space.Self);
		if (transform.localPosition.magnitude > recoilPos.magnitude)
			transform.localPosition *= .75f;
	}
}
