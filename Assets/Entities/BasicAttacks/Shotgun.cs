using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jesus Christ, why is this not a child class of "gun"?
public class Shotgun : BasicAttack
{
    public float angle;
    public int count;

    [Header("Bullet Details")]
    public float BulletSpeed = 5f;
    public float BulletLife = 2f;
    public GameObject BulletPrefab;
    public Transform BulletSpawn;
	public MuzzleFlash muzz;
	public Transform ejectionPort; 
	public GameObject shell;
	public Recoiler recoil; 

    [Space(8)]
    [Tooltip("Time in seconds before the gun is restored to its original rotation.")]
    public float resetDelay = 1.0f;

    //protected Quaternion _initialRotation;
    //protected Coroutine _resetCoroutine;

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

    //public virtual void AimGun(GenericCharacter attacker)
    //{
    //    // Figure out where the crosshair is aiming
    //    var ray = Camera.main.ScreenPointToRay(
    //        new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

    //    RaycastHit rayHit;
    //    if (Physics.Raycast(ray, out rayHit, 100))
    //    {
    //        // Rotate the gun to point to the... point
    //        transform.LookAt(rayHit.point);
    //    }

    //    // Start the coroutine to reset the gun
    //    if (_resetCoroutine != null)
    //        StopCoroutine(_resetCoroutine);

    //    _resetCoroutine = StartCoroutine(ResetGun());
    //}

    public override void DoAttack(GenericCharacter attacker)
    {
        if (!_ready) return;

        base.DoAttack(attacker);

        if (BulletPrefab == null)
            return;

        //AimGun(attacker);

        for (int i = 0; i < count; ++i)
        {
            // Create the Bullet from the Bullet Prefab
            var bullet = Instantiate(
                BulletPrefab,
                BulletSpawn.position,
                BulletSpawn.rotation);

			Vector3 angles = angle * (new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0)); //Vector3 angles = angle * (new Vector3(Random.value, Random.value, Random.value));
            bullet.transform.Rotate(angles);

            // Set damage value of the bullet
            bullet.GetComponent<Bullet>().damage = this.damage;
            bullet.GetComponent<Bullet>().attacker = attacker;
            bullet.GetComponent<Bullet>().direction = bullet.transform.forward;

            // Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * BulletSpeed;

            // Destroy the bullet after 2 seconds
            Destroy(bullet, Random.value * BulletLife);
        }
		muzzleFlash ();
		shellEjection ();
		applyRecoil ();
    }

    /// <summary>
    /// Resets the rotation of the gun to its intial position.
    /// </summary>
    /// <returns></returns>
    //IEnumerator ResetGun()
    //{
    //    yield return new WaitForSeconds(resetDelay);
    //    gameObject.transform.localRotation = _initialRotation;
    //}
	public void muzzleFlash ()
	{
		if (muzz != null)
			muzz.fire = true;
	}

	public void shellEjection ()
	{
		if (ejectionPort != null && shell != null)
			Instantiate (shell, ejectionPort.position, ejectionPort.rotation);
	}

	public void applyRecoil ()
	{
		if (recoil != null)
			recoil.recoil ();
	}
}

