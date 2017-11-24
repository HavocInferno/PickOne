using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float SwingSpeed = 3.0f;
    public float LifeTime = 1.0f;

	private bool animActive = false;
	private Quaternion defaultRot;

    public GameObject blade = null;

    private void Start()
    {
		defaultRot = transform.localRotation;
        blade.SetActive(false);
    }

    private void Update()
    {
		if (animActive)
        {
			var prevRot = transform.localRotation;

			// Rotate the sword
			transform.localRotation =
                Quaternion.LerpUnclamped(
				    transform.localRotation,
				    Quaternion.Euler(0f, -175f, -45f),  // This is not 180 to prevent rotating behind the attacker
				    SwingSpeed * Time.deltaTime);
		}
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check for friendly fire 
        if (collision.collider.tag == gameObject.transform.parent.tag)
            return;

        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeHit(30, this.transform.position, this.transform.forward);
        }
    }

	public void PlayAnimation()
    {
		transform.localRotation = defaultRot;
		animActive = true;
		StartCoroutine(AttackRoutine());
        blade.SetActive(true);
	}

	IEnumerator AttackRoutine()
    {
		yield return new WaitForSeconds(LifeTime);
        blade.SetActive(false);
        animActive = false;
	}
}
