using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
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
	 * if a death effect is specified for this unit, 
	 * 		network spawn it at the hitpoint and facing the hitdirection 
	 * 		[note, this can be done without network load, 
	 *		but requires sort of duplicate code to be executed 
	 * 		once on the host and on the client (since for some reason 
	 * 		Unity doesnt wanna consider a host a client in this specific situation)]
	*/
	public void TakeHit(int amount, Vector3 hitPoint, Vector3 hitDirection)
	{
		if (!isServer)
			return;

		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			if (destroyOnDeath)
            {
				FindObjectOfType<EndConditions>().MarkEnemyKilled(gameObject.GetComponent<Enemy>());
				Destroy(gameObject);
			}
            else
            {
                gameObject.GetComponent<Crawler>().isDead = true;
				FindObjectOfType<EndConditions>().CheckEndCondition();

				//currentHealth = maxHealth;
			}

			if (deathEffect != null)
            {
				var ded = Instantiate(
					deathEffect,
					hitPoint,
					Quaternion.LookRotation(hitDirection));

				NetworkServer.Spawn(ded);

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
	void RpcDie()
	{
		gameObject.GetComponentInChildren<Crawler>().isDead = true;
	}
}