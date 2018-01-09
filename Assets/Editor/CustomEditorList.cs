#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class CustomEditorList {

	private static GUIContent
		addButtonContent = new GUIContent("+", "add item"),
		deleteButtonContent = new GUIContent("x", "delete");

	public static void Show(SerializedProperty list, bool showListSize = false) {
		EditorGUILayout.PropertyField (list);
		EditorGUI.indentLevel += 1;
		if (list.isExpanded) {
			if(showListSize)
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("Array.size"));
			for (int i = 0; i < list.arraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), GUIContent.none);
				if (GUILayout.Button (deleteButtonContent, EditorStyles.miniButton, GUILayout.Width (20f))) {
					int oldSize = list.arraySize;
					list.DeleteArrayElementAtIndex (i);
					if (list.arraySize == oldSize) {
						list.DeleteArrayElementAtIndex (i);
					}
				}
				EditorGUILayout.EndHorizontal ();
			}
			if (GUILayout.Button (addButtonContent)) {
				list.InsertArrayElementAtIndex (list.arraySize);
			}
		}
		EditorGUI.indentLevel -= 1;
	}
}
#endif
