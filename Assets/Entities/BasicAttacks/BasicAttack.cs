using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAttack : MonoBehaviour
{
    // Public getters for external scripts
    [HideInInspector]
    public int BaseDamage { get { return baseDamage; } }
    [HideInInspector]
    public float BaseFireRate { get { return baseFireRate; } }
    [HideInInspector]
    public int Damage { get { return damage; } }
    [HideInInspector]
    public float FireRate { get { return fireRate; } }

    [Header("Basic Details")]
    [SerializeField]
    protected int baseDamage = 30;      // Base damage before damage modications
    [SerializeField]
    
    protected float baseFireRate = 1f;  // Base fire rate before buff/debuff

    protected int damage;               // Updated damage after damage calculations
    protected float fireRate;           // Updated fire rate after modications

    protected bool ready = true;

    protected virtual void Start()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
    }

    protected virtual void OnValidate()
    {
        baseDamage = Mathf.Clamp(baseDamage, 0, int.MaxValue);
        baseFireRate = Mathf.Clamp(baseFireRate, 0, float.MaxValue);
    }

    public virtual void UpdateDamage(int mDamage)
    {
        damage = Mathf.Clamp(mDamage, 0, int.MaxValue);
    }

    public virtual void UpdateFireRate(float mFireRate)
    {
        fireRate = Mathf.Clamp(mFireRate, 0, float.MaxValue);
    }
    
    // TODO: Pick a better name for this function
    public virtual void DoAttack()
    {
        ready = false;
        StartCoroutine(WaitForReload());
    }

    protected virtual IEnumerator WaitForReload()
    {
        yield return new WaitForSeconds(FireRate);
        ready = true;
    }
}
