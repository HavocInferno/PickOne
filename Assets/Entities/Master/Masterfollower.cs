using UnityEngine.Networking;
using UnityEngine;

public class MasterFollower : NetworkBehaviour
{
    public Transform followed;
	protected bool following = false;
	// Update is called once per frame
	protected virtual void Update()
    {
        if (!following && followed != null && followed.gameObject.activeInHierarchy)
        {
			//transform.parent = followed;
			//transform.localRotation = Quaternion.identity;
			//transform.localPosition = Vector3.zero;
			//following = true; 
			transform.position = followed.position;
			transform.rotation = followed.rotation;
        }
    }

	//unused
	/* supposed to check whether a newly "joined"/loaded crawler is a VR master, if so, disable the renderer of the masterfollower object */
	public void CheckFollowerStatus(Crawler cc)
    {
		Debug.Log ("Checking masterfollower " + this.gameObject.name + ": " + cc.pName + " is VR Master: " + cc.isVRMasterPlayer.ToString ());
		if(cc.isVRMasterPlayer)
			GetComponent<Renderer>().enabled = false;
	}
}
