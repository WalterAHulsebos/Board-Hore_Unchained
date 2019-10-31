using System;
using UnityEngine;

public class PopStateEvents : StateMachineBehaviour
{
	public PopStateEvents()
	{
	}

	public override void OnStateEnter(Animator p_animator, AnimatorStateInfo p_stateInfo, int p_layerIndex)
	{
		bool pAnimator = p_animator == PlayerController.Instance.animationController.ikAnim;
	}

	public override void OnStateExit(Animator p_animator, AnimatorStateInfo p_stateInfo, int p_layerIndex)
	{
		bool pAnimator = p_animator == PlayerController.Instance.animationController.ikAnim;
	}
}