using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LeftHandFollower : MasterFollower {

	// Use this for initialization

	public Controller controller;
	public Master master;
	public Transform pingArrow, capsule, healPool, firePool;
	public float growspeed =20;
	public Vector3 arrowscale, capsuleScale;

	//[SyncVar]
	//public Vector3 firepoolScale, healpoolScale;
	[SyncVar]
	bool gripping = false;

	void Start () {
		arrowscale = pingArrow.localScale;
		capsuleScale = capsule.localScale;
		if (followed.gameObject.activeInHierarchy) {
			firePool.gameObject.SetActive (false);
			healPool.gameObject.SetActive (false);
		}
	}
	
	// Update is called once per frame
	protected override void Update(){
		base.Update ();
		if (followed.gameObject.activeInHierarchy) {
			gripping = controller.getGrip ();
			//firepoolScale = master.firePool.transform.lossyScale;
			//healpoolScale = master.healPool.transform.lossyScale;
		}

		if (gripping) {
			pingArrow.localScale = Vector3.Lerp (pingArrow.localScale, arrowscale, Time.deltaTime * growspeed);
			capsule.localScale = Vector3.Lerp (capsule.localScale, Vector3.zero, Time.deltaTime * growspeed);
			//firePool.localScale = Vector3.Lerp (firePool.localScale, Vector3.zero, Time.deltaTime * growspeed);
			//healPool.localScale = Vector3.Lerp (healPool.localScale, Vector3.zero, Time.deltaTime * growspeed);
		} else {
			pingArrow.localScale = Vector3.Lerp (pingArrow.localScale, Vector3.zero, Time.deltaTime * growspeed);
			capsule.localScale = Vector3.Lerp (capsule.localScale, capsuleScale, Time.deltaTime * growspeed);
		}

	}
}
