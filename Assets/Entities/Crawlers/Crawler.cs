using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Crawler : GenericCharacter
{
    [Header("Player Properties")]

    public Text nameTag;
    [SyncVar(hook = "OnChangeName")]
    public string pName = "player";
    [SyncVar]
    public Color playerColor = Color.white;
    [SyncVar]
    public bool isVRMasterPlayer = false;

    [Header("Skills")]

    [SyncVar(hook = "OnChangeSkill1_Buffed")]
    public bool skill1_Buffed = false;
    [SyncVar(hook = "OnChangeSkill2_Debuffed")]
    public bool skill2_Debuffed = false;
    [SyncVar(hook = "OnChangeSkill3_Healed")]
    public bool skill3_Healed = false;

    /*public enum ActionState {
		NONE,
		ATTACK
	}
	[SyncVar(hook = "OnChangeActionState")]
	public ActionState actionState = ActionState.NONE;*/

    public override void OnStartServer()
    {
        base.OnStartServer();

        //on the server, add yourself to the level-wide player list
        if (isServer)
        {
            Debug.Log("SERVER: " + pName + " is here.");
            if (!isVRMasterPlayer)
                FindObjectOfType<PlayersManager>().players.Add(transform);
        }
    }

    //#######################################################################
    //called after scene loaded
    protected override void Start()
    {
        base.Start();

        gameObject.name = pName;
        //foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        //{
        //    mr.material = defaultMaterial;
        //    mr.material.color = playerColor;
        //}
        ParticleSystem.MainModule mm = GetComponentInChildren<ParticleSystem>().main;
        mm.startColor = playerColor;
        nameTag.text = pName;

        //scale up the player object if this is the VR master [this is a temporary visualisation, to be removed once a proper Master representation is done]
        if (isVRMasterPlayer)
        {
            transform.localScale *= 2f;
        }
    }

    //is called when the local client's scene starts
    public override void OnStartLocalPlayer()
    {
        /*if this is the VR Master, do:
		 * enable VR if not done already
		 * disable the default camera
		 * enable the OpenVR cam rig
		 * move transform up a bit (to compensate for the scale increase in Start()) [this is a temporary visualisation, to be removed once a proper Master representation is done]
		 * append (VR MASTER) to the player name */
        if (isVRMasterPlayer)
        {
            UnityEngine.XR.XRSettings.enabled = true;
            FindObjectOfType<CameraManager>().nonVRCamera.SetActive(false);
            FindObjectOfType<CameraManager>().vrCamera.SetActive(true);
            transform.position += Vector3.up;
            pName = pName + " (VR MASTER)";
        }
        /*if not, do:
		 * disable VR if not done already
		 * enable the default camera
		 * disable the OpenVR cam rig
		 * set this gameObject as the main cam target */
        else
        {
            UnityEngine.XR.XRSettings.enabled = false;
            FindObjectOfType<CameraManager>().vrCamera.SetActive(false);
            FindObjectOfType<CameraManager>().nonVRCamera.SetActive(true);
            Camera.main.GetComponent<DungeonCamera>().target = this.gameObject;
        }
    }

    public void Attack()
    {
        if (!isLocalPlayer)
            return;

        //weapon firing. dumb and unoptimized.
        CmdAttack();
    }

    public void ActivateAbility(int index)
    {
        if (!isLocalPlayer)
            return;

        CmdActivateAbility(index);
    }

    //###################### COMMAND CALLS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    [Command]
    void CmdAttack()
    {
        RpcAttack();
    }

    [Command]
    void CmdActivateAbility(int index)
    {
        RpcActivateAbility(index);
    }

    //###################### RPC CALLS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV

    [ClientRpc]
    void RpcActivateAbility(int index)
    {
        if (_activeAbilities.Count > index)
            _activeAbilities[index].Activate(this);
    }

    //###################### SYNCVAR HOOKS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    void OnChangeName(string newName)
    {
        nameTag.text = newName;
    }
    
    void OnChangeSkill1_Buffed(bool state)
    {
        skill1_Buffed = state;
		if(state)
			Debug.Log ("You are being buffed!");
		else
			Debug.Log ("You are not being buffed anymore!");
    }
    void OnChangeSkill2_Debuffed(bool state)
    {
        skill2_Debuffed = state;
    }
    void OnChangeSkill3_Healed(bool state)
    {
        skill3_Healed = state;
    }

	protected override void OnDeath()
	{
		if (isServer)
        {
			nameTag.text += " [DEAD]";
		}
		gameObject.GetComponentInChildren<CrawlerController>().enabled = false;
	}
}
