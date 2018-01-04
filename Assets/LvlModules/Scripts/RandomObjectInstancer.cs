using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomObjectInstancer : MonoBehaviour {

	public enum ObjectType {
		MISC,
		LIGHT,
		FURNITURE,
		DEBRIS
	}
	public ObjectType objectType;
	public Vector3 maxSize = Vector3.one;
	public Vector3 volumeOffset = Vector3.zero;
	public List<GameObject> objects = new List<GameObject>();
	public GameObject rolledObject;
	public GameObject instance;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void showObject() {
		if (instance)
			DestroyImmediate (instance);
		instance = Instantiate (rolledObject, transform.position, transform.rotation);
	}
	public void delObject() {
		if (instance)
			DestroyImmediate (instance);
	}

	#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (Selection.Contains (gameObject)) {
			Gizmos.color = new Color (1f, 0f, 0f, 0.03f);
		} else {
			Gizmos.color = new Color (1f, 0f, 0f, 0.3f);
		}
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero + volumeOffset, maxSize);

		if (Selection.Contains (gameObject)) {
			Gizmos.color = new Color (1f, 0f, 0f, 0.3f);
		} else {
			Gizmos.color = Color.red;
		}
		Gizmos.DrawWireCube(Vector3.zero + volumeOffset, maxSize);

		/*Gizmos.DrawLine(transform.position - Vector3.up * 1f, transform.position + Vector3.up * 1f);
		Gizmos.DrawLine(transform.position - Vector3.forward * 1f, transform.position + Vector3.forward * 1f);
		Gizmos.DrawLine(transform.position - Vector3.left * 1f, transform.position + Vector3.left * 1f);*/
	}
	#endif
}
