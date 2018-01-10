using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterHighlighter : MonoBehaviour
{

	public Material baseMat;
	public Material highlightMat;
	public Renderer target;

	public int matIndex = 0;

	// Use this for initialization
	void Start ()
    {
		target.materials [matIndex] = baseMat;
	}
	
	public void EnableHL()
    {
		target.materials [matIndex] = highlightMat;
	}

	public void DisableHL() {
		target.materials [matIndex] = baseMat;
	}
}
