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
	public List<GameObject> objects = new List<GameObject>();
	public GameObject rolledObject;
	private GameObject instance;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void showObject() {
		if (instance)
			DestroyImmediate (instance);
		instance = Instantiate (rolledObject, transform.position, Quaternion.identity);
	}

	#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = new Color (1f, 0f, 0f, 0.3f);
		Gizmos.DrawCube(transform.position, maxSize);

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, maxSize);

		/*Gizmos.DrawLine(transform.position - Vector3.up * 1f, transform.position + Vector3.up * 1f);
		Gizmos.DrawLine(transform.position - Vector3.forward * 1f, transform.position + Vector3.forward * 1f);
		Gizmos.DrawLine(transform.position - Vector3.left * 1f, transform.position + Vector3.left * 1f);*/
	}
	#endif
}
