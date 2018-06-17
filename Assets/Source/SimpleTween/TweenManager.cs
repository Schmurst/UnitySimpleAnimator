using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace SimpleTween
{
	public enum TweenType
	{
		Scale, 
		Position,
		Rotation,		
		Hue,

		NullOrLength
	}
	
	public static class TweenManager
	{
		public static readonly Dictionary<Type, HashSet<TweenType>> UsableTweensByType = new Dictionary<Type, HashSet<TweenType>>
		{
			{typeof(Transform), new HashSet<TweenType> {TweenType.Position, TweenType.Scale, TweenType.Rotation}},
			{typeof(Image), 	new HashSet<TweenType> {TweenType.Hue}},
		}; 	
	}
}
