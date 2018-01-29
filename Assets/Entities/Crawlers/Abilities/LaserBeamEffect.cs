using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When enabled, emit destructive laser beam.
/// </summary>
[CreateAssetMenu(menuName = "Assets/Effects/LaserBeamEffect")]
public class LaserBeamEffect : AbstractEffect
{
    public GameObject laserBeamPrefab;
    public float damagePerSecond = 20.0f;
    public float damageRegisterRate = 0.1f;
    public float maxLength = 100.0f;
    public LayerMask laserHitMask;

    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        if (laserBeamPrefab == null)
        {
            Debug.LogErrorFormat("{0} | Prefab is not set", name);
            return;
        }

        base.Enable(character, calledByLocalPlayer, calledByServer);

        var gun = character.GetComponentInChildren<Gun>();

        var component = gun.gameObject.AddComponent<_LaserBeamEffectScript>();

        component._Initialize(
            Instantiate(laserBeamPrefab, gun.bulletSpawn),
            damageRegisterRate,
            damagePerSecond,
            laserHitMask,
            character,
            calledByServer);
    }

    public override void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        if (laserBeamPrefab == null)
        {
            return;
        }

        base.Disable(character, calledByLocalPlayer, calledByServer);

        Destroy(character.gameObject.GetComponentInChildren<_LaserBeamEffectScript>());
    }

    [RequireComponent(typeof(GenericCharacter))]
    private class _LaserBeamEffectScript : MonoBehaviour
    {
        GameObject _laserInstance;
        float _nextDamageCheckTime;
        float _damageRegisterRate;
        float _damagePerSecond;
        float _maxLength;
        bool _registerDamage;
        LayerMask _laserHitMask;
        GenericCharacter _character;

        public void _Initialize(
            GameObject laserInstance,
            float damageRegisterRate,
            float damagePerSecond,
            LayerMask laserHitMask,
            GenericCharacter character,
            bool registerDamage)
        {
            _laserHitMask = laserHitMask;
            _damagePerSecond = damagePerSecond;
            _damageRegisterRate = damageRegisterRate;
            _laserInstance = laserInstance;
            _character = character;
            _nextDamageCheckTime = Time.time + _damageRegisterRate;
            _registerDamage = registerDamage;
        }

        private void Update()
        {
            if (!_registerDamage) return;

            if (Time.time < _damageRegisterRate) return;
            _nextDamageCheckTime = Time.time + _damageRegisterRate;

            LineRenderer line = _laserInstance.GetComponent<LineRenderer>();
            
            Vector3 origin = _laserInstance.transform.TransformPoint(
                line.GetPosition(0));
            Vector3 direction = _laserInstance.transform.TransformDirection(
                line.GetPosition(1) - line.GetPosition(0));

            RaycastHit[] hits = Physics.RaycastAll(
                origin,
                direction.normalized,
                direction.magnitude,
                _laserHitMask);

            foreach (var hit in hits)
            {
                // Move on if friendly fire
                if (hit.transform.CompareTag(GetComponentInParent<GenericCharacter>().tag))
                    continue;

                float damage = _damageRegisterRate * _damagePerSecond;
                var enemyStats = hit.transform.GetComponent<Stats>();
                if (enemyStats)
                {
                    enemyStats.Hit(
                        damage,
                        _character,
                        hit.point,
                        direction.normalized);
                }
                else
                {
                    Debug.LogWarning("Laser hit enemy without stats!");
                }
            }
        }

        private void OnDisable()
        {
            Destroy(_laserInstance);
        }
    }
}

