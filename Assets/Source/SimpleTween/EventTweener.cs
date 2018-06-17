using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleTween
{
	public enum EventType
	{
		none = 0,
		onEnable = 1,
		pointerUp = 2,
		pointerDown = 4,
		pointerEnter = 8,
		pointerExit = 16,
		pointerClick = 32,
	}
	
	public class EventTweener : Tweener, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
								IPointerUpHandler, IPointerClickHandler
	{
		[SerializeField] Component 		m_target;
		[SerializeField] EventType[] 	m_events;

		//--------------------------------------------------------------------------------
		public new string name {get {return "";}}
		//--------------------------------------------------------------------------------
		void OnEnable() { Execute (EventType.onEnable);}
		public void OnPointerEnter	(PointerEventData eventData){Execute (EventType.pointerEnter);}
		public void OnPointerExit 	(PointerEventData eventData){Execute (EventType.pointerExit);}
		public void OnPointerDown 	(PointerEventData eventData){Execute (EventType.pointerDown);}
		public void OnPointerUp  	(PointerEventData eventData){Execute (EventType.pointerUp);}
		public void OnPointerClick 	(PointerEventData eventData){Execute (EventType.pointerClick);}
		//--------------------------------------------------------------------------------
		void Start()
		{
			if (m_target == null)
				return;

			OnValidate();
		}

		//--------------------------------------------------------------------------------
		void OnValidate()
		{
			if (m_target == null)
				return;
			
			var type = m_target.GetType ();
			HashSet<TweenType> validTweenTypes;
			if(!TweenManager.UsableTweensByType.TryGetValue(type, out validTweenTypes))
			{
				Debug.LogFormat ("No TweenTypes for {0}", m_target.GetType());
				m_target = null;
				return;
			}

			for (int i = m_tweens.Count - 1; i >= 0; i--)
			{
				if (!validTweenTypes.Contains(m_tweens[i].Type))
					m_tweens.RemoveAt(i);
				else
					m_tweens[i].Initialise(m_target);
			}
			
			string log = string.Format ("TweenTypes for {0}", m_target.GetType ());
			foreach (var ttype in validTweenTypes)
				log += string.Format ("\n{0}", ttype);

			Debug.Log (log);
		}
		
		//--------------------------------------------------------------------------------
		bool ShouldPlayEvent(EventType _type)
		{
			if (_type == EventType.none)
				return false;

			for (int i = 0; i < m_events.Length; i++)
				if (m_events[i] == _type)
					return true;

			return false;
		}
		
		//--------------------------------------------------------------------------------
		void Execute(EventType _type)
		{
			if(ShouldPlayEvent(_type))
				Play();
		}
	}
}