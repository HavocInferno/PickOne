using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Crawler : NetworkBehaviour
{
    //public GameObject bulletPrefab;
    //public Transform bulletSpawn;
    public Sword sword;
    public float fireRate = 0.15f;
    private float lastFire;
    public float bulletSpeed = 16f;

    public Text nameTag;

    [SyncVar(hook = "OnChangeName")]
    public string pName = "player";
    [SyncVar]
    public Color playerColor = Color.white;
    [SyncVar]
    public bool isVRMasterPlayer = false;

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

    //#######################################################################
    //called after scene loaded
    void Start()
    {
        gameObject.name = pName;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.material.color = playerColor;
        }
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
            Debug.Log(pName + " is here.");
            if (!isVRMasterPlayer)
                FindObjectOfType<PlayersManager>().players.Add(transform);
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

    public void Attack()
    {
        if (!isLocalPlayer)
            return;

        //weapon firing. dumb and unoptimized.
        if (Time.time > lastFire)
        {
            lastFire = Time.time + fireRate;
            CmdAttack();
        }
    }

    [Command]
    void CmdAttack()
    {
        RpcAttack();
    }

    //###################### RPC CALLS #####################################
    //VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    [ClientRpc]
    void RpcAttack()
    {
        sword.PlayAnimation();
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
    }
    void OnChangeSkill2_Debuffed(bool state)
    {
        skill2_Debuffed = state;
    }
    void OnChangeSkill3_Healed(bool state)
    {
        skill3_Healed = state;
    }
}
