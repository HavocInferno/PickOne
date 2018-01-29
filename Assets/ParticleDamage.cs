using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    public BasicAttack attack;
    public GenericCharacter attacker;

    void OnParticleCollision(GameObject other)
    {
        if (other == attacker)
            return;

        // Get health component of collision object
        var stats = other.GetComponent<Stats>();

        // If it has one, call function to take damage
        if (stats != null)
        {
            stats.Hit(attack.damage, attacker, transform.position,
                (other.transform.position - transform.position).normalized);
        }
        else
        {
            if (!other.CompareTag("Untagged"))
                Debug.LogWarning("On " + other.tag + " stats were not found.");
        }

        Rigidbody body = other.GetComponent<Rigidbody>();
        if (body)
        {
            Vector3 direction = other.transform.position - transform.position;
            direction = direction.normalized;
            body.AddForce(direction * 5);
        }
    }
}