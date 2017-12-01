using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Crawler : NetworkBehaviour
{
    [Header("Player Properties")]

    public Text nameTag;
    [SyncVar(hook = "OnChangeName")]
    public string pName = "player";
    [SyncVar]
    public Color playerColor = Color.white;
    [SyncVar]
    public bool isVRMasterPlayer = false;
	[SyncVar(hook = "OnChangeDead")]
	public bool isDead = false;

    [Header("Attacks")]

    public BasicAttack basicAttack;
    //public float fireRate = 0.15f;
    //private float lastFire;
    //public float bulletSpeed = 16f;
    //public GameObject bulletPrefab;
    //public Transform bulletSpawn;

    [Space(8)]

    public Stats stats;

    [Header("Abilities")]

    public AbstractAbility primaryAbility;
    public AbstractAbility secondaryAbility;

    [SerializeField]
    protected List<CooldownAbilityInstance> _cooldownAbilities = new List<CooldownAbilityInstance>();

    [Header("Skills")]

    [SyncVar(hook = "OnChangeSkill1_Buffed")]
    public bool skill1_Buffed = false;
    [SyncVar(hook = "OnChangeSkill2_Debuffed")]
    public bool skill2_Debuffed = false;
    [SyncVar(hook = "OnChangeSkill3_Healed")]
    public bool skill3_Healed = false;

    // Class that handles rechargeable ability with cooldown.
    [System.Serializable]
    protected class CooldownAbilityInstance
    {
        private Crawler _crawler;

        private bool _isAvailable = true;
        private bool _isActive = false;

        public bool IsAvailable { get { return _isAvailable; } }
        public bool IsActive { get { return _isActive; } }

        [HideInInspector]
        public Coroutine deactivateCoroutine;
        [HideInInspector]
        public Coroutine rechargeCoroutine;

        [SerializeField]
        private AbstractAbility ability;

        private IEnumerator WaitAndDeactivate(float deltaTime)
        {
            yield return new WaitForSeconds(deltaTime);
            Deactivate();
        }

        private IEnumerator WaitAndRecharge(float deltaTime)
        {
            yield return new WaitForSeconds(deltaTime);
            Recharge();
        }

        public void Initialize(Crawler crawler)
        {
            _crawler = crawler;
        }

        // Activates this ability for the crawler if it is available.
        public void Activate()
        {
            if (!_isAvailable) return;
            _isAvailable = false;
            _isActive = true;

            ability.Activate(_crawler);
            Debug.Log("Crawler | Ability activated");

            deactivateCoroutine = _crawler.StartCoroutine(
                WaitAndDeactivate(ability.baseDuration));
        }

        // Explicitly deactivates this ability for the crawler if it is activated.
        public void Deactivate()
        {
            if (!_isActive) return;
            _isActive = false;

            Debug.Log("Crawler | Ability deactivated");
            ability.Deactivate(_crawler);

            _crawler.StopCoroutine(deactivateCoroutine);
            rechargeCoroutine = _crawler.StartCoroutine(
                WaitAndRecharge(ability.baseCost));
        }

        // Explicitly recharge this ability for the crawler if it is activated.
        public void Recharge()
        {
            if (_isAvailable) return;
            _isAvailable = true;
            _crawler.StopCoroutine(rechargeCoroutine);

            Debug.Log("Crawler | Ability recharged");
        }
    }

    /*public enum ActionState {
		NONE,
		ATTACK
	}
	[SyncVar(hook = "OnChangeActionState")]
	public ActionState actionState = ActionState.NONE;*/

    //#######################################################################
    //called after scene loaded
    void Start()
    {
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

        //on the server, add yourself to the level-wide player list
        if (isServer)
        {
            Debug.Log("SERVER: " + pName + " is here.");
            if (!isVRMasterPlayer)
                FindObjectOfType<PlayersManager>().players.Add(transform);
        }

        foreach (var ability in _cooldownAbilities)
        {
            ability.Initialize(this);
        }
    }

    void OnValidate()
    {
        if (basicAttack == null)
        {
            Debug.LogError("Basic attack not set.");
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
            UnityEngine.VR.VRSettings.enabled = true;
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
            UnityEngine.VR.VRSettings.enabled = false;
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

    public void TogglePrimaryAbility()
    {
        if (!isLocalPlayer)
            return;

        CmdPrimaryAbility();
    }

    public void ToggleSecondaryAbility()
    {
        if (!isLocalPlayer)
            return;

        CmdSecondaryAbility();
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

    [Command]
    void CmdAttack()
    {
        RpcAttack();
    }

    [Command]
    void CmdPrimaryAbility()
    {
        _cooldownAbilities[0].Activate();
    }

    [Command]
    void CmdSecondaryAbility()
    {
        secondaryAbility.Activate(this);
    }

    //###################### RPC CALLS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    [ClientRpc]
    void RpcAttack()
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

	void OnChangeDead(bool dead)
	{
		if (!isDead) {
			if (isServer) {
				nameTag.text += " [DEAD]";
			}
			gameObject.GetComponentInChildren<CrawlerController> ().enabled = false;
		}
	}
}
