using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DetectableObject : NetworkBehaviour
{
    public enum DetectionType
    {
        Visual,
        Other
    }

    public bool DetectableBy(DetectionType type)
    {
        if (type == DetectionType.Visual && !isVisuallyDetectable)
            return false;
        return true;
    }

    public HashSet<Enemy> detectedBy = new HashSet<Enemy>();
    public bool isVisuallyDetectable = true;
}
