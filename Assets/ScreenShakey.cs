using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakey : MonoBehaviour {

	public List<Screenshaker> shakeybakeys = new List<Screenshaker>();

	public float cumulativeShakeyStrength;
	public float maxCumStrength;
	private Vector3 originalPos;
	private bool shaking = false;

	void Start() {
		originalPos = transform.localPosition;
	}

	void Update() {

		//calculate total shake strength
		cumulativeShakeyStrength = 0f;
		foreach (Screenshaker ss in shakeybakeys) {
			float dist = Vector3.Magnitude (transform.position - ss.origin.position);
			if(dist < ss.maxShakeDistance) {
				cumulativeShakeyStrength += ss.shakeyStrength * (1 - dist / ss.maxShakeDistance);
			}
		}
		cumulativeShakeyStrength = Mathf.Clamp (cumulativeShakeyStrength, 0f, maxCumStrength);
			
		//actual camera shake part
		if (cumulativeShakeyStrength > 0) {
			transform.localPosition += Random.insideUnitSphere * cumulativeShakeyStrength * Time.deltaTime;
		}
		transform.localPosition = Vector3.Lerp (transform.localPosition, originalPos, Time.deltaTime);
	}
}
