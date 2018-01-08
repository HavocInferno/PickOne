using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        // Get the object being inspected
        MapGenerator mapGen = target as MapGenerator;

        if(GUILayout.Button("Reset Map"))
        {
            mapGen.ResetMap();
        }

        if (GUILayout.Button("Do Iteration Step"))
        {
            mapGen.PerformIteration();
        }

        if (GUILayout.Button("Do Given Iterations"))
        {
            for (int iterIdx = 0; iterIdx < mapGen.numberOfIterations; iterIdx++)
            {
                mapGen.PerformIteration();
            }
        }
    }
}
#endif
