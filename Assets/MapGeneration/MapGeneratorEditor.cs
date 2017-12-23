using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        // Get the object being inspected
        MapGenerator mapGen = target as MapGenerator;

        if(GUILayout.Button("Generate Map"))
        {
            mapGen.GenerateMap();
        }

        if (GUILayout.Button("Do Iteration Step"))
        {
            mapGen.PerformIteration();
        }
    }
}
