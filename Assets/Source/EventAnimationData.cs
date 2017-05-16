using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------------------------------------------------------------
public interface IAnimation
{
	AnimationType Type { get;}
	bool IsValid ();
	IEnumerator Co_Animate (Transform _target, System.Action _onComplete);
}

// If adding new types remember to update both event animation and event animator editor
public enum AnimationType
{
	Scale,
	Position,
	Rotation,

	// length
	NullOrLength,
}

//---------------------------------------------------------------------------------------------------------
[Serializable]
public abstract class AnimationBase : IAnimation
{
	[Tooltip("Duration of the Animation in seconds")]
	[SerializeField] protected float duration;
	[Tooltip("Delay the start of this animation in seconds")]
	[SerializeField] protected float delay = 0f;

	protected Transform target;
	public virtual AnimationType Type { get { return AnimationType.NullOrLength; } }

	#if UNITY_EDITOR
	const float s_editorDeltaTime = 1f / 60f;
	#endif

	//---------------------------------------------------------------------------------------------------------
	public abstract IEnumerator Co_Animate (Transform _target, System.Action _onComplete);
	//---------------------------------------------------------------------------------------------------------
	public virtual bool IsValid()
	{
		return duration > 0f && delay > 0f;
	}

	//---------------------------------------------------------------------------------------------------------
	protected virtual void UpdateAnimationTime(ref float time)
	{
		#if UNITY_EDITOR
		// update at 60fps when not in play mode
		if(!Application.isPlaying)
			time += s_editorDeltaTime;
		else
			time += Time.deltaTime;
		#else
		time += Time.deltaTime;
		#endif
	}

	//---------------------------------------------------------------------------------------------------------
	protected virtual void UpdateAnimationProgress(ref bool _isComplete, out float _pcnt, float _currentTime)
	{
		if (_currentTime > duration + delay)
			_isComplete = true;

		if (delay > 0f && _currentTime < delay)
		{
			_pcnt = 0f;
			return;
		}

		_pcnt = Mathf.Min((_currentTime - delay) / duration, 1f);
	}
}

//---------------------------------------------------------------------------------------------------------
[Serializable]
public class AnimationScale : AnimationBase
{
	[Tooltip("The x value of scale during the animation")]
	[SerializeField] protected AnimationCurve xCurve;
	[Tooltip("The y value of scale during the animation")]
	[SerializeField] protected AnimationCurve yCurve;
	[Tooltip("The z value of scale during the animation")]
	[SerializeField] protected AnimationCurve zCurve;

	public override AnimationType Type {get {return AnimationType.Scale;}}

	//---------------------------------------------------------------------------------------------------------
	void UpdateScale(float _pcnt)
	{
		var scale = Vector3.one;
		scale.x = xCurve.Evaluate (_pcnt);
		scale.y = yCurve.Evaluate (_pcnt);
		scale.z = zCurve.Evaluate (_pcnt);
		target.localScale = scale;
	}	

	//---------------------------------------------------------------------------------------------------------
	public override IEnumerator Co_Animate(Transform _target, Action _onComplete)
	{
		target = _target;
		float time = 0f;
		float tPcnt = 0f;
		bool isComplete = false;

		while(!isComplete)
		{
			UpdateAnimationTime (ref time);
			UpdateAnimationProgress (ref isComplete, out tPcnt, time);
			UpdateScale (tPcnt);
			yield return null;
		}

		if (_onComplete != null)
			_onComplete ();
	}
}

//---------------------------------------------------------------------------------------------------------
[Serializable]
public class AnimationPosition : AnimationBase
{
	[Tooltip("The offset in each dimention which is refered to by a value of +1 in the curve parameter")]
	[SerializeField] protected Vector3 OffsetMax;
	[Tooltip("The x position during the animation; 0 is the initial value, +1 is the offset seet by the xOffsetMax parameter")]
	[SerializeField] protected AnimationCurve xCurve;
	[Tooltip("The y position during the animation; 0 is the initial value, +1 is the offset seet by the yOffsetMax parameter")]
	[SerializeField] protected AnimationCurve yCurve;
	[Tooltip("The z position during the animation; 0 is the initial value, +1 is the offset seet by the zOffsetMax parameter")]
	[SerializeField] protected AnimationCurve zCurve;

	public override AnimationType Type {get {return AnimationType.Position;}}

	//---------------------------------------------------------------------------------------------------------
	public override IEnumerator Co_Animate(Transform _target, Action _onComplete)
	{
		target = _target;
		float time = 0f;
		float tPcnt = 0f;
		bool isComplete = false;

		Vector3 start = target.position;
		Vector3 end = start + OffsetMax;

		while(!isComplete)
		{
			UpdateAnimationTime (ref time);
			UpdateAnimationProgress (ref isComplete, out tPcnt, time);
			UpdatePosition (tPcnt, start, end);
			yield return null;
		}

		if (_onComplete != null)
			_onComplete ();
	}

	//---------------------------------------------------------------------------------------------------------
	void UpdatePosition(float _pcnt, Vector3 _start, Vector3 _end)
	{
		Vector3 position = Vector3.one;
		position.x = Mathf.LerpUnclamped (_start.x, _end.x, xCurve.Evaluate(_pcnt));
		position.y = Mathf.LerpUnclamped (_start.y, _end.y, yCurve.Evaluate(_pcnt));
		position.z = Mathf.LerpUnclamped (_start.z, _end.z, zCurve.Evaluate(_pcnt));
		target.position = position;
	}
}

//---------------------------------------------------------------------------------------------------------
[Serializable]
public class AnimationRotation : AnimationBase
{
	[Tooltip("The rotation around x during the animation where +1 is full turn anticlockwise")]
	[SerializeField] protected AnimationCurve xCurve;
	[Tooltip("The rotation around y during the animation where +1 is full turn anticlockwise")]
	[SerializeField] protected AnimationCurve yCurve;
	[Tooltip("The rotation around z during the animation where +1 is full turn anticlockwise")]
	[SerializeField] protected AnimationCurve zCurve;

	public override AnimationType Type {get {return AnimationType.Rotation;}}

	//---------------------------------------------------------------------------------------------------------
	void UpdateRotation(float _pcnt, Vector3 _startRot)
	{
		var rot = Vector3.zero;
		rot.x = _startRot.x + xCurve.Evaluate (_pcnt) * 360f;
		rot.y = _startRot.y + yCurve.Evaluate (_pcnt) * 360f;
		rot.z = _startRot.z + zCurve.Evaluate (_pcnt) * 360f;
		target.rotation = Quaternion.Euler (rot);
	}

	//---------------------------------------------------------------------------------------------------------
	public override IEnumerator Co_Animate(Transform _target, Action _onComplete)
	{
		target = _target;
		float time = 0f;
		float tPcnt = 0f;
		bool isComplete = false;

		Vector3 start = target.rotation.eulerAngles;

		while(!isComplete)
		{
			UpdateAnimationTime (ref time);
			UpdateAnimationProgress (ref isComplete, out tPcnt, time);
			UpdateRotation (tPcnt, start);
			yield return null;
		}

		if (_onComplete != null)
			_onComplete ();
	}
}