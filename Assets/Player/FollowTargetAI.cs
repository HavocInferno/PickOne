using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
public class FollowTargetAI : NetworkBehaviour
{
	public List<Transform> targets;
	private Transform bestTarget;
	private float closestTargetRange;

	public float scanRate;
	private float lastScan;
	public float maxScanRange;

    public float attackRange = 0.75f;
    public float attackRate = 1.0f;
    private float lastAttackTime;
    public GameObject SwordAttack;

    public LayerMask mask;

	void Start() {
		if (!isServer)
			return;

		targets = FindObjectOfType<playerlist> ().players;
	}

	void FixedUpdate ()
    {
		if (!isServer)
			return;

		//check whether last scan was at least scanrate fraction of a second ago
		if (Time.time > lastScan) {
			lastScan = Time.time + scanRate;

			/* reset bestTarget and closestRange to effectively null
			 * foreach target (i.e. crawler), do:
			 * check whether the target is in range
			 * raycast to the target
			 * if the target is "seen" directly, set it and its range as the "best" found
			*/
			bestTarget = null;
			closestTargetRange = maxScanRange + 1f;

            foreach (Transform t in targets)
            {
				float dist = (t.position - transform.position).magnitude;

                if (dist < maxScanRange)
                {
					Vector3 raycastDir = t.position - transform.position;
					RaycastHit hit;

                    if (Physics.Raycast (
						    transform.position,
						    raycastDir,
						    out hit,
						    maxScanRange,
						    mask)
					    && hit.collider.CompareTag ("Crawler")
						&& dist < closestTargetRange)
                    {
							closestTargetRange = dist;
							bestTarget = t;
					}
				}
			}

            if (closestTargetRange < attackRange
                && Time.time > lastAttackTime)
            {
                lastAttackTime = Time.time + attackRate;

				var sword = (GameObject)Instantiate(SwordAttack, transform);
				// Sword is destroyed
				Destroy(sword, SwordAttack.GetComponent<Sword>().LifeTime);
            }

            //if after all the raycasting a most suitable target is found, navigate towards it
            if (bestTarget != null) 
				GetComponent<NavMeshAgent> ().destination = bestTarget.localPosition;
		}
    }
}
