using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Create this ability
public class CloakAbility : CrawlerActiveAbility
{
    private Material defaultMaterial;   // Default crawler material
    public Material cloakMaterial;      // Cloak material

    public void Start()
    {
        // TODO: Debug if this gets correct material
        defaultMaterial = GetComponentInParent<Material>();

        // Initialize base class variables
        throw new System.NotImplementedException();
    }

    public override void Activate()
    {
        base.Activate();

        throw new System.NotImplementedException();

        var crawler = GetComponentInParent<Crawler>();
        
        //crawler.RpcSetMaterial(true);

        var detectionComponent =
            crawler.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = false;
    }

    // TODO:
    public override void ForceDeactivate()
    {
        base.ForceDeactivate();

        throw new System.NotImplementedException();
    }

    public override void ForceRefresh()
    {
        base.ForceRefresh();

        throw new System.NotImplementedException();
    }

    protected override void deactivate()
    {
        base.deactivate();

        throw new System.NotImplementedException();

        var crawler = GetComponentInParent<Crawler>();

        //crawler.RpcSetMaterial(false);
        
        var detectionComponent =
            crawler.gameObject.GetComponent<DetectableObject>();
        if (detectionComponent != null)
            detectionComponent.isVisuallyDetectable = true;
    }

    protected override void refresh()
    {
        base.refresh();

        throw new System.NotImplementedException();
    }
}
