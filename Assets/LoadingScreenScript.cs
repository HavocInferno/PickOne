using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenScript : MonoBehaviour
{
	public Image background;
    public Text loadingText;
    public Text flashingText;
	bool fade = false;
	float fadeTime = 1f;

    void Update()
    {
        // Flash the loading text
		if(!fade)
        	flashingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
    }

	public void FadeOut() {
		fade = true;
		background.CrossFadeColor (new Color (background.color.r, background.color.g, background.color.b, 0f), fadeTime, false, true);
		loadingText.CrossFadeColor (new Color (loadingText.color.r, loadingText.color.g, loadingText.color.b, 0f), fadeTime, false, true);
		flashingText.CrossFadeColor (new Color (flashingText.color.r, flashingText.color.g, flashingText.color.b, 0f), fadeTime, false, true);
		StartCoroutine (DisableAfter (fadeTime));
	}

	IEnumerator DisableAfter(float seconds) {
		yield return new WaitForSeconds (seconds);
		gameObject.SetActive (false);
	}
}
