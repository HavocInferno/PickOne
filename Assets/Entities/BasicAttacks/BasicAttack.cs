using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAttack : MonoBehaviour
{
    // Public getters for external scripts
    [HideInInspector]
    public float BaseDamage { get { return baseDamage; } }
    [HideInInspector]
    public float BaseFireRate { get { return baseFireRate; } }
    [HideInInspector]
    public float Damage { get { return damage; } }
    [HideInInspector]
    public float FireRate { get { return fireRate; } }

    [Header("Basic Details")]
    [SerializeField]
    protected float baseDamage = 30.0f;    // Base damage before damage modications
    [SerializeField]
    
    protected float baseFireRate = 1.0f;// Base fire rate before buff/debuff

    protected float damage;             // Updated damage after damage calculations
    protected float fireRate;           // Updated fire rate after modications

    protected bool ready = true;

    protected virtual void Start()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
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
    
    public virtual void DoAttack(GenericCharacter attacker)
    {
        if (!ready) return;

        PlayAnimation(attacker);
        ready = false;
        StartCoroutine(WaitForReload());
    }

    protected virtual IEnumerator WaitForReload()
    {
        yield return new WaitForSeconds(FireRate);
        ready = true;
    }

    protected void PlayAnimation(GenericCharacter character)
    {
        Animator animator = character.GetComponent<Animator>();
        if (animator != null)
        {
            Debug.LogWarning("TriggerAttack");
            animator.Play("Attack");
        }
    }
}
