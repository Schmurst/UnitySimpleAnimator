using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTween
{
	public class TweenScale : TweenTransform
	{
		[SerializeField] protected AnimationCurve m_xCurve;
		[SerializeField] protected AnimationCurve m_yCurve;
		[SerializeField] protected AnimationCurve m_zCurve;

		//--------------------------------------------------------------------------------
		public new string name {get {return "TweenScale";}}
		public override TweenType Type { get { return TweenType.Scale; }}
		//--------------------------------------------------------------------------------
		protected override void UpdateTarget (float _pcnt)
		{
			var scale = Vector3.one;
			scale.x = m_xCurve.Evaluate (_pcnt);
			scale.y = m_yCurve.Evaluate (_pcnt);
			scale.z = m_zCurve.Evaluate (_pcnt);
			m_target.localScale = scale;
		}
	}
}