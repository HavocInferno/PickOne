using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic attack for the sumo character damages multiple enemies in
/// a cone in front of the player.
/// </summary>
public class SumoSlap : BasicAttack
{ 
    public Collider attackCollider;
    public float colliderTime, animTime;
    public new ParticleSystem particleSystem;

	public Animator animator;
    
    //GenericCharacter _attacker;
    
    protected override void Start()
    {
        base.Start();

        if(!attackCollider)
        {
            Debug.LogErrorFormat("{0} does not have a collider set.", name);
        }

        attackCollider.enabled = false;

        if (particleSystem)
            particleSystem.Stop();
    }

    public override void DoAttack(GenericCharacter attacker)
    {
        // Default checks before attacking
        if (!_ready) return;

        //_ready = false;
        //_attacker = attacker;
        //selfAS.PlayOneShot(sound);
        //StartCoroutine(AttackRoutine());
        base.DoAttack(attacker);
    }

    protected void OnCollisionEnter(Collision other)
    {
        // Check for friendly fire
        if (other.collider.tag == gameObject.transform.parent.tag)
            return;

        // Get health component of collision object
        var stats = other.gameObject.GetComponent<Stats>();

        // If it has one, call function to take damage
        if (stats != null)
        {
            stats.Hit(damage, _attacker, transform.position,
                (other.transform.position - _attacker.transform.position).normalized);
        }
        else
        {
            if (!other.collider.CompareTag("Untagged"))
                Debug.LogWarning("On " + other.collider.tag + " stats were not found.");
        }
    }

    protected override IEnumerator AttackRoutine()
    {
        // Enable the collider in front of the crawler
		if (animator != null)
			animator.SetTrigger ("SumoSlap");
		yield return new WaitForSeconds(animTime);
        attackCollider.enabled = true;
		if (selfAS != null && sound != null)
			selfAS.PlayOneShot(sound);
        if (particleSystem)
        {
            particleSystem.Play();
        }

        yield return new WaitForSeconds(colliderTime);

        attackCollider.enabled = false;

        yield return new WaitForSeconds(_fireRate - colliderTime- animTime);
        _ready = true;
	}
}
