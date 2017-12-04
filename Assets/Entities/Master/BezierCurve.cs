using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour {

	private bool draw = true;
	public bool Draw
	{
		set{ draw = value;
			if (value)
				ShowLineSegments ();
			else {
				HideLineSegments ();
				destination = origin.position;
			}
		}
		get{ return draw;}
	}

	public Transform origin; 
	public Vector3 destination;
	public Material material;

	public int segmentCount = 60;
	public float thickness = 0.01f;

	private LineRenderer[] lineRenderers;
	private Vector3 midpoint;

	public string name;

	// Use this for initialization
	void Start () {
		lineRenderers = new LineRenderer[segmentCount];
		GameObject parent = new GameObject( name+"_BezierCurve");
		parent.transform.parent = transform;
		for ( int i = 0; i < segmentCount; ++i )
		{
			GameObject newObject = new GameObject( name+"_LineSegment_" + i );
			newObject.transform.parent = parent.transform;
			newObject.transform.position = parent.transform.position;
			lineRenderers[i] = newObject.AddComponent<LineRenderer>();

			lineRenderers[i].receiveShadows = false;
			lineRenderers[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			lineRenderers[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			lineRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			lineRenderers[i].material = material;
			#if (UNITY_5_4)
			lineRenderers[i].SetWidth( thickness, thickness );
			#else
			lineRenderers[i].startWidth = thickness;
			lineRenderers[i].endWidth = thickness;
			#endif
			lineRenderers[i].enabled = false;
		}
		if (origin == null)
			origin = transform;
		destination = origin.position;
		Draw = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (draw && origin != null) {
			midpoint = origin.position+Vector3.Project (destination - origin.position, origin.forward);
			for (int i = 0; i < lineRenderers.Length; i++) {
				DrawArcSegment (i, ((float)i) / lineRenderers.Length, ((float)i + 1) / lineRenderers.Length);
			}
		}
		
	}

	private void DrawArcSegment( int index, float lerper1, float lerper2)
	{
		lineRenderers[index].enabled = true;
		lineRenderers[index].SetPosition( 0,  Vector3.Lerp(Vector3.Lerp(origin.position, midpoint, lerper1), Vector3.Lerp(midpoint,destination, lerper1), lerper1));
		lineRenderers[index].SetPosition( 1,  Vector3.Lerp(Vector3.Lerp(origin.position, midpoint, lerper2), Vector3.Lerp(midpoint,destination, lerper2), lerper2));
	}

	private void HideLineSegments()
	{
		if ( lineRenderers != null )
		{
			for ( int i = 0; i < lineRenderers.Length; ++i )
			{
				lineRenderers[i].enabled = false;
			}
		}
	}
	private void ShowLineSegments()
	{
		if ( lineRenderers != null )
		{
			for ( int i = 0; i < lineRenderers.Length; ++i )
			{
				lineRenderers[i].enabled = true;
			}
		}
	}
}
