using FSMHelper;
using System;
using System.Collections.Generic;

public class PlayerState_Braking : PlayerState_OnBoard
{
	public PlayerState_Braking()
	{
	}

	public override void Enter()
	{
		PlayerController.Instance.CrossFadeAnimation("Braking", 0.2f);
	}

	public override void Exit()
	{
		PlayerController.Instance.AnimSetBraking(false);
	}

	public override void FixedUpdate()
	{
		PlayerController.Instance.comController.UpdateCOM(1f, 0);
		base.FixedUpdate();
		PlayerController.Instance.SkaterRotation(true, false);
		PlayerController.Instance.boardController.ApplyOnBoardMaxRoll();
		PlayerController.Instance.boardController.DoBoardLean();
	}

	public override bool IsOnGroundState()
	{
		return true;
	}

	public override void OnBrakeHeld()
	{
		PlayerController.Instance.Brake(PlayerController.Instance.GetBrakeForce());
	}

	public override void OnBrakeReleased()
	{
		DoTransition(typeof(PlayerState_Riding), null);
	}

	public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
	{
		stateType = FSMStateType.Type_OR;
	}

	public override void Update()
	{
		base.Update();
	}
}