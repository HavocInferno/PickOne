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

    public virtual void Enable(Crawler crawler) { }
    
    public virtual void Disable(Crawler crawler) { }
}
