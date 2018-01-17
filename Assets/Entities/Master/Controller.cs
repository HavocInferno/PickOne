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
	protected float itemDistance = 50;
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
	protected Color[] Colors = new Color[0];
	[SerializeField]
	protected float selectMagnitude = .3f;
	[SerializeField]
	protected GameObject UI;
	[SerializeField]
	protected GameObject textPrefab;
	[SerializeField]

	protected float trackPadMagnitude;
	protected int _currentItem =-1, lastItem =-1;
	protected bool _radialMenuAccessed = false;
	protected Text[] texts;
	protected Color[] colors;
	protected Vector3[] scales;

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
		colors = new Color[items.Length];
		texts = new Text[items.Length];
		scales = new Vector3[items.Length]; 
		for (int i = 0; i < items.Length; i++) {
			tempGameObject = Instantiate (textPrefab, UI.transform);
			float angle = (360 / items.Length) * i + 45;
			tempGameObject.transform.localPosition = new Vector3 (Mathf.Sin (angle / 180 * Mathf.PI), Mathf.Cos (angle / 180 * Mathf.PI), 0) * itemDistance;
			texts [i] = tempGameObject.GetComponent<Text> ();
			texts [i].text = items [i];
			foreach (var tex in tempGameObject.GetComponentsInChildren<Text>())
				tex.text = items [i];
			if(i<Colors.Length){
				texts [i].color = Colors [i];
			}
			colors[i] = texts [i].color;
			scales [i] = texts [i].rectTransform.localScale;
		}

	}

	protected void pollRadialMenu ()
	{
		device = SteamVR_Controller.Input ((int)trackedObject.index);
		trackpad = device.GetAxis ();
		trackPadMagnitude = trackpad.magnitude;
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
			if (currentItem != -1) {
				texts [_currentItem].transform.localScale *= highlightPopScale;
				texts [_currentItem].color = highlightPopColor;
			}
			hapticFeedback (hapticforce);
		}
		for (int i = 0; i < items.Length; i++) {
			if (currentItem == i) {
				texts [i].rectTransform.localScale = Vector3.Lerp (texts [i].rectTransform.localScale, highlightScale * scales [i], Time.deltaTime * highlightSpeed);
				texts [i].color = Color.Lerp (texts [i].color, Color.Lerp(Color.white, colors[i], 0.5f), Time.deltaTime * highlightSpeed);
			} else {
				texts [i].rectTransform.localScale = Vector3.Lerp (texts [i].rectTransform.localScale, scales [i], Time.deltaTime * highlightSpeed);
				texts [i].color = Color.Lerp (texts [i].color, colors [i], Time.deltaTime * highlightSpeed);
			}
		}
	}

	protected virtual int getCurrentRadialMenuItemIndex()
	{
		if (trackPadMagnitude < selectMagnitude)
			return -1;
		else
			return (int) ((items.Length-(((Mathf.Atan2(trackpad.y, trackpad.x) / Mathf.PI * 180)+315-(360/items.Length/2))%360) / (360 / items.Length)));
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
	public bool getTouchpad()
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		return device.GetPress (SteamVR_Controller.ButtonMask.Touchpad);
	}
	public bool getTouchpadDown()
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		return device.GetPressDown (SteamVR_Controller.ButtonMask.Touchpad);
	}	
	public bool getTouchpadUp()
	{
		if(trackedObject == null)
			trackedObject = GetComponent<SteamVR_TrackedObject>();
		if(device == null)
			device = SteamVR_Controller.Input ((int)trackedObject.index);
		return device.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad);
	}

	public void vibrateFrequently(ushort hapticforce,float vibrateFrequency)
	{
		if(Mathf.Sin(Time.time*Mathf.PI*2* vibrateFrequency) > 0)
			hapticFeedback (hapticforce);
	}

}