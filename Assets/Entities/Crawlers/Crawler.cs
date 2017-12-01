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

    [SerializeField]
    protected List<ActiveAbility> _activeAbilities = new List<ActiveAbility>();
    [SerializeField]
    protected List<AbstractEffect> _passiveEffects = new List<AbstractEffect>();

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

    [Header("Skills")]

    [SyncVar(hook = "OnChangeSkill1_Buffed")]
    public bool skill1_Buffed = false;
    [SyncVar(hook = "OnChangeSkill2_Debuffed")]
    public bool skill2_Debuffed = false;
    [SyncVar(hook = "OnChangeSkill3_Healed")]
    public bool skill3_Healed = false;

    // Class that handles rechargeable ability with cooldown.
    [System.Serializable]
    protected class ActiveAbility
    {
        private bool _isAvailable = true;

        public bool IsAvailable { get { return _isAvailable; } }
        public string name = "Generic Ability";

        [HideInInspector]
        public Coroutine rechargeCoroutine;

        [SerializeField]
        private List<AbstractEffect> effects = new List<AbstractEffect>();

        private IEnumerator WaitAndRecharge(float deltaTime, Crawler crawler)
        {
            yield return new WaitForSeconds(deltaTime);
            Recharge(crawler);
        }

        // Activates this ability for the crawler if it is available.
        public void Activate(Crawler crawler)
        {
            if (!_isAvailable)
            {
                Debug.LogFormat("Crawler | {0} is not available", name);
                return;
            }
            _isAvailable = false;

            float totalBaseCost = 0.0f;
            foreach (var effect in effects)
            {
                AppliedEffect comp = crawler.gameObject.AddComponent<AppliedEffect>();
                comp.Activate(effect, effect.baseDuration);
                totalBaseCost += effect.baseCost;
            }

            Debug.LogFormat("Crawler | {0} activated", name);

            rechargeCoroutine = crawler.StartCoroutine(
                WaitAndRecharge(totalBaseCost, crawler));
        }

        // Explicitly recharge this ability for the crawler if it is activated.
        public void Recharge(Crawler crawler)
        {
            if (_isAvailable) return;
            _isAvailable = true;
            crawler.StopCoroutine(rechargeCoroutine);

            Debug.LogFormat("Crawler | {0} recharged", name);
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

        foreach (var effect in _passiveEffects)
        {
            EnableEffect(effect);
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
        _activeAbilities[0].Activate(this);
    }

    [Command]
    void CmdSecondaryAbility()
    {
        _activeAbilities[1].Activate(this);
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
