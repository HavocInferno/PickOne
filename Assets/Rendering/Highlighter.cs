using UnityEngine;

/// <summary>
/// Attach this component to an object to
/// tell HighlightingRenderer that this object should be highlighted.
/// </summary>
[AddComponentMenu("Rendering/Highlighter")]
public class Highlighter : MonoBehaviour
{
    public string replacementTag = "Highlight";
    public string colorAttributeName = "_HighlightColor";
    public Color color = Color.red;

    private Color _setupColor;
    private string _setupReplacementTag = null;

    // Update is called once per frame
    void Update()
    {
        if (replacementTag != _setupReplacementTag)
        {
            RefreshReplacementTag();
        }

        if (color != _setupColor)
        {
            RefreshColor();
        }
    }

    void RefreshColor()
    {
        if (colorAttributeName == null)
        {
            Debug.LogError("Highlighter | Color attribute name is NULL", gameObject);
            return;
        }

        foreach (var material in Hierarchy.GetMaterials(transform))
        {
            material.SetColor(colorAttributeName, color);
        }

        _setupColor = color;
    }

    void RefreshReplacementTag()
    {
        if (replacementTag == null)
        {
            Debug.LogError("Highlighter | Replacement tag for Highlighter script is NULL");
            return;
        }

        foreach (var material in Hierarchy.GetMaterials(transform))
        {
            material.SetOverrideTag(_setupReplacementTag, "");
            material.SetOverrideTag(replacementTag, "On");
        }

        _setupReplacementTag = replacementTag;
    }


    private void OnDestroy()
    {
        if (_setupReplacementTag == null) return;

        foreach (var material in Hierarchy.GetMaterials(transform))
        {
            material.SetOverrideTag(_setupReplacementTag, "");
        }
    }
}
