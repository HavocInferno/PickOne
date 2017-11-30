using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CrawlerActiveAbility : CrawlerAbility
{
    [Header("Basic Details")]
    public float BaseDuration = 10;         // Duration the ability lasts
    public float BaseCooldown = 10;         // Cooldown of the ability

    [Header("Animations")]
    public GameObject StartEffect;          // Starting animation
    public float StartEffectDuration = 3;
    public GameObject EndEffect;            // Ending animation
    public float EndEffectDuration = 3;
    public GameObject ActiveEffect;         // Animation when ability is active
    public float ActiveEffectDuration = 3;

    // Read-only fields for external scripts
    [HideInInspector]
    public bool IsAvailable { get { return isAvailable; } }
    [HideInInspector]
    public bool IsActive { get { return isActive; } }

    // Internal variables
    protected bool isAvailable = true;      // Track whether ability can be used or not
    protected bool isActive = false;        // Track of whether the ability is in use or on cooldown
    protected Coroutine coroutine = null;   // Track what coroutine is executing to abort it if required

    /// <summary>
    /// Activates the ability on the crawler and performs additional work to ensure
    /// ability ends.
    /// </summary>
    public override void Activate()
    {
        isAvailable = false;
        isActive = true;

        // Play start effect
        playEffect(StartEffect, StartEffectDuration);
        // Play the active effect
        playEffect(ActiveEffect, ActiveEffectDuration);

        // Start duration countdown
        coroutine = StartCoroutine(countdownToEnd(BaseDuration));
    }

    /// <summary>
    /// Allows external scripts to force the ability to deactivate.
    /// </summary>
    public virtual void ForceDeactivate()
    {
        // If the ability is not in use then return
        if (!isActive)
            return;
        
        stopCountdown();
        deactivate();
    }

    /// <summary>
    /// Allows external scripts to force cooldown period to be over.
    /// </summary>
    public virtual void ForceRefresh()
    {
        // If the ability is currently in use then return
        if (isActive)
            return;

        stopCountdown();
        refresh();
    }

    /// <summary>
    /// Deactivates the ability and starts cooldown period.
    /// </summary>
    protected virtual void deactivate()
    {
        isAvailable = false;
        isActive = false;

        // Play the end effect
        playEffect(EndEffect, EndEffectDuration);

        // Start cooldown counter
        coroutine = StartCoroutine(countdownToRefresh(BaseCooldown));
    }

    /// <summary>
    /// Makes the ability available for use again.
    /// </summary>
    protected virtual void refresh()
    {
        isAvailable = true;
        isActive = false;
    }

    /// <summary>
    /// Stops any countdown coroutines that are currently active.
    /// </summary>
    protected virtual void stopCountdown()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
    
    /// <summary>
    /// Keeps track of how long to keep tthe ability active.
    /// </summary>
    /// <param name="mDuration">Cooldown is passed so as to make it possible to change
    /// duration at run-time e.g. a buff to increase durations.</param>
    /// <returns></returns>
    protected virtual IEnumerator countdownToEnd(float mDuration)
    {
        if (mDuration > 0)
            yield return new WaitForSeconds(mDuration);

        deactivate();
    }

    /// <summary>
    /// Makes the ability available again after the cooldown period has expired.
    /// </summary>
    /// <param name="mCooldown">Cooldown is passed so as to make it possible to change
    /// cooldown time at run-time e.g. a buff to reduce cooldowns.</param>
    /// <returns></returns>
    protected virtual IEnumerator countdownToRefresh(float mCooldown)
    {
        if (mCooldown > 0)
            yield return new WaitForSeconds(mCooldown);

        refresh();
    }

    /// <summary>
    /// Instantiates the effect and destroys it after a given duration.
    /// </summary>
    /// <param name="mEffect">Effect to be played.</param>
    /// <param name="mDuration">Lifetime of the effect.</param>
    protected virtual void playEffect(GameObject mEffect, float mDuration)
    {
        if (mEffect == null || mDuration <= 0)
            return;

        var effect = Instantiate(mEffect, GetComponentInParent<Transform>());
        Destroy(effect, mDuration);
    }
}
