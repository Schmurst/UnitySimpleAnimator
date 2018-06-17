using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTween
{
	public class TweenRotationEuler : TweenTransform
	{
		[SerializeField] Vector3 m_euler;
		[SerializeField] AnimationCurve m_easeCurve;

		Quaternion m_endRot;
		
		//--------------------------------------------------------------------------------
		public new string name {get {return "TweenRotation";}}
		public override TweenType Type { get { return TweenType.Rotation; }}
		//--------------------------------------------------------------------------------
		protected override void OnAnimationInitialisation()
		{
			base.OnAnimationInitialisation();
			m_endRot = Quaternion.Euler(m_euler);
		}

		//--------------------------------------------------------------------------------
		protected override void UpdateTarget(float _pcnt)
		{
			m_target.rotation = Quaternion.SlerpUnclamped(m_initialRotation, m_endRot, m_easeCurve.Evaluate(_pcnt));
		}
	}
}
