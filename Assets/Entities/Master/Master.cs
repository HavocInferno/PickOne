using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class Master : NetworkBehaviour {

	[SerializeField]
	private ushort hapticforce = 3999;
    [SerializeField]
    float vibrateFrequency = 10;
    [SerializeField]
	Controller mainHand, offHand;

	//Buff/Debuff Variables
	[SerializeField]
	public Material aidLineMaterial;
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
	public float maxBuffCharge = 50;	
	[SerializeField]
	private float BuffChargeRate = 8;	
	[SerializeField]
	private float BuffDechargeRate = 10;
	[SerializeField]
	private float buffCharge;
	public float BuffCharge{
		get{ 
			return buffCharge;
		}
	}

	[SerializeField]
	private BezierCurve debuffRay;	
	[SerializeField]
	private int currentDebuffTarget = -1;
	[SerializeField]
	public float maxDebuffCharge = 50;	
	[SerializeField]
	private float DebuffChargeRate = 8;	
	[SerializeField]
	private float DebuffDechargeRate = 10;
	[SerializeField]
	private float debuffCharge;
	public float DebuffCharge{
		get{ 
			return debuffCharge;
		}
	}

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
	public float chargeFire{
		get{if (mainHand.currentItem == 2)
			return charge/maxCharge;
			else
				return 0;}
	}
    public GameObject fireBallPrefab;
	public GameObject fireBallVis;
	Vector3 fireVisScale;
    public GameObject firePool;
    public ParticleAttractor fireAtt;
    Vector3 firePoolScale;

	[SerializeField]
	float maxFirePoolSize = 300;
	[SerializeField]
	float FirePoolRecharge = 3;
	[SerializeField]
	float firePoolSize = 30;

	//healOrb
	bool chargingHeal = false;
	public float chargeHeal{
		get{if (mainHand.currentItem == 3)
			return charge/maxCharge;
		else
			return 0;}
	}
	public GameObject healOrbPrefab;
	public GameObject healOrbVis;
	Vector3 healVisScale;
	public GameObject healPool;
	public ParticleAttractor healOrbAtt;
	Vector3 healPoolScale;

	[SerializeField]
	float maxHealPoolSize = 300;
	[SerializeField]
	float healPoolRecharge = 3;
	[SerializeField]
	float healPoolSize= 30;


	//Teleportation
	public Transform telePlat;
	public Material teleMat, teleMatHigh, teleMatLow, teleMatBlock;
	public float teleFrequency = 5f;
	public float telePlatThiccness =.1f;
	public SteamVR_PlayArea playArea;
	public Vector4 maxTel = new Vector4(2,1,-2,-1);
	public Vector2 currTel = Vector2.zero;

	private int lastTel;
	float teleLength, teleWidth; 

	// Use this for initialization
	void Start () {
		initRays ();
		initThrowables ();
		initTeleport ();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateAidRay();
        applyBuff ();
		applyDebuff ();
        UpdateAbilityPicker();
        applyFireBall();
		applyHealOrb ();
		applyTeleport ();
	}

	void applyBuff ()
	{
		//if we are not supposed to buff or the radial menu is open, don't buff
		if (mainHand.currentItem != 0 || mainHand.radialMenuAccessed) {
			if (buffing)
				stopBuffing ();
			buffCharge += BuffChargeRate * Time.deltaTime;
			return;
		}

		if (mainHand.getTrigger ()) {
			if (buffCharge > 0) {
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
					buffCharge -= BuffDechargeRate * Time.deltaTime;
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
						startBuffing ();
				} else {
					debuffDestination = rayOrigin.position;
					stopBuffing ();
				}
			} else {
				if (buffing)
					stopBuffing ();
				mainHand.vibrateFrequently (hapticforce, vibrateFrequency);
				
			}
		} else {
			buffCharge += BuffChargeRate * Time.deltaTime;
		}
		if (mainHand.getTriggerUp ()) {
			debuffDestination = rayOrigin.position;
			stopBuffing ();
		}


		buffCharge = Mathf.Clamp (buffCharge, 0, maxBuffCharge);
	}

	private void startBuffing()
	{
		if (currentBuffTarget != -1 && !buffing)
		{
			aidLineMaterial.Lerp (aidLineMaterial, pickerInvisible, 1);
			buffing = true;
			buffRay.Draw = true;
			playerManager.players[currentBuffTarget].GetComponent<Crawler>().EnableEffect(buffEffect);
		}
	}

	private void stopBuffing()
	{
        if (currentBuffTarget != -1 && playerManager.players[currentBuffTarget] != null && playerManager.players[currentBuffTarget].GetComponent<Crawler>() != null)
            playerManager.players[currentBuffTarget].GetComponent<Crawler>().DisableEffect(buffEffect);
		buffing = false;
		buffRay.Draw = false;
    }

	void applyDebuff ()
	{
		if (mainHand.currentItem != 1 || mainHand.radialMenuAccessed) {
			if (debuffing)
				stopDebuffing ();
			debuffCharge += DebuffChargeRate * Time.deltaTime;
			return;
		}

		if (mainHand.getTrigger ()) {
			if (debuffCharge > 0) {
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
					debuffCharge -= DebuffDechargeRate * Time.deltaTime;
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
						Debug.Log ("New enemy target! closest: " + closest + ", currentDebuffTarget: " + currentDebuffTarget);
						mainHand.hapticFeedback (hapticforce);
					}
					if (!debuffing)
						startDebuffing ();
				} else {
					debuffDestination = rayOrigin.position;
					stopDebuffing ();
				}
			} else {
				if (debuffing)
					stopDebuffing ();
				mainHand.vibrateFrequently (hapticforce, vibrateFrequency);

			}
		} else {
			debuffCharge += DebuffChargeRate * Time.deltaTime;
		}
		if (mainHand.getTriggerUp ()) {
			debuffDestination = rayOrigin.position;
			stopDebuffing ();
		}
		debuffCharge = Mathf.Clamp (debuffCharge, 0, maxDebuffCharge);
		
	}

	private void startDebuffing()
	{
		if (currentDebuffTarget != -1 && !debuffing)
		{
			aidLineMaterial.Lerp (aidLineMaterial, pickerInvisible, 1);
			debuffing = true;
			debuffRay.Draw = true;
			playerManager.enemies[currentDebuffTarget].GetComponent<Enemy>().EnableEffect(debuffEffect);
		}
	}

	private void stopDebuffing()
	{
        if (currentDebuffTarget != -1 && playerManager.enemies[currentDebuffTarget]!= null && playerManager.enemies[currentDebuffTarget].GetComponent<Enemy>() != null)
			playerManager.enemies[currentDebuffTarget].GetComponent<Enemy>().DisableEffect(debuffEffect);
		debuffing = false;
		debuffRay.Draw = false;
	}

	void initRays ()
	{
		buffCharge = maxBuffCharge;
		debuffCharge = maxDebuffCharge;
		debuffDestination = buffDestination = rayOrigin.position;
		debuffRay.origin = buffRay.origin = rayOrigin;
		playerManager = GameObject.Find ("PlayerManagers").GetComponent<PlayersManager>();
	}

    void UpdateAidRay()
    {
		if (((mainHand.currentItem == 0 && !buffing && buffCharge != 0) || (mainHand.currentItem == 1 && !debuffing && debuffCharge != 0)) && mainHand.getTrigger())
        {
            aidLineMaterial.Lerp(aidLineMaterial, pickerVisible, Time.deltaTime);
        }
        else
        {
            aidLineMaterial.Lerp(aidLineMaterial, pickerInvisible, Time.deltaTime * 4);
        }
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
		if (mainHand.currentItem == 2) {
			firePool.SetActive(true);
			if (firePool.transform.localScale.magnitude < 0.99 * firePoolScale.magnitude)
				firePool.transform.localScale = Vector3.Lerp(firePool.transform.localScale, firePoolScale*Mathf.Pow(Mathf.Clamp01(firePoolSize/maxFirePoolSize),1f/3f), Time.deltaTime* poolGrowSpeed );
		} else {
			if (firePool.transform.localScale.magnitude >= 0.01 * firePoolScale.magnitude)
				firePool.transform.localScale = Vector3.Lerp(firePool.transform.localScale, Vector3.zero, Time.deltaTime * poolGrowSpeed);
			else
				firePool.SetActive(false);
		}

		if (!chargingFire)
			firePoolSize += FirePoolRecharge * Time.deltaTime; 

        if (mainHand.currentItem != 2 || mainHand.radialMenuAccessed)
        {

            if (chargingFire)
                dropFireBall();
            return;
        }

		if (mainHand.getTrigger() && picker.pooling && !chargingFire && charge<maxCharge && firePoolSize >= minCharge)
        {
				chargingFire = true;
                fireBallVis.SetActive(true);
                fireAtt.attracting = true;
        }
		if (mainHand.getTrigger() && chargingFire && charge<maxCharge && (mainHand.transform.position-offHand.transform.position).magnitude < maxChargeDistance &&  firePoolSize > 0.1)
        {
			float chargePlus = Mathf.Min (chargeRate * Time.deltaTime, firePoolSize);
			charge += chargePlus;
			firePoolSize -= chargePlus;
			charge = Mathf.Clamp (charge, 0, maxCharge);
			fireBallVis.transform.localScale = charge / maxCharge * fireVisScale;
			mainHand.hapticFeedback ((ushort)(charge/maxCharge*hapticforce));
		}

		if (charge >= maxCharge || (mainHand.transform.position - offHand.transform.position).magnitude >= maxChargeDistance || firePoolSize <= 0.1)
        {
            fireAtt.attracting = false;
            chargingFire = false;
        }
        if(chargingFire || Mathf.Sin(Time.time*Mathf.PI*2* vibrateFrequency) > 0)
			mainHand.hapticFeedback ((ushort)(Mathf.Pow(charge/maxCharge,2)*hapticforce));

        if (mainHand.getTriggerUp())
            dropFireBall();
		
        lastPos = throwBase.position;

		firePoolSize = Mathf.Clamp (firePoolSize, 0, 300);
    }
    void dropFireBall()
    {
		if (charge > minCharge) {
			GameObject fireball = Instantiate (fireBallPrefab, throwBase.position, throwBase.rotation);
			fireball.GetComponent<Rigidbody> ().velocity = (throwBase.position - lastPos) / Time.deltaTime;
			fireball.GetComponent<ThrowableAbility> ().chargeMulti = charge / maxCharge;
			fireball.transform.localScale = fireBallVis.transform.lossyScale;
			NetworkServer.Spawn(fireball);
		}
		fireBallVis.SetActive (false);
        charge = 0;
		chargingFire = false;
        fireAtt.attracting = false;
    }

	public void FireBallCollected()
	{
		firePoolSize = Mathf.Clamp (firePoolSize+maxCharge, 0, maxFirePoolSize);
	}

	public void HealOrbCollected()
	{
		healPoolSize = Mathf.Clamp (healPoolSize+maxCharge, 0, maxHealPoolSize);
	}

    void UpdateAbilityPicker()
    {
		if (((mainHand.currentItem == 3  && !chargingHeal && healPoolSize >= minCharge)|| (mainHand.currentItem == 2 && !chargingFire  && firePoolSize >= minCharge)) && mainHand.getTrigger()&& charge < maxCharge)
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
		if (mainHand.currentItem == 3) {
			healPool.SetActive(true);
			if (healPool.transform.localScale.magnitude < 0.99 * healPoolScale.magnitude)
				healPool.transform.localScale = Vector3.Lerp(healPool.transform.localScale, healPoolScale*Mathf.Pow(Mathf.Clamp01(healPoolSize/maxHealPoolSize), 1f/3f), Time.deltaTime* poolGrowSpeed );
		} 
		else {			
			if (healPool.transform.localScale.magnitude >= 0.01 * healPoolScale.magnitude)
				healPool.transform.localScale = Vector3.Lerp(healPool.transform.localScale, Vector3.zero, Time.deltaTime * poolGrowSpeed);
			else
			healPool.SetActive(false);
		}
		if (!chargingHeal)
			healPoolSize += healPoolRecharge * Time.deltaTime; 
		if (mainHand.currentItem != 3 || mainHand.radialMenuAccessed)
		{
			if (chargingHeal)
				dropHealOrb();
			return;
		}

		if (mainHand.getTrigger() && picker.pooling && !chargingHeal && charge<maxCharge && healPoolSize >= minCharge)
		{
			chargingHeal = true;
			healOrbVis.SetActive(true);
			healOrbAtt.attracting = true;
		}
		if (mainHand.getTrigger() && chargingHeal && charge<maxCharge && (mainHand.transform.position-offHand.transform.position).magnitude < maxChargeDistance && healPoolSize > 0.1)
		{
			float chargePlus = Mathf.Min (chargeRate * Time.deltaTime, healPoolSize);
			charge += chargePlus;
			healPoolSize -= chargePlus;
			charge = Mathf.Clamp (charge, 0, maxCharge);
			healOrbVis.transform.localScale = charge / maxCharge * healVisScale;
			mainHand.hapticFeedback ((ushort)(charge/maxCharge*hapticforce));
		}

		if (charge >= maxCharge || (mainHand.transform.position - offHand.transform.position).magnitude >= maxChargeDistance || healPoolSize <= 0.1)
		{
			healOrbAtt.attracting = false;
			chargingHeal = false;
		}
		if(chargingHeal || Mathf.Sin(Time.time*Mathf.PI*2* vibrateFrequency) > 0)
			mainHand.hapticFeedback ((ushort)(Mathf.Pow(charge/maxCharge,2)*hapticforce));

		if (mainHand.getTriggerUp())
			dropHealOrb();
		
		healPoolSize = Mathf.Clamp (healPoolSize, 0, maxHealPoolSize);
		lastPos = throwBase.position;
	}
	void dropHealOrb()
	{
		if (charge > minCharge) {
			GameObject healOrb = Instantiate (healOrbPrefab, throwBase.position, throwBase.rotation);
			healOrb.GetComponent<Rigidbody> ().velocity = (throwBase.position - lastPos) / Time.deltaTime;
			healOrb.GetComponent<ThrowableAbility> ().chargeMulti = charge / maxCharge;
			healOrb.transform.localScale = healOrbVis.transform.lossyScale;

			NetworkServer.Spawn(healOrb);
			Debug.Log("Spawned healorb");

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

	void initTeleport()
	{
		HmdQuad_t pRect = new HmdQuad_t();
		SteamVR_PlayArea.GetBounds (playArea.size, ref pRect);
		teleWidth = pRect.vCorners0.v0 - pRect.vCorners2.v0;
		teleLength = pRect.vCorners0.v2 - pRect.vCorners2.v2;
		telePlat.localScale = new Vector3 (teleWidth, telePlatThiccness, teleLength);
	}
	void applyTeleport ()
	{
		teleMat.Lerp (teleMatLow, teleMatHigh, (Mathf.Sin (Time.time * teleFrequency) + 1) / 2);
		Vector3 position = Vector3.up * telePlatThiccness/2;
		if (offHand.radialMenuAccessed) {
			telePlat.gameObject.SetActive (true);
			switch (offHand.currentItem) {
			case 0:
				position += Vector3.back * teleLength;
				if (currTel.x >= maxTel.x) {
					teleMat.Lerp (teleMatBlock, teleMatBlock, .5f);
				}
				break;
			case 1: 
				position += Vector3.right * teleWidth;
				if (currTel.y >= maxTel.y) {
					teleMat.Lerp (teleMatBlock, teleMatBlock, .5f);
				}
				break;		
			case 2:
				position += Vector3.forward * teleLength;
				if (currTel.x <= maxTel.z) {
					teleMat.Lerp (teleMatBlock, teleMatBlock, .5f);
				}
				break;
			case 3: 
				position += Vector3.left * teleWidth;
				if (currTel.y <= maxTel.w) {
					teleMat.Lerp (teleMatBlock, teleMatBlock, .5f);
				}
				break;
			default:
				break;
			}
			telePlat.localPosition = position;
		} else {
			telePlat.gameObject.SetActive (false);
		}
		if (offHand.getTouchpadUp()) {
			switch (offHand.currentItem) {
			case 0:
				if (currTel.x < maxTel.x) {
					transform.position += transform.localScale.z * Vector3.back * teleLength;
					currTel += new Vector2 (1, 0);
				}
				break;
			case 1: 
				if (currTel.y < maxTel.y) {
					transform.position += transform.localScale.x * Vector3.right * teleWidth;
					currTel += new Vector2 (0, 1);
				}
				break;		
			case 2:
				if (currTel.x > maxTel.z) {
					transform.position += transform.localScale.z * Vector3.forward * teleLength;
					currTel += new Vector2 (-1, 0);
				}
				break;
			case 3: 
				if (currTel.y > maxTel.w) {
					transform.position += transform.localScale.x * Vector3.left * teleWidth;
					currTel += new Vector2 (0, -1);
				}
				break;
			default:
				break;
			}
		}
	}
}
