using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsPanel : MonoBehaviour {

	public GameObject[] panels = new GameObject[3];
	private int current = 0;

	void Start() {
		panels [0].SetActive (true);
		panels [1].SetActive (false);
		panels [2].SetActive (false);
	}

	public void Prev() {
		if (current > 0) {
			panels [current].SetActive (false);
			current--;
			panels [current].SetActive (true);
		}
		
	}
	public void Next() {
		if (current < 2) {
			panels [current].SetActive (false);
			current++;
			panels [current].SetActive (true);
		}
	}
}
