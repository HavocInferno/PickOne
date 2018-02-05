using UnityEngine;
using UnityEngine.Networking;

public class RightHandFollower : MasterFollower
{
	public BezierCurve buff, debuff;
	public Transform origin;
	[SyncVar]
	public Vector3 buffTarget, debuffTarget;
	public Controller controller;
	public Master master;
	[SyncVar]
	public bool isBuffed, isDebuffed;
	public Transform buffChargeIndicator;
	private Vector3 chargeIndicatorScale;
	public ParticleSystem[] buffSystems, debuffSystems;
	public AudioSource buffSource, debuffSource;
	public AudioSource raySpark, ping; 
	[SyncVar]
	public float buffCharge, debuffCharge;

	public GameObject healOrbVis;
	Vector3 healVisScale;
	public GameObject fireBallVis;
	Vector3 fireVisScale;

	public Material masterMaterial;
	public Material defaultMaterial;
	public Material[] abilityMaterials;
	public Material buffActive;
	public Material debuffActive;
	public Material pingMaterial;

	public Transform pingArrow, capsule;
	public float growspeed =15;
	public Vector3 arrowscale, capsuleScale;
	[SyncVar]
	float chargeFire =0; 
	[SyncVar]
	float chargeHeal =0; 

	[SyncVar]
	int lastItem = -1;
	[SyncVar]
	int currentitem =-1;
	[SyncVar]
	bool gripping = false;
	// Use this for initialization
	void Start()
    {
		chargeIndicatorScale = buffChargeIndicator.localScale;
		foreach(var buffSystem in buffSystems)
			buffSystem.enableEmission = false; 
		foreach(var debuffSystem in debuffSystems)
			debuffSystem.enableEmission = false; 
		arrowscale = pingArrow.localScale;
		capsuleScale = capsule.localScale;
        if (followed.gameObject.activeInHierarchy)
        {
			if(controller == null)
				controller = followed.GetComponentInParent<Controller> ();
        }
        else
        {
			debuff.origin = buff.origin = origin;
        }
		initThrowables ();
    }
	void initThrowables ()
	{
		fireVisScale = fireBallVis.transform.localScale;
		healVisScale = healOrbVis.transform.localScale;
		if (followed.gameObject.activeInHierarchy) {
			healOrbVis.SetActive (false);
			fireBallVis.SetActive (false);
		}
	}
	void updateThrowables ()
	{
		if (followed.gameObject.activeInHierarchy) {
			chargeFire = master.chargeFire;
			chargeHeal = master.chargeHeal;
			healOrbVis.SetActive (false);
			fireBallVis.SetActive (false);
		} else {
			fireBallVis.transform.localScale = fireVisScale * chargeFire;
			healOrbVis.transform.localScale = healVisScale * chargeHeal;

			if (fireBallVis.transform.localScale.magnitude < 0.01)
				fireBallVis.SetActive (false);
			else
				fireBallVis.SetActive (true);

			if (healOrbVis.transform.localScale.magnitude < 0.01)
				healOrbVis.SetActive (false);
			else
				healOrbVis.SetActive (true);
		}


	}
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update();
        if (followed.gameObject.activeInHierarchy)
        {

			origin.position = master.rayOrigin.position;
            origin.rotation = master.rayOrigin.rotation;

			buffTarget = master.buffDestination;
			isBuffed = master.buffing;
			debuffTarget = master.debuffDestination;
			isDebuffed = master.debuffing;
			lastItem = currentitem;
			buffCharge = Mathf.Clamp01(master.BuffCharge/master.maxBuffCharge);
			debuffCharge = Mathf.Clamp01(master.DebuffCharge/master.maxDebuffCharge);

			if (controller == null)
				controller = followed.GetComponentInParent<Controller> ();
			else {
				currentitem = controller.currentItem;
				gripping = controller.getGrip ();
			}
        }
        else
        {
            buff.destination = buffTarget;
            buff.Draw = isBuffed;
			debuff.destination = debuffTarget;
			debuff.Draw = isDebuffed;
        }
		buffSource.enabled = isBuffed;
		debuffSource.enabled = isDebuffed;

		foreach(var buffSystem in buffSystems)
			buffSystem.enableEmission = isBuffed;
		foreach(var debuffSystem in debuffSystems)
			debuffSystem.enableEmission = isDebuffed;

		Vector3 handScale = chargeIndicatorScale;
		if (currentitem == 0)
			handScale -= new Vector3 (0,0,(1-buffCharge)*chargeIndicatorScale.z);
		if(currentitem == 1)
			handScale -= new Vector3 (0,0,(1-debuffCharge)*chargeIndicatorScale.z);
		buffChargeIndicator.localScale = Vector3.Lerp(buffChargeIndicator.localScale, handScale, Time.deltaTime*growspeed);

		if (buffChargeIndicator.localScale.z < 0.001)
			buffChargeIndicator.localScale = new Vector3 (buffChargeIndicator.localScale.x, buffChargeIndicator.localScale.y, 0.001f);


		buffSource.pitch = (float) 0.5f + buffCharge * 0.5f;
		debuffSource.pitch = (float) 0.5f + debuffCharge * 0.5f;
		raySpark.enabled = isBuffed || isDebuffed;


		if (isBuffed) {
			masterMaterial.Lerp(masterMaterial,buffActive,Time.deltaTime*growspeed);
		}
		else if (isDebuffed) {
			masterMaterial.Lerp(masterMaterial,debuffActive,Time.deltaTime*growspeed);
		}
		else if (currentitem >= 0 && currentitem < abilityMaterials.Length)
			masterMaterial.Lerp (masterMaterial, abilityMaterials [currentitem], Time.deltaTime);
		else
			masterMaterial.Lerp (masterMaterial, defaultMaterial, Time.deltaTime);

		if (gripping) {
			//masterMaterial.Lerp (masterMaterial, pingMaterial, Time.deltaTime*5);
			pingArrow.localScale = Vector3.Lerp (pingArrow.localScale, arrowscale, Time.deltaTime*growspeed);
			capsule.localScale = Vector3.Lerp (capsule.localScale, Vector3.zero, Time.deltaTime*growspeed);
		} else {
			pingArrow.localScale = Vector3.Lerp (pingArrow.localScale, Vector3.zero, Time.deltaTime*growspeed);
			capsule.localScale = Vector3.Lerp (capsule.localScale, capsuleScale, Time.deltaTime*growspeed);
			
		}
		ping.enabled = (pingArrow.localScale.magnitude > 0.01);
		updateThrowables ();
    }
}