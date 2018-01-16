using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHoverInfo : MonoBehaviour {

	public GameObject hoverSub;

	public void OnHoverEnter() {
		hoverSub.SetActive (true);
	}
	public void OnHoverExit() {
		hoverSub.SetActive (false);
	}

	public void EnableHover() {
		gameObject.GetComponent<EventTrigger> ().enabled = true;
	}
	public void DisableHover() {
		gameObject.GetComponent<EventTrigger> ().enabled = false;
	}
}
