using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    // Set by the corresponding gun:
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public GenericCharacter attacker;

    //if the bullet hits another object that has a health component, deal 10 dmg to it and send the position and direction of itself at time of impact (used for death effect setup, not optimal yet)
    void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
		var stats = hit.GetComponent<Stats>();
		if (stats != null)
		{
            Debug.Log(damage);
			stats.Hit(damage, attacker, this.transform.position, this.transform.forward);
		}

		Destroy(gameObject);
	}
}