using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/CompoundEffect")]
public class CompoundEffect : AbstractEffect
{
    public List<AbstractEffect> effects = new List<AbstractEffect>();

    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Enable(character, calledByLocalPlayer, calledByServer);

        foreach (var effect in effects)
        {
            effect.Enable(character, calledByLocalPlayer, calledByServer);
        }
    }

    public override void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Disable(character, calledByLocalPlayer, calledByServer);

        foreach (var effect in effects)
        {
            effect.Disable(character, calledByLocalPlayer, calledByServer);
        }
    }

    public override void OnReceiveDamage(
        float amount,
        GenericCharacter attacker,
        GenericCharacter target,
        Vector3 hitPoint,
        Vector3 hitDirection,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.OnReceiveDamage(amount, attacker, target,
            hitPoint, hitDirection, calledByLocalPlayer, calledByServer);

        foreach (var effect in effects)
        {
            effect.OnReceiveDamage(amount, attacker, target,
                hitPoint, hitDirection, calledByLocalPlayer, calledByServer);
        }
    }

    public override void OnMakeDamage(
        float amount,
        GenericCharacter attacker,
        GenericCharacter target,
        Vector3 hitPoint,
        Vector3 hitDirection,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.OnMakeDamage(amount, attacker, target,
            hitPoint, hitDirection, calledByLocalPlayer, calledByServer);

        foreach (var effect in effects)
        {
            effect.OnMakeDamage(amount, attacker, target,
                hitPoint, hitDirection, calledByLocalPlayer, calledByServer);
        }
    }
}
