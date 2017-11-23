using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    public float SwingSpeed = 3.0f;
    public float LifeTime = 1.0f;

    private void Start()
    {
		
    }

    private void Update()
    {
        var prevRot = transform.localRotation;

        // Rotate the sword
        transform.localRotation =
            Quaternion.LerpUnclamped(
                transform.localRotation,
                Quaternion.Euler(0f, -175f, -45f),  // This is not 180 to prevent rotating behind the attacker
                SwingSpeed * Time.deltaTime);

        // If rotation is finished, disable collider to prevent further damage
		//ISSUE: inaccuracy of floats will lead to false results. also: we have the lifetime, why even check the rotation?
        if (transform.localRotation == prevRot
            && GetComponentInChildren<Collider>().enabled)
            GetComponentInChildren<Collider>().enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check for friendly fire 
        if (collision.collider.tag == gameObject.transform.parent.tag)
            return;

        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeHit(30, this.transform.position, this.transform.forward);
        }
    }
}
