using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Controller : MonoBehaviour {

    
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
	private Text[] texts;
    private Color currentColor;
    private Vector3 currentScale;


	//buff test
	public float maxRayOffset = 10;
	public Transform rayOrigin; 
	public Transform[] buffTargets;
	public BezierCurve buffRay;
	public int currentBuffTarget = -1;
	public float raySpeed = 30; 



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
			
		switch (currentItem) {
		case 0: 
			applyBuff ();
			break;
		}
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
	}

	void closeRadialMenu ()
	{
		device.TriggerHapticPulse (hapticforce);
		UI.SetActive (false);
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
		if (device.GetPress (SteamVR_Controller.ButtonMask.Trigger)) {
			int closest = -1;
			float closestdistance = maxRayOffset;
			for (int i = 0; i < buffTargets.Length; i++) {
				if (Vector3.Cross (rayOrigin.forward, buffTargets [i].position - rayOrigin.position).magnitude < closestdistance && Vector3.Dot (rayOrigin.forward, buffTargets [i].position - rayOrigin.position) > 0.1) {
					closestdistance = Vector3.Cross (rayOrigin.forward, buffTargets [i].position - rayOrigin.position).magnitude;
					closest = i;
				}
			}
			if (closest != -1) {
				if (buffRay.Draw == false)
					buffRay.Draw = true;
				buffRay.destination = Vector3.Lerp (buffRay.destination, buffTargets [closest].position, Time.deltaTime * raySpeed);
				if (closest == currentBuffTarget)
					device.TriggerHapticPulse ((ushort)(1000 * Mathf.Pow (Vector3.Cross (rayOrigin.forward, buffTargets [closest].position - rayOrigin.position).magnitude / maxRayOffset, 2)));
				else {
					device.TriggerHapticPulse (hapticforce);
					currentBuffTarget = closest;
				}
			}
			else
				buffRay.Draw = false;
		}
		//Draw bezierCurve
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger))
			buffRay.Draw = true;
		if (device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger))
			buffRay.Draw = false;
	}

	void initRays ()
	{
		buffRay.Draw = false;
		buffRay.origin = rayOrigin;
	}
}
