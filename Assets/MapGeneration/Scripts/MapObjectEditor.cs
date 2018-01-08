using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(MapObject))]
public class MapObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var mapObject = target as MapObject;

        if (GUILayout.Button("Default Allowed Connections"))
        {
            mapObject.SetDefaultConnections();
        }
    }
}
#endif
