using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : GenericCharacter
{
    public List<EnemyBehaviour> behaviours = new List<EnemyBehaviour>();
    public List<EnemyTargetDetector> detectors = new List<EnemyTargetDetector>();
    public List<Transform> possibleTargets = new List<Transform>();
    private List<Transform> detectedTargets = new List<Transform>();
    private float lastDetectionCheck = 0.0f;
    private int currentPriority = 0;
    public float detectionCheckRate = 1.0f;
    public float rotationSpeed = 240.0f;
    public float meleeRange = 10.0f;
    private PlayersManager playersManager = null;
    public bool isRunning = false;

    public bool isEndConditionKill = true;

    public ReadOnlyCollection<Transform> DetectedTargets
    {
        get { return detectedTargets.AsReadOnly(); }
    }

    private Vector3 Destination
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
	
    public void SetDestination(Vector3 point, int priority)
    {
        if (priority <= currentPriority)
            return;
        Destination = point;
    }

    private bool RotateTowards(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
		if(direction.magnitude < 1e-06f) return false;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, lookRotation);
        if (angle < 1e-06f) return false;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        return true;
    }

    void Update()
    {
        if (!isServer)
            return;

        if (playersManager == null)
            playersManager = FindObjectOfType<PlayersManager>();

        // Update enemy AI.
        foreach (var behaviour in behaviours)
        {
            behaviour.OnUpdate();
        }
        
        var navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && !RotateTowards(Destination))
        {
            if (isRunning)
            {
                isRunning = false;
                RpcSetRunningAnimation(false);
            }
        }
        else
        {
            if (!isRunning)
            {
                isRunning = true;
                RpcSetRunningAnimation(true);
            }
        }

        // Check if the enemy is idle.
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                currentPriority = 0;
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
		foreach (var target in playersManager.players)
        {
			if (target == null)
				continue;
            bool detected = detectors.Any((detector) => detector.Detect(target));
            DetectableObject comp = target.GetComponent<DetectableObject>();
            if (detected)
            {
                if (comp) comp.detectedBy.Add(this);
                DetectTarget(target);
            }
            else
            {
                if (comp) comp.detectedBy.Remove(this);
                LoseTarget(target);
            }
        }
    }

    [ClientRpc]
    void RpcSetRunningAnimation(bool on)
    {
        var animator = GetComponent<Animator>();
        if (animator != null) animator.SetBool("IsRunning", on);
    }

    public void Attack(Transform target)
    {
        RpcAttack();
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        if (isServer)
            FindObjectOfType<EndConditions>().MarkEnemyKilled(gameObject.GetComponent<Enemy>());

        NetworkServer.Destroy(gameObject);
    }

    /*
    public override void OnReceiveDamage(
        float amount,
        GenericCharacter attacker,
        Vector3 hitPoint,
        Vector3 hitDirection)
    {
        base.OnReceiveDamage(amount, attacker, hitPoint, hitDirection);

        if (!isServer)
            return;
        DetectTarget(attacker.transform);
    }
    */
}
