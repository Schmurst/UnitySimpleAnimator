using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  SimpleTween
{
	public abstract class TweenTransform : Tween
	{
		protected Transform m_target;
		//--------------------------------------------------------------------------------
		public override void Initialise(Component _target)
		{
			base.Initialise(_target);
			m_target = m_targetRaw as Transform;
		}
	}
}
