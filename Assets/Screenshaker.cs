using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshaker : MonoBehaviour {

	public float duration;
	public bool destroyAfterDuration;
	public Transform origin;
	public float shakeyStrength;
	float shakenibba;
	public float maxShakeDistance;

	void Start() {
		shakenibba = shakeyStrength;
		FindObjectOfType<ScreenShakey> ().shakeybakeys.Add (this);

		if (duration > 0) {
			if (destroyAfterDuration)
				Destroy (this, duration);
		}
	}

	void Update() {
		shakeyStrength -= shakenibba * Time.deltaTime/duration;
	}

	void OnDestroy() {
		FindObjectOfType<ScreenShakey> ().shakeybakeys.Remove(this);
	}
}
