using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AnimationData
{
	// must be kept in same order as AnimationType
	// and should be kept uptodate with eventAnimation

	[SerializeField] protected AnimationScale[] 	m_scaleAnims = null;
	[SerializeField] protected AnimationPosition[] 	m_positionAnims = null;
	[SerializeField] protected AnimationPosition[] 	m_rotationAnims = null;

	//---------------------------------------------------------------------------------------------------------
	public virtual IAnimation[] GetAnimations()
	{
		int num = m_scaleAnims != null ? m_scaleAnims.Length : 0;
		num += m_positionAnims != null ? m_positionAnims.Length : 0;
		num += m_rotationAnims != null ? m_rotationAnims.Length : 0;
		var anims = new IAnimation[num];

		int i = 0;
		if(m_scaleAnims != null)
			for (; i < m_scaleAnims.Length; i++)
				anims [i] = (IAnimation)m_scaleAnims [i];

		if(m_positionAnims != null)
			for (; i < m_positionAnims.Length; i++)
				anims [i] = (IAnimation)m_positionAnims [i];

		if(m_rotationAnims != null)
			for (; i < m_rotationAnims.Length; i++)
				anims [i] = (IAnimation)m_rotationAnims[i];

		return anims;
	}

	//---------------------------------------------------------------------------------------------------------
	// Editor Code
	//---------------------------------------------------------------------------------------------------------
	#if UNITY_EDITOR
	public readonly static List<string> s_eventAnimMembers = new List<string> 
	{
		"m_scaleAnims",
		"m_positionAnims",
		"m_rotationAnims"
	};
	//---------------------------------------------------------------------------------------------------------
	[SerializeField] protected bool Editor_isVisible = true;

	[CustomPropertyDrawer(typeof(AnimationData))]
	public class AnimationDataDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty (position, label, property);
			position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Show Hide Button
			var visBtnRect = new Rect (position.x, position.y, 50, position.height);
			var visibility = property.FindPropertyRelative ("Editor_isVisible");
			if (GUI.Button (visBtnRect, visibility.boolValue ? "Hide" : "Show"))
				visibility.boolValue = !visibility.boolValue;

			// delete this 
			var deleteBtnRect = new Rect(position.x + position.width - 20, position.y, 20, position.height);
			if (GUI.Button (deleteBtnRect, new GUIContent ("x", "Delete"), EditorStyles.miniButton))
				goto End;


			if (!visibility.boolValue)
				goto End;

			End:
			EditorGUI.EndProperty ();
		}
	}
	#endif
}