using UnityEngine;

[RequireComponent(typeof(Enemy))]
abstract public class EnemyBehaviour : MonoBehaviour
{
    public enum Priority
    {
        Low = 1000,
        Medium = 2000,
        High = 3000
    }

    protected Enemy enemy = null;

    public void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.behaviours.Add(this);
    }

    virtual public void OnDetectTarget(Transform target) { }
    virtual public void OnLoseTarget(Transform target) { }
    virtual public void OnUpdate() { }
    virtual public void OnReachDestination() { }
}
