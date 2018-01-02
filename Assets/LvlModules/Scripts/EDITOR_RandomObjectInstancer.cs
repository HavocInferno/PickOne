using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomObjectInstancer))]
[CanEditMultipleObjects]
public class EDITOR_RandomObjectInstancer : Editor {

	SerializedProperty roi_objectType;
	SerializedProperty roi_maxSize;
	SerializedProperty roi_objects;
	SerializedProperty roi_rolledObject;

	private static GUIContent
		rollButtonContent = new GUIContent ("Roll the dice!", "Randomly spawn an item from the list");

	void OnEnable() {
		roi_objectType = serializedObject.FindProperty ("objectType");
		roi_maxSize = serializedObject.FindProperty ("maxSize");
		roi_objects = serializedObject.FindProperty ("objects");
		roi_rolledObject = serializedObject.FindProperty ("rolledObject");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		//EditorGUILayout.LabelField ("skrrraaa");
		EditorGUILayout.PropertyField (roi_objectType);
		EditorGUILayout.PropertyField (roi_maxSize);
		//EditorGUILayout.PropertyField (roi_objects,true);
		CustomEditorList.Show(roi_objects);

		if (GUILayout.Button (rollButtonContent)) {
			foreach (Object roi in targets) {
				((RandomObjectInstancer)roi).rolledObject = ((RandomObjectInstancer)roi).objects [Random.Range (0, ((RandomObjectInstancer)roi).objects.Count)];
			}
			//roi_rolledObject.objectReferenceValue = roi_objects.GetArrayElementAtIndex(Random.Range(0, roi_objects.arraySize)).objectReferenceValue;
			serializedObject.ApplyModifiedProperties ();
			serializedObject.Update ();
			foreach (Object roi in targets) {
				((RandomObjectInstancer)roi).showObject ();
			}
		}
		EditorGUILayout.PropertyField (roi_rolledObject);
		serializedObject.ApplyModifiedProperties ();

		//base.OnInspectorGUI ();
	}
}
