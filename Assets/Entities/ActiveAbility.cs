using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles rechargeable abilities.
/// </summary>
[System.Serializable]
public class ActiveAbility
{
    public enum AvailabilityType
    {
        Cooldown,   // player should wait efore the ability is recharged
        Attribute   // player should wait until he has enough energy (or any other atribute)
    }

    public AvailabilityType type;
    public string attributeName;

    public Sprite icon;

    private bool _isAvailable = true;

    public bool IsAvailable { get { return _isAvailable; } }
    public string name;

    [HideInInspector]
    public Coroutine rechargeCoroutine;

    [SerializeField]
    private List<AbstractEffect> effects;
    
    private IEnumerator WaitAndRecharge(float deltaTime, GenericCharacter character)
    {
        yield return new WaitForSeconds(deltaTime);
        Recharge(character);
    }

    public void Initialize()
    {
        _isAvailable = true;
    }

    public void Update(GenericCharacter character)
    {
        if (!_isAvailable && type == AvailabilityType.Attribute)
        {
            Stats stats = character.GetComponent<Stats>();
            bool hasAttribute = stats != null && stats.HasAttribute(attributeName);
            if (!hasAttribute)
            {
                Debug.LogErrorFormat(
                    "Ability {0} of character {1} requires attribute {2} but it is not found",
                    name, character.name, attributeName);
                _isAvailable = true;
            }

            float totalBaseCost = 0.0f;
            foreach (var effect in effects)
            {
                totalBaseCost += effect.baseCost;
            }

            if (stats.GetAttributeValue(attributeName) >= totalBaseCost)
            {
                Debug.LogFormat("{0} | {1} recharged", character.name, name);
                _isAvailable = true;
            }
        }
    }

    // Activates this ability for the character if it is available.
    public void Activate(GenericCharacter character)
    {
        if (!_isAvailable)
        {
            Debug.LogFormat("{0} | {1} is not available", character.name, name);
            return;
        }
        _isAvailable = false;

        float totalBaseCost = 0.0f;
        foreach (var effect in effects)
        {
            character.EnableEffectDuration(effect, effect.baseDuration);
            totalBaseCost += effect.baseCost;
        }

        Debug.LogFormat("{0} | {1} activated", character.name, name);

        if (type == AvailabilityType.Cooldown)
        {
            rechargeCoroutine = character.StartCoroutine(
                WaitAndRecharge(totalBaseCost, character));
        }
        else if (type == AvailabilityType.Attribute)
        {
            Stats stats = character.GetComponent<Stats>();
            bool hasAttribute = stats != null && stats.HasAttribute(attributeName);
            if (!hasAttribute) return;
            float newValue = stats.GetAttributeValue(attributeName) - totalBaseCost;
            stats.SetAttributeValue(attributeName, newValue);
        }
    }

    // Explicitly recharge this ability for the character if it is activated.
    public void Recharge(GenericCharacter character)
    {
        if (_isAvailable) return;
        _isAvailable = true;
        if (rechargeCoroutine != null)
            character.StopCoroutine(rechargeCoroutine);

        Debug.LogFormat("{0} | {1} recharged", character.name, name);
    }
}
