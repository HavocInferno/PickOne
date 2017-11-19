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

	public LayerMask mask;

	void Start() {
		if (!isServer)
			return;

		targets = FindObjectOfType<playerlist> ().players;
		/*targets.Clear ();
		if (targets.Count == 0) {
			foreach (CrawlerController cc in FindObjectsOfType<CrawlerController> ()) {
				if(!cc.isVRMasterPlayer)
					targets.Add (cc.gameObject.transform);
			}
		}*/
	}

	void FixedUpdate ()
    {
		if (!isServer)
			return;

		if (Time.time > lastScan) {
			lastScan = Time.time + scanRate;

			bestTarget = null;
			closestTargetRange = maxScanRange + 1f;
			foreach (Transform t in targets) {
				float dist = (transform.position - t.position).magnitude;
				if (dist < maxScanRange) {
					Vector3 raycastDir = t.position - transform.position;
					RaycastHit hit;
					if (Physics.Raycast (
						    transform.position,
						    raycastDir,
						    out hit,
							maxScanRange,
							mask) 
						&& hit.collider.CompareTag("Crawler")) 
					{
						//Debug.Log("In sight: " + hit.collider.gameObject.name);
						if (dist < closestTargetRange) {
							closestTargetRange = dist;
							bestTarget = t;
						}
					} else {
						//Debug.Log ("No crawler in sight");
					}
				}
			}
			if(bestTarget != null) 
				GetComponent<NavMeshAgent> ().destination = bestTarget.localPosition;
		}
    }
}
