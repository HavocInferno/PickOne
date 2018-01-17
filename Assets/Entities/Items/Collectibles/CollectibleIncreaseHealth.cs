using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleIncreaseHealth : Collectible
{
    public override void Collect(GameObject character)
    {
        base.Collect(character);

        if (isServer)
        {
            GameObject masterTransform = GameObject.Find("[CameraRig]");
            if (masterTransform == null) return;
			foreach (var mas in GameObject.Find("[CameraRig]").GetComponents<Master>()) {
				mas.HealOrbCollected ();
			}
        }
    }
}