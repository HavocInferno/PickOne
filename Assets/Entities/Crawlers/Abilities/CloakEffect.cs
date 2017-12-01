using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/CloakEffect")]
public class CloakEffect : AbstractEffect
{
    public Material cloakMaterial;      // Cloak material

    public override void Enable(GenericCharacter character)
    {
        base.Enable(character);
        
        var detectionComponent =
            character.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = false;
    }

    public override void Disable(GenericCharacter character)
    {
        base.Disable(character);

        var detectionComponent =
            character.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = true;
    }
}
