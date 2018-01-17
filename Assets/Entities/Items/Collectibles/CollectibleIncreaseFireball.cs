﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleIncreaseFireball : Collectible
{
    public override void Collect(GameObject character)
    {
        base.Collect(character);

        if (isServer)
        {
			foreach (var mas in GameObject.Find("[CameraRig]").GetComponents<Master>()) {
				mas.FireBallCollected ();
			}
        }
    }
}
