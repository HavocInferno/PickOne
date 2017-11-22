using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CrawlerController: NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float fireRate = 0.15f;
	private float lastFire;
    public float bulletSpeed = 16f;

    public GameObject swordAttackPrefab;

	public Text nameTag;

	[SyncVar(hook = "OnChangeName")]
	public string pName = "player";

	[SyncVar]
	public Color playerColor = Color.white;

	[SyncVar]
	public bool isVRMasterPlayer = false;

	public Vector2 movSpeed = new Vector2(4f,4f);

	void Update()
	{
		if (!isLocalPlayer)
			return;

		//weapon firing. dumb and unoptimized.
		if (Input.GetButtonDown("Fire1") && Time.time > lastFire)
		{
			lastFire = Time.time + fireRate;

            if (GetComponentInChildren<Sword>() != null)
                Destroy(GetComponentInChildren<Sword>().gameObject);

			CmdAttack();
		}

		//player movement..hor is forward/backward, ver is strafing
		var hor = Input.GetAxis("Horizontal") * Time.deltaTime * movSpeed.x;
		var ver = Input.GetAxis("Vertical") * Time.deltaTime * movSpeed.y;
		transform.Translate(hor, 0, ver);
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
		if (isVRMasterPlayer) {
            UnityEngine.VR.VRSettings.enabled = true;
            FindObjectOfType<CamManager> ().nonVRCamera.SetActive(false);
			FindObjectOfType<CamManager> ().vrCamera.SetActive(true);
			transform.position = new Vector3 (transform.position.x, transform.position.y + 1f, transform.position.z);
			pName = pName + " (VR MASTER)";
        } 
		/*if not, do:
		 * disable VR if not done already
		 * enable the default camera
		 * disable the OpenVR cam rig
		 * set this gameObject as the main cam target */
		else {
            UnityEngine.VR.VRSettings.enabled = false;
            FindObjectOfType<CamManager> ().vrCamera.SetActive(false);
			FindObjectOfType<CamManager> ().nonVRCamera.SetActive(true);
			Camera.main.GetComponent<DungeonCam> ().target = this.gameObject;
        }
	}

	void Start() {
		this.gameObject.name = pName;
		GetComponent<MeshRenderer>().material.color = playerColor;
		nameTag.text = pName;

		//scale up the player object if this is the VR master [this is a temporary visualisation, to be removed once a proper Master representation is done]
		if (isVRMasterPlayer) {
			transform.localScale *= 2f;
		}

		//on the server, add yourself to the level-wide player list
		if (isServer) {
			Debug.Log (pName + " is here.");
			if(!isVRMasterPlayer)
                FindObjectOfType<playerlist> ().players.Add (transform);
		}
	}

	void OnChangeName(string newName) {
		nameTag.text = newName;
	}

	// This [Command] code is called on the Client …
	// … but it is run on the Server!
	[Command]
	void CmdFire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate (
			bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

		NetworkServer.Spawn (bullet);

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}

    [Command]
    void CmdAttack()
    {
        // Instantiate the sword attack prefab
        var sword = Instantiate(
            swordAttackPrefab, 
            gameObject.transform
            );

        NetworkServer.Spawn(sword);

        // Sword destroys itself
    }
}