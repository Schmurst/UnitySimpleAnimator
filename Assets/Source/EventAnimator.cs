using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

//---------------------------------------------------------------------------------------------------------
public enum EventAnimationType
{
	onEnable,
	pointerUp,
	pointerDown,
	pointerEnter,
	pointerExit,
	pointerClick,

	// length
	nullOrLength
}
	
//---------------------------------------------------------------------------------------------------------
[Serializable]
public class EventAnimation
{
	public EventAnimationType 	Type;
	public AnimationScale[]		ScaleData;
	public AnimationPosition[] 	PositionData;
	public AnimationRotation[] 	RotationData;
}

//---------------------------------------------------------------------------------------------------------
public class EventAnimator : 	MonoBehaviour,
								IPointerEnterHandler,
								IPointerExitHandler,
								IPointerDownHandler,
								IPointerUpHandler,
								IPointerClickHandler
{
	[SerializeField] List<EventAnimation> m_eventAnimations = new List<EventAnimation>();
	private Coroutine m_currentAnimations;

	//---------------------------------------------------------------------------------------------------------
	void OnEnable()
	{
		Execute (EventAnimationType.onEnable);
	}

	//---------------------------------------------------------------------------------------------------------
	public void OnPointerEnter(PointerEventData _baseEventData)
	{
		Execute (EventAnimationType.pointerEnter);
	}
		
	//---------------------------------------------------------------------------------------------------------
	public void OnPointerExit (PointerEventData eventData)
	{
		Execute (EventAnimationType.pointerExit);
	}

	//---------------------------------------------------------------------------------------------------------
	public void OnPointerDown (PointerEventData eventData)
	{
		Execute (EventAnimationType.pointerDown);
	}

	//---------------------------------------------------------------------------------------------------------
	public void OnPointerUp (PointerEventData eventData)
	{
		Execute (EventAnimationType.pointerUp);
	}

	//---------------------------------------------------------------------------------------------------------
	public void OnPointerClick (PointerEventData eventData)
	{
		Execute (EventAnimationType.pointerClick);
	}

	//---------------------------------------------------------------------------------------------------------
	void Execute(EventAnimationType _type)
	{
		if (m_currentAnimations != null)
		{
			StopCoroutine (m_currentAnimations);
			m_currentAnimations = null;
		}

		for (int i = 0; i < m_eventAnimations.Count; i++)
		{
			if (m_eventAnimations [i].Type == _type)
			{
				m_currentAnimations = StartCoroutine(Co_Animate (m_eventAnimations [i]));
				return;
			}
		}
	}
		
	//---------------------------------------------------------------------------------------------------------
	IEnumerator Co_Animate(EventAnimation _data)
	{
		var animations = new List<Coroutine> ();
		for (int i = 0; i < _data.ScaleData.Length; i++)
			animations.Add(StartCoroutine (_data.ScaleData[i].Co_Animate(transform, null)));

		yield return null;
	}

	//---------------------------------------------------------------------------------------------------------
	// Editor Code
	//---------------------------------------------------------------------------------------------------------
	#if UNITY_EDITOR
	List<IEnumerator> m_editorRoutines = new List<IEnumerator>();
	//---------------------------------------------------------------------------------------------------------
	void Editor_Update()
	{
		for (int i = m_editorRoutines.Count - 1; i >= 0; i--)
			if (!m_editorRoutines [i].MoveNext ())
				m_editorRoutines.RemoveAt (i);

		if (m_editorRoutines.Count == 0)
			EditorApplication.update -= Editor_Update;
	}

	//---------------------------------------------------------------------------------------------------------
	public void Editor_StartAnimation(EventAnimationType _type)
	{
		for (int i = 0; i < m_eventAnimations.Count; i++)
		{
			if (m_eventAnimations [i].Type == _type)
			{
				var animData = m_eventAnimations [i];

				foreach (var anim in animData.ScaleData)
					m_editorRoutines.Add (anim.Co_Animate(transform, null));

				EditorApplication.update += Editor_Update;
				return;
			}
		}
	}
	#endif
}