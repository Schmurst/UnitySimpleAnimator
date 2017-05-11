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
public abstract class AnimationBase
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
		// update at 30fps when not in play mode
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

	public override AnimationType Type {get {return AnimationType.Scale;}}

	//---------------------------------------------------------------------------------------------------------
	void UpdateScale(float _pcnt)
	{
		var scale = Vector3.one;
		scale.x = xCurve.Evaluate (_pcnt);
		scale.y = yCurve.Evaluate (_pcnt);
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
	[Tooltip("The x offset which is refered to by a value of +1 in the xCurve parameter")]
	[SerializeField] protected float xOffsetMax = 0f;
	[Tooltip("The y offset which is refered to by a value of +1 in the yCurve parameter")]
	[SerializeField] protected float yOffsetMax = 0f;
	[Tooltip("The x position during the animation; 0 is the initial value, +1 is the offset seet by the xOffsetMax parameter")]
	[SerializeField] protected AnimationCurve xCurve;
	[Tooltip("The y position during the animation; 0 is the initial value, +1 is the offset seet by the yOffsetMax parameter")]
	[SerializeField] protected AnimationCurve yCurve;

	public override AnimationType Type {get {return AnimationType.Position;}}

	//---------------------------------------------------------------------------------------------------------
	public override IEnumerator Co_Animate(Transform _target, Action _onComplete)
	{
		target = _target;
		float time = 0f;
		float tPcnt = 0f;
		bool isComplete = false;

		Vector3 start = target.position;
		Vector3 end = new Vector2 (start.x + xOffsetMax, start.y + yOffsetMax);

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
	void UpdatePosition(float _pcnt, Vector2 _start, Vector2 _end)
	{
		Vector2 position = Vector2.one;
		position.x = Mathf.LerpUnclamped (_start.x, _end.x, xCurve.Evaluate(_pcnt));
		position.y = Mathf.LerpUnclamped (_start.y, _end.y, yCurve.Evaluate(_pcnt));
		target.position = position;
	}
}

//---------------------------------------------------------------------------------------------------------
[Serializable]
public class AnimationRotation : AnimationBase
{
	[Tooltip("The rotation around z during the animation where +1 is full turn clockwise")]
	[SerializeField] protected AnimationCurve zRotationCurve;

	public override AnimationType Type {get {return AnimationType.Rotation;}}

	//---------------------------------------------------------------------------------------------------------
	void UpdateRotation(float _pcnt, float _startZRot)
	{
		float z = _startZRot + zRotationCurve.Evaluate (_pcnt) * 360f;
		target.rotation = Quaternion.Euler (0f, 0f, z);
	}

	//---------------------------------------------------------------------------------------------------------
	public override IEnumerator Co_Animate(Transform _target, Action _onComplete)
	{
		target = _target;
		float time = 0f;
		float tPcnt = 0f;
		bool isComplete = false;

		float zStart = target.rotation.z;

		while(!isComplete)
		{
			UpdateAnimationTime (ref time);
			UpdateAnimationProgress (ref isComplete, out tPcnt, time);
			UpdateRotation (tPcnt, zStart);
			yield return null;
		}

		if (_onComplete != null)
			_onComplete ();
	}
}