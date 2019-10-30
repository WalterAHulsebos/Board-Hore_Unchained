using System;
using UnityEngine;

public class SetupStateEvents : StateMachineBehaviour
{
	public SetupStateEvents()
	{
	}

	public override void OnStateEnter(Animator p_animator, AnimatorStateInfo p_stateInfo, int p_layerIndex)
	{
		if (p_animator == PlayerController.Instance.animationController.ikAnim)
		{
			PlayerController.Instance.OnEnterSetupState();
			PlayerController.Instance.animationController.SetValue("EndImpact", false);
		}
	}

	public override void OnStateExit(Animator p_animator, AnimatorStateInfo p_stateInfo, int p_layerIndex)
	{
		if (p_animator == PlayerController.Instance.animationController.ikAnim)
		{
			PlayerController.Instance.OnExitSetupState();
			PlayerController.Instance.AnimOllieTransition(false);
		}
	}
}