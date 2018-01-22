using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
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

    //public CrawlerClass crawlerClass;
    public List<ActiveAbility> activeAbilities = new List<ActiveAbility>();

	[Header("UI (to be disabled for local)")]
	public GameObject tpsUI;

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

    protected virtual void FixedUpdate()
    {
        foreach (var ability in activeAbilities)
        {
            ability.Update(this);
        }
    }

    //#######################################################################
    //called after scene loaded
    protected override void Start()
    {
        //if (crawlerClass != null)
        //    crawlerClass.Apply(this);

        foreach (var activeAbility in activeAbilities)
            activeAbility.Recharge(this);

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
			gameObject.GetComponentInChildren<CrawlerController>().enabled = false;
			gameObject.SetActive (false);
        }

        if (isLocalPlayer)
        {
            FindObjectOfType<AttributesPanel>().Register(GetComponent<Stats>());
			if (tpsUI)
				tpsUI.SetActive (false);
			if (!isVRMasterPlayer) {
				FindObjectOfType<CUI_crosshair> ().registerCrawler (this);
                FindObjectOfType<AbilitiesPanel>().Initialize(activeAbilities);
			}
        }

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
    }

    //is called when the local client's scene starts
    public override void OnStartLocalPlayer()
    {
        float minDistance = 0.0f;
        NetworkStartPosition closestPosition = null;
        foreach (var point in FindObjectsOfType<NetworkStartPosition>())
        {
            float dist = Vector3.Distance(point.transform.position, transform.position);
            if (closestPosition == null || dist < minDistance)
            {
                minDistance = dist;
                closestPosition = point;
            }
        }

        // CANNOT ROTATE CAMERA WITH THE PLAYER.
        // transform.rotation = closestPosition.transform.rotation;

        if (GetComponent<NavMeshAgent>())
            GetComponent<NavMeshAgent>().enabled = true;

        /*if this is the VR Master, do:
        * enable VR if not done already
        * disable the default camera
        * enable the OpenVR cam rig
        * move transform up a bit (to compensate for the scale increase in Start()) [this is a temporary visualisation, to be removed once a proper Master representation is done]
        * append (VR MASTER) to the player name */
        if (isVRMasterPlayer)
        {
            UnityEngine.XR.XRSettings.enabled = true;
			QualitySettings.vSyncCount = 0; //this is an ugly fix, proper fix would be to globally save the player-set vsync state and recover it after the match is done.
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
			Camera.main.GetComponent<DungeonCamera>().shakeDistanceTarget = transform;
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
        if (activeAbilities.Count > index)
            activeAbilities[index].Activate(this);
    }

    //###################### SYNCVAR HOOKS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    void OnChangeName(string newName)
    {
        nameTag.text = newName;
    }

	protected override void OnDeath()
	{
        FindObjectOfType<EndConditions>().CheckEndCondition();
		nameTag.text += " [DEAD]";
		gameObject.GetComponentInChildren<CrawlerController>().enabled = false;
        Destroy(gameObject.GetComponent<DetectableObject>());
		if (isLocalPlayer && isDead) {
			FindObjectOfType<EndScreenUI> ().SetDeathScreen (true);
		}
	}
}
