using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Sword : BasicAttack
{
    [Header("Sword Details")]
    public float swingSpeed = 3.0f;
    public float lifeTime = 1.0f;

    public GameObject blade = null;
	public Collider bladeC;

    private bool _animActive = false;
	private Quaternion _defaultRot;
    
	public bool hasAnimator = false;

	public Animator animator;

	public Transform parent;

    protected override void Start()
    {
        base.Start();

        _defaultRot = transform.localRotation;
		if (hasAnimator)
			bladeC.enabled = false;
		else
        	blade.SetActive(false);
	}

    protected void Update()
    {
		if (_animActive)
        {
			if (!hasAnimator) {
				var prevRot = transform.localRotation;

				// Rotate the sword
				transform.localRotation =
                Quaternion.LerpUnclamped (
					transform.localRotation,
					Quaternion.Euler (0f, -175f, -45f),	//  Quaternion.Euler(0f, -120f, 45f), This is not 180 to prevent rotating behind the attacker
					swingSpeed * Time.deltaTime);
			}	    
		}
	
			if (hasAnimator && parent != null) {
			transform.position = parent.position;
			transform.rotation = parent.rotation;
			}
    }

    protected virtual void OnValidate()
    {
        if (blade == null)
        {
            // Try to find the prefab automatically
            blade = transform.Find("Blade").gameObject;

            if (!blade)
                Debug.LogError("Blade prefab not set.");
        }
		if(bladeC == null)
		{
			// Try to find the prefab automatically
			bladeC = blade.GetComponentInChildren<Collider>();

			if (!bladeC)
				Debug.LogError("Blade collider not set.");
		}
    }

    protected void OnCollisionEnter(Collision collision)
    {
        // Check for friendly fire
        if (collision.collider.tag == gameObject.transform.parent.tag)
            return;

        // Get health component of collision object
        var stats = collision.gameObject.GetComponent<Stats>();

        // If it has one, call function to take damage
        if (stats != null)
        {
            stats.Hit(damage, _attacker, transform.position,
                (collision.transform.position - _attacker.transform.position).normalized);
        }
        else
        {
            if (!collision.collider.CompareTag("Untagged"))
                Debug.LogWarning("On " + collision.collider.tag + " stats were not found.");
        }
    }

    public override void DoAttack(GenericCharacter attacker)
    {
        if (!_ready) return;

        base.DoAttack(attacker);

        //_ready = false;
		if (hasAnimator && animator)
			animator.SetTrigger ("Attack");
		_animActive = true;		
		if (hasAnimator)
			bladeC.enabled = true;
		else{
			blade.SetActive (true);
			transform.localRotation = _defaultRot;
		}
		//selfAS.PlayOneShot(sound);
		//StartCoroutine(WaitForReload());
	}

    private void OnDisable()
    {
        StopAllCoroutines();
		_ready = true;		
		if (hasAnimator)
			bladeC.enabled = false;
		else{
			blade.SetActive (false);
			transform.localRotation = _defaultRot;
		}
        _animActive = false;

    }

    protected override IEnumerator AttackRoutine()
    {
		yield return base.AttackRoutine();		
		if (hasAnimator)
			bladeC.enabled = false;
		else
        	blade.SetActive(false);
        _animActive = false;
	}
}
