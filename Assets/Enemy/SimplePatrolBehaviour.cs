using System.Collections.Generic;
using UnityEngine;

public class SimplePatrolBehaviour : EnemyBehaviour
{
    public List<Transform> points = new List<Transform>();
    public int nextPointIndex = 0;
    public float delayMin = 0.0f;
    public float delayMax = 10.0f;
    public float delayOutOfPath = 60.0f;
    public float threshold = 1.0f;

    private float nextMovementTime = 0.0f;
    private bool waits = true;

    public enum Type
    {
        Once,
        Return,
        Loop,
    }

    [SerializeField]
    public Type type = Type.Loop;

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (waits && Time.time > nextMovementTime)
        {
            enemy.Destination = points[nextPointIndex].position;
            waits = false;
        }
    }

    public override void OnReachDestination()
    {
        base.OnReachDestination();

        if (waits || enemy.DetectedTargets.Count > 0)
            return;

        if (Vector3.Distance(enemy.transform.position,
            points[nextPointIndex].position) < threshold)
        {
            nextPointIndex = (nextPointIndex + 1) % points.Count;
            nextMovementTime = Time.time + Random.Range(delayMin, delayMax);
        }
        else
        {
            nextMovementTime = Time.time + delayOutOfPath;
        }
        waits = true;
    }
}