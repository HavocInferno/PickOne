using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyDamage : MonoBehaviour {

    public int Damage = 30;

    void OnCollisionEnter(Collision collision)
    {
        // Check for friendly fire
        if (collision.collider.tag == GetComponent<Collider>().tag)
            return;

        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeHit(Damage, this.transform.position, this.transform.forward);//Vector3.Normalize(hit.transform.position - this.transform.position));
        }
    }
}
