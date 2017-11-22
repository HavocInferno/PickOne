using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    public float SwingSpeed = 3.0f;
    
    private void Update()
    {
        // Rotate the sword
        gameObject.transform.rotation =
            Quaternion.LerpUnclamped(
                gameObject.transform.rotation,
                Quaternion.Euler(180f, 180f, 90f),
                SwingSpeed * Time.deltaTime);
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
            health.TakeHit(30, this.transform.position, this.transform.forward);//Vector3.Normalize(hit.transform.position - this.transform.position));
        }
    }
}
