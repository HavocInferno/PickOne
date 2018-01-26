using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRendererAlphaHelper : MonoBehaviour {

	public float startCanvasRAlpha;
	public Color startColor;

	// Use this for initialization
	void Start () {
		GetComponent<Image> ().canvasRenderer.SetAlpha (startCanvasRAlpha);
		GetComponent<Image> ().color = startColor;
	}
}
