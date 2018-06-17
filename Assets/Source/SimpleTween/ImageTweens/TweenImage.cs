using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleTween
{
	public abstract class TweenImage : Tween
	{
		protected Image m_target;
		//--------------------------------------------------------------------------------
		public override void Initialise(Component _target)
		{
			base.Initialise(_target);
			m_target = m_targetRaw as Image;
		}
	}
}
