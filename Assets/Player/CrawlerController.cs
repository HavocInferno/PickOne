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

	void Update()
	{
		if (!isLocalPlayer) {
			return;
		}

		if (Input.GetButton("Fire1") && Time.time > lastFire)
		{
			lastFire = Time.time + fireRate;
			CmdAttack();
		}

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);
	}

	public override void OnStartLocalPlayer()
	{
		if (isVRMasterPlayer) {
			FindObjectOfType<CamManager> ().nonVRCamera.SetActive(false);
			FindObjectOfType<CamManager> ().vrCamera.SetActive(true);
			transform.position = new Vector3 (transform.position.x, transform.position.y + 1f, transform.position.z);
			pName = pName + " (VR MASTER)";
		} else {
			FindObjectOfType<CamManager> ().vrCamera.SetActive(false);
			FindObjectOfType<CamManager> ().nonVRCamera.SetActive(true);
			Camera.main.GetComponent<DungeonCam> ().target = this.gameObject;
		}
	}

	void Start() {
		this.gameObject.name = pName;
		GetComponent<MeshRenderer>().material.color = playerColor;
		nameTag.text = pName;

		if (isVRMasterPlayer) {
			transform.localScale *= 2f;
		}

		if (isServer) {
			Debug.Log (pName + " is here.");
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

        // Destroy sword after 1 sec
        Destroy(sword, 1f);
    }
}