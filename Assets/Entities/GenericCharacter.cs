﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    public Stats stats;

    [Header("Abilities")]
    
    public List<ActiveAbility> _activeAbilities = new List<ActiveAbility>();
    public List<AbstractEffect> _passiveEffects = new List<AbstractEffect>();

    private HashSet<AbstractEffect> _appliedEffects = new HashSet<AbstractEffect>();

    public void EnableEffect(AbstractEffect effect)
    {
        effect.Enable(this);
        _appliedEffects.Add(effect);
    }

    public void DisableEffect(AbstractEffect effect)
    {
        effect.Disable(this);
        _appliedEffects.Remove(effect);
    }

    public bool EffectIsEnabled(AbstractEffect effect)
    {
        effect.Disable(this);
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
        foreach (var effect in _passiveEffects) EnableEffect(effect);
    }

    void OnValidate()
    {
        if (basicAttack == null)
        {
            Debug.LogErrorFormat("{0} | Basic attack not set", name);
        }
    }

    //###################### COMMAND CALLS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    // This [Command] code is called on the Client …
    // … but it is run on the Server!
    /*[Command]
	void CmdFire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = Instantiate(
			bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

		NetworkServer.Spawn (bullet);

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}*/

    //###################### RPC CALLS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    [ClientRpc]
    protected void RpcAttack()
    {
        // TODO: Damage calculation
        basicAttack.DoAttack();
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
        if (!isDead) OnDeath();
    }

    protected virtual void OnDeath() { }
}

