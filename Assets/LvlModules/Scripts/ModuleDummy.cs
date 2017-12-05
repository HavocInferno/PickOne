using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ModuleDummy : MonoBehaviour {

	public Rect area = new Rect(0f, 0f, 18f, 18f);
	public float height = 8f;
	public Color gizmoColor = new Color(1, 0, 0, 0.13f);

    #if UNITY_EDITOR
    void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(area.width, height, area.height));

		if (!Selection.Contains (gameObject)) {
			Gizmos.color = gizmoColor;
			Gizmos.DrawCube (transform.position, new Vector3 (area.width, height, area.height));
		}

		Gizmos.DrawLine(transform.position - Vector3.up * 1f, transform.position + Vector3.up * 1f);
		Gizmos.DrawLine(transform.position - Vector3.forward * 1f, transform.position + Vector3.forward * 1f);
		Gizmos.DrawLine(transform.position - Vector3.left * 1f, transform.position + Vector3.left * 1f);
	}
	#endif
}
