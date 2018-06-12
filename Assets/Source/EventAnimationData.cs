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

// If adding new types remember to update both EventAnimation and EventAnimatorEditor
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
	#if UNITY_EDITOR
	const float s_editorDeltaTime = 1f / 60f;
	#endif

	[Tooltip("Duration of the Animation in seconds")]
	[SerializeField] protected float duration;
	[Tooltip("Delay the start of this animation in seconds")]
	[SerializeField] protected float delay = 0f;

	protected Transform target;
	protected bool m_isWaitingForDelay = false;

	//---------------------------------------------------------------------------------------------------------
	public virtual AnimationType Type { get { return AnimationType.NullOrLength; } }
	protected bool ShouldUpdateTransform { get { return !m_isWaitingForDelay; } }
	//---------------------------------------------------------------------------------------------------------
	protected virtual void UpdateTargetTransform(float _pcnt){}
	protected virtual void OnAnimationInitialisation (){}
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
		#endif
		time += Time.deltaTime;
	}

	//---------------------------------------------------------------------------------------------------------
	protected virtual void UpdateAnimationProgress(ref bool _isComplete, out float _pcnt, float _currentTime)
	{
		if (_currentTime > duration + delay)
			_isComplete = true;

		if (delay > 0f && _currentTime < delay)
		{
			m_isWaitingForDelay = true;
			_pcnt = 0f;
			return;
		}
		else if (delay > 0f && _currentTime > delay && m_isWaitingForDelay)
		{
			m_isWaitingForDelay = false;
		}
			
		_pcnt = Mathf.Min((_currentTime - delay) / duration, 1f);
	}

	//---------------------------------------------------------------------------------------------------------
	public virtual IEnumerator Co_Animate (Transform _target, System.Action _onComplete)
	{
		target = _target;
		float time = 0f;
		float tPcnt = 0f;
		bool isComplete = false;

		OnAnimationInitialisation ();

		while(!isComplete)
		{
			UpdateAnimationTime (ref time);
			UpdateAnimationProgress (ref isComplete, out tPcnt, time);
			if (ShouldUpdateTransform)
				UpdateTargetTransform (tPcnt);
			
			yield return null;
		}

		if (_onComplete != null)
			_onComplete ();
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
	// Update Local Scale
	protected override void UpdateTargetTransform (float _pcnt)
	{
		var scale = Vector3.one;
		scale.x = xCurve.Evaluate (_pcnt);
		scale.y = yCurve.Evaluate (_pcnt);
		scale.z = zCurve.Evaluate (_pcnt);
		target.localScale = scale;
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

	Vector3 m_start;
	Vector3 m_end;

	//---------------------------------------------------------------------------------------------------------
	protected override void OnAnimationInitialisation ()
	{
		m_start = target.position;
		m_end = m_start + OffsetMax;
	}

	//---------------------------------------------------------------------------------------------------------
	protected override void UpdateTargetTransform (float _pcnt)
	{
		Vector3 position = Vector3.one;
		position.x = Mathf.LerpUnclamped (m_start.x, m_end.x, xCurve.Evaluate(_pcnt));
		position.y = Mathf.LerpUnclamped (m_start.y, m_end.y, yCurve.Evaluate(_pcnt));
		position.z = Mathf.LerpUnclamped (m_start.z, m_end.z, zCurve.Evaluate(_pcnt));
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
	Vector3 m_start;

	//---------------------------------------------------------------------------------------------------------
	protected override void OnAnimationInitialisation ()
	{
		m_start = target.rotation.eulerAngles;
	}

	//---------------------------------------------------------------------------------------------------------
	protected override void UpdateTargetTransform (float _pcnt)
	{
		var rot = Vector3.zero;
		rot.x = m_start.x + xCurve.Evaluate (_pcnt) * 360f;
		rot.y = m_start.y + yCurve.Evaluate (_pcnt) * 360f;
		rot.z = m_start.z + zCurve.Evaluate (_pcnt) * 360f;
		target.rotation = Quaternion.Euler (rot);
	}
}