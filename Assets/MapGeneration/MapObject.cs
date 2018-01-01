using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
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

    private void OnValidate()
    {
        // Populate allowedConnections with default values when type is changed
        allowedConnections.Clear();

        switch(type)
        {
            case ObjectTag.Junction:
            case ObjectTag.Room:
                allowedConnections.Add(ObjectTag.Corridor);
                break;

            case ObjectTag.Corridor:
                allowedConnections.Add(ObjectTag.Junction);
                allowedConnections.Add(ObjectTag.Room);
                break;

            default:
                Debug.LogErrorFormat("Invalid object type assigned to {0}", this.name);
                break;
        }

        // Fix BoxCollider's dimensions when Floor's size is changed.
        var collider = GetComponent<BoxCollider>();
        var floorChild = GetComponentsInChildren<Transform>()
            .SingleOrDefault(x => x.name == "Floor");

        if (floorChild == null)
        {
            Debug.LogErrorFormat("{0} has no floor child!", this.name);
            return;
        }

        collider.size = new Vector3(floorChild.lossyScale.x * 10, 8, floorChild.lossyScale.z * 10);
    }
}
