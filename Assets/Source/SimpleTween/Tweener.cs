using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleTween
{
	public class Tweener : MonoBehaviour
	{
		[SerializeField] Component m_target;
		[SerializeField] protected List<Tween> m_tweens = new List<Tween>();

		protected bool m_isTweenInProgress = false;

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
		public virtual void Play(Action _onComplete = null)
		{
			if (m_isTweenInProgress)
				return;
			StartCoroutine (Co_PlayTweens ());
		}

		//--------------------------------------------------------------------------------
		protected IEnumerator Co_PlayTweens(Action _onComplete = null)
		{
			Debug.LogFormat("Started Tween on {0}", name);
			m_isTweenInProgress = true;

			var animations = new List<Coroutine> ();
			int started = 0, completed = 0;
			Action onCompleted = ()=>{++completed;};
			var waitForAnimsToFinish = new WaitUntil(()=>{return started == completed;});
			
			for (int i = 0; i < m_tweens.Count; i++)
			{
				var routine = StartCoroutine (m_tweens [i].Co_PlayTween (onCompleted));
				animations.Add (routine);
				started++;
			}

			yield return waitForAnimsToFinish;

			if (_onComplete != null)
				_onComplete();
			
			m_isTweenInProgress = false;
		}

		//--------------------------------------------------------------------------------
		#if UNITY_EDITOR
		[CustomEditor(typeof(Tweener), true)]
		class TweenerEditor : Editor
		{
			public override void OnInspectorGUI ()
			{ 
				var tweener = (Tweener)target;
				if (GUILayout.Button ("Play"))
					tweener.Play ();

				base.OnInspectorGUI ();
			}
		}
		#endif
	}
}