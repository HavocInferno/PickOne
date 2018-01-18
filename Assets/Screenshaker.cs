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

    void Start()
    {
        ScreenShakey screenShakey = Camera.main.GetComponent<ScreenShakey>();
        DungeonCamera dungeonCamera = Camera.main.GetComponent<DungeonCamera>();
        shakenibba = shakeyStrength;
		if (screenShakey) screenShakey.shakeybakeys.Add(this);
		if (dungeonCamera) dungeonCamera.shakeybakeys.Add(this);

		if (duration > 0) {
			if (destroyAfterDuration)
				Destroy (this, duration);
		}
	}

	void Update()
    {
		shakeyStrength -= shakenibba * Time.deltaTime / duration;
	}

	void OnDestroy()
    {
        ScreenShakey screenShakey = Camera.main.GetComponent<ScreenShakey>();
        DungeonCamera dungeonCamera = Camera.main.GetComponent<DungeonCamera>();

        if (screenShakey) screenShakey.shakeybakeys.Remove(this);
		if (dungeonCamera) dungeonCamera.shakeybakeys.Remove(this);
	}
}
