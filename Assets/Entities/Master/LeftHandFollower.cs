using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LeftHandFollower : MasterFollower {

	// Use this for initialization

	public Controller MainController;
	public Controller OffController;
	public Master master;
	public Transform pingArrow, capsule, healPool, firePool;
	public float growspeed =20;
	public Vector3 arrowscale, capsuleScale;
	public ParticleAttractor healA, fireA;
	public AudioSource fireSound, healSound, ping;

	[SyncVar]
	public Vector3 firepoolScale, healpoolScale;
	[SyncVar]
	bool gripping = false;
	[SyncVar]
	bool healAtt, fireAtt;
	[SyncVar]
	int currentItem;

	void Start () {
		arrowscale = pingArrow.localScale;
		capsuleScale = capsule.localScale;
		healAtt = fireAtt = false;
	}
	
	// Update is called once per frame
	protected override void Update(){
		base.Update ();
		if (followed.gameObject.activeInHierarchy) {
			gripping = OffController.getGrip ();
			firepoolScale = master.firePool.transform.localScale;
			healpoolScale = master.healPool.transform.localScale;
			healAtt = master.healOrbAtt.attracting;
			fireAtt = master.fireAtt.attracting;
			currentItem = MainController.currentItem;
			healPool.gameObject.SetActive (false);
			firePool.gameObject.SetActive (false);
		} else {
			fireA.attracting = fireAtt;
			healA.attracting = healAtt;

			if (healPool.localScale.magnitude < 0.01)
				healPool.gameObject.SetActive (false);
			else
				healPool.gameObject.SetActive (true);

			if (firePool.localScale.magnitude < 0.01)
				firePool.gameObject.SetActive (false);
			else
				firePool.gameObject.SetActive (true);
		}

		fireSound.enabled = (firepoolScale.magnitude > 0.15);
		fireSound.pitch = (float) (0.5+0.5*(1- Mathf.Clamp01(firepoolScale.magnitude)));
		fireSound.volume = (float) Mathf.Clamp01(firepoolScale.magnitude);
		healSound.enabled = (healpoolScale.magnitude > 0.15);
		healSound.pitch = (float) (0.5+0.5*(1- Mathf.Clamp01(healpoolScale.magnitude)));
		healSound.volume = (float) Mathf.Clamp01(healpoolScale.magnitude);

		if (gripping) {
			pingArrow.localScale = Vector3.Lerp (pingArrow.localScale, arrowscale, Time.deltaTime * growspeed);
			capsule.localScale = Vector3.Lerp (capsule.localScale, Vector3.zero, Time.deltaTime * growspeed);
			firePool.localScale = Vector3.Lerp (firePool.localScale, Vector3.zero, Time.deltaTime * growspeed);
			healPool.localScale = Vector3.Lerp (healPool.localScale, Vector3.zero, Time.deltaTime * growspeed);
		} else {
			pingArrow.localScale = Vector3.Lerp (pingArrow.localScale, Vector3.zero, Time.deltaTime * growspeed);

			//firepool grows
			if (currentItem == 2) {
				capsule.localScale = Vector3.Lerp (capsule.localScale, Vector3.zero, Time.deltaTime * growspeed);
				firePool.localScale = Vector3.Lerp (firePool.localScale, firepoolScale, Time.deltaTime * growspeed);
				healPool.localScale = Vector3.Lerp (healPool.localScale, Vector3.zero, Time.deltaTime * growspeed);
			} else if (currentItem == 3) {
				capsule.localScale = Vector3.Lerp (capsule.localScale, Vector3.zero, Time.deltaTime * growspeed);
				firePool.localScale = Vector3.Lerp (firePool.localScale, Vector3.zero, Time.deltaTime * growspeed);
				healPool.localScale = Vector3.Lerp (healPool.localScale, healpoolScale, Time.deltaTime * growspeed);
			} else {
				capsule.localScale = Vector3.Lerp (capsule.localScale, capsuleScale, Time.deltaTime * growspeed);			
				firePool.localScale = Vector3.Lerp (firePool.localScale, Vector3.zero, Time.deltaTime * growspeed);
				healPool.localScale = Vector3.Lerp (healPool.localScale, Vector3.zero, Time.deltaTime * growspeed);
			}
		}
		ping.enabled = (pingArrow.localScale.magnitude > 0.01);


	}
}
