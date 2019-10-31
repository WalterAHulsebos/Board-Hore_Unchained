using FSMHelper;
using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
	public Animator skaterAnim;

	[SerializeField]
	public Animator ikAnim;

	[SerializeField]
	private Animator _steezeAnim;

	[SerializeField]
	private string _activeAnimation;

	public AnimationController()
	{
	}

	public void CrossFadeAnimation(string p_value, float p_transitionDuration)
	{
		skaterAnim.CrossFadeInFixedTime(p_value, p_transitionDuration);
		ikAnim.CrossFadeInFixedTime(p_value, p_transitionDuration);
	}

	public void ForceAnimation(string p_anim)
	{
		skaterAnim.Play(p_anim, 0, 0f);
		ikAnim.Play(p_anim, 0, 0f);
	}

	public void ForceBeginPop()
	{
		PlayerController.Instance.playerSM.SendEventBeginPopSM();
	}

	public float GetAnimatorSpeed()
	{
		return skaterAnim.speed;
	}

	public void GetCurrentAnim()
	{
		float single = 0f;
		string str = "";
		AnimatorClipInfo[] currentAnimatorClipInfo = skaterAnim.GetCurrentAnimatorClipInfo(0);
		for (int i = 0; i < (int)currentAnimatorClipInfo.Length; i++)
		{
			AnimatorClipInfo animatorClipInfo = currentAnimatorClipInfo[i];
			if (animatorClipInfo.weight > single)
			{
				str = animatorClipInfo.clip.name;
				single = animatorClipInfo.weight;
			}
		}
		_activeAnimation = str;
	}

	public void ScaleAnimSpeed(float p_speed)
	{
		skaterAnim.speed = p_speed;
		ikAnim.speed = p_speed;
	}

	public void SendEventBeginPop(string p_animName)
	{
		GetCurrentAnim();
		if (p_animName == _activeAnimation)
		{
			PlayerController.Instance.playerSM.SendEventBeginPopSM();
		}
	}

	public void SendEventEndFlipPeriod(string p_animName)
	{
		PlayerController.Instance.playerSM.SendEventEndFlipPeriodSM();
	}

	public void SendEventExtend(AnimationEvent p_animationEvent)
	{
	}

	public void SendEventImpactEnd(string p_animName)
	{
	}

	public void SendEventLastPushCheck(string p_animName)
	{
		if (p_animName == _activeAnimation)
		{
			PlayerController.Instance.playerSM.OnPushLastCheckSM();
		}
	}

	public void SendEventPop(AnimationEvent p_animationEvent)
	{
		float pAnimationEvent = p_animationEvent.floatParameter;
		string str = p_animationEvent.stringParameter;
		GetCurrentAnim();
	}

	public void SendEventPush(string p_animName)
	{
		GetCurrentAnim();
		PlayerController.Instance.playerSM.OnPushSM();
	}

	public void SendEventPushEnd(string p_animName)
	{
		if (p_animName == _activeAnimation)
		{
			PlayerController.Instance.playerSM.OnPushEndSM();
		}
	}

	public void SendEventReleased(string p_animName)
	{
	}

	public void SetGrindTweakValue(float p_tweak)
	{
		SetValue("GrindTweak", p_tweak);
	}

	public void SetNollieSteezeIK(float p_value)
	{
		_steezeAnim.SetFloat("Nollie", p_value);
	}

	public void SetSteezeValue(string p_animName, float p_value)
	{
		_steezeAnim.SetFloat(p_animName, p_value);
	}

	public void SetTweakMagnitude(float p_frontMagnitude, float p_backMagnitude)
	{
		SetValue("TweakMagnitude", p_frontMagnitude - p_backMagnitude);
	}

	public void SetTweakValues(float p_forwardAxis, float p_toeAxis)
	{
		SetValue("ForwardTweak", p_forwardAxis);
		SetValue("ToeSideTweak", p_toeAxis);
	}

	public void SetValue(string p_animName, bool p_value)
	{
		skaterAnim.SetBool(p_animName, p_value);
		ikAnim.SetBool(p_animName, p_value);
	}

	public void SetValue(string p_animName, float p_value)
	{
		skaterAnim.SetFloat(p_animName, p_value);
		ikAnim.SetFloat(p_animName, p_value);
	}

	public void SetValue(string p_animName, int p_value)
	{
		skaterAnim.SetInteger(p_animName, p_value);
		ikAnim.SetInteger(p_animName, p_value);
	}

	public void SetValue(string p_animName)
	{
		skaterAnim.SetTrigger(p_animName);
		ikAnim.SetTrigger(p_animName);
	}
}