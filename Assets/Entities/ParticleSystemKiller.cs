using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemKiller : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ParticleSystem part = gameObject.GetComponent<ParticleSystem>();
        if (part == null)
            Destroy(this);
        Destroy(gameObject, part.main.startLifetime.constantMax + part.main.duration);
	}
}
