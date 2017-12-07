using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;
    private Vector2 trackpad;

    //radial menu
	[SerializeField]
	private float itemDistance = 15;
	[SerializeField]
	private ushort hapticforce = 3999;
	[SerializeField]
	private float highlightPopScale = 1.6f;
	[SerializeField]
	private float highlightScale = 1.3f;
	[SerializeField]
	private float highlightSpeed = 10f;
	[SerializeField]
	private Color highlightPopColor = Color.white;
	[SerializeField]
	private Color highlightColor = Color.blue;
	[SerializeField]
	private string[] items = {"Buff", "Debuff"};
	[SerializeField]
	private GameObject UI;
	[SerializeField]
	private GameObject textPrefab;
	[SerializeField]

    private int currentItem =-1, lastItem =-1;
	private bool radialMenuAccessed = false;
	private Text[] texts;
    private Color currentColor;
    private Vector3 currentScale;


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
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        UI.SetActive(false);
        
		initRadialMenu ();
		initRays ();
    }
		
	// Update is called once per frame
	void Update () {

		pollRadialMenu ();
		applyBuff ();
		applyDebuff ();
	}

	void initRadialMenu ()
	{
		GameObject tempGameObject;
		texts = new Text[items.Length];
		for (int i = 0; i < items.Length; i++) {
			tempGameObject = Instantiate (textPrefab, UI.transform);
			float angle = (360 / items.Length) * i;
			tempGameObject.transform.localPosition = new Vector3 (Mathf.Sin (angle / 180 * Mathf.PI), Mathf.Cos (angle / 180 * Mathf.PI), 0) * itemDistance;
			texts [i] = tempGameObject.GetComponent<Text> ();
			texts [i].text = items [i];
		}
		currentColor = texts [0].color;
	}

	void pollRadialMenu ()
	{
		device = SteamVR_Controller.Input ((int)trackedObject.index);
		trackpad = device.GetAxis ();
		if (trackpad.x != 0 || trackpad.y != 0) {
			trackpad.Normalize ();
		}
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Touchpad)) {
			openRadialMenu ();
		}
		if (device.GetPress (SteamVR_Controller.ButtonMask.Touchpad)) {
			updateRadialMenu ();
		}
		if (device.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad)) {
			closeRadialMenu ();
		}
	}

	void openRadialMenu ()
	{
		device.TriggerHapticPulse (hapticforce);
		UI.SetActive (true);
		radialMenuAccessed = true;
	}

	void closeRadialMenu ()
	{
		device.TriggerHapticPulse (hapticforce);
		UI.SetActive (false);
		radialMenuAccessed = false;
	}

	void updateRadialMenu ()
	{
		lastItem = currentItem;
		currentItem = getCurrentRadialMenuItemIndex ();
		if (currentItem != lastItem) {
			if (lastItem != -1)
				texts [lastItem].color = currentColor;
			currentColor = texts [currentItem].color;
			texts [currentItem].color = highlightPopColor;
			if (lastItem != -1)
				texts [lastItem].rectTransform.localScale = currentScale;
			currentScale = texts [currentItem].rectTransform.localScale;
			texts [currentItem].transform.localScale *= highlightPopScale;
			device.TriggerHapticPulse (hapticforce);
		}
		texts [currentItem].rectTransform.localScale = Vector3.Lerp (texts [currentItem].rectTransform.localScale, highlightScale * currentScale, Time.deltaTime * highlightSpeed);
		texts [currentItem].color = Color.Lerp (texts [currentItem].color, highlightColor, Time.deltaTime * highlightSpeed);
	}

	private int getCurrentRadialMenuItemIndex()
	{
		return (int) ((items.Length-(((Mathf.Atan2(trackpad.y, trackpad.x) / Mathf.PI * 180)+270-(360/items.Length/2))%360) / (360 / items.Length)));
	}

	void applyBuff ()
	{
		//if we are not supposed to buff or the radial menu is open, don't buff
		if (currentItem != 0 || radialMenuAccessed) {
			if (buffing)
				stopBuffing ();
			return;
		}

		if (device.GetPress (SteamVR_Controller.ButtonMask.Trigger)) {
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
					device.TriggerHapticPulse ((ushort)(1000 * Mathf.Pow (Vector3.Cross (rayOrigin.forward, playerManager.players [closest].position - rayOrigin.position).magnitude / maxRayOffset, 2)));
				else {
					//new target
					if (currentBuffTarget != -1)
						stopBuffing ();
					currentBuffTarget = closest;
					startBuffing ();
					device.TriggerHapticPulse (hapticforce);
				}
				//for some weird reason we might not be buffing at this stage
				if (!buffing)
					startBuffing();
			}
			else
                stopBuffing();
		}
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            stopBuffing();
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
		if (currentItem != 1 || radialMenuAccessed) {
			if (debuffing)
				stopDebuffing ();
			return;
		}

		if (device.GetPress (SteamVR_Controller.ButtonMask.Trigger) && currentItem == 1) {
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
					device.TriggerHapticPulse ((ushort)(1000 * Mathf.Pow (Vector3.Cross (rayOrigin.forward, playerManager.enemies [closest].position - rayOrigin.position).magnitude / maxRayOffset, 2)));
				else {
                    //new target
					if (currentDebuffTarget != -1)
						stopDebuffing (); 
                    currentDebuffTarget = closest;
					startDebuffing ();
					Debug.Log("New enemy target! closest: " + closest +", currentDebuffTarget: "+currentDebuffTarget);
					device.TriggerHapticPulse(hapticforce);
				}
				if (!debuffing)
					startDebuffing();
			}
			else
                stopDebuffing();
        }
        if (device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger))
            stopDebuffing();
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
