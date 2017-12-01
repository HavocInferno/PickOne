using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/CloakEffect")]
public class CloakEffect : AbstractEffect
{
    public Material cloakMaterial;      // Cloak material

    public override void Enable(Crawler crawler)
    {
        base.Enable(crawler);
        
        var detectionComponent =
            crawler.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = false;
    }

    public override void Disable(Crawler crawler)
    {
        base.Disable(crawler);

        var detectionComponent =
            crawler.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = true;
    }
}
