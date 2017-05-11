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
	public AnimationScale[]		ScaleAnims;
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
	private Coroutine m_currentAnimation;

	//---------------------------------------------------------------------------------------------------------
	void OnEnable() { Execute (EventAnimationType.onEnable);}
	public void OnPointerEnter	(PointerEventData eventData){Execute (EventAnimationType.pointerEnter);}
	public void OnPointerExit 	(PointerEventData eventData){Execute (EventAnimationType.pointerExit);}
	public void OnPointerDown 	(PointerEventData eventData){Execute (EventAnimationType.pointerDown);}
	public void OnPointerUp  	(PointerEventData eventData){Execute (EventAnimationType.pointerUp);}
	public void OnPointerClick 	(PointerEventData eventData){Execute (EventAnimationType.pointerClick);}

	//---------------------------------------------------------------------------------------------------------
	void Execute(EventAnimationType _type)
	{
		if (m_currentAnimation != null)
			return;

		for (int i = 0; i < m_eventAnimations.Count; i++)
		{
			if (m_eventAnimations [i].Type == _type)
			{
				m_currentAnimation = StartCoroutine(Co_Animate (m_eventAnimations [i]));
				return;
			}
		}
	}
		
	//---------------------------------------------------------------------------------------------------------
	IEnumerator Co_Animate(EventAnimation _data)
	{
		var animations = new List<Coroutine> ();
		int started = 0, completed = 0;
		Action onComplete = () => {++completed;};
		var waitForAnimsToFinish = new WaitUntil(()=>{return started == completed;});

		for (int i = 0; i < _data.ScaleAnims.Length; i++)
		{
			animations.Add (StartCoroutine (_data.ScaleAnims[i].Co_Animate (transform, onComplete)));
			++started;
		}

		yield return waitForAnimsToFinish;

		m_currentAnimation = null;
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

				foreach (var anim in animData.ScaleAnims)
					m_editorRoutines.Add (anim.Co_Animate(transform, null));

				EditorApplication.update += Editor_Update;
				return;
			}
		}
	}
	#endif
}