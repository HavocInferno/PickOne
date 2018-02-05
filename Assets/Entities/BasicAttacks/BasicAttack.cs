using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAttack : MonoBehaviour
{
    // Public getters for external scripts
    [HideInInspector]
    public float baseDamage { get { return _baseDamage; } }
    [HideInInspector]
    public float baseFireRate { get { return _baseFireRate; } }
    [HideInInspector]
    public float damage { get { return _damage; } }
    [HideInInspector]
    public float fireRate { get { return _fireRate; } }

    [Header("Basic Details")]
    [SerializeField]
    protected float _baseDamage = 30.0f;    // Base damage before damage modications
    [SerializeField]
    
    protected float _baseFireRate = 1.0f;// Base fire rate before buff/debuff

    protected float _damage;             // Updated damage after damage calculations
    protected float _fireRate;           // Updated fire rate after modications

    protected bool _ready = true;
    protected GenericCharacter _attacker;

    public AudioClip sound;
	public AudioSource selfAS;

	public GameObject dropWeaponPrefab;

	public bool playSoundInCoroutine = false;

    protected virtual void Start()
    {
        _damage = _baseDamage;
        _fireRate = _baseFireRate;
        _baseDamage = Mathf.Clamp(_baseDamage, 0, int.MaxValue);
        _baseFireRate = Mathf.Clamp(_baseFireRate, 0, float.MaxValue);

        selfAS = GetComponent<AudioSource>();
    }

    public virtual void UpdateDamage(int mDamage)
    {
        _damage = Mathf.Clamp(mDamage, 0, int.MaxValue);
    }

    public virtual void UpdateFireRate(float mFireRate)
    {
        _fireRate = Mathf.Clamp(mFireRate, 0, float.MaxValue);
    }
    
    public virtual void DoAttack(GenericCharacter attacker)
    {
        if (!_ready) return;

        _ready = false;
        _attacker = attacker;

        PlayAnimation(attacker);
		if(selfAS != null && sound != null && !playSoundInCoroutine)
		    selfAS.PlayOneShot(sound);

        StartCoroutine(AttackRoutine());
    }

    protected virtual IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(fireRate);
        _ready = true;
    }

    protected void PlayAnimation(GenericCharacter character)
    {
        Animator animator = character.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Attack");
        }
    }

    private void OnEnable()
    {
        _ready = true;
    }
}
