using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_lowStat : MonoBehaviour {

	public float fadeTime = 1f;

	[System.Serializable]
	private struct AttributeInfo
	{
		public string name;
	}

	[SerializeField]
	GameObject elementPrefab;

	[SerializeField]
	List<AttributeInfo> attributes = new List<AttributeInfo>();

	public Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();
	public Dictionary<string, bool> states = new Dictionary<string, bool>();

	public void Register(Stats stats)
	{
		foreach (var attributeInfo in attributes)
		{
			if (stats.HasAttribute(attributeInfo.name))
			{
				GameObject instance = Instantiate(elementPrefab, transform);
				instance.name += "_" + attributeInfo.name;
				panels.Add (attributeInfo.name, instance);
				states.Add (attributeInfo.name, false);
				stats.attributes [attributeInfo.name].lowS = this;
			}
		}
	}

	/*
	void Update () {
		if (show) {
			imageCol = Color.Lerp (imageCol, Color.white, Time.deltaTime * fadeSpeed);
		} else {
			imageCol= Color.Lerp (imageCol, Color.clear, Time.deltaTime * fadeSpeed);
		}
	}
	*/

	public void UpdateValues(float newVal, float newMax, string name) {
		if (newVal <= 0.2f * newMax) {
			if (!states [name]) {
				Debug.Log ("HP low");
				GameObject panel = panels [name];
				panel.GetComponent<Image> ().CrossFadeAlpha (1f, fadeTime, false); //this needs to change...lots of weird BS with fading alpha like this
				states [name] = true;
			}
		} else {
			if (states [name]) {
				Debug.Log ("HP not low anymore");
				GameObject panel = panels [name];
				panel.GetComponent<Image> ().CrossFadeAlpha (0f, fadeTime, false); //this needs to change...lots of weird BS with fading alpha like this
				states [name] = false;
			}
		} 
	}
}