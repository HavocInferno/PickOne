using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAttack : BasicAttack
{
    public float angle;
    public int count;

    [Header("Bullet Details")]
    public float BulletSpeed = 5f;
    public float BulletLife = 2f;
    public GameObject BulletPrefab;
    public Transform BulletSpawn;

    override protected void Start()
    {
        base.Start();

        if (BulletPrefab == null)
        {
            Debug.LogError("Bullet prefab not set.");
        }

        if (BulletSpawn == null)
        {
            Debug.LogError("Bullet spawn transform not set.");
        }
    }

    public override void DoAttack(GenericCharacter attacker)
    {
        if (!_ready) return;

        base.DoAttack(attacker);

        Debug.LogError("Attack!");

        if (BulletPrefab == null)
            return;

        for (int i = 0; i < count; ++i)
        {
            Debug.LogError("Spawn bullet!");
            // Create the Bullet from the Bullet Prefab
            var bullet = Instantiate(
                BulletPrefab,
                BulletSpawn.position,
                BulletSpawn.rotation);

            Vector3 angles = angle * (new Vector3(Random.value, Random.value, Random.value));
            bullet.transform.Rotate(angles);

            // Set damage value of the bullet
            bullet.GetComponent<Bullet>().damage = this.Damage;
            bullet.GetComponent<Bullet>().attacker = attacker;
            bullet.GetComponent<Bullet>().direction = bullet.transform.forward;

            // Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * BulletSpeed;

            // Destroy the bullet after 2 seconds
            Destroy(bullet, Random.value * BulletLife);
        }
    }
}

