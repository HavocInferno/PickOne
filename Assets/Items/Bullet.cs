using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	//if the bullet hits another object that has a health component, deal 10 dmg to it and send the position and direction of itself at time of impact (used for death effect setup, not optimal yet)
	void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
		var health = hit.GetComponent<Health>();
		if (health != null)
		{
			health.TakeHit (10, this.transform.position, this.transform.forward);//Vector3.Normalize(hit.transform.position - this.transform.position));
		}

		Destroy(gameObject);
	}
}