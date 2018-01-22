using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class ActiveEffectsPanel : MonoBehaviour
{
    public GameObject elementPrefab;
    public float dissapearSpeed = 500.0f;

    private Dictionary<AbstractEffect, GameObject> _elements =
        new Dictionary<AbstractEffect, GameObject>();
    private Queue<GameObject> _toRemove = new Queue<GameObject>();

    public void AddElement(AbstractEffect effect)
    {
        GameObject newObject = GameObject.Instantiate(elementPrefab, transform);
        Image image = newObject.transform.Find("EffectIcon").GetComponentInChildren<Image>();
        image.sprite = effect.icon;
        _elements.Add(effect, newObject);
    }

    public void RemoveElement(AbstractEffect effect)
    {
		if (_elements.ContainsKey (effect)) {
            Transform iconObject = _elements[effect].transform.Find("EffectIcon");
            iconObject.GetComponent<Image> ().color = new Color (0.0f, 0.0f, 0.0f, 0.0f);
            iconObject.GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            _toRemove.Enqueue (_elements [effect]);
			_elements.Remove (effect);
		}
    }

    private void FixedUpdate()
    {
        if (_toRemove.Count == 0) return;

        GameObject obj = _toRemove.Peek();
        RectTransform transform = obj.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(
            transform.sizeDelta.x - dissapearSpeed * Time.fixedDeltaTime,
            transform.sizeDelta.y);
        if (transform.sizeDelta.x <= -GetComponent<HorizontalLayoutGroup>().spacing)
        {
            _toRemove.Dequeue();
            Destroy(obj);
        }
    }

    // Set alpha of the icon (TODO)
    public void SetValue(AbstractEffect effect, float value)
    {
        if (!(value <= 1.0f && value >= 0.0f))
            Debug.LogErrorFormat(
                "{0} | Expected value from 0 to 1 for effect {1}, got {2}",
                name, effect.name, value);
    }
}
