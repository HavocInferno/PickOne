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
            // TODO
        }
    }
}