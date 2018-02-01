using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSync : MonoBehaviour {


	public CrawlerController controller;
	public Animator animator;

	public bool melee = false;

	public bool rightHand = false;
	public bool leftHand = false;
	public bool head = false;

	public float leftHandWeight = 1;
	public float rightHandWeight = 1;
	public float headWeight = 1;

	public Transform rightHandObj = null;
	public Transform leftHandObj = null;
	public Transform lookObj = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		animator.SetFloat ("Forward", controller.mov.y);
		animator.SetFloat ("Left", -controller.mov.x);
		if(melee)
			animator.SetBool ("Attacking",controller.att);
	}

	public void takeDamage()
	{
		animator.SetTrigger ("Hit");
	}
	void OnAnimatorIK()
	{
		if (animator)
		{

			//if the IK is active, set the position and rotation directly to the goal. 
			// Set the look target position, if one has been assigned
			if (head && lookObj != null) {
				animator.SetLookAtWeight (1);
				animator.SetLookAtPosition (lookObj.position);
			} else {
				animator.SetLookAtWeight(0);
			}
			// Set the right hand target position and rotation, if one has been assigned
			if (rightHand && rightHandObj != null)
			{
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
				animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
				animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
			}			
			else
			{
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
			}

			if (leftHand && leftHandObj != null)
			{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
				animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
				animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
			}
			else
			{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, headWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, headWeight);
			}

		}
	}
}
