using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(RandomObjectInstancer))]
[CanEditMultipleObjects]
public class EDITOR_RandomObjectInstancer : Editor {

	SerializedProperty roi_objectType;
	SerializedProperty roi_maxSize;
	SerializedProperty roi_volumeOffset;
	SerializedProperty roi_objects;
	SerializedProperty roi_rolledObject;
	SerializedProperty roi_instance;

	private static GUIContent
		rollButtonContent = new GUIContent ("Roll the dice!", "Randomly spawn an item from the list"),
		resetButtonContent = new GUIContent ("Delete instances", "Delete current random instance");

	void OnEnable() {
		roi_objectType = serializedObject.FindProperty ("objectType");
		roi_maxSize = serializedObject.FindProperty ("maxSize");
		roi_objects = serializedObject.FindProperty ("objects");
		roi_rolledObject = serializedObject.FindProperty ("rolledObject");
		roi_volumeOffset= serializedObject.FindProperty ("volumeOffset");
		roi_instance = serializedObject.FindProperty ("instance");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		//EditorGUILayout.LabelField ("skrrraaa");
		EditorGUILayout.PropertyField (roi_objectType);
		EditorGUILayout.PropertyField (roi_maxSize);
		EditorGUILayout.PropertyField (roi_volumeOffset);
		//EditorGUILayout.PropertyField (roi_objects,true);
		CustomEditorList.Show(roi_objects);

		if (GUILayout.Button (rollButtonContent)) {
			foreach (Object roi in targets) {
				((RandomObjectInstancer)roi).rolledObject = ((RandomObjectInstancer)roi).objects [Random.Range (0, ((RandomObjectInstancer)roi).objects.Count)];
			}
			//roi_rolledObject.objectReferenceValue = roi_objects.GetArrayElementAtIndex(Random.Range(0, roi_objects.arraySize)).objectReferenceValue;
			//serializedObject.ApplyModifiedProperties ();
			//serializedObject.Update ();
			foreach (Object roi in targets) {
				((RandomObjectInstancer)roi).showObject ();
				EditorUtility.SetDirty (roi);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
			serializedObject.Update ();
			serializedObject.ApplyModifiedProperties ();
		}
		if (GUILayout.Button (resetButtonContent)) {
			foreach (Object roi in targets) {
				((RandomObjectInstancer)roi).delObject ();
			}
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.PropertyField (roi_rolledObject);
		EditorGUILayout.PropertyField (roi_instance);
		serializedObject.ApplyModifiedProperties ();

		//base.OnInspectorGUI ();
	}
}
