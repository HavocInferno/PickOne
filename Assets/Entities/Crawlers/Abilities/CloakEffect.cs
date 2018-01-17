using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/CloakEffect")]
public class CloakEffect : AbstractEffect
{
    public Material cloakMaterial;

    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Enable(character, calledByLocalPlayer, calledByServer);

        character.gameObject.AddComponent<_CloakEffectScript>()._Initialize(cloakMaterial);
    }

    public override void OnMakeDamage(
        float amount, 
        GenericCharacter attacker,
        GenericCharacter target, 
        Vector3 hitPoint, 
        Vector3 hitDirection, 
        bool calledByLocalPlayer, 
        bool calledByServer)
    {
        base.OnMakeDamage(
            amount, 
            attacker,
            target, 
            hitPoint,
            hitDirection,
            calledByLocalPlayer, 
            calledByServer);

        // On damage of an enemy, disable this ability
        Disable(attacker, calledByLocalPlayer, calledByServer);
    }

    public override void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Disable(character, calledByLocalPlayer, calledByServer);

        Destroy(character.gameObject.GetComponent<_CloakEffectScript>());
    }

    [RequireComponent(typeof(GenericCharacter))]
    private class _CloakEffectScript : MonoBehaviour
    {
        Material oldMaterial = null; 
        public void _Initialize(Material cloakMaterial)
        {
            var detectionComponent =
                gameObject.GetComponent<DetectableObject>();
            if (detectionComponent != null)
                detectionComponent.isVisuallyDetectable = false;
            var meshRenderer = transform.Find("Body").GetComponent<MeshRenderer>();
            oldMaterial = meshRenderer.material;
            meshRenderer.material = cloakMaterial;
        }

        private void OnDisable()
        {
            var detectionComponent =
                gameObject.gameObject.GetComponent<DetectableObject>();
            if (detectionComponent != null)
                detectionComponent.isVisuallyDetectable = true;
            var meshRenderer = transform.Find("Body").GetComponent<MeshRenderer>();
            meshRenderer.material = oldMaterial;
        }
    }
}
