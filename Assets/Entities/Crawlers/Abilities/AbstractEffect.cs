using UnityEngine;

/// <summary>
/// Every effect (of ability, item, passive skill, trap, buff or debuff)
/// that can be applied to GenericCharacter.
/// </summary>
abstract public class AbstractEffect : ScriptableObject
{
    public float baseCost;      // energy/money cost or recharge time
    public float baseDuration;  // time duration intended between activation and deactivation
    public Sprite icon;         // icon to show when the effect is applied

    public virtual void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    { }
    
    public virtual void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    { }

    public virtual void OnReceiveDamage(
        float amount,
        GenericCharacter attacker,
        GenericCharacter target,
        Vector3 hitPoint,
        Vector3 hitDirection,
        bool calledByLocalPlayer,
        bool calledByServer)
    { }

    public virtual void OnMakeDamage(
        float amount,
        GenericCharacter attacker,
        GenericCharacter target,
        Vector3 hitPoint,
        Vector3 hitDirection,
        bool calledByLocalPlayer,
        bool calledByServer)
    { }
}
