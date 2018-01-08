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
	public Material masterMaterial;
	public Material[] abilityMaterials;
	[SyncVar]
	int lastItem = -1;
	[SyncVar]
	int currentitem =-1;
	// Use this for initialization
	void Start()
    {
        if (followed.gameObject.activeInHierarchy)
        {
            controller = followed.GetComponent<Controller>();
        }
        else
        {
			debuff.origin = buff.origin = origin;
        }
    }
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update();
        if (followed.gameObject.activeInHierarchy)
        {
            if (controller == null)
                controller = followed.GetComponent<Controller>();
			origin.position = master.rayOrigin.position;
            origin.rotation = master.rayOrigin.rotation;

			buffTarget = master.buffDestination;
			isBuffed = master.buffing;
			debuffTarget = master.debuffDestination;
			isDebuffed = master.debuffing;
			lastItem = currentitem;
			currentitem = controller.currentItem;
			//color change
        }
        else
        {
            buff.destination = buffTarget;
            buff.Draw = isBuffed;
			debuff.destination = debuffTarget;
			debuff.Draw = isDebuffed;
        }
		if (currentitem >= 0 && currentitem < abilityMaterials.Length)
			masterMaterial.Lerp (masterMaterial, abilityMaterials [currentitem], Time.deltaTime);
    }
}