﻿using System.Collections;
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
		if(FindObjectOfType<ScreenShakey> ())
			FindObjectOfType<ScreenShakey> ().shakeybakeys.Add (this);
		if(FindObjectOfType<DungeonCamera> ())
			FindObjectOfType<DungeonCamera> ().shakeybakeys.Add (this);

		if (duration > 0) {
			if (destroyAfterDuration)
				Destroy (this, duration);
		}
	}

	void Update() {
		shakeyStrength -= shakenibba * Time.deltaTime/duration;
	}

	void OnDestroy() {
		if(FindObjectOfType<ScreenShakey> ())
			FindObjectOfType<ScreenShakey> ().shakeybakeys.Remove (this);
		if(FindObjectOfType<DungeonCamera> ())
			FindObjectOfType<DungeonCamera> ().shakeybakeys.Remove (this);
	}
}
