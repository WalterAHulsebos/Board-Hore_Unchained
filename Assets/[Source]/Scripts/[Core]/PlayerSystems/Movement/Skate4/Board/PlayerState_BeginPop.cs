using FSMHelper;
using System;
using UnityEngine;

public class PlayerState_BeginPop : PlayerState_OnBoard
{
	private StickInput _popStick;

	private StickInput _flipStick;

	private bool _forwardLoad;

	private bool _flipDetected;

	private bool _potentialFlip;

	private Vector2 _initialFlipDir = Vector2.zero;

	private int _flipFrameCount;

	private int _flipFrameMax = 25;

	private float _toeAxis;

	private float _flip;

	private float _flipVel;

	private float _popVel;

	private float _popDir;

	private float _timer;

	private float _popForce;

	private bool _wasGrinding;

	private float _invertVel;

	private float _augmentedLeftAngle;

	private float _augmentedRightAngle;

	private float _kickAddSoFar;

	private PlayerController.SetupDir _setupDir;

	public PlayerState_BeginPop(StickInput p_popStick, StickInput p_flipStick, float p_popForce, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_popVel, float p_toeAxis, float p_popDir)
	{
		_popStick = p_popStick;
		_flipStick = p_flipStick;
		_popForce = p_popForce;
		_forwardLoad = p_forwardLoad;
		_invertVel = p_invertVel;
		_setupDir = p_setupDir;
		_augmentedLeftAngle = p_augmentedLeftAngle;
		_augmentedRightAngle = p_augmentedRightAngle;
		_popVel = p_popVel;
		_toeAxis = p_toeAxis;
		_popDir = p_popDir;
		PlayerController.Instance.animationController.skaterAnim.CrossFadeInFixedTime("Pop", 0.1f);
		PlayerController.Instance.animationController.ikAnim.CrossFadeInFixedTime("Pop", 0.1f);
	}

	public PlayerState_BeginPop(StickInput p_popStick, StickInput p_flipStick, float p_popForce, bool p_wasGrinding, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_popVel, float p_toeAxis, float p_popDir)
	{
		_popStick = p_popStick;
		_flipStick = p_flipStick;
		_popForce = p_popForce;
		_wasGrinding = p_wasGrinding;
		_forwardLoad = p_forwardLoad;
		_invertVel = p_invertVel;
		_setupDir = p_setupDir;
		_augmentedLeftAngle = p_augmentedLeftAngle;
		_augmentedRightAngle = p_augmentedRightAngle;
		_popVel = p_popVel;
		_toeAxis = p_toeAxis;
		_popDir = p_popDir;
	}

	public PlayerState_BeginPop(StickInput p_popStick, StickInput p_flipStick, Vector2 p_initialFlipDir, float p_flipVel, float p_popVel, float p_toeAxis, float p_popDir, bool p_flipDetected, float p_flip, float p_popForce, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle)
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
		_popForce = p_popForce;
		_forwardLoad = p_forwardLoad;
		_invertVel = p_invertVel;
		_setupDir = p_setupDir;
		_augmentedLeftAngle = p_augmentedLeftAngle;
		_augmentedRightAngle = p_augmentedRightAngle;
		PlayerController.Instance.animationController.skaterAnim.CrossFadeInFixedTime("Pop", 0.1f);
		PlayerController.Instance.animationController.ikAnim.CrossFadeInFixedTime("Pop", 0.1f);
	}

	public PlayerState_BeginPop(StickInput p_popStick, StickInput p_flipStick, Vector2 p_initialFlipDir, float p_flipVel, float p_popVel, float p_toeAxis, float p_popDir, bool p_flipDetected, float p_flip, float p_popForce, bool p_wasGrinding, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle)
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
		_popForce = p_popForce;
		_wasGrinding = p_wasGrinding;
		_forwardLoad = p_forwardLoad;
		_invertVel = p_invertVel;
		_setupDir = p_setupDir;
		_augmentedLeftAngle = p_augmentedLeftAngle;
		_augmentedRightAngle = p_augmentedRightAngle;
	}

	public override bool CanGrind()
	{
		return false;
	}

	public override void Enter()
	{
		PlayerController.Instance.SetKneeBendWeight(0.8f);
		PlayerController.Instance.CrossFadeAnimation("Pop", 0.06f);
		PlayerController instance = PlayerController.Instance;
		Vector3 vector3 = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterTransform.position - PlayerController.Instance.boardController.boardTransform.position, PlayerController.Instance.skaterController.skaterTransform.forward);
		instance.ScaleDisplacementCurve(vector3.magnitude + 0.0935936f);
		PlayerController.Instance.boardController.ResetTweakValues();
		PlayerController.Instance.boardController.CacheBoardUp();
		PlayerController.Instance.boardController.UpdateReferenceBoardTargetRotation();
		KickAdd();
	}

	public override void Exit()
	{
	}

	public override void FixedUpdate()
	{
		if (_timer > 1f)
		{
			PlayerController.Instance.AnimPopInterruptedTransitions(true);
			PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
			PlayerController.Instance.SetBoardToMaster();
			DoTransition(typeof(PlayerState_Riding), null);
		}
		PlayerController.Instance.comController.UpdateCOM();
		_timer += Time.deltaTime;
		PlayerController.Instance.AddUpwardDisplacement(_timer);
		if (_timer > 0.06f)
		{
			SendEventPop(0f);
			return;
		}
		KickAdd();
		PlayerController.Instance.UpdateSkaterDuringPop();
		PlayerController.Instance.MoveCameraToPlayer();
		PlayerController.Instance.boardController.Rotate(true, false);
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
		float single = 5f;
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

	public override void OnAnimatorUpdate()
	{
	}

	public override void OnFlipStickUpdate()
	{
		float single;
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
		instance.OnFlipStickUpdate(ref _flipDetected, ref flagPointer, ref vector2Pointer, ref numPointer, ref numPointer1, ref singlePointer, ref singlePointer1, ref singlePointer2, ref singlePointer3, ref singlePointer4, stickInput, false, false, ref singlePointer5, single, false, _forwardLoad, ref single1);
	}

	public override void OnPopStickUpdate()
	{
		float single;
		PlayerController instance = PlayerController.Instance;
		bool flag = PlayerController.Instance.IsGrounded();
		StickInput stickInput = _popStick;
		bool flag1 = _forwardLoad;
		ref PlayerController.SetupDir setupDirPointer = ref _setupDir;
		ref float singlePointer = ref _invertVel;
		single = (_popStick.IsRightStick ? _augmentedRightAngle : _augmentedLeftAngle);
		instance.OnPopStickUpdate(0.1f, flag, stickInput, ref _popVel, 10f, flag1, ref setupDirPointer, ref singlePointer, single);
	}

	public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
	{
	}

	public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
	{
		PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, false, PlayerController.Instance.GetAnimReleased());
		PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, false, PlayerController.Instance.GetAnimReleased());
	}

	public override void SendEventPop(float p_value)
	{
		if (!_wasGrinding)
		{
			PlayerController.Instance.AnimSetupTransition(false);
			PlayerController.Instance.OnPop(_popForce, _popVel);
			object[] objArray = new object[] { _popStick, _flipStick, _initialFlipDir, _flipVel, _popVel, _toeAxis, _popDir, _flipDetected, _flip, _forwardLoad, _invertVel, _setupDir, _augmentedLeftAngle, _augmentedRightAngle, _kickAddSoFar };
			DoTransition(typeof(PlayerState_Pop), objArray);
			return;
		}
		Vector3 vector3 = (PlayerController.Instance.boardController.triggerManager.sideEnteredGrind == TriggerManager.SideEnteredGrind.Right ? PlayerController.Instance.boardController.triggerManager.grindRight : -PlayerController.Instance.boardController.triggerManager.grindRight);
		PlayerController.Instance.AnimSetupTransition(false);
		if (PlayerController.Instance.boardController.triggerManager.sideEnteredGrind == TriggerManager.SideEnteredGrind.Center)
		{
			PlayerController.Instance.OnPop(_popForce, _popVel);
		}
		else
		{
			PlayerController.Instance.OnPop(_popForce, _popStick.AugmentedToeAxisVel, vector3 * 0.5f);
		}
		object[] objArray1 = new object[] { _popStick, _flipStick, _initialFlipDir, _flipVel, _popVel, _toeAxis, _popDir, _flipDetected, _flip, true, _forwardLoad, _invertVel, _setupDir, _augmentedLeftAngle, _augmentedRightAngle, _kickAddSoFar };
		DoTransition(typeof(PlayerState_Pop), objArray1);
	}

	public override void Update()
	{
		base.Update();
	}
}