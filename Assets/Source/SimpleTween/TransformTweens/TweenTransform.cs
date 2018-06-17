using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  SimpleTween
{
	public abstract class TweenTransform : Tween
	{
		protected Transform m_target;

		protected Vector3 m_initialPosition;
		protected Vector3 m_initialScale;
		protected Quaternion m_initialRotation;
		
		//--------------------------------------------------------------------------------
		public override void Initialise(Component _target)
		{
			base.Initialise(_target);
			m_target = m_targetRaw as Transform;
		}
		
		//--------------------------------------------------------------------------------
		protected override void OnAnimationInitialisation()
		{
			m_initialPosition = m_target.position;
			m_initialScale = m_target.localScale;
			m_initialRotation = m_target.rotation;
		}
		
		//--------------------------------------------------------------------------------
		protected override void ResetTarget()
		{
			m_target.position = m_initialPosition;
			m_target.localScale = m_initialScale;
			m_target.rotation = m_initialRotation;
		}
	}
}
