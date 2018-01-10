﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/SumoBlastEffect")]
public class SumoBlastEffect : AbstractEffect
{
    public GameObject sumoBlastPrefab;
    public float maxDamage = 100.0f;
    public float minDamage = 50.0f;
    public float maxDistance = 4;

    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        if (sumoBlastPrefab == null)
        {
            Debug.LogErrorFormat("{0} | Prefab is not set", name);
            return;
        }

        base.Enable(character, calledByLocalPlayer, calledByServer);

        var blast = Instantiate(sumoBlastPrefab, character.transform);
        var component = blast.AddComponent<_SumoBlastEffect>();
        component._Initialize(
            blast,
            maxDamage,
            minDamage,
            maxDistance,
            character,
            calledByServer);
    }

    public override void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        if (sumoBlastPrefab == null)
        {
            return;
        }

        base.Disable(character, calledByLocalPlayer, calledByServer);

        Destroy(character.gameObject.GetComponentInChildren<_SumoBlastEffect>());
    }

    [RequireComponent(typeof(GenericCharacter))]
    private class _SumoBlastEffect : MonoBehaviour
    {
        GameObject _sumoBlastInstance;
        float _maxDamage;
        float _minDamage;
        float _maxDistance;
        bool _registerDamage;
        GenericCharacter _character;

        public void _Initialize(
            GameObject blastInstance,
            float maxDamage,
            float minDamage,
            float maxDistance,
            GenericCharacter character,
            bool registerDamage)
        {
            _maxDamage = maxDamage;
            _minDamage = minDamage;
            _maxDistance = maxDistance;
            _registerDamage = registerDamage;

            _sumoBlastInstance = blastInstance;
            _character = character;
            
            _registerDamage = registerDamage;

            // Prevents pre-emptive collisions
            var collider = GetComponent<SphereCollider>();
            collider.radius = _maxDistance;
            collider.enabled = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
             // Check for friendly fire
            if (collision.collider.tag == gameObject.transform.parent.tag)
                return;

            // Get health component of collision object
            var stats = collision.gameObject.GetComponent<Stats>();

            // If it has one, call function to take damage
            if (stats != null)
            {
                // Calculate distance based damage to the enemy
                var distance = Vector3.Distance(_character.transform.position, collision.transform.position);
                var percent = distance / _maxDistance;

                var damage = (1.0f - percent) * (_maxDamage - _minDamage) + _minDamage;

                stats.Hit(damage, _character, _character.transform.position,
                    (collision.transform.position - _character.transform.position).normalized);
            }
            else
            {
                if (!collision.collider.CompareTag("Untagged"))
                    Debug.LogWarning("On " + collision.collider.tag + " stats were not found.");
            }
        }

        private void OnDisable()
        {
            Destroy(_sumoBlastInstance);
        }
    }
}
