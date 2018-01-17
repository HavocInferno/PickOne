using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour {

	AudioSource AS;
	public AudioClip[] sounds;

	// Use this for initialization
	void Start () {
		AS = GetComponent<AudioSource> ();

		AS.PlayOneShot (sounds [Random.Range (0, sounds.Length - 1)]);
	}
}
