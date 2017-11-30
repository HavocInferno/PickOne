using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : BasicAttack
{
    [Header("Bullet Details")]
    public float BulletSpeed = 5f;
    public float BulletLife = 2f;
    public GameObject BulletPrefab;
    public Transform BulletSpawn;

    override protected void OnValidate()
    {
        if (BulletPrefab == null)
        {
            Debug.LogError("Bullet prefab not set.");
        }

        if (BulletSpawn == null)
        {
            Debug.LogError("Bullet spawn transform not set.");
        }
    }

    public override void DoAttack()
    {
        if (ready != true)
            return;

        base.DoAttack();

        if (BulletPrefab == null)
            return;

        // Create the Bullet from the Bullet Prefab
        var bullet = Instantiate(
            BulletPrefab,
            BulletSpawn.position,
            BulletSpawn.rotation);

        // Set damage value of the bullet
        bullet.GetComponent<Bullet>().Damage = this.Damage;

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * BulletSpeed;

        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, BulletLife);
    }
}
