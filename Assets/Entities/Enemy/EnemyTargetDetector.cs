using UnityEngine;


[RequireComponent(typeof(Enemy))]
abstract public class EnemyTargetDetector : MonoBehaviour
{
    protected Enemy enemy = null;

    public void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.detectors.Add(this);
    }

    virtual public bool Detect(Transform target)
    {
        return false;
    }
}
