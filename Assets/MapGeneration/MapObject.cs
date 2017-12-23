using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public enum ObjectTag
    {
        Room,
        Corridor,
        Junction
    }

    public ObjectTag type;
    public List<ObjectTag> allowedConnections;

    public List<MapExitMarker> exitMarkers;
}
