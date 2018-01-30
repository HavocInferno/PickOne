using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/SoldierSpecialAttackEffect")]
public class SoldierSpecialAttackEffect : AbstractEffect
{
    public float bulletDamage = 10;
    public float angle = 3.0f;
    public int count = 10;
    public float delay = 0.1f;

    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        character.gameObject.AddComponent<_SoldierSpecialAttackEffect>().Initialize(count, bulletDamage, angle, delay);
    }

    public override void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Disable(character, calledByLocalPlayer, calledByServer);
        
        Destroy(character.gameObject.GetComponent<_SoldierSpecialAttackEffect>());
    }

    [RequireComponent(typeof(GenericCharacter))]
    private class _SoldierSpecialAttackEffect : MonoBehaviour
    {
        int _step = 0;
        bool _back = false;
        int _count = 0;
        float _bulletDamage = 0;
        float _angle = 0.0f;
        float _delay = 0.0f;
        float _nextTime = 0.0f;
        Gun _gun;
        GenericCharacter _character;
        
        public void Initialize(int count, float damage, float angle, float delay)
        {
            _step = -count / 2;
            _bulletDamage = damage;
            _count = count;
            _angle = angle;
            _delay = delay;
            _gun = gameObject.GetComponentInChildren<Gun>();
            _character = GetComponent<GenericCharacter>();

            SpawnNext();
        }

        void SpawnNext()
        {
            _step += _back ? -1 : 1;
            if (_step == _count)
            {
                _step = _count - 2;
                _back = true;
            }
            if (_step == -1)
            {
                _step = 1;
                _back = false;
            }

			_gun.selfAS.PlayOneShot (_gun.sound);
			_gun.muzzleFlash ();
			_gun.shellEjection ();
			_gun.applyRecoil ();

            // Create the Bullet from the Bullet Prefab
            var bullet = Instantiate(
                _gun.bulletPrefab,
                _gun.bulletSpawn.position,
                _gun.bulletSpawn.rotation);

            bullet.transform.Rotate(bullet.transform.up, (_step - _count / 2) * _angle);

            // Set damage value of the bullet
            bullet.GetComponent<Bullet>().damage = _bulletDamage;
            bullet.GetComponent<Bullet>().attacker = _character;
            bullet.GetComponent<Bullet>().direction = bullet.transform.forward;

            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * _gun.bulletSpeed;

            Destroy(bullet, _gun.bulletLife * 0.33f);

            _nextTime = Time.time + _delay;
        }

        private void FixedUpdate()
        {
            if (Time.time > _nextTime)
            {
                SpawnNext();
            }
        }
    }
}

