using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MasterHeadFollower : MasterFollower {

	// Use this for initialization
	[SerializeField]
	private Transform left, right;
	[SerializeField]
	private Light leftLight, rightLight;
	[SerializeField]
	float blinkMin = 2, blinkMax = 5, blinkTime = 0.1f;
	[SerializeField]
	float singleWeight = 5, doubleWeight =2;


	void Start () {
		StartCoroutine (blink()); 
	}
	
	// Update is called once per frame
	void Update () {
		base.Update();

	}

	IEnumerator blink()
	{
		float eyeScaleY = left.localScale.y;
		float intensity = leftLight.intensity;

		while (true) {
			yield return new WaitForSeconds (Random.Range(blinkMin, blinkMax));
			if (Random.Range (0, singleWeight + doubleWeight) < singleWeight) {
				//simple blink
				while (left.localScale.y > 0) {
					yield return new WaitForEndOfFrame ();
					changeEyes (-eyeScaleY, -intensity);
				}
				setEyes (0,0);

				while (left.localScale.y < eyeScaleY) {
					yield return new WaitForEndOfFrame ();
					changeEyes (eyeScaleY, intensity);
				}
				setEyes (eyeScaleY,intensity);
			} else {
				//double blink
				while (left.localScale.y > 0) {
					yield return new WaitForEndOfFrame ();
					changeEyes (-eyeScaleY, -intensity);
				}
				setEyes (0,0);

				while (left.localScale.y < eyeScaleY) {
					yield return new WaitForEndOfFrame ();
					changeEyes (eyeScaleY, intensity);
				}
				setEyes (eyeScaleY,intensity);				

				while (left.localScale.y > 0) {
					yield return new WaitForEndOfFrame ();
					changeEyes (-eyeScaleY, -intensity);
				}
				setEyes (0,0);

				while (left.localScale.y < eyeScaleY) {
					yield return new WaitForEndOfFrame ();
					changeEyes (eyeScaleY, intensity);
				}
				setEyes (eyeScaleY,intensity);
			}
		}
	}

	void changeEyes (float scaleChange, float intensityChange)
	{
		left.localScale += new Vector3(0,(((Time.deltaTime) * (scaleChange)) / blinkTime),0) ;
		right.localScale += new Vector3(0,(((Time.deltaTime) * (scaleChange)) / blinkTime),0) ;
		leftLight.intensity += ((Time.deltaTime) * (intensityChange)) / blinkTime;
		rightLight.intensity += ((Time.deltaTime) * (intensityChange)) / blinkTime;
	}

	void setEyes (float scale, float intensity)
	{
		left.localScale = new Vector3(left.localScale.x,scale,left.localScale.z) ;
		right.localScale = new Vector3(right.localScale.x,scale,right.localScale.z) ;
		leftLight.intensity = intensity;
		rightLight.intensity = intensity;
	}
}
