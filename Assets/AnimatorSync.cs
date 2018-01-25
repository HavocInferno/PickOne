using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSync : MonoBehaviour {


	public Animator anim;
	public CrawlerController cont;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		anim.SetFloat ("Forward", cont.mov.y);
		anim.SetFloat ("Left", -cont.mov.x);
		anim.SetBool ("Attacking",cont.att);
	}
}
