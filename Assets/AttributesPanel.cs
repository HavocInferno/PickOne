using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributesPanel : MonoBehaviour
{
    [System.Serializable]
    private struct AttributeInfo
    {
        public string name;
        public Color color;
		public Color backgroundColor;
		public Color textColor;
        public Sprite icon;
    }

    [SerializeField]
    GameObject elementPrefab;

    [SerializeField]
    List<AttributeInfo> attributes = new List<AttributeInfo>();

    public void Register(Stats stats)
    {
        foreach (var attributeInfo in attributes)
        {
            if (stats.HasAttribute(attributeInfo.name))
            {
                GameObject instance = Instantiate(elementPrefab, transform);
                instance.name += "_" + attributeInfo.name;
                Transform text = instance.transform.Find("Text");
                Transform background = instance.transform.Find("Background");
                Transform foreground = background == null ? null : background.Find("Foreground");
                if (text != null)
                {
                    text.GetComponent<Text>().text = attributeInfo.name;
					text.GetComponent<Text> ().color = attributeInfo.textColor; /*new Color(
                        attributeInfo.color.r * 0.9f,
                        attributeInfo.color.g * 0.9f,
                        attributeInfo.color.b * 0.9f,
                        attributeInfo.color.a * 0.9f);*/
                }
                if (foreground != null)
                {
                    foreground.GetComponent<Image>().color = attributeInfo.color;
                    stats.attributes[attributeInfo.name].bar = foreground.GetComponent<RectTransform>();
                }
                if (background != null)
                {
					background.GetComponent<Image>().color = attributeInfo.backgroundColor; /*new Color(
                        attributeInfo.color.r * 0.3f,
                        attributeInfo.color.g * 0.3f,
                        attributeInfo.color.b * 0.3f,
                        attributeInfo.color.a * 0.7f);*/
                }
            }
        }
    }
}
