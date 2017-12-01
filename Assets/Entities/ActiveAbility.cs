using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles rechargeable ability with cooldown.
/// </summary>
[System.Serializable]
public class ActiveAbility
{
    private bool _isAvailable = true;

    public bool IsAvailable { get { return _isAvailable; } }
    public string name = "Generic Ability";

    [HideInInspector]
    public Coroutine rechargeCoroutine;

    [SerializeField]
    private List<AbstractEffect> effects = new List<AbstractEffect>();

    private IEnumerator WaitAndRecharge(float deltaTime, GenericCharacter character)
    {
        yield return new WaitForSeconds(deltaTime);
        Recharge(character);
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
            AppliedEffect comp = character.gameObject.AddComponent<AppliedEffect>();
            comp.Initialize(effect, effect.baseDuration);
            totalBaseCost += effect.baseCost;
        }

        Debug.LogFormat("{0} | {1} activated", character.name, name);

        rechargeCoroutine = character.StartCoroutine(
            WaitAndRecharge(totalBaseCost, character));
    }

    // Explicitly recharge this ability for the character if it is activated.
    public void Recharge(GenericCharacter character)
    {
        if (_isAvailable) return;
        _isAvailable = true;
        character.StopCoroutine(rechargeCoroutine);

        Debug.LogFormat("{0} | {1} recharged", character.name, name);
    }
}
