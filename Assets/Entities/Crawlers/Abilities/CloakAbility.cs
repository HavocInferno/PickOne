using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Abilities/CloakAbility")]
public class CloakAbility : AbstractAbility
{
    public Material cloakMaterial;      // Cloak material

    public override void Activate(Crawler crawler)
    {
        base.Activate(crawler);
        
        var detectionComponent =
            crawler.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = false;
    }

    public override void EnableEffect(Crawler crawler)
    {
        base.EnableEffect(crawler);
    }

    public override void Deactivate(Crawler crawler)
    {
        base.Deactivate(crawler);

        var detectionComponent =
            crawler.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = true;
    }

    public override void DisableEffect(Crawler crawler)
    {
        base.DisableEffect(crawler);
    }
}
