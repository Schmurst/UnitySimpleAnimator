using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SimpleTween
{
	public static class TweenManager
	{
		public static readonly Dictionary<Type, HashSet<TweenType>> UsableTweensByType = new Dictionary<Type, HashSet<TweenType>>
		{
			{typeof(Transform), new HashSet<TweenType> {TweenType.Position, TweenType.Scale, TweenType.Rotation}},
		}; 	
	}
}
