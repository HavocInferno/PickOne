using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Health : NetworkBehaviour {

	public const int maxHealth = 100;

	public bool destroyOnDeath;
	public GameObject deathEffect;

	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;

	public RectTransform healthBar;

	private NetworkStartPosition[] spawnPoints;

	void Start ()
	{
		if (isLocalPlayer)
		{
			spawnPoints = FindObjectsOfType<NetworkStartPosition>();
		}
	}

	/* dmg handling, supposed to be entirely server-side for core data
	 * if the health of the unit associated with this script reaches 0, 
	 * it is either destroyed, or
	 * respawned at any random spawn point of the available ones
	 * if a death effect is specified for this unit, network spawn it at the hitpoint and facing the hitdirection [note, this can be done without network load, 
	 * 																												but requires sort of duplicate code to be executed 
	 * 																												once on the host and on the client (since for some reason 
	 * 																												Unity doesnt wanna consider a host a client in this specific situation)]
	*/
	public void TakeHit(int amount, Vector3 hitPoint, Vector3 hitDirection)
	{
		if (!isServer)
			return;

		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			if (destroyOnDeath) {
				Destroy (gameObject);
			} else {
				currentHealth = maxHealth;

				// Rpc ==> called on the Server, but invoked on the Clients
				RpcRespawn ();
			}

			if (deathEffect != null) {
				var ded = (GameObject)Instantiate (
					deathEffect,
					hitPoint,
					Quaternion.LookRotation(hitDirection));

				NetworkServer.Spawn (ded);

				// Destroy the effect after 2.15 seconds
				Destroy(ded, 2.15f);
			}
		}
	}

	//invoked whenever the currenthealth syncvar is changed, so the UI healthbar is appropriately resized
	void OnChangeHealth (int health)
	{
		healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
	}

	[ClientRpc]
	void RpcRespawn()
	{
		if (isLocalPlayer)
		{
			// Set the spawn point to origin as a default value
			Vector3 spawnPoint = Vector3.zero;

			// If there is a spawn point array and the array is not empty, pick a spawn point at random
			if (spawnPoints != null && spawnPoints.Length > 0)
			{
				spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
			}

			// Set the player’s position to the chosen spawn point
			transform.position = spawnPoint;
		}
	}
}