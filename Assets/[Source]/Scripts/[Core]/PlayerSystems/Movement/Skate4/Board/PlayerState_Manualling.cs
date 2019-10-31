using FSMHelper;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Manualling : PlayerState_OnBoard
{
	private StickInput _popStick;

	private StickInput _flipStick;

	private bool _manual;

	private bool _delayExit;

	private float _delayTimer;

	private bool _flipDetected;

	private bool _potentialFlip;

	private Vector2 _initialFlipDir = Vector2.zero;

	private int _flipFrameCount;

	private int _flipFrameMax = 20;

	private float _toeAxis;

	private float _flipVel;

	private float _popVel;

	private float _popDir;

	private float _popWait;

	private bool _canPop;

	private float _manualStrength;

	private float _flip;

	private float _popForce = 2.5f;

	private float _invertVel;

	private bool _forwardLoad;

	private float _augmentedLeftAngle;

	private float _augmentedRightAngle;

	private float _flipWindowTimer;

	private float _boardAngleToGround;

	private float _manualAxis;

	private int _manualSign = 1;

	private PlayerController.SetupDir _setupDir;

	private float frontTruckSpringCache;

	private float frontTruckDampCache;

	private float backTruckSpringCache;

	private float backTruckDampCache;

	public PlayerState_Manualling(StickInput p_popStick, StickInput p_flipStick, bool p_manual)
	{
		_popStick = p_popStick;
		_flipStick = p_flipStick;
		_manual = p_manual;
		if (!PlayerController.Instance.IsSwitch)
		{
			if (_manual)
			{
				_manualSign = -1;
				return;
			}
			_manualSign = 1;
			return;
		}
		if (_manual)
		{
			_manualSign = 1;
			return;
		}
		_manualSign = -1;
	}

	public override void BothTriggersReleased(InputController.TurningMode p_turningMode)
	{
		PlayerController.Instance.RemoveTurnTorque(0.3f, p_turningMode);
	}

	public override bool CanGrind()
	{
		if (!_canPop)
		{
			return true;
		}
		return false;
	}

	public override void Enter()
	{
		PlayerController.Instance.boardController.ResetAll();
		PlayerController.Instance.CrossFadeAnimation("Manual", 0.3f);
		SetManualTruckSprings();
		SetCenterOfMass();
		PlayerController.Instance.SetTurnMultiplier(1f);
		PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
		if (!PlayerController.Instance.IsGrounded())
		{
			_manualStrength = 0f;
			PlayerController.Instance.SetManualStrength(0f);
		}
		else
		{
			_manualStrength = 1f;
			PlayerController.Instance.SetManualStrength(1f);
		}
		PlayerController.Instance.OnImpact();
	}

	public override void Exit()
	{
		PlayerController.Instance.VelocityOnPop = PlayerController.Instance.boardController.boardRigidbody.velocity;
		UnsetManualTruckSprings();
		PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
		PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
		PlayerController.Instance.ResetBoardCenterOfMass();
		PlayerController.Instance.ResetBackTruckCenterOfMass();
		PlayerController.Instance.ResetFrontTruckCenterOfMass();
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		PlayerController.Instance.comController.UpdateCOM(0.95f, 0);
		PlayerController.Instance.ManualRotation((_popStick.ForwardDir > 0.1f ? false : true), _popStick.ForwardDir, -_flipStick.PopDir, _flipStick.ToeAxis);
		PlayerController.Instance.SkaterRotation(true, true);
		PlayerController.Instance.ReduceImpactBounce();
	}

	public override StickInput GetPopStick()
	{
		return _popStick;
	}

	public override bool IsOnGroundState()
	{
		return true;
	}

	public override void OnCollisionEnterEvent()
	{
		if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Board)
		{
			PlayerController.Instance.ResetAllAnimations();
			PlayerController.Instance.SetBoardToMaster();
			PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
		}
	}

	public override void OnFirstWheelDown()
	{
	}

	public override void OnFlipStickUpdate()
	{
		float single;
		if (_canPop && PlayerController.Instance.IsGrounded())
		{
			PlayerController instance = PlayerController.Instance;
			ref bool flagPointer = ref _potentialFlip;
			ref Vector2 vector2Pointer = ref _initialFlipDir;
			ref int numPointer = ref _flipFrameCount;
			ref int numPointer1 = ref _flipFrameMax;
			ref float singlePointer = ref _toeAxis;
			ref float singlePointer1 = ref _flipVel;
			ref float singlePointer2 = ref _popVel;
			ref float singlePointer3 = ref _popDir;
			ref float singlePointer4 = ref _flip;
			StickInput stickInput = _flipStick;
			ref float singlePointer5 = ref _invertVel;
			single = (_popStick.IsRightStick ? _augmentedLeftAngle : _augmentedRightAngle);
			instance.OnFlipStickUpdate(ref _flipDetected, ref flagPointer, ref vector2Pointer, ref numPointer, ref numPointer1, ref singlePointer, ref singlePointer1, ref singlePointer2, ref singlePointer3, ref singlePointer4, stickInput, false, true, ref singlePointer5, single, false, _forwardLoad, ref _flipWindowTimer);
		}
	}

	public override void OnGrindDetected()
	{
		if (!_canPop)
		{
			DoTransition(typeof(PlayerState_Grinding), null);
		}
	}

	public override void OnImpactUpdate()
	{
	}

	public override void OnManualExit()
	{
		if (!_canPop)
		{
			_delayExit = true;
			return;
		}
		PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
		DoTransition(typeof(PlayerState_Riding), null);
	}

	public override void OnNextState()
	{
		PlayerController.Instance.AnimSetPopStrength(0f);
		PlayerController.Instance.boardController.ReferenceBoardRotation();
		PlayerController.Instance.SetTurnMultiplier(1.2f);
		PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
		PlayerController.Instance.FixTargetNormal();
		PlayerController.Instance.SetTargetToMaster();
		PlayerController.Instance.AnimOllieTransition(true);
		if (!_potentialFlip)
		{
			object[] objArray = new object[] { _popStick, _flipStick, _popForce, false, _forwardLoad, _invertVel, _setupDir, _augmentedLeftAngle, _augmentedRightAngle, _popVel, _toeAxis, _popDir };
			DoTransition(typeof(PlayerState_BeginPop), objArray);
			return;
		}
		object[] objArray1 = new object[] { _popStick, _flipStick, _initialFlipDir, _flipVel, _popVel, _toeAxis, _popDir, _flipDetected, _flip, _popForce, false, _forwardLoad, _invertVel, _setupDir, _augmentedLeftAngle, _augmentedRightAngle };
		DoTransition(typeof(PlayerState_BeginPop), objArray1);
	}

	public override void OnNoseManualExit()
	{
		if (!_canPop)
		{
			_delayExit = true;
			return;
		}
		PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
		DoTransition(typeof(PlayerState_Riding), null);
	}

	public override void OnPopStickUpdate()
	{
		if (_canPop && PlayerController.Instance.IsGrounded())
		{
			_forwardLoad = PlayerController.Instance.GetNollie(_popStick.IsRightStick) > 0.1f;
			PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(_popStick.IsRightStick));
			PlayerController.Instance.OnPopStartCheck(true, _popStick, ref _setupDir, _forwardLoad, 10f, ref _invertVel, 0f, ref _popVel);
		}
	}

	public override void OnPredictedCollisionEvent()
	{
		if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Board)
		{
			PlayerController.Instance.ResetAllAnimations();
			PlayerController.Instance.SetBoardToMaster();
			PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
		}
	}

	public override void OnWheelsLeftGround()
	{
		PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
		PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
		PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
		PlayerController.Instance.SetSkaterToMaster();
		PlayerController.Instance.AnimSetRollOff(true);
		PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
		Vector3 vector3 = PlayerController.Instance.skaterController.PredictLanding(PlayerController.Instance.skaterController.skaterRigidbody.velocity);
		PlayerController.Instance.skaterController.skaterRigidbody.AddForce(vector3, ForceMode.Impulse);
		object[] objArray = new object[] { false, false };
		DoTransition(typeof(PlayerState_InAir), objArray);
	}

	private void SetCenterOfMass()
	{
		bool flag;
		if (!PlayerController.Instance.GetBoardBackwards())
		{
			flag = (_popStick.ForwardDir > 0f ? false : true);
		}
		else
		{
			flag = (_popStick.ForwardDir > 0f ? true : false);
		}
		_manual = flag;
		Vector3 vector3 = (_manual ? PlayerController.Instance.boardController.backTruckCoM.position : PlayerController.Instance.boardController.frontTruckCoM.position);
		PlayerController.Instance.SetBoardCenterOfMass(PlayerController.Instance.boardController.boardTransform.InverseTransformPoint(vector3));
		PlayerController.Instance.SetBackTruckCenterOfMass(PlayerController.Instance.boardController.backTruckRigidbody.transform.InverseTransformPoint(vector3));
		PlayerController.Instance.SetFrontTruckCenterOfMass(PlayerController.Instance.boardController.frontTruckRigidbody.transform.InverseTransformPoint(vector3));
	}

	private void SetManualTruckSprings()
	{
		JointDrive instance = PlayerController.Instance.boardController.frontTruckJoint.angularXDrive;
		frontTruckSpringCache = instance.positionSpring;
		instance = PlayerController.Instance.boardController.frontTruckJoint.angularXDrive;
		frontTruckDampCache = instance.positionDamper;
		instance = PlayerController.Instance.boardController.backTruckJoint.angularXDrive;
		backTruckSpringCache = instance.positionSpring;
		instance = PlayerController.Instance.boardController.backTruckJoint.angularXDrive;
		backTruckDampCache = instance.positionDamper;
		JointDrive jointDrive = new JointDrive()
		{
			positionDamper = 1f,
			positionSpring = 20f
		};
		instance = PlayerController.Instance.boardController.frontTruckJoint.angularXDrive;
		jointDrive.maximumForce = instance.maximumForce;
		PlayerController.Instance.boardController.frontTruckJoint.angularXDrive = jointDrive;
		PlayerController.Instance.boardController.backTruckJoint.angularXDrive = jointDrive;
	}

	public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
	{
		stateType = FSMStateType.Type_OR;
	}

	private void UnsetManualTruckSprings()
	{
		JointDrive jointDrive = new JointDrive()
		{
			positionDamper = frontTruckDampCache,
			positionSpring = frontTruckSpringCache
		};
		JointDrive instance = PlayerController.Instance.boardController.frontTruckJoint.angularXDrive;
		jointDrive.maximumForce = instance.maximumForce;
		PlayerController.Instance.boardController.frontTruckJoint.angularXDrive = jointDrive;
		jointDrive.positionDamper = backTruckDampCache;
		jointDrive.positionSpring = backTruckSpringCache;
		instance = PlayerController.Instance.boardController.backTruckJoint.angularXDrive;
		jointDrive.maximumForce = instance.maximumForce;
		PlayerController.Instance.boardController.backTruckJoint.angularXDrive = jointDrive;
	}

	public override void Update()
	{
		base.Update();
		_boardAngleToGround = Vector3.Angle(PlayerController.Instance.boardController.boardTransform.up, PlayerController.Instance.GetGroundNormal());
		_boardAngleToGround *= (float)_manualSign;
		_boardAngleToGround = Mathf.Clamp(_boardAngleToGround, -30f, 30f);
		_boardAngleToGround /= 30f;
		_manualAxis = Mathf.Lerp(_manualAxis, _boardAngleToGround, Time.deltaTime * 10f);
		PlayerController.Instance.AnimSetManualAxis(_manualAxis);
		_manualStrength = Mathf.Clamp(_manualStrength + Time.deltaTime * 2f, 0f, 1f);
		PlayerController.Instance.SetManualStrength(_manualStrength);
		if (_popWait >= 0.2f)
		{
			PlayerController.Instance.ResetAfterGrinds();
			_canPop = true;
		}
		else if (PlayerController.Instance.IsGrounded())
		{
			_popWait += Time.deltaTime;
		}
		if (_delayExit)
		{
			_delayTimer += Time.deltaTime;
			if (_delayTimer > 0.2f)
			{
				PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
				PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
				DoTransition(typeof(PlayerState_Riding), null);
			}
		}
		PlayerController.Instance.SetBoardTargetPosition(0f);
		PlayerController.Instance.SetFrontPivotRotation(0f);
		PlayerController.Instance.SetBackPivotRotation(0f);
		PlayerController.Instance.SetPivotForwardRotation(0f, 40f);
		PlayerController.Instance.SetPivotSideRotation(0f);
	}
}