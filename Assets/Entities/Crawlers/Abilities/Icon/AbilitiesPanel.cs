using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class AbilitiesPanel : MonoBehaviour
{
    public GameObject elementPrefab;
    public float disappearSpeed = 400f;

    private Dictionary<ActiveAbility, GameObject> _elements =
        new Dictionary<ActiveAbility, GameObject>();

    // Initializes the view
    public void Initialize(List<ActiveAbility> abilities)
    {
        foreach (var ability in abilities)
        {
            GameObject newObject = Instantiate(elementPrefab, transform);
            Image image = newObject.transform.Find("EffectIcon").GetComponentInChildren<Image>();
            image.sprite = ability.icon;
            image.color = new Color(0.0f, 0.0f, 0.0f, 0.9f);
            _elements.Add(ability, newObject);
        }
    }

    private void Update()
    {
        foreach (var ability in _elements.Keys)
        {
            if (ability.IsAvailable)
            {
                _elements[ability].GetComponent<Image>().color =
                    new Color(0.1f, 0.9f, 0.1f, 1.0f);
            }
            else
            {
                _elements[ability].GetComponent<Image>().color =
                    new Color(0.5f, 0.1f, 0.1f, 1.0f);
            }
        }
    }
}
