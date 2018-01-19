﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMasterIndicator : MonoBehaviour {

	public GameObject indicatorArrow;
	public Text indicatorText;
	public GameObject master;
	public Color arrowCol;
	public Color textCol;
	public Vector3 arrowOffset;
	public float fadeSpeed = 8f;
	public float onScreenRim = 0.2f;

	void Start() {
		arrowCol = indicatorArrow.GetComponent<Renderer> ().material.color;
		textCol = indicatorText.color;
	}

	// Update is called once per frame
	void Update () {
		Vector3 masterScreenPoint = Camera.main.WorldToViewportPoint (master.transform.position);
		bool onScreen = (masterScreenPoint.z > 0
						&& masterScreenPoint.x > (0f+onScreenRim)
						&& masterScreenPoint.x < (1f-onScreenRim) 
						&& masterScreenPoint.y > (0f+onScreenRim) 
						&& masterScreenPoint.y < (1f-onScreenRim));
		if (onScreen) {
			//indicatorArrow.GetComponent<Renderer> ().material.color = Color.Lerp (indicatorArrow.GetComponent<Renderer> ().material.color, Color.clear, Time.deltaTime * fadeSpeed);
			indicatorArrow.transform.localPosition = Vector3.Lerp (indicatorArrow.transform.localPosition, arrowOffset, Time.deltaTime * fadeSpeed);
			indicatorText.color = Color.Lerp (indicatorText.color, Color.clear, Time.deltaTime * fadeSpeed);
		} else {
			//indicatorArrow.GetComponent<Renderer> ().material.color = Color.Lerp (indicatorArrow.GetComponent<Renderer> ().material.color, arrowCol, Time.deltaTime * fadeSpeed);
			indicatorArrow.transform.localPosition = Vector3.Lerp (indicatorArrow.transform.localPosition, Vector3.zero, Time.deltaTime * fadeSpeed);
			indicatorText.color = Color.Lerp (indicatorText.color, textCol, Time.deltaTime * fadeSpeed);
		}

		indicatorArrow.transform.LookAt (master.transform);
	}
}
