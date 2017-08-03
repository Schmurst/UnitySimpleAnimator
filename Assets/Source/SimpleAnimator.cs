using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------------------------------------------------------------
public class SimpleAnimator : MonoBehaviour
{
	//TODO: combine this and eventanimdata into one data strcuture
	Coroutine m_currentAnimation = null;
	[SerializeField] AnimationData[] m_animations;

	//---------------------------------------------------------------------------------------------------------
	public bool IsAnimating {get{return m_currentAnimation != null;}}
	//---------------------------------------------------------------------------------------------------------
	public void Execute()
	{
		if (m_currentAnimation != null)
			return;
	}
}