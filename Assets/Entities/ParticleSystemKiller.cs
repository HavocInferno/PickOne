using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemKiller : MonoBehaviour {

    public bool dimming = true;
    public float dimSpeed = 3;
    Light lit;
    float lifetime;
	// Use this for initialization
	void Start () {
        ParticleSystem part = gameObject.GetComponent<ParticleSystem>();
        if (part == null)
            Destroy(this);
        lifetime = Mathf.Max(part.main.startLifetime.constantMax, part.main.duration);
        Destroy(gameObject, lifetime);
        lit = GetComponent<Light>();
		foreach (var s in GetComponents<AudioSource>()) {
			s.volume = (float)Mathf.Clamp01 (transform.localScale.magnitude/3f);
			s.pitch *= (float)(0.5+0.5*(1- Mathf.Clamp01 (transform.localScale.magnitude/3f)));
		}
	}
    private void Update()
    {
        if (lit != null)
            lit.intensity -= lit.intensity*(Time.deltaTime/lifetime*dimSpeed);
		foreach (var s in GetComponents<AudioSource>()) {
			s.volume -= s.volume*(Time.deltaTime/lifetime*dimSpeed);
		}

	}
}
