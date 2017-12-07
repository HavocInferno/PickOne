using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Master : MonoBehaviour {

	[SerializeField]
	private ushort hapticforce = 3999;
	[SerializeField]
	Controller mainHand, offHand;

	//Buff/Debuff Variables
	[SerializeField]
	private float maxRayOffset = 10;
	public Transform rayOrigin; 
	[SerializeField]
	private PlayersManager playerManager;
	[SerializeField]
	private float raySpeed = 30;

	[SerializeField]
	private BezierCurve buffRay;
	[SerializeField]
	private int currentBuffTarget = -1;

	[SerializeField]
	private BezierCurve debuffRay;	
	[SerializeField]
	private int currentDebuffTarget = -1;

	[SerializeField]
	private AbstractEffect buffEffect;
	[SerializeField]
	private AbstractEffect debuffEffect;

	public Vector3 buffDestination;
	public bool buffing = false;
	public Vector3 debuffDestination;
	public bool debuffing = false;
	// Use this for initialization

	void Start () {
		initRays ();
	}
	
	// Update is called once per frame
	void Update () {
		applyBuff ();
		applyDebuff ();
	}

	void applyBuff ()
	{
		//if we are not supposed to buff or the radial menu is open, don't buff
		if (mainHand.currentItem != 0 || mainHand.radialMenuAccessed) {
			if (buffing)
				stopBuffing ();
			return;
		}

		if (mainHand.getTrigger()) {
			int closest = -1;
			float closestdistance = maxRayOffset;
			for (int i = 0; i < playerManager.players.Count; i++) {
				if (playerManager.players [i] == null)
					continue;
				if (Vector3.Cross (rayOrigin.forward, playerManager.players [i].position - rayOrigin.position).magnitude < closestdistance && Vector3.Dot (rayOrigin.forward, playerManager.players [i].position - rayOrigin.position) > 0.1) {
					closestdistance = Vector3.Cross (rayOrigin.forward, playerManager.players [i].position - rayOrigin.position).magnitude;
					closest = i;
				}
			}
			if (closest != -1) {
				buffDestination = Vector3.Lerp (buffDestination, playerManager.players [closest].position, Time.deltaTime * raySpeed);
				buffRay.destination = buffDestination; 
				if (closest == currentBuffTarget)
					mainHand.hapticFeedback ((ushort)(1000 * Mathf.Pow (Vector3.Cross (rayOrigin.forward, playerManager.players [closest].position - rayOrigin.position).magnitude / maxRayOffset, 2)));
				else {
					//new target
					if (currentBuffTarget != -1)
						stopBuffing ();
					currentBuffTarget = closest;
					startBuffing ();
					mainHand.hapticFeedback (hapticforce);
				}
				//for some weird reason we might not be buffing at this stage
				if (!buffing)
					startBuffing();
			}
			else
				stopBuffing();
		}
		if (mainHand.getTriggerUp ()) 
			stopBuffing ();
	}

	private void startBuffing()
	{
		if (currentBuffTarget != -1 && !buffing)
		{
			buffing = true;
			buffRay.Draw = true;
			playerManager.players[currentBuffTarget].GetComponent<Crawler>().EnableEffect(buffEffect);
		}
	}

	private void stopBuffing()
	{
		if (currentBuffTarget != -1 && buffing && playerManager.players[currentBuffTarget]!= null)
			playerManager.players[currentBuffTarget].GetComponent<Crawler>().DisableEffect(buffEffect);
		buffing = false;
		buffRay.Draw = false;
	}

	void applyDebuff ()
	{
		if (mainHand.currentItem != 1 || mainHand.radialMenuAccessed) {
			if (debuffing)
				stopDebuffing ();
			return;
		}

		if (mainHand.getTrigger() && mainHand.currentItem == 1) {
			int closest = -1;
			float closestdistance = maxRayOffset;
			for (int i = 0; i < playerManager.enemies.Count; i++) {
				if (playerManager.enemies [i] == null)
					continue;
				if (Vector3.Cross (rayOrigin.forward, playerManager.enemies [i].position - rayOrigin.position).magnitude < closestdistance && Vector3.Dot (rayOrigin.forward, playerManager.enemies [i].position - rayOrigin.position) > 0.1) {
					closestdistance = Vector3.Cross (rayOrigin.forward, playerManager.enemies [i].position - rayOrigin.position).magnitude;
					closest = i;
				}
			}
			if (closest != -1) {
				debuffDestination = Vector3.Lerp (debuffDestination, playerManager.enemies [closest].position, Time.deltaTime * raySpeed);
				debuffRay.destination = debuffDestination; 
				if (closest == currentDebuffTarget)
					mainHand.hapticFeedback ((ushort)(1000 * Mathf.Pow (Vector3.Cross (rayOrigin.forward, playerManager.enemies [closest].position - rayOrigin.position).magnitude / maxRayOffset, 2)));
				else {
					//new target
					if (currentDebuffTarget != -1)
						stopDebuffing (); 
					currentDebuffTarget = closest;
					startDebuffing ();
					Debug.Log("New enemy target! closest: " + closest +", currentDebuffTarget: "+currentDebuffTarget);
					mainHand.hapticFeedback(hapticforce);
				}
				if (!debuffing)
					startDebuffing();
			}
			else
				stopDebuffing();
		}
		if (mainHand.getTriggerUp ()) 
			stopDebuffing ();
		
	}

	private void startDebuffing()
	{
		if (currentDebuffTarget != -1 && !debuffing)
		{
			debuffing = true;
			debuffRay.Draw = true;
			playerManager.enemies[currentDebuffTarget].GetComponent<Enemy>().EnableEffect(debuffEffect);
		}
	}

	private void stopDebuffing()
	{
		if (currentDebuffTarget != -1 && debuffing && playerManager.enemies[currentDebuffTarget]!= null)
			playerManager.enemies[currentDebuffTarget].GetComponent<Enemy>().DisableEffect(debuffEffect);
		debuffing = false;
		debuffRay.Draw = false;
	}

	void initRays ()
	{
		debuffDestination = buffDestination = rayOrigin.position;
		debuffRay.origin = buffRay.origin = rayOrigin;
		playerManager = GameObject.Find ("PlayerManagers").GetComponent<PlayersManager>();
	}
}
