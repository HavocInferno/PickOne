using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : ThrowableAbility
{
    public float maxDamage;
    public override void applyEffect(float multi, GenericCharacter target)
    {
		
		if (target && target.GetComponent<Stats> ())
			target.GetComponent<Stats>().Hit( maxDamage * multi,GetComponent<GenericCharacter>(), target.transform.position, -GetComponent<Rigidbody>().velocity.normalized);
    }
}
