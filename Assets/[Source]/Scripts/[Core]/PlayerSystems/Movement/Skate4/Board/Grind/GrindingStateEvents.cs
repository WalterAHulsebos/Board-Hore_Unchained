using System;
using UnityEngine;

public class GrindingStateEvents : StateMachineBehaviour
{
	public GrindingStateEvents()
	{
	}

	public override void OnStateEnter(Animator p_animator, AnimatorStateInfo p_stateInfo, int p_layerIndex)
	{
		bool pAnimator = p_animator == PlayerController.Instance.animationController.ikAnim;
	}

	public override void OnStateExit(Animator p_animator, AnimatorStateInfo p_stateInfo, int p_layerIndex)
	{
		if (p_animator == PlayerController.Instance.animationController.ikAnim && !PlayerController.Instance.animationController.skaterAnim.GetBool("Ollie"))
		{
			PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
		}
	}
}