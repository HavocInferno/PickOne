using System.Collections;
using UnityEngine;

public class Sword : BasicAttack
{
    [Header("Sword Details")]
    public float swingSpeed = 3.0f;
    public float lifeTime = 1.0f;

    public GameObject blade = null;

    private bool _animActive = false;
	private Quaternion _defaultRot;
    

    protected override void Start()
    {
        base.Start();

        _defaultRot = transform.localRotation;
        blade.SetActive(false);
    }

    protected void Update()
    {
		if (_animActive)
        {
			var prevRot = transform.localRotation;

			// Rotate the sword
			transform.localRotation =
                Quaternion.LerpUnclamped(
				    transform.localRotation,
				    Quaternion.Euler(0f, -175f, -45f),  // This is not 180 to prevent rotating behind the attacker
				    swingSpeed * Time.deltaTime);
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
        base.DoAttack(attacker);

        //if (!_ready) return;

        //_ready = false;
        
		transform.localRotation = _defaultRot;
		_animActive = true;
        blade.SetActive(true);
		//selfAS.PlayOneShot(sound);
		//StartCoroutine(WaitForReload());
	}

    private void OnDisable()
    {
        StopAllCoroutines();
        _ready = true;
        blade.SetActive(false);
        _animActive = false;
        transform.localRotation = _defaultRot;
    }

    protected override IEnumerator AttackRoutine()
    {
        yield return base.AttackRoutine();

        blade.SetActive(false);
        _animActive = false;
	}
}
