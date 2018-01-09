using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
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

    public void SetDefaultConnections()
    {
        // Populate allowedConnections with default values when type is changed
        allowedConnections.Clear();

        switch (type)
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
    }

    private void OnValidate()
    {
        // Fix BoxCollider's dimensions when Floor's size is changed.
        var collider = GetComponent<BoxCollider>();
        var floorChild = GetComponentsInChildren<Transform>()
            .SingleOrDefault(x => x.name == "Floor");

        if (floorChild == null)
        {
            Debug.LogErrorFormat("{0} has no floor child!", this.name);
            return;
        }

        var floorCollider = floorChild.GetComponent<BoxCollider>();

        if(!floorCollider)
        {
            Debug.LogWarningFormat("{0} has no box collider!", floorChild.name);
            collider.size = new Vector3(floorChild.lossyScale.x, 8, floorChild.lossyScale.z);
        }
        else
        {
            collider.center = new Vector3(
                floorChild.localPosition.x,
                0,
                floorChild.localPosition.z
            );

            collider.size = new Vector3(
                floorCollider.size.x * floorChild.localScale.x,
                 8,
                 floorCollider.size.z * floorChild.localScale.z);
        }
    }
}
#endif
