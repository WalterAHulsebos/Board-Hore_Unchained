using FSMHelper;
using System;
using UnityEngine;

public class HeadIK : MonoBehaviour
{
	public Transform head;

	public Transform currentRot;

	public Transform targetRot;

	public Transform goofyTargetRot;

	public float speed = 15f;

	public HeadIK()
	{
	}

	private bool IsAllowedAnimation()
	{
		if (PlayerController.Instance.IsCurrentAnimationPlaying("Riding") || PlayerController.Instance.IsCurrentAnimationPlaying("RidingToPush") || PlayerController.Instance.IsCurrentAnimationPlaying("PushToRiding") || PlayerController.Instance.IsCurrentAnimationPlaying("Braking"))
		{
			return false;
		}
		return !PlayerController.Instance.IsCurrentAnimationPlaying("Push Button");
	}

	private void LateUpdate()
	{
		if (!IsAllowedAnimation() || !PlayerController.Instance.IsGrounded() || !PlayerController.Instance.IsAnimSwitch || PlayerController.Instance.playerSM.IsPushingSM())
		{
			currentRot.rotation = Quaternion.Slerp(currentRot.rotation, head.rotation, Time.deltaTime * speed);
		}
		else
		{
			currentRot.rotation = Quaternion.Slerp(currentRot.rotation, (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? targetRot.rotation : goofyTargetRot.rotation), Time.deltaTime * speed);
		}
		head.rotation = currentRot.rotation;
	}
}