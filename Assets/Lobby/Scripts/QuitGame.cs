using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour {

	public void OnQuitClicked()
	{
		Debug.Log ("Quitting application");
		Application.Quit ();
	}
}
