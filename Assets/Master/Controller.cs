using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{

    // Use this for initialization

    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;
    private Vector2 trackpad;

    //radial menu
    public float itemDistance = 15;
    public string[] items;
    private Text[] texts;
   // public Transform pointer;
    public GameObject UI;
    public GameObject textPrefab; 
    private int currentItem =-1, lastItem =-1;
    
    private Color currentColor;
    private Vector3 currentScale;
    public float highlightPopScale = 1.6f;
    public float highlightScale = 1.3f;
    public Color highlightPopColor = Color.white;
    public Color highlightColor = Color.blue;
    public float highlightSpeed = 10f;
    private ushort hapticforce = 3999;

	//buff test
	public float maxdistance = 10;
	public Transform origin; 
	public Transform[] targets;
	public Texture2D tex;
	public BezierCurve bez;
	public int currentTarget = -1;
	public float rayspeed = 30; 


    void Start () {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        UI.SetActive(false);
        
        GameObject nibba;
        texts = new Text[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            nibba = Instantiate(textPrefab, UI.transform);
            float angle = (360 / items.Length)*i;
            nibba.transform.localPosition = new Vector3(Mathf.Sin(angle/180*Mathf.PI), Mathf.Cos(angle / 180 * Mathf.PI), 0)*itemDistance;
            texts[i] = nibba.GetComponent<Text>();
            texts[i].text = items[i];
        }
        currentColor = texts[0].color;
		bez.Draw = false;
		bez.origin = origin;
    }
    private int getCurrentItemIndex()
    {
        return (int) ((items.Length-(((Mathf.Atan2(trackpad.y, trackpad.x) / Mathf.PI * 180)+270-(360/items.Length/2))%360) / (360 / items.Length)));
    }
	
	// Update is called once per frame
	void Update () {

        lastItem = currentItem;
        device = SteamVR_Controller.Input((int)trackedObject.index);
        trackpad = device.GetAxis();
        if (trackpad.x !=0 || trackpad.y !=0)
        {
            trackpad.Normalize();
            //pointer.transform.localRotation = Quaternion.Euler(0, -Mathf.Atan2(trackpad.y,trackpad.x)/Mathf.PI*180+90,0);
            Debug.Log("Current Item: "+items[getCurrentItemIndex()]+ " Jockdipimm:" + (Mathf.Atan2(trackpad.y, trackpad.x) / Mathf.PI * 180));
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            device.TriggerHapticPulse(hapticforce);
            //pointer.gameObject.SetActive(true);
            UI.SetActive(true);
        }
        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            currentItem = getCurrentItemIndex();
            if (currentItem != lastItem)
            {
                if (lastItem != -1)
                    texts[lastItem].color = currentColor;
                currentColor = texts[currentItem].color;
                texts[currentItem].color = highlightPopColor;
                if (lastItem != -1)
                    texts[lastItem].rectTransform.localScale = currentScale;
                currentScale = texts[currentItem].rectTransform.localScale;
                texts[currentItem].transform.localScale *= highlightPopScale;
                device.TriggerHapticPulse(hapticforce);
            }
            texts[currentItem].rectTransform.localScale = Vector3.Lerp(texts[currentItem].rectTransform.localScale, highlightScale*currentScale, Time.deltaTime*highlightSpeed);
            texts[currentItem].color = Color.Lerp(texts[currentItem].color, highlightColor, Time.deltaTime * highlightSpeed);
        }
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
           // pointer.gameObject.SetActive(false);
            device.TriggerHapticPulse(hapticforce);
            UI.SetActive(false);
        }


        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            device.TriggerHapticPulse(2000);
            Debug.Log("Pew!");
        }



		if (currentItem == 0 ) {
			if(device.GetPress(SteamVR_Controller.ButtonMask.Trigger)){
				int closest = -1;
				float closestdistance = maxdistance;

				for (int i = 0; i < targets.Length; i++) {
					if (Vector3.Cross (origin.forward, targets [i].position - origin.position).magnitude < closestdistance && Vector3.Dot(origin.forward, targets [i].position - origin.position) > 0.1) {
						closestdistance = Vector3.Cross (origin.forward, targets [i].position - origin.position).magnitude;
						closest = i;
					}
				}
				if (closest != -1) {
					if (bez.Draw == false)
						bez.Draw = true;
					bez.destination = Vector3.Lerp(bez.destination,targets [closest].position,Time.deltaTime*rayspeed);
					if (closest == currentTarget)
						device.TriggerHapticPulse ((ushort)(1000 * Mathf.Pow(Vector3.Cross (origin.forward, targets [closest].position - origin.position).magnitude / maxdistance,2)));
					else {
						device.TriggerHapticPulse (hapticforce);
						currentTarget = closest;
					}
				}
				else
					bez.Draw = false;

			}
			if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
				bez.Draw = true;
			if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
				bez.Draw = false;
				
		}
	}
}
