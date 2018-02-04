using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour {

	// Use this for initialization
	public ParticleSystem[] systems;
	public Light[] lights;
	float[] intens;
	public bool fire;

	public float extinguishSpeed;

	void Start()
	{
		intens = new float[lights.Length];
		for(int i = 0; i< lights.Length; i++)
		{
			intens [i] = lights [i].intensity;
			lights [i].intensity = 0;
		}
	}
	// Update is called once per frame
	void Update () {
		foreach (var l in lights) {
			l.intensity = Mathf.Lerp (l.intensity, 0, Time.deltaTime*extinguishSpeed);
		}
		if (fire) {
			for(int i = 0; i< lights.Length; i++)
			{
				lights [i].intensity = intens [i];
			}
			foreach (var sys in systems) {
				sys.Play ();
			}
			fire = false;
		}
	}
}
