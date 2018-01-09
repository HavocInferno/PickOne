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
	public ParticleSystem buffSystem, debuffSystem;

	public GameObject healOrbVis;
	Vector3 healVisScale;
	public GameObject fireBallVis;
	Vector3 fireVisScale;

	public Material masterMaterial;
	public Material defaultMaterial;
	public Material[] abilityMaterials;
	public Material pingMaterial;

	public Transform pingArrow, capsule;
	public float growspeed =20;
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
		buffSystem.enableEmission = false; 
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

		buffSystem.enableEmission = isBuffed; 
		debuffSystem.enableEmission = isDebuffed; 

		if (currentitem >= 0 && currentitem < abilityMaterials.Length)
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
		updateThrowables ();
    }
}