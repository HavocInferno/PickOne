using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : ThrowableAbility
{
    public override void applyEffect(float multi, GenericCharacter target)
    {
        Debug.Log(" gets rekt by a fireball with "+multi+" times max damage.");
    }
}
