using FSMHelper;
using System;
using UnityEngine;

public class PlayerState_Pop : PlayerState_OnBoard
{
	private StickInput _popStick;

	private StickInput _flipStick;

	private bool _popRotationDone;

	private bool _scooppFlipInputWindowDone;

	private float _scoopFlipInputWindowTimer;

	private float _popRotationRemovalTimer;

	private bool _forwardLoad;

	private bool _checkForCollisions;

	private float _collisionTimer;

	private bool _flipDetected;

	private bool _potentialFlip;

	private Vector2 _initialFlipDir = Vector2.zero;

	private int _flipFrameCount;

	private int _flipFrameMax = 25;

	private float _toeAxis;

	private float _flipVel;

	private float _popVel;

	private float _popDir;

	private bool _canManual;

	private float _flip;

	private bool _wasGrinding;

	private float _invertVel;

	private float _augmentedLeftAngle;

	private float _augmentedRightAngle;

	private PlayerController.SetupDir _setupDir;

	private float _kickAddSoFar;

	private bool _leftCaught;

	private bool _rightCaught;

	private bool _catchRegistered;

	private bool _leftCaughtFirst;

	private bool _rightCaughtFirst;

	private float _steezeTimer;

	private bool _rightOn = true;

	private bool _leftOn = true;

	private float _timer;

	public PlayerState_Pop(StickInput p_popStick, StickInput p_flipStick, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_kickAddSoFar)
	{
		_popStick = p_popStick;
		_flipStick = p_flipStick;
		_forwardLoad = p_forwardLoad;
		_setupDir = p_setupDir;
		_augmentedLeftAngle = p_augmentedLeftAngle;
		_augmentedRightAngle = p_augmentedRightAngle;
		_kickAddSoFar = p_kickAddSoFar;
	}

	public PlayerState_Pop(StickInput p_popStick, StickInput p_flipStick, bool p_wasGrinding, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_popVel, float p_toeAxis, float p_popDir, float p_kickAddSoFar)
	{
		_popStick = p_popStick;
		_flipStick = p_flipStick;
		_wasGrinding = p_wasGrinding;
		_forwardLoad = p_forwardLoad;
		_invertVel = p_invertVel;
		_setupDir = p_setupDir;
		_augmentedLeftAngle = p_augmentedLeftAngle;
		_augmentedRightAngle = p_augmentedRightAngle;
		_popVel = p_popVel;
		_toeAxis = p_toeAxis;
		_popDir = p_popDir;
		_kickAddSoFar = p_kickAddSoFar;
	}

	public PlayerState_Pop(StickInput p_popStick, StickInput p_flipStick, Vector2 p_initialFlipDir, float p_flipVel, float p_popVel, float p_toeAxis, float p_popDir, bool p_flipDetected, float p_flip, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_kickAddSoFar)
	{
		_popStick = p_popStick;
		_flipStick = p_flipStick;
		_potentialFlip = false;
		_flipDetected = p_flipDetected;
		_initialFlipDir = p_initialFlipDir;
		_toeAxis = p_toeAxis;
		_popDir = p_popDir;
		_flipVel = p_flipVel;
		_popVel = p_popVel;
		_flip = p_flip;
		_forwardLoad = p_forwardLoad;
		_invertVel = p_invertVel;
		_setupDir = p_setupDir;
		_augmentedLeftAngle = p_augmentedLeftAngle;
		_augmentedRightAngle = p_augmentedRightAngle;
		_kickAddSoFar = p_kickAddSoFar;
	}

	public PlayerState_Pop(StickInput p_popStick, StickInput p_flipStick, Vector2 p_initialFlipDir, float p_flipVel, float p_popVel, float p_toeAxis, float p_popDir, bool p_flipDetected, float p_flip, bool p_wasGrinding, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_kickAddSoFar)
	{
		_popStick = p_popStick;
		_flipStick = p_flipStick;
		_potentialFlip = false;
		_flipDetected = p_flipDetected;
		_initialFlipDir = p_initialFlipDir;
		_toeAxis = p_toeAxis;
		_popDir = p_popDir;
		_flipVel = p_flipVel;
		_popVel = p_popVel;
		_flip = p_flip;
		_wasGrinding = p_wasGrinding;
		_forwardLoad = p_forwardLoad;
		_invertVel = p_invertVel;
		_setupDir = p_setupDir;
		_augmentedLeftAngle = p_augmentedLeftAngle;
		_augmentedRightAngle = p_augmentedRightAngle;
		_kickAddSoFar = p_kickAddSoFar;
	}

	private void AddForwardVel()
	{
		float single = 15f;
		float single1 = Mathf.Clamp(Mathf.Abs(_popVel) / single, -1f, 1f);
		float single2 = (1f - single1) * _invertVel;
		PlayerController.Instance.AddForwardSpeed(single2);
	}

	public override bool CanGrind()
	{
		if (!_checkForCollisions)
		{
			return false;
		}
		return !_flipDetected;
	}

	public override void Enter()
	{
		float num = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
		num = (num <= 0f ? -2f : 2f);
		PlayerController.Instance.ForcePivotForwardRotation(num);
		PlayerController.Instance.AnimSetGrinding(false);
		if (PlayerController.Instance.inputController.turningMode != InputController.TurningMode.FastLeft && PlayerController.Instance.inputController.turningMode != InputController.TurningMode.FastRight)
		{
			PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
		}
	}

	public override void Exit()
	{
		PlayerController.Instance.SetKneeBendWeight(0f);
	}

	public override void FixedUpdate()
	{
		PlayerController.Instance.comController.UpdateCOM();
		base.FixedUpdate();
		_scoopFlipInputWindowTimer += Time.deltaTime;
		_popRotationRemovalTimer += Time.deltaTime;
		if (!_popRotationDone)
		{
			KickAdd();
			if (_popRotationRemovalTimer >= 0.04f)
			{
				KillPopRotation();
			}
		}
		if (!_scooppFlipInputWindowDone)
		{
			if (_scoopFlipInputWindowTimer >= PlayerController.Instance.scoopFlipWindowNoFlipDetected + (_flipDetected ? 0.075f : 0f))
			{
				_scooppFlipInputWindowDone = true;
				AddForwardVel();
				if (_flipDetected || PlayerController.Instance.animationController.skaterAnim.GetBool("Released"))
				{
					if (!_popRotationDone)
					{
						KillPopRotation();
					}
					PlayerController.Instance.playerSM.OnNextStateSM();
					PlayerController.Instance.SetLeftIKLerpTarget(1f);
					PlayerController.Instance.SetRightIKLerpTarget(1f);
				}
			}
		}
		if (!_checkForCollisions)
		{
			_collisionTimer += Time.deltaTime;
			if (_collisionTimer > 0.2f)
			{
				_checkForCollisions = true;
			}
		}
		_timer += Time.deltaTime;
		if (PlayerController.Instance.GetAnimReleased())
		{
			if (_popRotationDone)
			{
				PlayerController.Instance.FlipTrickRotation();
			}
			else
			{
				PlayerController.Instance.boardController.Rotate(true, false);
			}
		}
		if (!PlayerController.Instance.GetAnimReleased())
		{
			if (!_wasGrinding)
			{
				if (!_popRotationDone)
				{
					PlayerController.Instance.boardController.Rotate(true, false);
					return;
				}
				PlayerController.Instance.PhysicsRotation(50f, 10f);
				PlayerController.Instance.boardController.UpdateReferenceBoardTargetRotation();
				return;
			}
			if (!_popRotationDone)
			{
				PlayerController.Instance.boardController.Rotate(true, false);
				return;
			}
			PlayerController.Instance.PhysicsRotation(50f, 10f);
			PlayerController.Instance.boardController.UpdateReferenceBoardTargetRotation();
		}
	}

	public override float GetAugmentedAngle(StickInput p_stick)
	{
		if (p_stick.IsRightStick)
		{
			return _augmentedRightAngle;
		}
		return _augmentedLeftAngle;
	}

	public override StickInput GetPopStick()
	{
		return _popStick;
	}

	private void KickAdd()
	{
		float single = 15f;
		float single1 = Mathf.Clamp(Mathf.Abs(_popVel) / single, -0.7f, 0.7f);
		float single2 = 1.1f;
		if (_wasGrinding)
		{
			single2 *= 0.5f;
		}
		float single3 = single2 - single2 * single1 - _kickAddSoFar;
		_kickAddSoFar += single3;
		PlayerController.Instance.DoKick(_forwardLoad, single3);
	}

	private void KillPopRotation()
	{
		float single = 15f;
		float single1 = Mathf.Clamp(Mathf.Abs(Mathf.Abs(_popVel)) / single, -1f, 1f);
		float single2 = 1.2f;
		if (_wasGrinding)
		{
			single2 *= 0.5f;
		}
		float single3 = single2 - single2 * single1;
		PlayerController.Instance.DoKick(!_forwardLoad, single3);
		_popRotationDone = true;
	}

	public override bool LeftFootOff()
	{
		return !_leftOn;
	}

	public override void OnAnimatorUpdate()
	{
	}

	public override void OnCanManual()
	{
		_canManual = true;
	}

	public override void OnCollisionStayEvent()
	{
	}

	public override void OnFlipStickUpdate()
	{
		float single;
		if (!_scooppFlipInputWindowDone)
		{
			float single1 = 0f;
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
			instance.OnFlipStickUpdate(ref _flipDetected, ref flagPointer, ref vector2Pointer, ref numPointer, ref numPointer1, ref singlePointer, ref singlePointer1, ref singlePointer2, ref singlePointer3, ref singlePointer4, stickInput, true, false, ref singlePointer5, single, _scooppFlipInputWindowDone, _forwardLoad, ref single1);
		}
	}

	public override void OnGrindDetected()
	{
		if (!_wasGrinding && _checkForCollisions && !_flipDetected)
		{
			DoTransition(typeof(PlayerState_Grinding), null);
		}
	}

	public override void OnLeftStickCenteredUpdate()
	{
		if (_scooppFlipInputWindowDone && !_leftCaught)
		{
			_leftCaught = true;
			if (_rightCaught)
			{
				_rightCaughtFirst = true;
				_catchRegistered = true;
			}
		}
	}

	public override void OnNextState()
	{
		if (!_wasGrinding)
		{
			object[] objArray = new object[] { _popStick, _flipStick, _initialFlipDir, _flipVel, _popVel, _toeAxis, _popDir, _flipDetected, _flip, _forwardLoad, _invertVel, _setupDir, _catchRegistered, _leftCaughtFirst, _rightCaughtFirst };
			DoTransition(typeof(PlayerState_Released), objArray);
			return;
		}
		object[] objArray1 = new object[] { _popStick, _flipStick, _initialFlipDir, _flipVel, _popVel, _toeAxis, _popDir, _flipDetected, _flip, _forwardLoad, _invertVel, _setupDir, _wasGrinding, _catchRegistered, _leftCaughtFirst, _rightCaughtFirst };
		DoTransition(typeof(PlayerState_Released), objArray1);
	}

	public override void OnPopStickUpdate()
	{
		float single;
		if (!_scooppFlipInputWindowDone)
		{
			PlayerController instance = PlayerController.Instance;
			bool flag = PlayerController.Instance.IsGrounded();
			StickInput stickInput = _popStick;
			bool flag1 = _forwardLoad;
			ref PlayerController.SetupDir setupDirPointer = ref _setupDir;
			ref float singlePointer = ref _invertVel;
			single = (_popStick.IsRightStick ? _augmentedRightAngle : _augmentedLeftAngle);
			instance.OnPopStickUpdate(0.1f, flag, stickInput, ref _popVel, 10f, flag1, ref setupDirPointer, ref singlePointer, single);
		}
	}

	public override void OnPredictedCollisionEvent()
	{
		if (!_flipDetected && _checkForCollisions)
		{
			PlayerController.Instance.SetBoardToMaster();
			PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
			PlayerController.Instance.AnimOllieTransition(false);
			PlayerController.Instance.AnimSetupTransition(false);
			DoTransition(typeof(PlayerState_Impact), null);
			PlayerController.Instance.skaterController.AddCollisionOffset();
		}
	}

	public override void OnRightStickCenteredUpdate()
	{
		if (_scooppFlipInputWindowDone && !_rightCaught)
		{
			_rightCaught = true;
			if (_leftCaught)
			{
				_leftCaughtFirst = true;
				_catchRegistered = true;
			}
		}
	}

	public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
	{
		if (_scooppFlipInputWindowDone)
		{
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
					{
						PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
						PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
						PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
						PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
						PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
						return;
					}
					PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
					PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
					PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
					PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
					PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
					return;
				}
				case SettingsManager.ControlType.Swap:
				{
					if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
					{
						PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
						PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
						PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
						PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
						PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
						return;
					}
					PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
					PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
					PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
					PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
					PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
					return;
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
						{
							PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
							PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
							PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
							PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
							PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
							return;
						}
						PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
						PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
						PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
						PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
						PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
						return;
					}
					if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
					{
						PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
						PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
						PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
						PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
						PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
						return;
					}
					PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
					PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
					PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
					PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
					PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
					return;
				}
				default:
				{
					return;
				}
			}
		}
		switch (SettingsManager.Instance.controlType)
		{
			case SettingsManager.ControlType.Same:
			{
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
					PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
					PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
					float num = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
					num = (num <= 0f ? -2f : 2f);
					PlayerController.Instance.SetPivotForwardRotation(num, 20f);
					PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
					return;
				}
				PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
				PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
				PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
				float single = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
				single = (single <= 0f ? -2f : 2f);
				PlayerController.Instance.SetPivotForwardRotation(single, 20f);
				PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
				return;
			}
			case SettingsManager.ControlType.Swap:
			{
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
					PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
					PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
					float num1 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
					num1 = (num1 <= 0f ? -2f : 2f);
					PlayerController.Instance.SetPivotForwardRotation(num1, 20f);
					PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
					return;
				}
				PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
				PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
				PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
				float single1 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
				single1 = (single1 <= 0f ? -2f : 2f);
				PlayerController.Instance.SetPivotForwardRotation(single1, 20f);
				PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
				return;
			}
			case SettingsManager.ControlType.Simple:
			{
				if (!PlayerController.Instance.IsSwitch)
				{
					if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
					{
						PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
						PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
						PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
						float num2 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
						num2 = (num2 <= 0f ? -2f : 2f);
						PlayerController.Instance.SetPivotForwardRotation(num2, 20f);
						PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
						return;
					}
					PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
					PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
					PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
					float single2 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
					single2 = (single2 <= 0f ? -2f : 2f);
					PlayerController.Instance.SetPivotForwardRotation(single2, 20f);
					PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
					return;
				}
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
					PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
					PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
					float num3 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
					num3 = (num3 <= 0f ? -2f : 2f);
					PlayerController.Instance.SetPivotForwardRotation(num3, 20f);
					PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
					return;
				}
				PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
				PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
				PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
				float single3 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
				single3 = (single3 <= 0f ? -2f : 2f);
				PlayerController.Instance.SetPivotForwardRotation(single3, 20f);
				PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
				return;
			}
			default:
			{
				return;
			}
		}
	}

	public override void OnStickPressed(bool p_right)
	{
		if (!PlayerController.Instance.GetAnimReleased())
		{
			if (!p_right)
			{
				if (_leftOn && _rightOn)
				{
					PlayerController.Instance.SetLeftIKLerpTarget(1f, 1f);
					PlayerController.Instance.SetLeftSteezeWeight(1f);
					PlayerController.Instance.SetMaxSteezeLeft(1f);
					_leftOn = false;
					return;
				}
				if (!_leftOn && _rightOn)
				{
					SoundManager.Instance.PlayCatchSound();
					PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
					PlayerController.Instance.SetLeftSteezeWeight(0f);
					PlayerController.Instance.SetMaxSteezeLeft(0f);
					_leftOn = true;
				}
			}
			else
			{
				if (_rightOn && _leftOn)
				{
					PlayerController.Instance.SetRightIKLerpTarget(1f, 1f);
					PlayerController.Instance.SetRightSteezeWeight(1f);
					PlayerController.Instance.SetMaxSteezeRight(1f);
					_rightOn = false;
					return;
				}
				if (!_rightOn && _leftOn)
				{
					SoundManager.Instance.PlayCatchSound();
					PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
					PlayerController.Instance.SetRightSteezeWeight(0f);
					PlayerController.Instance.SetMaxSteezeRight(0f);
					_rightOn = true;
					return;
				}
			}
		}
	}

	public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
	{
		PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, false, PlayerController.Instance.GetAnimReleased());
		PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, false, PlayerController.Instance.GetAnimReleased());
	}

	public override bool Popped()
	{
		return true;
	}

	public override bool RightFootOff()
	{
		return !_rightOn;
	}

	public override void SendEventEndFlipPeriod()
	{
	}

	public override void SendEventExtend(float value)
	{
		if (!_wasGrinding)
		{
			if (_leftOn && _rightOn)
			{
				object[] objArray = new object[] { 1 };
				DoTransition(typeof(PlayerState_InAir), objArray);
				return;
			}
			object[] objArray1 = new object[] { _wasGrinding, _leftOn, _rightOn, true };
			DoTransition(typeof(PlayerState_InAir), objArray1);
			return;
		}
		if (_leftOn && _rightOn)
		{
			object[] objArray2 = new object[] { _wasGrinding, 1 };
			DoTransition(typeof(PlayerState_InAir), objArray2);
			return;
		}
		object[] objArray3 = new object[] { _wasGrinding, _leftOn, _rightOn, true };
		DoTransition(typeof(PlayerState_InAir), objArray3);
	}

	private void SetPopSteezeWeight()
	{
		if (_popStick.IsRightStick)
		{
			PlayerController.Instance.SetMaxSteezeRight(1f);
			return;
		}
		PlayerController.Instance.SetMaxSteezeLeft(1f);
	}

	public override void Update()
	{
		base.Update();
		if (_popRotationDone)
		{
			PlayerController.Instance.SetKneeBendWeight(Mathf.Lerp(PlayerController.Instance.GetKneeBendWeight(), 0f, Time.deltaTime * 10f));
		}
		_steezeTimer += Time.deltaTime;
		if (_steezeTimer > 0.08f && PlayerController.Instance.GetAnimReleased())
		{
			SetPopSteezeWeight();
		}
	}
}