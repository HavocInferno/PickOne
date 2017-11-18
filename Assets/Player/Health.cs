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
				// existing Respawn code      
				currentHealth = maxHealth;

				// called on the Server, but invoked on the Clients
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