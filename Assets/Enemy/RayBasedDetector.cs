using UnityEngine;

public class RayBasedDetector : EnemyTargetDetector
{
    public float minScanRange = 0.0f;
    public float maxScanRange = 1.0f;
    public LayerMask mask;

    override public bool Detect(Transform target)
    {
        Vector3 direction = target.position - enemy.transform.position;
        float distance = direction.magnitude;

        if (distance < maxScanRange)
        {
            RaycastHit hit;

            bool hitSomething = Physics.Raycast(
                    enemy.transform.position,
                    direction,
                    out hit,
                    maxScanRange,
                    mask);
            return hitSomething
                && hit.collider.transform == target
                && distance < distance + 1.0f;
        }

        return false;
    }
}
