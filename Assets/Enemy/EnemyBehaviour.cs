using UnityEngine;

[RequireComponent(typeof(Enemy))]
abstract public class EnemyBehaviour : MonoBehaviour
{
    protected Enemy enemy = null;

    public void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.behaviours.Add(this);
    }

    virtual public void OnDetectTarget(Transform target) { }
    virtual public void OnLoseTarget(Transform target) { }
    virtual public void OnFixedUpdate() { }
    virtual public void OnReachDestination() { }
}
