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

    private List<AbstractEffect> _appliedEffects = new List<AbstractEffect>();

    public Renderer mainRenderer = null;

	public Animator anim; 

    // Enable specified effect for a given period of time.
    public void EnableEffectDuration(AbstractEffect effect, float duration)
    {
        AppliedEffect comp = gameObject.AddComponent<AppliedEffect>();
        comp.Initialize(effect, duration);
    }

    public void EnableEffect(AbstractEffect effect)
    {
        if (!isServer) return;
        RpcEnableEffect(effect.name);
    }

    // Enable specified effect (can be disabled manually via Disable call)
    [ClientRpc]
    private void RpcEnableEffect(string effectName)
    {
        var effect = Effects.GetByName(effectName);
        LocalEnableEffect(effect);
    }

    private void LocalEnableEffect(AbstractEffect effect)
    {
		if (_appliedEffects.Contains (effect))
			return;
        Debug.LogFormat("{0} | Effect {1} enabled", name, effect.name);
        effect.Enable(this, isLocalPlayer, isServer);
        _appliedEffects.Add(effect);
        if (isLocalPlayer && effect.icon != null)
            FindObjectOfType<ActiveEffectsPanel>().AddElement(effect);
    }

    public void DisableEffect(AbstractEffect effect)
    {
        if (!isServer) return;
        RpcDisableEffect(effect.name);
    }

    [ClientRpc]
    private void RpcDisableEffect(string effectName)
    {
        var effect = Effects.GetByName(effectName);
        LocalDisableEffect(effect);
    }

    private void LocalDisableEffect(AbstractEffect effect)
    {
		if (!_appliedEffects.Contains (effect))
			return;
        effect.Disable(this, isLocalPlayer, isServer);
        Debug.LogFormat("{0} | Effect {1} disabled", name, effect.name);
        _appliedEffects.Remove(effect);
        if (isLocalPlayer && effect.icon != null)
            FindObjectOfType<ActiveEffectsPanel>().RemoveElement(effect);
    }

    public bool EffectIsEnabled(AbstractEffect effect)
    {
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
        foreach (var effect in passiveEffects)
        {
            LocalEnableEffect(effect);
        }
		var netanim = GetComponentInChildren<NetworkAnimator> ();
		if (netanim != null) {
			netanim.SetParameterAutoSend (0, true);
			netanim.SetParameterAutoSend (1, true);
		}
    }

    void OnValidate()
    {
        if (basicAttack == null)
        {
            Debug.LogWarningFormat("{0} | Basic attack not set", name);
        }
    }

    //###################### RPC CALLS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    [ClientRpc]
    protected void RpcAttack()
    {
        if (basicAttack != null)
            basicAttack.DoAttack(this);
        else
            Debug.LogErrorFormat("{0} | BasicAttack is not set", name);
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

		if (anim != null)
			anim.SetTrigger ("Hit");
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
