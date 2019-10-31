using FSMHelper;
using System;
using UnityEngine;

public class ExtendStateEvents : StateMachineBehaviour
{
	private float _midTime = 0.32f;

	private float _highTime = 0.39f;

	public ExtendStateEvents()
	{
	}

	public override void OnStateEnter(Animator p_animator, AnimatorStateInfo p_stateInfo, int p_layerIndex)
	{
		if (p_animator == PlayerController.Instance.animationController.ikAnim)
		{
			PlayerController.Instance.SetLeftIKLerpTarget(0f);
			PlayerController.Instance.SetRightIKLerpTarget(0f);
			PlayerController.Instance.AnimSetGrinding(false);
			float single = Mathf.Lerp(_midTime, _highTime, PlayerController.Instance.GetPopStrength());
			PlayerController.Instance.OnInAir(single);
			PlayerController.Instance.playerSM.SendEventExtendSM(0.3f);
		}
	}

	public override void OnStateExit(Animator p_animator, AnimatorStateInfo p_stateInfo, int p_layerIndex)
	{
		if (p_animator == PlayerController.Instance.animationController.ikAnim)
		{
			PlayerController.Instance.AnimOllieTransition(false);
			PlayerController.Instance.AnimSetRollOff(false);
		}
	}
}