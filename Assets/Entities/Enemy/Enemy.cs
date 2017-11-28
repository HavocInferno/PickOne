using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : NetworkBehaviour
{
    public List<EnemyBehaviour> behaviours = new List<EnemyBehaviour>();
    public List<EnemyTargetDetector> detectors = new List<EnemyTargetDetector>();
    public List<Transform> possibleTargets = new List<Transform>();
    public Sword sword;
    private List<Transform> detectedTargets = new List<Transform>();
    private float lastDetectionCheck = 0.0f;
    public float detectionCheckRate = 1.0f;

	public bool isEndConditionKill = true;

    public ReadOnlyCollection<Transform> DetectedTargets
    {
        get { return detectedTargets.AsReadOnly(); }
    }

    public Vector3 Destination
    {
        get { return GetComponent<NavMeshAgent>().destination; }
        set { GetComponent<NavMeshAgent>().destination = value; }
    }

    public void DetectTarget(Transform target)
    {
        if (!detectedTargets.Contains(target))
        {
            detectedTargets.Add(target);
            foreach (var behaviour in behaviours)
            {
                behaviour.OnDetectTarget(target);
            }
        }
    }

    private void LoseTarget(Transform target)
    {
        if (detectedTargets.Remove(target))
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.OnLoseTarget(target);
            }
        }
    }

    public Transform GetClosestDetectedTarget()
    {
        Transform closestTarget = null;
        float minDistance = 0.0f;
        foreach (var target in detectedTargets)
        {
            float distance = Vector3.Dot(transform.position, target.position);
            if (closestTarget == null || minDistance > distance)
            {
                minDistance = distance;
                closestTarget = target;
            }
        }
        return closestTarget;
    }

	void Start()
    {
        if (!isServer)
        {
            sword.blade.GetComponent<Collider>().enabled = false;
            return;
        }

        possibleTargets = FindObjectOfType<PlayersManager>().players;

        //on the server, add yourself to the level-wide enemies list
        Debug.Log("SERVER: " + gameObject.name + " is here.");
        FindObjectOfType<PlayersManager>().enemies.Add(transform);
		if(isEndConditionKill)
			FindObjectOfType<EndConditions>().enemiesToKill.Add(this);
    }
	
	void FixedUpdate()
    {
        if (!isServer)
            return;

        foreach (var behaviour in behaviours)
        {
            behaviour.OnFixedUpdate();
        }

        var navMeshAgent = GetComponent<NavMeshAgent>();
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    foreach (var behaviour in behaviours)
                    {
                        behaviour.OnReachDestination();
                    }
                }
            }
        }

        if (Time.time <= lastDetectionCheck)
            return;

        lastDetectionCheck = Time.time + detectionCheckRate;
            
        foreach (var target in possibleTargets)
        {
            bool detected = detectors.Any((detector) => detector.Detect(target));
            if (detected)
            {
                DetectTarget(target);
            }
            else
            {
                LoseTarget(target);
            }
        }
    }

    public void Attack(Transform target)
    {
        RpcAttack();
    }

    [ClientRpc]
    void RpcAttack()
    {
        sword.DoAttack(30);
    }
}
