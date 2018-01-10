using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/BuffVisualEffect")]
public class BuffVisualEffect : AbstractEffect
{
    public Color color;
    public bool createLight = false;

    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Enable(character, calledByLocalPlayer, calledByServer);

        var component = character.gameObject.AddComponent<_BuffVisualEffectScript>();
        component._Initialize(
            color, createLight,
            character);
    }

    public override void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Disable(character, calledByLocalPlayer, calledByServer);

        Destroy(character.gameObject.GetComponent<_BuffVisualEffectScript>());
    }

    [RequireComponent(typeof(GenericCharacter))]
    private class _BuffVisualEffectScript : MonoBehaviour
    {
        Color _color;
        bool _createLight;
        Color _baseColor;
        GenericCharacter _character;
        GameObject _lightNode;
        float _startTime = 0.0f;

        static float GetWeight(float x)
        {
            if (x < 0.25f) return 4.0f * x;
            if (x < 0.45f) return 4.0f * (0.5f - x);
            return 4.0f * (0.5f - 0.45f);
        }

        public void _Initialize(
            Color color,
            bool createLight,
            GenericCharacter character)
        {
            _color = color;
            _createLight = createLight;
            _character = character;

            MeshRenderer renderer = GetBody().GetComponent<MeshRenderer>();
			if(renderer != null)
            	renderer.material.EnableKeyword("EMISSION");

            if (_createLight)
            {
                _lightNode = new GameObject("LightEffect", typeof(Light));
                _lightNode.transform.SetParent(GetBody(), false);
                Light light = _lightNode.GetComponent<Light>();
                light.type = LightType.Point;
                light.shadows = LightShadows.None;
                light.color = _color;
                light.intensity = 0.0f;
                light.renderMode = LightRenderMode.ForcePixel;
            }

            _startTime = Time.time;
        }

        Transform GetBody()
        {
            Transform body = transform.Find("Body");
            if (body != null) return body;
            return transform;
        }

        private void FixedUpdate()
        {
            float weight = GetWeight(Time.time - _startTime);

			MeshRenderer renderer = GetBody().GetComponent<MeshRenderer>();

            Color newColor = _color * 2.0f * weight; newColor.a = 1.0f;
			if(renderer != null)
            	renderer.material.SetColor("_EmissionColor", _baseColor + newColor);

            if (_createLight)
            {
                Light light = _lightNode.GetComponent<Light>();
                light.intensity = 10.0f * weight;
            }
        }

        private void OnDisable()
        {
            MeshRenderer renderer = GetBody().GetComponent<MeshRenderer>();
			if(renderer != null)
            	renderer.material.SetColor("_EmissionColor", _baseColor);

            if (_createLight)
                Destroy(_lightNode);
        }
    }
}
