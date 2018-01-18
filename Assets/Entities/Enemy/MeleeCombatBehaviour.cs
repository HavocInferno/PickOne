using UnityEngine;

public class MeleeCombatBehaviour : EnemyBehaviour
{
    public float attackRange = 1.0f;
    public float attackRate = 1.0f;
    public float updateRate = 1.0f;
    private float lastUpdate = 0.0f;
    private float lastAttackTime = 0.0f;
    private Transform currentTarget = null;

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Time.time <= lastUpdate)
            return;
        lastUpdate = Time.time + updateRate;

        if (currentTarget != null)
        {
            enemy.SetDestination(currentTarget.position, (int)Priority.Medium);
            float distance = Vector3.Distance(currentTarget.position, enemy.transform.position);
            if (distance < attackRange
                && Time.time > lastAttackTime)
            {
                lastAttackTime = Time.time + attackRate;
                enemy.Attack(currentTarget);
            }
        }
    }

    public override void OnDetectTarget(Transform target)
    {
        base.OnDetectTarget(target);
        currentTarget = enemy.GetClosestDetectedTarget();
    }

    public override void OnLoseTarget(Transform target)
    {
        base.OnLoseTarget(target);
        if (currentTarget == target)
            currentTarget = enemy.GetClosestDetectedTarget();
    }
}