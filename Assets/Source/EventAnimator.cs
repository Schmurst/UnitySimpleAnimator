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
[System.Flags]
public enum EventAnimationType
{
	onEnable = 1,
	pointerUp = 2,
	pointerDown = 4,
	pointerEnter = 8,
	pointerExit = 16,
	pointerClick = 32,
}
	
//---------------------------------------------------------------------------------------------------------
[Serializable]
public class EventAnimation : AnimationData
{
	// when changing these must update paramter names in EventAnimatorEditor
	[SerializeField] EventAnimationType m_type = 0;
	public EventAnimationType Type { get { return m_type; } }
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
			if ((m_eventAnimations [i].Type & _type) != 0)
				m_currentAnimation = StartCoroutine(Co_Animate (m_eventAnimations [i]));
	}
		
	//---------------------------------------------------------------------------------------------------------
	IEnumerator Co_Animate(EventAnimation _data)
	{
		var animations = new List<Coroutine> ();
		int started = 0, completed = 0;
		Action onComplete = () => {++completed;};
		var waitForAnimsToFinish = new WaitUntil(()=>{return started == completed;});
		var allAnims = _data.GetAnimations();

		for (int i = 0; i < allAnims.Length; i++)
		{
			animations.Add (StartCoroutine (allAnims[i].Co_Animate (transform, onComplete)));
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
	Vector3 m_initialPosition;
	Quaternion m_initialRotation;
	Vector3 m_initialScale;
	//---------------------------------------------------------------------------------------------------------
	void Editor_Update()
	{
		for (int i = m_editorRoutines.Count - 1; i >= 0; i--)
			if (!m_editorRoutines [i].MoveNext ())
				m_editorRoutines.RemoveAt (i);

		if (m_editorRoutines.Count == 0 || Application.isPlaying)
		{
			EditorApplication.update -= Editor_Update;
			//SH: 2017-jun-24: restore trasnform
			this.transform.position = m_initialPosition;
			this.transform.rotation = m_initialRotation;
			this.transform.localScale = m_initialScale;
		}
	}

	//---------------------------------------------------------------------------------------------------------
	public void Editor_StartAnimation(EventAnimationType _type)
	{
		m_initialPosition = transform.position;
		m_initialRotation = transform.rotation;
		m_initialScale = transform.localScale;

		for (int i = 0; i < m_eventAnimations.Count; i++)
		{
			if (m_eventAnimations [i].Type == _type)
			{
				var animData = m_eventAnimations [i].GetAnimations();
				for (int j = 0; j < animData.Length; j++) 
					m_editorRoutines.Add (animData[j].Co_Animate(transform, null));

				EditorApplication.update += Editor_Update;
				return;
			}
		}
	}
	#endif
}