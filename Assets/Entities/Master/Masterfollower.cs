using UnityEngine.Networking;
using UnityEngine;

public class MasterFollower : NetworkBehaviour
{
    public Transform followed;
	
	// Update is called once per frame
	protected virtual void Update()
    {
        if (followed != null && followed.gameObject.activeInHierarchy)
        {
            transform.position = followed.position;
            transform.rotation = followed.rotation;
			GetComponent<Renderer>().enabled = false; //temporary, pending a better solution
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
