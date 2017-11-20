using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Masterfollower : NetworkBehaviour {

    public Transform followed;
	// Use this for initialization
	void Start () {   
	}
	
	// Update is called once per frame
	void Update () {
        if (followed.gameObject.activeInHierarchy)
        {
            transform.position = followed.position;
            transform.rotation = followed.rotation;
            this.GetComponent<Renderer>().enabled = false;
        }
    }
}
