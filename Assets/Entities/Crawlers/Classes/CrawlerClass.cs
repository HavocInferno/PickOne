using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Crawler Class")]
public class CrawlerClass : ScriptableObject
{
    public List<AbstractEffect> passiveEffects = new List<AbstractEffect>();
    public List<ActiveAbility> activeAbilities = new List<ActiveAbility>();

    public void Apply(Crawler crawler)
    {
        crawler.activeAbilities.AddRange(activeAbilities);
        crawler.passiveEffects.AddRange(passiveEffects);
    }
}
