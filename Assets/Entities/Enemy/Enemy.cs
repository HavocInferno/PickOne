using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : GenericCharacter
{
    public List<EnemyBehaviour> behaviours = new List<EnemyBehaviour>();
    public List<EnemyTargetDetector> detectors = new List<EnemyTargetDetector>();
    public List<Transform> possibleTargets = new List<Transform>();
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
            Debug.LogFormat("{0} detected {1}", name, target.name);
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
            Debug.LogFormat("{0} lost {1}", name, target.name);
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

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (isServer)
        {
            // On the server, add yourself to the level-wide enemies list.
            Debug.Log("SERVER: " + gameObject.name + " is here.");
            FindObjectOfType<PlayersManager>().enemies.Add(transform);

            if (isEndConditionKill)
                FindObjectOfType<EndConditions>().enemiesToKill.Add(this);
        }
    }

    protected override void Start()
    {
        // Populate the list of possible targets for detection.
        foreach (var crawler in FindObjectOfType<PlayersManager>().players)
        {
            possibleTargets.Add(crawler);
        }
    }
	
	void FixedUpdate()
    {
        if (!isServer)
            return;

        // Update enemy AI.
        foreach (var behaviour in behaviours)
        {
            behaviour.OnFixedUpdate();
        }

        // Check if the enemy is idle.
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

        // Perform detection checks against all possible targets.
        foreach (var target in possibleTargets)
        {
            bool detected = detectors.Any((detector) => detector.Detect(target));
            if (detected)
            {
                target.GetComponent<DetectableObject>().detectedBy.Add(this);
                DetectTarget(target);
            }
            else
            {
                target.GetComponent<DetectableObject>().detectedBy.Remove(this);
                LoseTarget(target);
            }
        }
    }

    public void Attack(Transform target)
    {
        RpcAttack();
    }
}
