using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassController : Controller {

	// Use this for initialization
	void Start () {
		base.Start ();
	}

	// Update is called once per frame
	void Update () {
		updateRadialMenu ();
		pollRadialMenu ();
	}

	void updateRadialMenu ()
	{
		for (int i = 0; i < items.Length; i++) {
			float angle = ((360 / items.Length) * i) - transform.rotation.eulerAngles.y;
			texts [i].gameObject.transform.localPosition = new Vector3 (Mathf.Sin (angle / 180 * Mathf.PI), Mathf.Cos (angle / 180 * Mathf.PI), 0) * itemDistance;
		}
	}
	protected override int getCurrentRadialMenuItemIndex()
	{
		if (trackPadMagnitude < selectMagnitude)
			return -1;
		else
			return (int) ((items.Length-(((Mathf.Atan2(trackpad.y, trackpad.x) / Mathf.PI * 180)- transform.rotation.eulerAngles.y+630-(360/items.Length/2))%360) / (360 / items.Length)))%items.Length;
	}
}
