using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Lidted from:http://catlikecoding.com/unity/tutorials/editor/custom-list/ 
public static class EditorList
{
	public static void Display(SerializedProperty _list, bool _showSize = true)
	{
		EditorGUILayout.PropertyField (_list);
		if (!_list.isExpanded)
			return;

		EditorGUI.indentLevel++;
		if(_showSize)
			EditorGUILayout.PropertyField (_list.FindPropertyRelative ("Array.size"));
		
		for (int i = 0; i < _list.arraySize; i++) {
			EditorGUILayout.PropertyField (_list.GetArrayElementAtIndex (i), true);
		}
		EditorGUI.indentLevel--;
	}
}