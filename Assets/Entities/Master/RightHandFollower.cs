using UnityEngine;
using UnityEngine.Networking;

public class RightHandFollower : MasterFollower
{
	public BezierCurve buff, debuff;
	public Transform origin;
	[SyncVar]
	public Vector3 bufftarget;
	public Controller controller;
	[SyncVar]
	public bool isBuffed;
	// Use this for initialization
	void Start()
    {
        if (followed.gameObject.activeInHierarchy)
        {
            controller = followed.GetComponent<Controller>();
        }
        else
        {
            buff.origin = origin;
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
            bufftarget = controller.buffDestination;
            isBuffed = controller.buffing;
        }
        else
        {
            buff.destination = bufftarget;
            buff.Draw = isBuffed;
        }
    }
}