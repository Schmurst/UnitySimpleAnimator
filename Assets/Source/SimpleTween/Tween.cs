using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTween
{
	[System.Serializable]
	public abstract class Tween : MonoBehaviour
	{
		#if UNITY_EDITOR
		const float EDITOR_DELTA_TIME = 1f / 60f;
		#endif

		[SerializeField] protected float m_duration = 1f;
		[SerializeField] protected float m_delay = 0f;
		[SerializeField] protected bool m_resetOnComplete;

		protected Component m_targetRaw;
		
		protected bool m_isWaitingForDelay = false;

		//--------------------------------------------------------------------------------
		public abstract TweenType Type{get;}
		//--------------------------------------------------------------------------------
		protected virtual void UpdateTarget(float _pcnt){}
		protected virtual void OnAnimationInitialisation (){}
		protected virtual void OnTweenComplete (){}
		protected abstract void ResetTarget();
		//--------------------------------------------------------------------------------
		public virtual void Initialise(Component _target)
		{
			m_targetRaw = _target;
		}
		
		//--------------------------------------------------------------------------------
		protected virtual void UpdateAnimationTime(ref float _time)
		{
			#if UNITY_EDITOR
			// update at 60fps when not in play mode
			if(!Application.isPlaying)
				_time += EDITOR_DELTA_TIME;
			else
			#endif
				_time += Time.deltaTime;
		}

		//---------------------------------------------------------------------------------------------------------
		protected virtual void UpdateAnimationProgress(ref bool _isComplete, out float _pcnt, float _currentTime)
		{
			if (_currentTime > m_duration + m_delay)
				_isComplete = true;

			if (m_delay > 0f && _currentTime < m_delay)
			{
				m_isWaitingForDelay = true;
				_pcnt = 0f;
				return;
			}

			if (m_delay > 0f && _currentTime > m_delay && m_isWaitingForDelay)
				m_isWaitingForDelay = false;

			_pcnt = Mathf.Min((_currentTime - m_delay) / m_duration, 1f);
		}

		//---------------------------------------------------------------------------------------------------------
		public virtual IEnumerator Co_PlayTween (Action _onComplete)
		{
			float time = 0f;
			float tPcnt = 0f;
			bool isComplete = false;

			OnAnimationInitialisation ();

			while(!isComplete)
			{
				UpdateAnimationTime (ref time);
				UpdateAnimationProgress (ref isComplete, out tPcnt, time);
				if (!m_isWaitingForDelay)
					UpdateTarget (tPcnt);

				yield return null;
			}

			if(m_resetOnComplete)
				ResetTarget();
			
			OnTweenComplete();
			
			if (_onComplete != null)
				_onComplete ();
		}
	}
}