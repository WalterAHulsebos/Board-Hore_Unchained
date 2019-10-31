using FSMHelper;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Impact : PlayerState_OnBoard
{
	private PlayerController.SetupDir _setupDir;

	private bool _canGrind = true;

	private float _impactTimer;

	private bool _impactFinished;

	private float _totalTimeInState;

	private float _maxImpactTime = 0.3f;

	private bool _correctBoard = true;

	private float _grindTimer;

	private float _maxGrindTimer = 0.3f;

	private float _rollOffWait;

	private int groundedFrameCount;

	private int groundedFrameMax = 1;

	public PlayerState_Impact()
	{
	}

	public PlayerState_Impact(bool p_canGrind)
	{
		_canGrind = p_canGrind;
	}

	public PlayerState_Impact(Vector3 p_inAirVelocity)
	{
	}

	public override bool CanGrind()
	{
		return _canGrind;
	}

	public override void Enter()
	{
		if (!PlayerController.Instance.IsRespawning)
		{
			PlayerController.Instance.AnimSetNoComply(false);
			PlayerController.Instance.boardController.ResetAll();
			PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
			PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
			PlayerController.Instance.SetTurnMultiplier(1f);
			PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
			PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
			PlayerController.Instance.AnimSetGrinding(false);
			PlayerController.Instance.ResetAnimationsAfterImpact();
			PlayerController.Instance.OnImpact();
			PlayerController.Instance.CrossFadeAnimation("Impact", 0.1f);
		}
	}

	public override void Exit()
	{
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		PlayerController.Instance.comController.UpdateCOM(1.06433f, 1);
		PlayerController.Instance.ReduceImpactBounce();
		PlayerController.Instance.boardController.ApplyOnBoardMaxRoll();
		PlayerController.Instance.SkaterRotation(true, false);
		PlayerController.Instance.boardController.DoBoardLean();
		if (PlayerController.Instance.IsGrounded())
		{
			if (!PlayerController.Instance.boardController.AllDown)
			{
				PlayerController.Instance.ApplyWeightOnBoard();
			}
			else
			{
				PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
				if (groundedFrameCount < groundedFrameMax)
				{
					groundedFrameCount++;
					return;
				}
			}
		}
	}

	public override bool IsInImpactState()
	{
		return true;
	}

	public override bool IsOnGroundState()
	{
		return true;
	}

	public override void OnAllWheelsDown()
	{
		PlayerController.Instance.AnimSetRollOff(false);
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

	public override void OnEndImpact()
	{
		DoTransition(typeof(PlayerState_Riding), null);
	}

	public override void OnFirstWheelDown()
	{
	}

	public override void OnGrindDetected()
	{
		if (_canGrind)
		{
			DoTransition(typeof(PlayerState_Grinding), null);
		}
	}

	public override void OnImpactUpdate()
	{
	}

	public override void OnPushButtonPressed(bool p_mongo)
	{
		if (!PlayerController.Instance.IsRespawning)
		{
			object[] pMongo = new object[] { p_mongo };
			DoTransition(typeof(PlayerState_Pushing), pMongo);
		}
	}

	public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
	{
		PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, true, false);
		PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, true, false);
		PlayerController.Instance.SetBoardTargetPosition(0f);
		PlayerController.Instance.SetFrontPivotRotation(0f);
		PlayerController.Instance.SetBackPivotRotation(0f);
		PlayerController.Instance.SetPivotForwardRotation(0f, 40f);
		PlayerController.Instance.SetPivotSideRotation(0f);
		if (_impactFinished)
		{
			if (p_leftStick.SetupDir > 0.8f || (new Vector2(p_leftStick.ToeAxis, p_leftStick.SetupDir)).magnitude > 0.8f && p_leftStick.SetupDir > 0.325f)
			{
				PlayerController.Instance.AnimSetSetupBlend(0f);
				PlayerController.Instance.animationController.SetValue("EndImpact", true);
				PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(false));
				PlayerController.Instance.CrossFadeAnimation("Setup", 0.2f);
				PlayerController.Instance.SetupDirection(p_leftStick, ref _setupDir);
				object[] pLeftStick = new object[] { p_leftStick, p_rightStick, p_leftStick.ForwardDir > 0.2f, _setupDir };
				DoTransition(typeof(PlayerState_Setup), pLeftStick);
				return;
			}
			if (p_rightStick.AugmentedSetupDir > 0.8f || (new Vector2(p_rightStick.ToeAxis, p_rightStick.SetupDir)).magnitude > 0.8f && p_rightStick.SetupDir > 0.325f)
			{
				PlayerController.Instance.AnimSetSetupBlend(0f);
				PlayerController.Instance.animationController.SetValue("EndImpact", true);
				PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(true));
				PlayerController.Instance.CrossFadeAnimation("Setup", 0.2f);
				PlayerController.Instance.SetupDirection(p_rightStick, ref _setupDir);
				object[] pRightStick = new object[] { p_rightStick, p_leftStick, p_rightStick.ForwardDir > 0.2f, _setupDir };
				DoTransition(typeof(PlayerState_Setup), pRightStick);
			}
		}
	}

	public override void SetupDefinition(ref FSMStateType p_stateType, ref List<Type> children)
	{
		p_stateType = FSMStateType.Type_OR;
	}

	public override void Update()
	{
		base.Update();
		_totalTimeInState += Time.deltaTime;
		if (_totalTimeInState > 5f)
		{
			PlayerController.Instance.ForceBail();
		}
		if (_grindTimer >= _maxGrindTimer)
		{
			_canGrind = false;
		}
		else
		{
			_grindTimer += Time.deltaTime;
		}
		if (!_impactFinished)
		{
			if (_impactTimer >= _maxImpactTime)
			{
				_impactFinished = true;
			}
			else
			{
				_impactTimer += Time.deltaTime;
			}
		}
		if (!PlayerController.Instance.IsGrounded() && !PlayerController.Instance.IsRespawning)
		{
			PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
			PlayerController.Instance.SetSkaterToMaster();
			PlayerController.Instance.AnimSetRollOff(true);
			PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
			Vector3 vector3 = PlayerController.Instance.skaterController.PredictLanding(PlayerController.Instance.skaterController.skaterRigidbody.velocity);
			PlayerController.Instance.skaterController.skaterRigidbody.AddForce(vector3, ForceMode.Impulse);
			object[] objArray = new object[] { false, false };
			DoTransition(typeof(PlayerState_InAir), objArray);
		}
		else if (PlayerController.Instance.IsGrounded())
		{
			PlayerController.Instance.CacheRidingTransforms();
			_rollOffWait = 0f;
		}
		if (PlayerController.Instance.IsCurrentAnimationPlaying("Riding"))
		{
			DoTransition(typeof(PlayerState_Riding), null);
		}
		if (PlayerController.Instance.AngleToBoardTargetRotation() > 90f)
		{
			PlayerController.Instance.ForceBail();
		}
	}
}