using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// After enabled, damage of backstabbing is multiplied by the specified factor
/// </summary>
[CreateAssetMenu(menuName = "Assets/Effects/MultiplyDamageFromBehindEffect")]
public class MultiplyDamageFromBehindEffect : AbstractEffect
{
    public float damageMultiplier = 2.0f;
    public GameObject popupPrefab = null;

    public override void OnMakeDamage(
        float amount,
        GenericCharacter attacker,
        GenericCharacter target,
        Vector3 hitPoint,
        Vector3 hitDirection,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.OnMakeDamage(
            amount,
            attacker,
            target,
            hitPoint,
            hitDirection,
            calledByLocalPlayer,
            calledByServer);
        
        if (calledByServer && Vector3.Dot(hitDirection, target.transform.forward) > 0.7f)
        {
            target.GetComponent<Stats>().Health -= (damageMultiplier - 1.0f) * amount;
        }
    }
}
