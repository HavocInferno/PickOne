using UnityEngine;
using UnityEngine.Networking;

public class Gun : BasicAttack
{
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

        if (BulletPrefab == null)
            return;

        // Create the Bullet from the Bullet Prefab
        var bullet = Instantiate(
            BulletPrefab,
            BulletSpawn.position,
            BulletSpawn.rotation);

        // Set damage value of the bullet
        bullet.GetComponent<Bullet>().damage = this.damage;
        bullet.GetComponent<Bullet>().attacker = attacker;
        bullet.GetComponent<Bullet>().direction = bullet.transform.forward;

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * BulletSpeed;

        // Destroy the bullet after 2 seconds
        Destroy(bullet, BulletLife);
    }
}
