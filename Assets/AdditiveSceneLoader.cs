using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour {

	public int sceneBuildIndex;

	// Use this for initialization
	void Start () {
		//SceneManager.LoadSceneAsync (sceneBuildIndex);
		SceneManager.LoadScene (sceneBuildIndex, LoadSceneMode.Additive);
	}
}
