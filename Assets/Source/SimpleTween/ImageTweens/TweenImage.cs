using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleTween
{
	public abstract class TweenImage : Tween
	{
		protected Image m_target;

		Color m_initialColor;
		
		//--------------------------------------------------------------------------------
		public override void Initialise(Component _target)
		{
			base.Initialise(_target);
			m_target = m_targetRaw as Image;
		}
		
		//--------------------------------------------------------------------------------
		protected override void OnAnimationInitialisation()
		{
			m_initialColor = m_target.color;
		}
		
		//--------------------------------------------------------------------------------
		protected override void ResetTarget()
		{
			m_target.color = m_initialColor;
		}
	}
}
