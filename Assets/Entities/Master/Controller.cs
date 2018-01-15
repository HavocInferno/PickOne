using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
	protected SteamVR_TrackedObject trackedObject;
	protected SteamVR_Controller.Device device;
	protected Vector2 trackpad;

    //radial menu
	[SerializeField]
	protected float itemDistance = 15;
	[SerializeField]
	protected ushort hapticforce = 3999;
	[SerializeField]
	protected float highlightPopScale = 1.6f;
	[SerializeField]
	protected float highlightScale = 1.3f;
	[SerializeField]
	protected float highlightSpeed = 10f;
	[SerializeField]
	protected Color highlightPopColor = Color.white;
	[SerializeField]
	protected Color highlightColor = Color.blue;
	[SerializeField]
	protected string[] items = {"Buff", "Debuff"};
	[SerializeField]
	protected GameObject UI;
	[SerializeField]
	protected GameObject textPrefab;
	[SerializeField]

	protected int _currentItem =-1, lastItem =-1;
	protected bool _radialMenuAccessed = false;
	protected Text[] texts;
	protected Color currentColor;
	protected Vector3 currentScale;

	public int currentItem{ get{ return _currentItem;}}
	public bool radialMenuAccessed{get{return _radialMenuAccessed;}}

	// Use this for initialization
    public void Start () {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        UI.SetActive(false);
		initRadialMenu ();
		device = SteamVR_Controller.Input ((int)trackedObject.index);
    }
		
	// Update is called once per frame
	public void Update () {
		pollRadialMenu ();
	}

	protected void initRadialMenu ()
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

	protected void pollRadialMenu ()
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

	protected void openRadialMenu ()
	{
		hapticFeedback (hapticforce);
		UI.SetActive (true);
		_radialMenuAccessed = true;
	}

	protected void closeRadialMenu ()
	{
		hapticFeedback (hapticforce);
		UI.SetActive (false);
		_radialMenuAccessed = false;
	}

	protected void updateRadialMenu ()
	{
		lastItem = _currentItem;
		_currentItem = getCurrentRadialMenuItemIndex ();
		if (_currentItem != lastItem) {
			if (lastItem != -1)
				texts [lastItem].color = currentColor;
			currentColor = texts [_currentItem].color;
			texts [_currentItem].color = highlightPopColor;
			if (lastItem != -1)
				texts [lastItem].rectTransform.localScale = currentScale;
			currentScale = texts [_currentItem].rectTransform.localScale;
			texts [_currentItem].transform.localScale *= highlightPopScale;
			hapticFeedback (hapticforce);
		}
		texts [_currentItem].rectTransform.localScale = Vector3.Lerp (texts [_currentItem].rectTransform.localScale, highlightScale * currentScale, Time.deltaTime * highlightSpeed);
		texts [_currentItem].color = Color.Lerp (texts [_currentItem].color, highlightColor, Time.deltaTime * highlightSpeed);
	}

	protected virtual int getCurrentRadialMenuItemIndex()
	{
		return (int) ((items.Length-(((Mathf.Atan2(trackpad.y, trackpad.x) / Mathf.PI * 180)+270-(360/items.Length/2))%360) / (360 / items.Length)));
	}

	public void hapticFeedback (ushort hapticforce)
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		device.TriggerHapticPulse (hapticforce);
	}
	public bool getTrigger()
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		return device.GetPress (SteamVR_Controller.ButtonMask.Trigger);
	}
	public bool getTriggerDown()
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		return device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger);
	}	
	public bool getTriggerUp()
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		return device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger);
	}
	public bool getGrip()
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		return device.GetPress (SteamVR_Controller.ButtonMask.Grip);
	}
	public bool getGripDown()
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		return device.GetPressDown (SteamVR_Controller.ButtonMask.Grip);
	}	
	public bool getGripUp()
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		return device.GetPressUp (SteamVR_Controller.ButtonMask.Grip);
	}



}