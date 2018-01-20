using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveInfoHelper : MonoBehaviour {

	public float lifetime;
	RectTransform selfRT;
	bool hide;

	// Use this for initialization
	void Start () {
		selfRT = GetComponent<RectTransform> ();
		StartCoroutine (disableAfter (lifetime));
	}

	void Update() {
		if (!hide)
			return;

		selfRT.anchoredPosition = new Vector2(selfRT.anchoredPosition.x, Mathf.Lerp (selfRT.anchoredPosition.y, 200f, Time.deltaTime * 3f));
	}

	IEnumerator disableAfter(float lifetime) {
		yield return new WaitForSeconds (lifetime);
		hide = true;
		yield return new WaitForSeconds (5f);
		gameObject.SetActive (false);
	}
}
