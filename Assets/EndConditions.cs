using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EndConditions : NetworkBehaviour {


	public bool hasToKillAllEnemies = true;
	public List<Enemy> enemiesToKill;
	public bool gameEnded = false;

	void Start() {
		if (!isServer)
			return;
		
		if (hasToKillAllEnemies && enemiesToKill.Count == 0)
			Debug.Log ("SERVER: No enemy list! Win guaranteed...");
	}

	public void markEnemyKilled(Enemy e) {
		if (!isServer)
			return;
		
		if (enemiesToKill.Contains (e)) {
			enemiesToKill.Remove (e);
			checkEndCondition ();
		}
	}

	public void checkEndCondition() {
		if (gameEnded)
			return;
		
		bool anyCrawlerAlive = false;
		foreach (Transform c in FindObjectOfType<PlayersManager>().players) {
			if (!c.GetComponentInChildren<Crawler> ().isDead) {
				anyCrawlerAlive = true;
				break;
			}
		}
		if (!anyCrawlerAlive) {
			gameEnded = true;
			RpcTriggerLOSE ();
			return;
		}
		
		if (enemiesToKill.Count == 0) {
			gameEnded = true;
			RpcTriggerWIN ();
			return;
		}
	}

	[ClientRpc]
	void RpcTriggerWIN() {
		Debug.Log ("RPC: All enemies dead, WON!");
	}

	[ClientRpc]
	void RpcTriggerLOSE() {
		Debug.Log ("RPC: All crawlers dead, LOSE!");
	}
}
