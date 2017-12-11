using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EndConditions : NetworkBehaviour
{
	public bool hasToKillAllEnemies = true;
	public List<Enemy> enemiesToKill = new List<Enemy>();
	public bool gameEnded = false;
	public EndScreenUI endScreenUI;

    void Start()
    {
		if (FindObjectOfType<Master> ())//.gameObject.activeInHierarchy)
			endScreenUI = FindObjectOfType<Master> ().vrEndScreenUI;

		if (!isServer)
			return;

        NetworkServer.Spawn(gameObject);

        if (hasToKillAllEnemies && enemiesToKill.Count == 0)
			Debug.Log ("SERVER: No enemy list! Win guaranteed...");
	}

	public void MarkEnemyKilled(Enemy e)
    {
		if (!isServer)
			return;
		
		if (enemiesToKill.Contains(e))
        {
			enemiesToKill.Remove(e);
			CheckEndCondition();
		}
	}

	public void CheckEndCondition()
    {
		if (gameEnded || !isServer)
			return;
		
		bool anyCrawlerAlive = false;
		foreach (Transform c in FindObjectOfType<PlayersManager>().players)
        {
			if (!c.GetComponentInChildren<Crawler>().isDead)
            {
				anyCrawlerAlive = true;
				break;
			}
		}
		if (!anyCrawlerAlive)
        {
			gameEnded = true;
			RpcTriggerLOSE();
			return;
		}
		
		if (enemiesToKill.Count == 0)
        {
			gameEnded = true;
			RpcTriggerWIN();
			return;
		}
	}

	[ClientRpc]
	void RpcTriggerWIN()
    {
		Debug.Log("RPC: All enemies dead, WON!");
		endScreenUI.gameObject.SetActive(true);
		endScreenUI.SetEndScreen (true);

		if (FindObjectOfType<Master> ()) {
			FindObjectOfType<SteamVR_LaserPointer> ().enabled = true;
		}
	}

	[ClientRpc]
	void RpcTriggerLOSE()
    {
		Debug.Log ("RPC: All crawlers dead, LOSE!");
		endScreenUI.gameObject.SetActive (true);
		endScreenUI.SetEndScreen (false);

		if (FindObjectOfType<Master> ()) {
			FindObjectOfType<SteamVR_LaserPointer> ().enabled = true;
		}
	}
}
