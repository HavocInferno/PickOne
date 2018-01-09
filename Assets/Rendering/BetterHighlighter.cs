using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterHighlighter : MonoBehaviour {

	public Material baseMat;
	public Material highlightMat;
	public Renderer target;

	// Use this for initialization
	void Start () {
		target.material = baseMat;
	}
	
	public void EnableHL() {
		target.material = highlightMat;
	}

	public void DisableHL() {
		target.material = baseMat;
	}
}
