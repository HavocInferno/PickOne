using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CrawlerAbility : ScriptableObject
{
    public float cost = 0.0f;

    virtual public void Activate(Crawler crawler)
    {
    }

    virtual public void Deactivate(Crawler crawler)
    {
    }
}
