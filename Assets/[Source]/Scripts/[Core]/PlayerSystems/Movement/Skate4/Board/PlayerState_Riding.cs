using FSMHelper;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Riding : PlayerState_OnBoard
{
	private PlayerController.SetupDir _setupDir;

	private float _inAirTimer;

	public PlayerState_Riding()
	{
		PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
		PlayerController.Instance.SetTurnMultiplier(1f);
		PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
		if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Board)
		{
			PlayerController.Instance.movementMaster = PlayerController.MovementMaster.Board;
		}
	}

	public PlayerState_Riding(bool wasPushing)
	{
		PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
		PlayerController.Instance.SetTurnMultiplier(1f);
		PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
		if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Board)
		{
			PlayerController.Instance.movementMaster = PlayerController.MovementMaster.Board;
		}
	}

	public override bool CapsuleEnabled()
	{
		return true;
	}

	public override void Enter()
	{
		PlayerController.Instance.SetKneeBendWeight(0f);
		if (PlayerController.Instance.IsCurrentAnimationPlaying("Extend") && !PlayerController.Instance.IsRespawning)
		{
			PlayerController.Instance.CrossFadeAnimation("Impact", 0.1f);
		}
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		PlayerController.Instance.LimitAngularVelocity(5f);
		PlayerController.Instance.SkaterRotation(true, false);
		PlayerController.Instance.boardController.ApplyOnBoardMaxRoll();
		PlayerController.Instance.boardController.DoBoardLean();
		PlayerController.Instance.comController.UpdateCOM(1.04196f, 0);
	}

	public override bool IsOnGroundState()
	{
		return true;
	}

	public override void OnBrakeHeld()
	{
		PlayerController.Instance.AnimSetBraking(true);
		DoTransition(typeof(PlayerState_Braking), null);
	}

	public override void OnBrakePressed()
	{
		PlayerController.Instance.AnimSetBraking(true);
		DoTransition(typeof(PlayerState_Braking), null);
	}

	public override void OnGrindDetected()
	{
	}

	public override void OnManualEnter(StickInput p_popStick, StickInput p_flipStick)
	{
		PlayerController.Instance.SetManualStrength(1f);
		PlayerController.Instance.AnimSetManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
		object[] pPopStick = new object[] { p_popStick, p_flipStick, true };
		DoTransition(typeof(PlayerState_Manualling), pPopStick);
	}

	public override void OnNoseManualEnter(StickInput p_popStick, StickInput p_flipStick)
	{
		PlayerController.Instance.AnimSetNoseManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
		object[] pPopStick = new object[] { p_popStick, p_flipStick, false };
		DoTransition(typeof(PlayerState_Manualling), pPopStick);
	}

	public override void OnPushButtonHeld(bool p_mongo)
	{
		if (!PlayerController.Instance.IsRespawning)
		{
			PlayerController.Instance.CacheRidingTransforms();
			object[] pMongo = new object[] { p_mongo };
			DoTransition(typeof(PlayerState_Pushing), pMongo);
		}
	}

	public override void OnPushButtonPressed(bool p_mongo)
	{
		Debug.Log(string.Concat("OnPushButtonPressed: ", p_mongo.ToString()));
		if (!PlayerController.Instance.IsRespawning)
		{
			PlayerController.Instance.CacheRidingTransforms();
			object[] pMongo = new object[] { p_mongo };
			DoTransition(typeof(PlayerState_Pushing), pMongo);
		}
	}

	public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
	{
		SetupCheck(p_leftStick, p_rightStick);
		PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, false, false);
		PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, false, false);
		PlayerController.Instance.SetBoardTargetPosition(0f);
		PlayerController.Instance.SetFrontPivotRotation(0f);
		PlayerController.Instance.SetBackPivotRotation(0f);
		PlayerController.Instance.SetPivotForwardRotation(0f, 40f);
		PlayerController.Instance.SetPivotSideRotation(0f);
	}

	public override void OnWheelsLeftGround()
	{
	}

	private void SetupCheck(StickInput p_leftStick, StickInput p_rightStick)
	{
		if (p_leftStick.SetupDir > 0.8f || (new Vector2(p_leftStick.ToeAxis, p_leftStick.SetupDir)).magnitude > 0.8f && p_leftStick.SetupDir > 0.325f)
		{
			PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(false));
			PlayerController.Instance.SetupDirection(p_leftStick, ref _setupDir);
			object[] pLeftStick = new object[] { p_leftStick, p_rightStick, p_leftStick.ForwardDir > 0.2f, _setupDir };
			DoTransition(typeof(PlayerState_Setup), pLeftStick);
			return;
		}
		if (p_rightStick.AugmentedSetupDir <= 0.8f && ((new Vector2(p_rightStick.ToeAxis, p_rightStick.SetupDir)).magnitude <= 0.8f || p_rightStick.SetupDir <= 0.325f))
		{
			PlayerController.Instance.AnimSetupTransition(false);
			return;
		}
		PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(true));
		PlayerController.Instance.SetupDirection(p_rightStick, ref _setupDir);
		object[] pRightStick = new object[] { p_rightStick, p_leftStick, p_rightStick.ForwardDir > 0.2f, _setupDir };
		DoTransition(typeof(PlayerState_Setup), pRightStick);
	}

	public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
	{
		stateType = FSMStateType.Type_OR;
	}

	public override void Update()
	{
		base.Update();
		PlayerController.Instance.CacheRidingTransforms();
		if (PlayerController.Instance.IsGrounded() || PlayerController.Instance.IsRespawning)
		{
			if (PlayerController.Instance.IsRespawning)
			{
				PlayerController.Instance.animationController.ForceAnimation("Riding");
			}
			_inAirTimer = 0f;
			return;
		}
		_inAirTimer += Time.deltaTime;
		PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
		PlayerController.Instance.SetSkaterToMaster();
		PlayerController.Instance.AnimSetRollOff(true);
		PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
		PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
		Vector3 vector3 = PlayerController.Instance.skaterController.PredictLanding(PlayerController.Instance.skaterController.skaterRigidbody.velocity);
		PlayerController.Instance.skaterController.skaterRigidbody.AddForce(vector3, ForceMode.Impulse);
		object[] objArray = new object[] { false, false };
		DoTransition(typeof(PlayerState_InAir), objArray);
	}
}