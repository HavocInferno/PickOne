#if UNITY_EDITOR
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
		resetButtonContent = new GUIContent ("Delete instance", "Delete current random instance");

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

		EditorGUILayout.PropertyField (roi_objectType);
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (roi_maxSize);
		EditorGUILayout.PropertyField (roi_volumeOffset);

		EditorGUILayout.Space ();
		CustomEditorList.Show(roi_objects);
		EditorGUILayout.Space ();

		if (GUILayout.Button (rollButtonContent)) {
			foreach (Object roi in targets) {
				((RandomObjectInstancer)roi).rolledObject = ((RandomObjectInstancer)roi).objects [Random.Range (0, ((RandomObjectInstancer)roi).objects.Count)];
			}
			foreach (Object roi in targets) {
				((RandomObjectInstancer)roi).showObject ();
				EditorUtility.SetDirty (roi);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
		}
		if (GUILayout.Button (resetButtonContent)) {
			foreach (Object roi in targets) {
				((RandomObjectInstancer)roi).delObject ();
			}
		}
		EditorGUI.BeginDisabledGroup (true);
		EditorGUILayout.PropertyField (roi_rolledObject);
		EditorGUILayout.PropertyField (roi_instance);
		EditorGUI.EndDisabledGroup ();
		serializedObject.ApplyModifiedProperties ();

		//base.OnInspectorGUI ();
	}
}
#endif
