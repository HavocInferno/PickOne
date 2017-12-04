using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When attached to GenericCharacter and initialized,
/// applies the specific effect to it. This effect is automatically
/// deapplied after the specified number of seconds and the
/// component destroys itself.
/// </summary>
[RequireComponent(typeof(Crawler))]
public class AppliedEffect : MonoBehaviour
{
    private float _duration = 0.0f;
    private Crawler _crawler;
    private AbstractEffect _effect;
    
    // This method need to be called in order to apply the effect.
    public void Initialize(AbstractEffect effect, float duration)
    {
        _effect = effect;
        _duration = duration;
        _crawler = GetComponent<Crawler>();

        if (_crawler.EffectIsEnabled(_effect))
            _crawler.DisableEffect(_effect);
        _crawler.EnableEffect(_effect);

        Destroy(this, _duration);
    }

    void OnDisable()
    {
        if (_crawler.EffectIsEnabled(_effect))
            _crawler.DisableEffect(_effect);
    }
}
