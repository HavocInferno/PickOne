using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : BasicAttack
{
    [Header("Bullet Details")]
    public float bulletSpeed = 5f;
    public float bulletLife = 2f;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
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

        //_defaultPosiiton = gameObject.transform.position;

        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab not set.");
        }

        if (bulletSpawn == null)
        {
            Debug.LogError("Bullet spawn transform not set.");
        }
    }

    protected void OnValidate()
    {

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

        if (!bulletPrefab || !bulletSpawn)
            return;

        //AimGun(attacker);
        
        // Create the Bullet from the Bullet Prefab
        var bullet = Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Set damage value of the bullet
        bullet.GetComponent<Bullet>().damage = this.damage;
        bullet.GetComponent<Bullet>().attacker = attacker;
        bullet.GetComponent<Bullet>().direction = bullet.transform.forward;

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;



        // Destroy the bullet after 2 seconds
		muzzleFlash ();
		shellEjection ();
		applyRecoil ();
        Destroy(bullet, bulletLife);
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
