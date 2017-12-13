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
	}
    private void Update()
    {
        if (lit != null)
            lit.intensity -= lit.intensity*(Time.deltaTime/lifetime*dimSpeed);
    }
}
