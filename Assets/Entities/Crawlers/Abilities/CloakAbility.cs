using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Cloak")]
public class CloakAbility : CrawlerAbility
{
    public override void Activate(Crawler crawler)
    {
        base.Activate(crawler);

        crawler.RpcSetMaterial(true);
        var detectionComponent =
            crawler.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = false;
    }

    public override void Deactivate(Crawler crawler)
    {
        base.Deactivate(crawler);

        crawler.RpcSetMaterial(false);
        var detectionComponent =
            crawler.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = true;
    }
}
