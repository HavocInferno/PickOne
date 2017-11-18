using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

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