using FSMHelper;
using System;
using System.Collections.Generic;

public class PlayerState_OffBoard : BaseFSMState
{
	public PlayerState_OffBoard()
	{
	}

	public override void Enter()
	{
	}

	public override void Exit()
	{
	}

	public override void FixedUpdate()
	{
		PlayerController.Instance.CameraLookAtPlayer();
	}

	public override void OnRespawn()
	{
		PlayerController.Instance.ResetAllAnimations();
		PlayerController.Instance.AnimGrindTransition(false);
		PlayerController.Instance.AnimOllieTransition(false);
		PlayerController.Instance.AnimSetupTransition(false);
		DoTransition(typeof(PlayerState_Riding), null);
	}

	public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
	{
		stateType = FSMStateType.Type_OR;
	}

	public override void Update()
	{
	}
}