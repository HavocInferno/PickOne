using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// After enabled, damage of backstabbing is multiplied by the specified factor
/// </summary>
[CreateAssetMenu(menuName = "Assets/Effects/GainAttributeFromKills")]
public class GainAttributeFromKillsEffect : AbstractEffect
{
    public float valuePerKill = 10.0f;
    public string attributeName = "Health";

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

        if (calledByServer)
        {
            if (target.GetComponent<Stats>().Health <= 0.0f)
            {
                Stats stats = attacker.GetComponent<Stats>();
                float current = stats.GetAttributeValue(attributeName);
                stats.SetAttributeValue(attributeName, current + valuePerKill);
            }
        }
    }
}

