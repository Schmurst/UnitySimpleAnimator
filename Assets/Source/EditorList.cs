using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Lifted from:http://catlikecoding.com/unity/tutorials/editor/custom-list/ 

[System.Flags]
public enum EditorListOption
{
	None = 0,
	ListSize = 1,
	ListLabel = 2,
	ElementLabels = 4,
	ListButtons = 8,
	ElementButtons = 16,

	Default = ListSize | ListLabel | ElementLabels,
	AllButtons = ListButtons | ElementButtons,
	All = Default | AllButtons,
}

public static class EditorList
{
	private static GUIContent
	BtnDelete = new GUIContent ("x", "Delete"),
	BtnMoveUp = new GUIContent ("\u21A5", "Move Up"),
	BtnMoveDn = new GUIContent ("\u21A7", "Move Down"),
	BtnAddNew = new GUIContent ("+", "Add New"),
	BtnClearAll = new GUIContent ("x", "Clear All");

	private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

	//---------------------------------------------------------------------------------------------------------
	public static void Display(SerializedProperty _list, EditorListOption _options = EditorListOption.Default)
	{
		bool showLabel = (_options & EditorListOption.ListLabel) != 0;
		bool showSize  = (_options & EditorListOption.ListSize)  != 0;
		bool showButtons = (_options & EditorListOption.ListButtons)  != 0;

		if (showLabel)
		{
			if (showButtons)
			{
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.PropertyField (_list);
				if (GUILayout.Button (BtnAddNew, EditorStyles.miniButtonLeft, miniButtonWidth))
					_list.arraySize++;
				if (GUILayout.Button (BtnClearAll, EditorStyles.miniButtonRight, miniButtonWidth))
					_list.ClearArray ();
				EditorGUILayout.EndHorizontal ();
				EditorGUI.indentLevel++;
			}
			else
			{	
				EditorGUILayout.PropertyField (_list);
				EditorGUI.indentLevel++;
			}
		}

		if (!_list.isExpanded)
		{
			if (showSize)
				EditorGUILayout.PropertyField (_list.FindPropertyRelative ("Array.size"));
		
			DisplayElements (_list, _options);
		}

		if (showLabel)
			EditorGUI.indentLevel--;
	}

	//---------------------------------------------------------------------------------------------------------
	private static void DisplayElements(SerializedProperty _list, EditorListOption _options)
	{
		bool showLabels = (_options & EditorListOption.ElementLabels)  != 0;
		bool showButtons = (_options & EditorListOption.ElementButtons)  != 0;


		for (int i = 0; i < _list.arraySize; i++)
		{
			if (showButtons)
				EditorGUILayout.BeginHorizontal ();
			
			var element = _list.GetArrayElementAtIndex (i);
			if (showLabels)
				EditorGUILayout.PropertyField(element, true);	
			else
				EditorGUILayout.PropertyField(element, GUIContent.none, true);
			
			if (showButtons)
			{
				DrawAndHandleElementButtons (_list, i);
				EditorGUILayout.EndHorizontal ();
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------
	private static void DrawAndHandleElementButtons(SerializedProperty _list, int _idx)
	{
		bool isFirst = _idx == 0;
		bool isLast = _idx == _list.arraySize - 1;

		if (_list.arraySize > 0)
		{
			var style = (isFirst && isLast) ? EditorStyles.miniButton : EditorStyles.miniButtonRight;
			if (GUILayout.Button (BtnDelete, style, miniButtonWidth))
			{
				int oldSize = _list.arraySize;
				_list.DeleteArrayElementAtIndex (_idx);
				if(oldSize == _list.arraySize)
					_list.DeleteArrayElementAtIndex (_idx);	
			}
		}
	}
}	