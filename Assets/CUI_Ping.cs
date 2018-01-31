using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_Ping : MonoBehaviour {

	public Image iconBase;

	public Sprite iconAvailable;
	public Sprite iconUsed;

	public void TogglePingIcon(bool state) {
		if(iconBase && iconAvailable && iconUsed)
			iconBase.sprite = (state ? iconAvailable : iconUsed);
	}
}
