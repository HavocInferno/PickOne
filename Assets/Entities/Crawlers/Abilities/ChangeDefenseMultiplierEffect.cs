using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/ChangeDefenseMultiplierEffect")]
public class ChangeDefenseMultiplierEffect : AbstractEffect
{
    public float multiplier = 1.0f;

    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Enable(character, calledByLocalPlayer, calledByServer);

        character.GetComponent<Stats>().defenseMultiplier *= multiplier;
    }

    public override void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Disable(character, calledByLocalPlayer, calledByServer);

        character.GetComponent<Stats>().defenseMultiplier /= multiplier;
    }
}
