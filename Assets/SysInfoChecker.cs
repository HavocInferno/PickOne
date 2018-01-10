using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SysInfoChecker : MonoBehaviour {

	public Text sysinfoText;

	// Use this for initialization
	void Start () {
		if (!sysinfoText)
			sysinfoText = GetComponent<Text> ();

		getAvailableSysInfo ();
	}

	void getAvailableSysInfo () {
		string infotext = "System Info: \n";

		infotext += SystemInfo.deviceName + " (" + /*SystemInfo.deviceType + ", " +*/ SystemInfo.operatingSystem + ")\n";

		float cpuGHz = (float)(SystemInfo.processorFrequency) / 1000f;
		infotext += SystemInfo.processorType + " (" + SystemInfo.processorCount + "T x " + cpuGHz.ToString("F2") + "GHz)\n";

		infotext += SystemInfo.systemMemorySize + "MB RAM\n";

		infotext += SystemInfo.graphicsDeviceName + " (" + SystemInfo.graphicsMemorySize + "MB VRAM)\n";

		sysinfoText.text = infotext;
	}
}
