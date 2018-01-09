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
    GenericCharacter _attacker;

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
        if (!_ready) return;

        _ready = false;

        _attacker = attacker;
		transform.localRotation = _defaultRot;
		_animActive = true;
		StartCoroutine(AttackRoutine());
        blade.SetActive(true);
	}

	IEnumerator AttackRoutine()
    {
		yield return new WaitForSeconds(fireRate);
        _ready = true;
        blade.SetActive(false);
        _animActive = false;
	}
}
