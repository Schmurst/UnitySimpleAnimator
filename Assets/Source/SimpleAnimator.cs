using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------------------------------------------------------------
[Serializable]
public class SimpleAnimationData
{
	[SerializeField] AnimationScale[]		m_scaleAnims = null;
	[SerializeField] AnimationPosition[]	m_positionAnims = null;
	[SerializeField] AnimationRotation[]	m_rotationAnims = null;

	//---------------------------------------------------------------------------------------------------------
	public virtual IAnimation[] GetAnimations()
	{
		int idx = 0, count = 0;
		var anims = new IAnimation[m_scaleAnims.Length + m_positionAnims.Length + m_rotationAnims.Length];

		count = m_scaleAnims.Length;
		for (; m_scaleAnims != null && idx < m_scaleAnims.Length; idx++)
			anims [idx] = m_scaleAnims [idx] as IAnimation;
		
		//for (int i = 0; m_positionAnims != null && i < m_positionAnims.Length; i++, idx++)

		return null;
	}
}

//---------------------------------------------------------------------------------------------------------
public class SimpleAnimator : MonoBehaviour
{
	//TODO: combine this and eventanimdata into one data strcuture
	Coroutine m_currentAnimation = null;

	//---------------------------------------------------------------------------------------------------------
	public bool IsAnimating {get{return m_currentAnimation != null;}}
	//---------------------------------------------------------------------------------------------------------
	public void Execute()
	{
		if (m_currentAnimation != null)
			return;
	}
}