using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPicker : MonoBehaviour {

	public bool pooling = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("abilityPool"))
			pooling = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("abilityPool"))
			pooling = false;
	}
}
