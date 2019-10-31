using Dreamteck.Splines;
using FSMHelper;
using System;
using System.Collections.Generic;

public class PlayerState_OnBoard : BaseFSMState
{
	private float _augmentedLeftAngle;

	private float _augmentedRightAngle;

	private float turnSpeed;

	private InputController.TurningMode turningMode;

	public PlayerState_OnBoard()
	{
	}

	public override void BothTriggersReleased(InputController.TurningMode turningMode)
	{
		PlayerController.Instance.RemoveTurnTorque(PlayerController.Instance.spinDeccelerate, turningMode);
	}

	public override void Enter()
	{
	}

	public override void Exit()
	{
	}

	public override void FixedUpdate()
	{
		PlayerController.Instance.UpdateBoardPosition();
		PlayerController.Instance.UpdateSkaterPosition();
		PlayerController.Instance.MoveCameraToPlayer();
	}

	public override float GetAugmentedAngle(StickInput p_stick)
	{
		if (p_stick.IsRightStick)
		{
			return _augmentedRightAngle;
		}
		return _augmentedLeftAngle;
	}

	public override bool IsCurrentSpline(SplineComputer p_spline)
	{
		return false;
	}

	public override bool IsGrinding()
	{
		return false;
	}

	public override void LeftTriggerHeld(float value, InputController.TurningMode turningMode)
	{
		PlayerController.Instance.TurnLeft(value, turningMode);
	}

	public override void OnBailed()
	{
		PlayerController.Instance.ResetAllAnimations();
		PlayerController.Instance.AnimGrindTransition(false);
		PlayerController.Instance.AnimOllieTransition(false);
		PlayerController.Instance.AnimSetupTransition(false);
		DoTransition(typeof(PlayerState_Bailed), null);
	}

	public override void OnRespawn()
	{
		PlayerController.Instance.ResetAllAnimations();
		PlayerController.Instance.AnimGrindTransition(false);
		PlayerController.Instance.AnimOllieTransition(false);
		PlayerController.Instance.AnimSetupTransition(false);
		DoTransition(typeof(PlayerState_Riding), null);
	}

	public override void RightTriggerHeld(float value, InputController.TurningMode turningMode)
	{
		PlayerController.Instance.TurnRight(value, turningMode);
	}

	public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
	{
		stateType = FSMStateType.Type_OR;
	}

	public override void Update()
	{
	}
}