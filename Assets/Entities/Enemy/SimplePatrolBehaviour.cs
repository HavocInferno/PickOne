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
    private bool returnsBack = false;

    public enum Type
    {
        Once,
        Return,
        Loop,
    }

    [SerializeField]
    public Type type = Type.Loop;

    public override void OnUpdate()
    {
        base.OnUpdate();

		if (waits && Time.time > nextMovementTime && points.Count > 0)
        {
            enemy.SetDestination(points[nextPointIndex].position, (int)Priority.Low);
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
            if (type == Type.Loop)
                nextPointIndex = (nextPointIndex + 1) % points.Count;
            else if (type == Type.Return)
            {
                if (nextPointIndex == 0 && returnsBack)
                    returnsBack = false;
                if (nextPointIndex == points.Count - 1 && !returnsBack)
                    returnsBack = true;
                nextPointIndex += returnsBack ? -1 : +1;
            }
            else if (type == Type.Once)
            {
                if (nextPointIndex < points.Count - 1)
                    nextPointIndex++;
            }
            else
                Debug.LogError("SimplePatrolBehaviour | Unknown patrol type");
            nextMovementTime = Time.time + Random.Range(delayMin, delayMax);
        }
        else
        {
            nextMovementTime = Time.time + delayOutOfPath;
        }
        waits = true;
    }
}