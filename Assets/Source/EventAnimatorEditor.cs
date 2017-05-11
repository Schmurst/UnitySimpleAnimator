using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


using UnityEditor;
[CustomEditor (typeof(EventAnimator))]
public class EventAnimatorEditor : Editor
{
	GUIContent[] m_eventTypes;
	GUIContent[] m_dataTypes;
	SerializedProperty m_eventAnimations;
	List<bool> m_eventAnimVisibilities = new List<bool>();
	int m_selectedEventAnim;

	//---------------------------------------------------------------------------------------------------------
	void OnEnable()
	{
		m_eventAnimations = serializedObject.FindProperty ("m_eventAnimations");
		string[] eventNames = Enum.GetNames (typeof(EventAnimationType));
		string[] dataNames = Enum.GetNames(typeof(AnimationType));
		m_eventTypes = new GUIContent[eventNames.Length - 1];
		m_dataTypes = new GUIContent[dataNames.Length - 1];
		m_eventAnimVisibilities.Clear ();

		for (int i = 0; i < m_eventAnimations.arraySize; i++)
			m_eventAnimVisibilities.Add (true);

		for (int i = 0; i < eventNames.Length; i++)
			if(eventNames[i] != EventAnimationType.nullOrLength.ToString())
				m_eventTypes [i] = new GUIContent (eventNames [i]);

		for (int i = 0; i < dataNames.Length; i++)
			if(dataNames[i] != AnimationType.NullOrLength.ToString())
				m_dataTypes [i] = new GUIContent (dataNames [i]);
	}

	//---------------------------------------------------------------------------------------------------------
	public override void OnInspectorGUI ()
	{	
		serializedObject.Update ();

		// add new event button
		DrawAndHandleAddNewAnimationButton();

		// start layout of anim data
		int toBeRemoved = -1;
		bool selectedForRemoval = false;
		for (int i = 0; i < m_eventAnimations.arraySize; i++)
		{
			DrawAnimationData (m_eventAnimations.GetArrayElementAtIndex (i), ref selectedForRemoval, i);
			if (selectedForRemoval)
				toBeRemoved = i;
		}

		// remove this event animation
		if (toBeRemoved >= 0)
		{
			m_eventAnimations.DeleteArrayElementAtIndex (toBeRemoved);
			m_eventAnimVisibilities.RemoveAt (toBeRemoved);
		}

		serializedObject.ApplyModifiedProperties();
	}

	//---------------------------------------------------------------------------------------------------------
	void DrawAndHandleAddNewAnimationButton()
	{
		if (m_eventAnimations.arraySize < m_eventTypes.Length)
		{
			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Add New Event Type", GUILayout.MaxWidth (180f)))
			{
				ShowAddAnimationMenu ();
			}
			GUILayout.FlexibleSpace ();
			EditorGUILayout.EndHorizontal ();
		}
	}

	//---------------------------------------------------------------------------------------------------------
	void ShowAddAnimationMenu()
	{
		// SH: taken from UnityEngine.EventTrigger custom inspector
		GenericMenu menu = new GenericMenu();
		for (int i = 0; i < m_eventTypes.Length; ++i)
		{
			bool isValidChoice = true;
			for (int p = 0; p <m_eventAnimations.arraySize; ++p)
			{
				SerializedProperty animation = m_eventAnimations.GetArrayElementAtIndex(p);
				SerializedProperty type = animation.FindPropertyRelative("Type");
				if (type.enumValueIndex == i)
					isValidChoice = false;
			}

			if (isValidChoice)
				menu.AddItem(m_eventTypes[i], false, OnAddNewEventAnimation, i);
			else
				menu.AddDisabledItem(m_eventTypes[i]);
		}
		menu.ShowAsContext ();
		Event.current.Use();
	}

	//---------------------------------------------------------------------------------------------------------
	void DrawAnimationData(SerializedProperty _eventAnim, ref bool _toBeRemoved, int _idx)
	{
		var type = (EventAnimationType)_eventAnim.FindPropertyRelative ("Type").enumValueIndex;
		string typeName = type.ToString ();

		bool play = false;
		bool show = m_eventAnimVisibilities [_idx];
		DrawAnimationButtonBar (_eventAnim, typeName, _idx, ref show, ref play, ref _toBeRemoved);
		m_eventAnimVisibilities [_idx] = show;

		EditorGUI.indentLevel++;
		if (show)
		{
			EditorGUILayout.PropertyField (_eventAnim.FindPropertyRelative("ScaleAnims"), true);
		}
		EditorGUI.indentLevel--;

		if (play)
		{
			PlayAnimation (type);
		}
	}

	//---------------------------------------------------------------------------------------------------------
	void ShowAddAnimationDataMenu(SerializedProperty _eventAnim, int _idx)
	{
		m_selectedEventAnim = _idx;
		GenericMenu menu = new GenericMenu();
		for (int i = 0; i < m_dataTypes.Length; ++i)
			menu.AddItem(m_dataTypes[i], false, OnAddNewAnimationData, i);

		menu.ShowAsContext ();
		Event.current.Use();
	}

	//---------------------------------------------------------------------------------------------------------
	void DrawAnimationButtonBar(SerializedProperty _eventAnim, string _typeName, int _idx, ref bool _shown, 
								ref bool _play, ref bool _remove)
	{
		float defaultLabelWidth = EditorGUIUtility.labelWidth;
		var singleLineHeight = GUILayout.MaxHeight (EditorGUIUtility.singleLineHeight);

		// start top bar
		EditorGUILayout.BeginHorizontal ();
		EditorGUIUtility.labelWidth = 20f;
		EditorGUILayout.LabelField (_typeName);
		if (GUILayout.Button ("Add", singleLineHeight))
			ShowAddAnimationDataMenu (_eventAnim, _idx);
		if (GUILayout.Button (_shown ? "Hide" : "Show", singleLineHeight))
			_shown = !_shown;
		_play = GUILayout.Button ("Play", singleLineHeight);
		_remove = GUILayout.Button ("Delete", singleLineHeight);
		EditorGUILayout.EndHorizontal ();

		// start add animation bar
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.EndHorizontal ();

		//restore label
		EditorGUIUtility.labelWidth = defaultLabelWidth;
	}


	//---------------------------------------------------------------------------------------------------------
	void PlayAnimation(EventAnimationType _type)
	{
		var ea = serializedObject.targetObject as EventAnimator;
		ea.Editor_StartAnimation (_type);
	}

	//---------------------------------------------------------------------------------------------------------
	void OnAddNewAnimationData(object index)
	{
		var selection = (AnimationType)(int)index;
		var animation = m_eventAnimations.GetArrayElementAtIndex(m_selectedEventAnim);
		string typeName;

		//names here must match parameter names in EventAnimation
		switch (selection)
		{
		default:
		case AnimationType.NullOrLength:
			return;
		case AnimationType.Scale:
			typeName = "ScaleAnims";
			break;
		case AnimationType.Position:
			typeName = "PosAnims";
			break;
		case AnimationType.Rotation:
			typeName = "RotAnims";
			break;
		}

		var dataList = animation.FindPropertyRelative (typeName);
		dataList.arraySize += 1;
		serializedObject.ApplyModifiedProperties();
	}

	//---------------------------------------------------------------------------------------------------------
	void OnAddNewEventAnimation(object index)
	{
		int selected = (int)index;

		m_eventAnimations.arraySize += 1;
		SerializedProperty animation = m_eventAnimations.GetArrayElementAtIndex(m_eventAnimations.arraySize - 1);
		SerializedProperty type = animation.FindPropertyRelative("Type");
		type.enumValueIndex = selected;
		m_eventAnimVisibilities.Add (true);
		serializedObject.ApplyModifiedProperties();
	}
}