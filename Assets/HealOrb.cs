using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOrb : ThrowableAbility {

	public float maxHeal;
	public override void applyEffect(float multi, GenericCharacter target)
	{
		try{
		if (target && target.GetComponent<Stats> ())
			target.GetComponent<Stats>().Health += multi*maxHeal;
		}
		catch(System.Exception e){
			Debug.Log ("Artjom fucked this up.");
		}
	}
}
