using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FollowTargetAI : MonoBehaviour
{
    public Transform target;
    public LayerMask mask;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 raycastDir = target.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(
            transform.position,
            raycastDir,
            out hit,
            raycastDir.magnitude,
            mask))
        {
            // Debug.Log("Occluded");
        }
        else
        {
            Debug.Log("Not Occluded");
            GetComponent<NavMeshAgent>().destination = target.localPosition;
        }
    }
}
