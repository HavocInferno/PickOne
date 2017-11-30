﻿using System.Collections;
using UnityEngine;

public class Sword : BasicAttack
{ 
    [Header("Sword Details")]
    public float SwingSpeed = 3.0f;
    public float LifeTime = 1.0f;

    public GameObject blade = null;

    private bool animActive = false;
	private Quaternion defaultRot;
    
    private void Start()
    {
		defaultRot = transform.localRotation;
        blade.SetActive(false);
    }

    private void Update()
    {
		if (animActive)
        {
			var prevRot = transform.localRotation;

			// Rotate the sword
			transform.localRotation =
                Quaternion.LerpUnclamped(
				    transform.localRotation,
				    Quaternion.Euler(0f, -175f, -45f),  // This is not 180 to prevent rotating behind the attacker
				    SwingSpeed * Time.deltaTime);
		}
    }

    void OnValidate()
    {
        if (blade == null)
        {
            Debug.LogError("Blade prefab not set.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check for friendly fire 
        if (collision.collider.tag == gameObject.transform.parent.tag)
            return;

        // Get health component of collision object
        var health = collision.gameObject.GetComponent<Health>();

        // If it has one, call function to take damage
        if (health != null)
        {
            health.TakeHit(Damage, transform.position, transform.forward);
        }
    }

    public override void DoAttack()
    {
		transform.localRotation = defaultRot;
		animActive = true;
		StartCoroutine(AttackRoutine());
        blade.SetActive(true);
	}

	IEnumerator AttackRoutine()
    {
		yield return new WaitForSeconds(LifeTime);
        blade.SetActive(false);
        animActive = false;
	}
}
