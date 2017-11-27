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

    public bool isVisuallyDetectable = true;
}
