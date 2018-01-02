using UnityEngine;

public class Master : MonoBehaviour {

	[SerializeField]
	private ushort hapticforce = 3999;
    [SerializeField]
    float vibrateFrequency = 10;
    [SerializeField]
	Controller mainHand, offHand;

	//Buff/Debuff Variables
	[SerializeField]
	private float maxRayOffset = 10;
	public Transform rayOrigin; 
	[SerializeField]
	private PlayersManager playerManager;
	[SerializeField]
	private float raySpeed = 30;

	[SerializeField]
	private BezierCurve buffRay;
	[SerializeField]
	private int currentBuffTarget = -1;

	[SerializeField]
	private BezierCurve debuffRay;	
	[SerializeField]
	private int currentDebuffTarget = -1;

	[SerializeField]
	private AbstractEffect buffEffect;
	[SerializeField]
	private AbstractEffect debuffEffect;

	[SerializeField]
	public EndScreenUIVR vrEndScreenUI;

	public Vector3 buffDestination;
	public bool buffing = false;
	public Vector3 debuffDestination;
	public bool debuffing = false;

    //general physics based ability
    [SerializeField]
    float maxChargeDistance = 15;
    [SerializeField]
    float minCharge = 10, maxCharge = 100, chargeRate = 30;
    [SerializeField]
    Transform throwBase;
    [SerializeField]
    AbilityPicker picker;
    [SerializeField]
    Material abilityPicker, pickerVisible, pickerInvisible;
    [SerializeField]
    float poolGrowSpeed = 5;

    float charge;
    Vector3 lastPos;

	//fireBall
	bool chargingFire = false;
    public GameObject fireBallPrefab;
	public GameObject fireBallVis;
	Vector3 fireVisScale;
    public GameObject firePool;
    public ParticleAttractor fireAtt;
    Vector3 firePoolScale;

	//healOrb
	bool chargingHeal = false;
	public GameObject healOrbPrefab;
	public GameObject healOrbVis;
	Vector3 healVisScale;
	public GameObject healPool;
	public ParticleAttractor healOrbAtt;
	Vector3 healPoolScale;


	// Use this for initialization
	void Start () {
		initRays ();
		initThrowables ();
		//initVRUI ();
	}
	
	// Update is called once per frame
	void Update () {
		applyBuff ();
		applyDebuff ();
        UpdateAbilityPicker();
        applyFireBall();
		applyHealOrb ();
	}

	void applyBuff ()
	{
		//if we are not supposed to buff or the radial menu is open, don't buff
		if (mainHand.currentItem != 0 || mainHand.radialMenuAccessed) {
			if (buffing)
				stopBuffing ();
			return;
		}

		if (mainHand.getTrigger()) {
			int closest = -1;
			float closestdistance = maxRayOffset;
			for (int i = 0; i < playerManager.players.Count; i++) {
				if (playerManager.players [i] == null)
					continue;
				if (Vector3.Cross (rayOrigin.forward, playerManager.players [i].position - rayOrigin.position).magnitude < closestdistance && Vector3.Dot (rayOrigin.forward, playerManager.players [i].position - rayOrigin.position) > 0.1) {
					closestdistance = Vector3.Cross (rayOrigin.forward, playerManager.players [i].position - rayOrigin.position).magnitude;
					closest = i;
				}
			}
			if (closest != -1) {
				buffDestination = Vector3.Lerp (buffDestination, playerManager.players [closest].position, Time.deltaTime * raySpeed);
				buffRay.destination = buffDestination; 
				if (closest == currentBuffTarget)
					mainHand.hapticFeedback ((ushort)(1000 * Mathf.Pow (Vector3.Cross (rayOrigin.forward, playerManager.players [closest].position - rayOrigin.position).magnitude / maxRayOffset, 2)));
				else {
					//new target
					if (currentBuffTarget != -1)
						stopBuffing ();
					currentBuffTarget = closest;
					startBuffing ();
					mainHand.hapticFeedback (hapticforce);
				}
				//for some weird reason we might not be buffing at this stage
				if (!buffing)
					startBuffing();
			}
			else
				stopBuffing();
		}
		if (mainHand.getTriggerUp ()) 
			stopBuffing ();
	}

	private void startBuffing()
	{
		if (currentBuffTarget != -1 && !buffing)
		{
			buffing = true;
			buffRay.Draw = true;
			playerManager.players[currentBuffTarget].GetComponent<Crawler>().EnableEffect(buffEffect);
		}
	}

	private void stopBuffing()
	{
		buffing = false;
		buffRay.Draw = false;
        if (currentBuffTarget != -1 && buffing && playerManager.players[currentBuffTarget] != null && playerManager.players[currentBuffTarget].GetComponent<Crawler>() != null)
            playerManager.players[currentBuffTarget].GetComponent<Crawler>().DisableEffect(buffEffect);
    }

	void applyDebuff ()
	{
		if (mainHand.currentItem != 1 || mainHand.radialMenuAccessed) {
			if (debuffing)
				stopDebuffing ();
			return;
		}

		if (mainHand.getTrigger() && mainHand.currentItem == 1) {
			int closest = -1;
			float closestdistance = maxRayOffset;
			for (int i = 0; i < playerManager.enemies.Count; i++) {
				if (playerManager.enemies [i] == null)
					continue;
				if (Vector3.Cross (rayOrigin.forward, playerManager.enemies [i].position - rayOrigin.position).magnitude < closestdistance && Vector3.Dot (rayOrigin.forward, playerManager.enemies [i].position - rayOrigin.position) > 0.1) {
					closestdistance = Vector3.Cross (rayOrigin.forward, playerManager.enemies [i].position - rayOrigin.position).magnitude;
					closest = i;
				}
			}
			if (closest != -1) {
				debuffDestination = Vector3.Lerp (debuffDestination, playerManager.enemies [closest].position, Time.deltaTime * raySpeed);
				debuffRay.destination = debuffDestination; 
				if (closest == currentDebuffTarget)
					mainHand.hapticFeedback ((ushort)(1000 * Mathf.Pow (Vector3.Cross (rayOrigin.forward, playerManager.enemies [closest].position - rayOrigin.position).magnitude / maxRayOffset, 2)));
				else {
					//new target
					if (currentDebuffTarget != -1)
						stopDebuffing (); 
					currentDebuffTarget = closest;
					startDebuffing ();
					Debug.Log("New enemy target! closest: " + closest +", currentDebuffTarget: "+currentDebuffTarget);
					mainHand.hapticFeedback(hapticforce);
				}
				if (!debuffing)
					startDebuffing();
			}
			else
				stopDebuffing();
		}
		if (mainHand.getTriggerUp ()) 
			stopDebuffing ();
		
	}

	private void startDebuffing()
	{
		if (currentDebuffTarget != -1 && !debuffing)
		{
			debuffing = true;
			debuffRay.Draw = true;
			playerManager.enemies[currentDebuffTarget].GetComponent<Enemy>().EnableEffect(debuffEffect);
		}
	}

	private void stopDebuffing()
	{
        debuffing = false;
        debuffRay.Draw = false;
        if (currentDebuffTarget != -1 && debuffing && playerManager.enemies[currentDebuffTarget]!= null && playerManager.enemies[currentDebuffTarget].GetComponent<Enemy>() != null)
			playerManager.enemies[currentDebuffTarget].GetComponent<Enemy>().DisableEffect(debuffEffect);
	}

	void initRays ()
	{
		debuffDestination = buffDestination = rayOrigin.position;
		debuffRay.origin = buffRay.origin = rayOrigin;
		playerManager = GameObject.Find ("PlayerManagers").GetComponent<PlayersManager>();
	}

	void initThrowables ()
	{
		fireBallVis.SetActive (false);
		fireVisScale = fireBallVis.transform.localScale;
		firePoolScale = firePool.transform.localScale;
		healOrbVis.SetActive (false);
		healVisScale = fireBallVis.transform.localScale;
		healPoolScale = firePool.transform.localScale;
	}

    void applyFireBall()
    {
        if (mainHand.currentItem != 2 || mainHand.radialMenuAccessed)
        {
            if (firePool.transform.localScale.magnitude >= 0.01 * firePoolScale.magnitude)
                firePool.transform.localScale = Vector3.Lerp(firePool.transform.localScale, Vector3.zero, Time.deltaTime * poolGrowSpeed);
            else
                firePool.SetActive(false);
            if (chargingFire)
                dropFireBall();
            return;
        }
        else
        {
            firePool.SetActive(true);
            if (firePool.transform.localScale.magnitude < 0.99 * firePoolScale.magnitude)
                firePool.transform.localScale = Vector3.Lerp(firePool.transform.localScale, firePoolScale, Time.deltaTime* poolGrowSpeed );
        }

        if (mainHand.getTrigger() && picker.pooling && !chargingFire && charge<maxCharge)
        {
				chargingFire = true;
                fireBallVis.SetActive(true);
                fireAtt.attracting = true;
        }
        if (mainHand.getTrigger() && chargingFire && charge<maxCharge && (mainHand.transform.position-offHand.transform.position).magnitude < maxChargeDistance)
        {
            charge += chargeRate * Time.deltaTime;
			charge = Mathf.Clamp (charge, 0, maxCharge);
			fireBallVis.transform.localScale = charge / maxCharge * fireVisScale;
			mainHand.hapticFeedback ((ushort)(charge/maxCharge*hapticforce));
		}

        if (charge >= maxCharge || (mainHand.transform.position - offHand.transform.position).magnitude >= maxChargeDistance)
        {
            fireAtt.attracting = false;
            chargingFire = false;
        }
        if(chargingFire || Mathf.Sin(Time.time*Mathf.PI*2* vibrateFrequency) > 0)
			mainHand.hapticFeedback ((ushort)(Mathf.Pow(charge/maxCharge,2)*hapticforce));

        if (mainHand.getTriggerUp())
            dropFireBall();
		

        lastPos = throwBase.position;
    }
    void dropFireBall()
    {
		if (charge > minCharge) {
			GameObject fireball = Instantiate (fireBallPrefab, throwBase.position, throwBase.rotation);
			fireball.GetComponent<Rigidbody> ().velocity = (throwBase.position - lastPos) / Time.deltaTime;
			fireball.GetComponent<ThrowableAbility> ().chargeMulti = charge / maxCharge;
			fireball.transform.localScale = fireBallVis.transform.lossyScale;
		}
		fireBallVis.SetActive (false);
        charge = 0;
		chargingFire = false;
        fireAtt.attracting = false;
    }


    void UpdateAbilityPicker()
    {
		if (((mainHand.currentItem == 3  && !chargingHeal)|| (mainHand.currentItem == 2 && !chargingFire )) && mainHand.getTrigger()&& charge < maxCharge)
        {
            abilityPicker.Lerp(abilityPicker, pickerVisible, Time.deltaTime);
        }
        else
        {
            abilityPicker.Lerp(abilityPicker, pickerInvisible, Time.deltaTime*4);
        }
    }

	void applyHealOrb()
	{
		if (mainHand.currentItem != 3 || mainHand.radialMenuAccessed)
		{
			if (healPool.transform.localScale.magnitude >= 0.01 * healPoolScale.magnitude)
				healPool.transform.localScale = Vector3.Lerp(healPool.transform.localScale, Vector3.zero, Time.deltaTime * poolGrowSpeed);
			else
				healPool.SetActive(false);
			if (chargingHeal)
				dropHealOrb();
			return;
		}
		else
		{
			healPool.SetActive(true);
			if (healPool.transform.localScale.magnitude < 0.99 * healPoolScale.magnitude)
				healPool.transform.localScale = Vector3.Lerp(healPool.transform.localScale, healPoolScale, Time.deltaTime* poolGrowSpeed );
		}

		if (mainHand.getTrigger() && picker.pooling && !chargingHeal && charge<maxCharge)
		{
			chargingHeal = true;
			healOrbVis.SetActive(true);
			healOrbAtt.attracting = true;
		}
		if (mainHand.getTrigger() && chargingHeal && charge<maxCharge && (mainHand.transform.position-offHand.transform.position).magnitude < maxChargeDistance)
		{
			charge += chargeRate * Time.deltaTime;
			charge = Mathf.Clamp (charge, 0, maxCharge);
			healOrbVis.transform.localScale = charge / maxCharge * healVisScale;
			mainHand.hapticFeedback ((ushort)(charge/maxCharge*hapticforce));
		}

		if (charge >= maxCharge || (mainHand.transform.position - offHand.transform.position).magnitude >= maxChargeDistance)
		{
			healOrbAtt.attracting = false;
			chargingHeal = false;
		}
		if(chargingHeal || Mathf.Sin(Time.time*Mathf.PI*2* vibrateFrequency) > 0)
			mainHand.hapticFeedback ((ushort)(Mathf.Pow(charge/maxCharge,2)*hapticforce));

		if (mainHand.getTriggerUp())
			dropHealOrb();


		lastPos = throwBase.position;
	}
	void dropHealOrb()
	{
		if (charge > minCharge) {
			GameObject healOrb = Instantiate (healOrbPrefab, throwBase.position, throwBase.rotation);
			healOrb.GetComponent<Rigidbody> ().velocity = (throwBase.position - lastPos) / Time.deltaTime;
			healOrb.GetComponent<ThrowableAbility> ().chargeMulti = charge / maxCharge;
			healOrb.transform.localScale = healOrbVis.transform.lossyScale;
		}
		healOrbVis.SetActive (false);
		charge = 0;
		chargingHeal = false;
		healOrbAtt.attracting = false;
	}

    void initVRUI ()
	{
		//vrEndScreenUI.gameObject.SetActive (true);
		if(FindObjectOfType<EndConditions> ())//.endScreenUI.gameObject.activeInHierarchy)
			FindObjectOfType<EndConditions> ().endScreenUI.gameObject.SetActive (false);
		FindObjectOfType<EndConditions> ().endScreenUI = vrEndScreenUI;
	}
}
