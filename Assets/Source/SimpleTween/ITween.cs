using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleTween
{
	public interface ITween
	{
		TweenType Type { get; }
		IEnumerator Co_PlayTween(Action _onComplete);
	}
}
