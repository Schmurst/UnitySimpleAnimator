using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTween
{
	public class TweenPositionToDestination : TweenTransform
	{
		[SerializeField] protected Transform m_destination;
		
		[SerializeField] protected AnimationCurve m_xCurve;
		[SerializeField] protected AnimationCurve m_yCurve;
		[SerializeField] protected AnimationCurve m_zCurve;
		
		//--------------------------------------------------------------------------------
		public new string name {get {return "TweenPosition";}}
		public override TweenType Type { get { return TweenType.Position; }}
		//--------------------------------------------------------------------------------
		protected override void UpdateTarget(float _pcnt)
		{
			var pos = m_target.position;
			pos.x = Mathf.LerpUnclamped(m_initialPosition.x, m_destination.position.x, m_xCurve.Evaluate(_pcnt));
			pos.y = Mathf.LerpUnclamped(m_initialPosition.y, m_destination.position.y, m_yCurve.Evaluate(_pcnt));
			pos.z = Mathf.LerpUnclamped(m_initialPosition.z, m_destination.position.z, m_zCurve.Evaluate(_pcnt));
			m_target.position = pos;
		}
	}
}
