using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/DetectEnemiesEffect")]
public class DetectEnemiesEffect : AbstractEffect
{
    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Enable(character, calledByLocalPlayer, calledByServer);

        if (calledByLocalPlayer)
        {
            GameObject camera = GameObject.FindWithTag("MainCamera");
            camera.GetComponent<HighlightingRenderer>().enabled = true;
			// camera.GetComponent<BetterHLRendering>().EnableHighlightAll();
        }
    }

    public override void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Disable(character, calledByLocalPlayer, calledByServer);

        if (calledByLocalPlayer)
        {
            GameObject camera = GameObject.FindWithTag("MainCamera");
            camera.GetComponent<HighlightingRenderer>().enabled = false;
			// camera.GetComponent<BetterHLRendering>().DisableHighlightAll();
        }
    }
}
