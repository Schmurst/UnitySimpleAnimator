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
		[SerializeField] protected List<Tween> m_tweens = new List<Tween>();

		protected bool m_isTweenInProgress = false;

		//--------------------------------------------------------------------------------
		public virtual void Play()
		{
			if (m_isTweenInProgress)
				return;
			StartCoroutine (Co_PlayTweens ());
		}

		//--------------------------------------------------------------------------------
		protected IEnumerator Co_PlayTweens()
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
			
			m_isTweenInProgress = false;
		}

		//--------------------------------------------------------------------------------
		#if UNITY_EDITOR
		[CustomEditor(typeof(Tweener))]
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