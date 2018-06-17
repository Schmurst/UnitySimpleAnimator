using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTween
{
	public class TweenHue : TweenImage
	{
		[SerializeField] protected Color m_targetColour;
		[SerializeField] protected AnimationCurve m_curve; 
			
		private Color m_startColour;
		
		//--------------------------------------------------------------------------------
		public new string name {get {return "TweenHue";}}
		public override TweenType Type { get { return TweenType.Hue; }}
		//--------------------------------------------------------------------------------
		protected override void OnAnimationInitialisation()
		{
			m_startColour = m_target.color;
		}

		//--------------------------------------------------------------------------------
		protected override void UpdateTarget (float _pcnt)
		{
			m_target.color = Color.Lerp(m_startColour, m_targetColour, m_curve.Evaluate(_pcnt));
		}
	}
}