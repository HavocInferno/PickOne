using UnityEngine;
using UnityEngine.Networking;

public class RightHandFollower : MasterFollower
{
	public BezierCurve buff, debuff;
	public Transform origin;
	[SyncVar]
	public Vector3 buffTarget, debuffTarget;
	public Controller controller;
	[SyncVar]
	public bool isBuffed, isDebuffed;
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
            origin.position = controller.rayOrigin.position;
            origin.rotation = controller.rayOrigin.rotation;

			buffTarget = controller.buffDestination;
            isBuffed = controller.buffing;
			debuffTarget = controller.debuffDestination;
			isDebuffed = controller.debuffing;
        }
        else
        {
            buff.destination = buffTarget;
            buff.Draw = isBuffed;
			debuff.destination = debuffTarget;
			debuff.Draw = isDebuffed;
        }
    }
}