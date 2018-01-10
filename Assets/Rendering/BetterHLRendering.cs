using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterHLRendering : MonoBehaviour {

	public void EnableHighlightAll() {
		foreach (Transform tfE in FindObjectOfType<PlayersManager>().enemies) {
			tfE.gameObject.GetComponent<BetterHighlighter>().EnableHL();
		}
	}

	public void DisableHighlightAll() {
		foreach (Transform tfE in FindObjectOfType<PlayersManager>().enemies) {
			tfE.gameObject.GetComponent<BetterHighlighter>().DisableHL();
		}
	}
}
