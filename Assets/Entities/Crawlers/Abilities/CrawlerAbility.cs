using UnityEngine;

/// <summary>
/// Every ability that can be activated and, optionally, deactivated.
/// </summary>
abstract public class AbstractAbility : ScriptableObject
{
    public float baseCost; // energy/money cost or recharge time
    public float baseDuration; // time duration intended between activation and deactivation

    // Run the ability activation logic (run on server).
    public virtual void Activate(Crawler crawler) { }

    // Apply appearance effect corresponding to this ability (run on clients).
    public virtual void EnableEffect(Crawler crawler) { }

    // Run the ability deactivation logic (run on server).
    public virtual void Deactivate(Crawler crawler) { }

    // Remove appearance effect corresponding to this ability (run on clients).
    public virtual void DisableEffect(Crawler crawler) { }
}
