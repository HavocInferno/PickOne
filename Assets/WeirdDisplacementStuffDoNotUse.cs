using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeirdDisplacementStuffDoNotUse : MonoBehaviour {

    Mesh mesh;
    public Vector3 hit; 
    public float fuckup, speed, width;
    Vector3[] vertices, normals;
    float passedTime = 0;
    // Use this for initialization
    void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        normals = mesh.normals;
        hit = vertices[0];
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3[] vertices2 = new Vector3[vertices.Length];

        int i = 0;  
        while (i < vertices.Length)
        {
            vertices2[i] = vertices[i] + normals[i]*Mathf.Sin(Mathf.Clamp(passedTime*speed-((hit-vertices[i]).magnitude)/width,0,Mathf.PI))*fuckup;
            i++;
        }
        mesh.vertices = vertices2;
        passedTime += Time.deltaTime;

    }
}
