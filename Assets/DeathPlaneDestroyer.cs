using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlaneDestroyer : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.SetActive(false);
        Destroy(collision.gameObject);
    }
}
