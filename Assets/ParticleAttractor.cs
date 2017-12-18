using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttractor : MonoBehaviour {
   
    public Transform goal;
    public bool attracting = false;
    public float acceleration = 20;

    ParticleSystem part;
    ParticleSystem.Particle[] particles;
    int numParticlesAlive;
    // Use this for initialization
    void Start () {
        part = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[part.main.maxParticles];
    }
	
	// Update is called once per frame
	void Update () {
        if (attracting)
        {
            numParticlesAlive = part.GetParticles(particles);
            // Change only the particles that are alive
            for (int i = 0; i < numParticlesAlive; i++)
            {
                particles[i].velocity += (goal.position - particles[i].position).normalized * (goal.position - particles[i].position).magnitude * acceleration*Time.deltaTime;
                //  Debug.Log((goal.position - particles[i].position).normalized * acceleration);
                if ((goal.position - particles[i].position).magnitude < 1)
                    particles[i].remainingLifetime *= 1-Mathf.Clamp((Time.deltaTime),0,1);
                else
                    particles[i].remainingLifetime += Time.deltaTime*0.75f;
            }
            // Apply the particle changes to the particle system
            part.SetParticles(particles, numParticlesAlive);
        }

    }
}
