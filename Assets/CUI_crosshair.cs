using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_crosshair : MonoBehaviour {

	public Transform tCrawler;
	public Crawler cCrawler;
	public Camera cam; 

	void Start() {
		cam = Camera.main;
	}

	void Update() {
		if (tCrawler) {
			cam = Camera.main;
			Vector3 screenPos = cam.WorldToScreenPoint (tCrawler.position + tCrawler.forward * 15f);
			GetComponent<RectTransform> ().position = screenPos;
		}
	}

	public void registerCrawler(Crawler crlr) {
		cCrawler = crlr;
		tCrawler = crlr.transform;
	}
}
