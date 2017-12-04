using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsRegistryScript : MonoBehaviour
{
    public List<AbstractEffect> effects = new List<AbstractEffect>();
    public Dictionary<string, AbstractEffect> strToEffect = new Dictionary<string, AbstractEffect>();

    void Start()
    {
        foreach (var effect in effects)
        {
            strToEffect.Add(effect.name, effect);
        }
    }
}

public static class Effects
{
    private static EffectsRegistryScript _registry;
    public static AbstractEffect GetByName(string name)
    {
        if (_registry == null)
            _registry = Object.FindObjectOfType<EffectsRegistryScript>();
        return _registry.strToEffect[name];
    }
}
