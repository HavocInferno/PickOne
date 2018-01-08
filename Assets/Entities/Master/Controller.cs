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

    private int _currentItem =-1, lastItem =-1;
	private bool _radialMenuAccessed = false;
	private Text[] texts;
    private Color currentColor;
	private Vector3 currentScale;

	public int currentItem{ get{ return _currentItem;}}
	public bool radialMenuAccessed{get{return _radialMenuAccessed;}}

	// Use this for initialization
    void Start () {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        UI.SetActive(false);
		initRadialMenu ();
    }
		
	// Update is called once per frame
	void Update () {
		pollRadialMenu ();
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
		hapticFeedback (hapticforce);
		UI.SetActive (true);
		_radialMenuAccessed = true;
	}

	void closeRadialMenu ()
	{
		hapticFeedback (hapticforce);
		UI.SetActive (false);
		_radialMenuAccessed = false;
	}

	void updateRadialMenu ()
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

	private int getCurrentRadialMenuItemIndex()
	{
		return (int) ((items.Length-(((Mathf.Atan2(trackpad.y, trackpad.x) / Mathf.PI * 180)+270-(360/items.Length/2))%360) / (360 / items.Length)));
	}

	public void hapticFeedback (ushort hapticforce)
	{
		device.TriggerHapticPulse (hapticforce);
	}
	public bool getTrigger()
	{
		return device.GetPress (SteamVR_Controller.ButtonMask.Trigger);
	}
	public bool getTriggerDown()
	{
		return device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger);
	}	
	public bool getTriggerUp()
	{
		return device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger);
	}
	public bool getGrip()
	{
		return device.GetPress (SteamVR_Controller.ButtonMask.Grip);
	}
	public bool getGripDown()
	{
		return device.GetPressDown (SteamVR_Controller.ButtonMask.Grip);
	}	
	public bool getGripUp()
	{
		return device.GetPressUp (SteamVR_Controller.ButtonMask.Grip);
	}



}