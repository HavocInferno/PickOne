using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
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
            GameObject newObject = GameObject.Instantiate(elementPrefab, transform);
            Image image = newObject.transform.Find("EffectIcon").GetComponentInChildren<Image>();
            image.sprite = ability.readyIcon;
            _elements.Add(ability, newObject);
        }
    }

    private void Update()
    {
        foreach (var ability in _elements.Keys)
        {
            if (ability.IsAvailable)
            {
                _elements[ability]
                    .transform.Find("EffectIcon")
                    .GetComponentInChildren<Image>().sprite = ability.readyIcon;
            }
            else
            {
                _elements[ability]
                    .transform.Find("EffectIcon")
                    .GetComponentInChildren<Image>().sprite = ability.unavailableIcon;
            }
        }
    }
}
