using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBasedAttack : BasicAttack
{
    //GenericCharacter _attacker;

    protected override void Start()
    {
        base.Start();
    }

    protected void OnCollisionEnter(Collision collision)
    {
        // GetComponent<Collider>().enabled = false;

        // Check for friendly fire 
        if (collision.collider.tag == gameObject.transform.parent.tag)
            return;

        // Get health component of collision object
        var stats = collision.gameObject.GetComponent<Stats>();

        // If it has one, call function to take damage
        if (stats != null)
        {
            stats.Hit(damage, _attacker, transform.position,
                (collision.transform.position - _attacker.transform.position).normalized);
            GetComponent<Collider>().enabled = false;
        }
        else
        {
            if (!collision.collider.CompareTag("Untagged"))
                Debug.LogWarning("On " + collision.collider.tag + " stats were not found.");
        }
    }

    public override void DoAttack(GenericCharacter attacker)
    {
        if (!_ready) return;

        //_ready = false;
        //_attacker = attacker;
        //PlayAnimation(attacker);
        //StartCoroutine(AttackRoutine());

        base.DoAttack(attacker);

        GetComponent<Collider>().enabled = true;
    }

    protected override IEnumerator AttackRoutine()
    {
        yield return base.AttackRoutine();

        GetComponent<Collider>().enabled = false;
        
        //_ready = true;
    }
}