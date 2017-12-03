using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

/// <summary>
/// Generic character that can move, attack, has body and stats.
/// Can interact with other level objects such as traps and other characters.
/// Can be buffed or debuffed.
/// </summary>
public class GenericCharacter : NetworkBehaviour
{
    [Header("Generic Properties")]

    [SyncVar(hook = "OnChangeDead")]
    public bool isDead = false;

    [Header("Attacks")]

    public BasicAttack basicAttack;

    [Space(8)]

    [Header("Abilities")]
    
    public List<AbstractEffect> passiveEffects = new List<AbstractEffect>();

    private HashSet<AbstractEffect> _appliedEffects = new HashSet<AbstractEffect>();

    public void EnableEffect(AbstractEffect effect)
    {
        Debug.LogFormat("{0} | Effect {1} enabled", name, effect.name);
        effect.Enable(this, isLocalPlayer, isServer);
        _appliedEffects.Add(effect);
        if (isLocalPlayer)
            FindObjectOfType<ActiveEffectsPanel>().AddElement(effect);
    }

    public void DisableEffect(AbstractEffect effect)
    {
        effect.Disable(this, isLocalPlayer, isServer);
        _appliedEffects.Remove(effect);
        if (isLocalPlayer)
            FindObjectOfType<ActiveEffectsPanel>().RemoveElement(effect);
    }

    public bool EffectIsEnabled(AbstractEffect effect)
    {
        effect.Disable(this, isLocalPlayer, isServer);
        return _appliedEffects.Contains(effect);
    }

    /*public enum ActionState {
		NONE,
		ATTACK
	}
	[SyncVar(hook = "OnChangeActionState")]
	public ActionState actionState = ActionState.NONE;*/

    //#######################################################################
    //called after scene loaded

    protected virtual void Start()
    {
        foreach (var effect in passiveEffects) EnableEffect(effect);
    }

    void OnValidate()
    {
        if (basicAttack == null)
        {
            Debug.LogErrorFormat("{0} | Basic attack not set", name);
        }
    }

    //###################### RPC CALLS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    [ClientRpc]
    protected void RpcAttack()
    {
        // TODO: Damage calculation
        basicAttack.DoAttack(this);
    }

    //[ClientRpc]
    //public void RpcSetMaterial(bool cloak)
    //{
    //    foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
    //    {
    //        mr.material = cloak ? cloakMaterial : defaultMaterial;
    //        if (!cloak)
    //        {
    //            mr.material.color = playerColor;
    //        }
    //    }
    //}

    //###################### SYNCVAR HOOKS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    void OnChangeDead(bool dead)
    {
        if (!isDead)
        {
            isDead = true;
            OnDeath();
        }
    }

    protected virtual void OnDeath() { }

    public virtual void OnReceiveDamage(
        float amount,
        GenericCharacter attacker,
        Vector3 hitPoint,
        Vector3 hitDirection)
    {
        foreach (AbstractEffect effect in _appliedEffects)
        {
            effect.OnReceiveDamage(
                amount,
                attacker,
                this,
                hitPoint,
                hitDirection,
                isLocalPlayer,
                isServer);
        }
    }

    public virtual void OnMakeDamage(
        float amount,
        GenericCharacter target,
        Vector3 hitPoint,
        Vector3 hitDirection)
    {
        foreach (AbstractEffect effect in _appliedEffects)
        {
            effect.OnMakeDamage(
                amount,
                this,
                target,
                hitPoint,
                hitDirection,
                isLocalPlayer,
                isServer);
        }
    }
}

