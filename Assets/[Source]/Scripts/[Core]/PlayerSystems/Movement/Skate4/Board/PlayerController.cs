using Dreamteck.Splines;
using FSMHelper;
using RootMotion.Dynamics;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	private static PlayerController _instance;

	public string currentState = "";

	public Respawn respawn;

	public Transform CenterOfMass;

	public float popForce;

	public float horizontalSpeed = 200f;

	public float verticalSpeed = 100f;

	public float flipStickDeadZone;

	public float scoopFlipWindowNoFlipDetected;

	public float olliexVelThreshold;

	public float flipThreshold;

	public float popThreshold;

	public float shuvMult;

	public float doubleShuvMult;

	public float shuvFlip;

	public float shuvMax;

	public float flipMult;

	public float invertMult;

	public float boneMult;

	public float kickForce;

	public float topSpeed;

	public float spinDeccelerate;

	public COMController comController;

	public CameraController cameraController;

	public SkaterController skaterController;

	public BoardController boardController;

	public AnimationController animationController;

	public InputController inputController;

	public IKController ikController;

	public Transform playerRotationReference;

	public Transform boardOffsetRoot;

	public Transform playerOffsetRoot;

	[SerializeField]
	private CoMDisplacement _comDisplacement;

	public PlayerController.MovementMaster movementMaster;

	public PlayerStateMachine playerSM;

	private Vector3 _ridingCameraForward = Vector3.zero;

	private Vector3 _ridingPlayerForward = Vector3.zero;

	private Vector3 _lastRidingPosition = Vector3.zero;

	private bool _isRespawning;

	private Quaternion _playerGrindRotation = Quaternion.identity;

	private Quaternion _boardGrindRotation = Quaternion.identity;

	private bool _manualling;

	private Vector3 _velocityOnPop = Vector3.zero;

	private bool _isInSetupState;

	private Vector2 _flick = Vector2.zero;

	private float _flickSpeed;

	private float _stickAngle;

	public bool popped;

	private float _turnAnimAmount;

	private float _turnVel;

	public float turnAnimSpring;

	public float turnAnimDamp;

	public float torsoTorqueMult;

	private float _frontFootForwardAxis;

	private float _backFootForwardAxis;

	private float _frontFootToeAxis;

	private float _backFootToeAxis;

	private Vector2 _frontMagnitude = Vector2.zero;

	private Vector2 _backMagnitude = Vector2.zero;

	private float _forwardTweakAxis;

	private float _toeTweakAxis;

	private float _boardAngleToGround;

	private float _flipAxis;

	private float _flipAxisTarget;

	public float impactBoardDownForce;

	private Quaternion newRot = Quaternion.identity;

	public Image leftRawVector;

	public Image rightRawVector;

	public Image leftAugmentedVector;

	public Image rightAugmentedVector;

	private Vector3 _leftRawRot;

	private Vector3 _rightRawRot;

	private Vector3 _leftAugRot;

	private Vector3 _rightAugRot;

	private Vector3 _leftRawScale;

	private Vector3 _rightRawScale;

	private Vector3 _leftAugScale;

	private Vector3 _rightAugScale;

	private Vector2 _tempAugmentedUp;

	public Quaternion BoardGrindRotation
	{
		get => _boardGrindRotation;
		set => _boardGrindRotation = value;
	}

	public static PlayerController Instance => _instance;

	public bool IsAnimSwitch
	{
		get
		{
			if (Vector3.Angle(cameraController._actualCam.forward, skaterController.skaterTransform.forward) > 90f)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsInSetupState => _isInSetupState;

	public bool IsRespawning
	{
		get => _isRespawning;
		set => _isRespawning = value;
	}

	public bool IsSwitch
	{
		get
		{
			if (Vector3.Angle(_ridingCameraForward, _ridingPlayerForward) > 90f)
			{
				return true;
			}
			return false;
		}
	}

	public bool Manualling
	{
		get => _manualling;
		set => _manualling = value;
	}

	public Quaternion PlayerGrindRotation
	{
		get => _playerGrindRotation;
		set => _playerGrindRotation = value;
	}

	public Vector3 VelocityOnPop
	{
		get => _velocityOnPop;
		set => _velocityOnPop = value;
	}

	public PlayerController()
	{
	}

	public void AddForceAtPosition(Rigidbody p_rb, Vector3 p_position, Vector3 p_direction, float p_force, ForceMode p_forceMode)
	{
		Debug.DrawLine(p_position, p_position + ((p_direction * p_force) * Time.fixedDeltaTime), Color.red, 0.5f);
		p_rb.AddForceAtPosition((p_direction * p_force) * Time.fixedDeltaTime, p_position, p_forceMode);
	}

	public void AddForwardSpeed(float p_value)
	{
		BoardController boardController = this.boardController;
		boardController.firstVel = boardController.firstVel + (this.boardController.IsBoardBackwards ? -p_value : p_value);
	}

	public void AddPushForce(float p_value)
	{
		boardController.AddPushForce(p_value);
	}

	public void AddSidwaysGrindVelocity(Rigidbody p_rb, Vector3 p_up, Vector3 p_direction, float p_force)
	{
		if (p_direction != Vector3.zero && p_direction.x != Single.NaN && p_direction.y != Single.NaN && p_direction.z != Single.NaN)
		{
			Rigidbody pRb = p_rb;
			pRb.velocity = pRb.velocity + ((Vector3.ProjectOnPlane(p_direction.normalized, p_up) * Time.fixedDeltaTime) * p_force);
		}
	}

	public void AddToScoopSpeed(float p_value)
	{
		Debug.LogError(string.Concat("add tp ", p_value));
		boardController.secondVel += p_value;
	}

	public void AddUpwardDisplacement(float p_timeStep)
	{
		skaterController.AddUpwardDisplacement(p_timeStep);
	}

	public bool AngleMagnitudeGrindCheck(Vector3 p_velocity, SplineResult p_splineResult)
	{
		float single = Vector3.Angle(Vector3.ProjectOnPlane(Instance.boardController.boardRigidbody.velocity, p_splineResult.normal), p_splineResult.direction);
		if (single < 60f)
		{
			return true;
		}
		if (single > 110f)
		{
			return true;
		}
		return false;
	}

	public float AngleToBoardTargetRotation()
	{
		return Quaternion.Angle(boardController.boardRigidbody.rotation, boardController.currentRotationTarget);
	}

	public void AnimCaught(bool p_value)
	{
		animationController.SetValue("Caught", p_value);
	}

	public void AnimEndImpactEarly(bool p_value)
	{
		animationController.SetValue("EndImpact", p_value);
	}

	public void AnimForceFlipValue(float p_value)
	{
		_flipAxisTarget = p_value;
		_flipAxis = p_value;
		animationController.SetValue("FlipAxis", p_value);
	}

	public void AnimForceScoopValue(float p_value)
	{
		animationController.SetValue("ScoopAxis", p_value);
	}

	public float AnimGetManualAxis()
	{
		return animationController.skaterAnim.GetFloat("ManualAxis");
	}

	public void AnimGrindTransition(bool p_value)
	{
		animationController.SetValue("Grind", p_value);
	}

	public void AnimLandedEarly(bool p_value)
	{
		animationController.SetValue("LandedEarly", p_value);
	}

	public void AnimOllieTransition(bool p_value)
	{
		animationController.SetValue("Ollie", p_value);
	}

	public void AnimPopInterruptedTransitions(bool p_value)
	{
		animationController.SetValue("PopInterrupted", p_value);
	}

	public void AnimRelease(bool p_value)
	{
		animationController.SetValue("Released", p_value);
	}

	public void AnimSetAllDown(bool p_value)
	{
		animationController.SetValue("AllDown", p_value);
	}

	public void AnimSetBraking(bool p_value)
	{
		animationController.SetValue("Braking", p_value);
	}

	public void AnimSetCatchAngle(float p_value)
	{
		animationController.SetValue("CatchAngle", p_value);
	}

	public void AnimSetFlip(float p_value)
	{
		_flipAxisTarget = p_value;
	}

	public void AnimSetGrindBlend(float p_x, float p_y)
	{
		animationController.SetValue("GrindX", p_x);
		animationController.SetValue("GrindY", p_y);
	}

	public void AnimSetGrinding(bool p_value)
	{
		animationController.SetValue("Grinding", p_value);
	}

	public void AnimSetInAirTurn(float p_value)
	{
		animationController.SetValue("InAirTurn", p_value);
	}

	public void AnimSetManual(bool p_value, float p_manualAxis)
	{
		float single;
		if (!p_value)
		{
			AnimSetManualAxis(p_manualAxis);
			animationController.SetValue("Manual", p_value);
			animationController.SetValue("NoseManual", p_value);
		}
		else
		{
			_boardAngleToGround = Vector3.Angle(Instance.boardController.boardTransform.up, Instance.GetGroundNormal());
			if (p_manualAxis > 0.1f)
			{
				single = _boardAngleToGround;
			}
			else
			{
				single = (p_manualAxis < -0.1f ? -_boardAngleToGround : _boardAngleToGround);
			}
			_boardAngleToGround = single;
			_boardAngleToGround = Mathf.Clamp(_boardAngleToGround, -30f, 30f);
			_boardAngleToGround /= 30f;
			AnimSetManualAxis(_boardAngleToGround);
			animationController.SetValue("Manual", p_value);
		}
		Manualling = p_value;
	}

	public void AnimSetManualAxis(float p_value)
	{
		animationController.SetValue("ManualAxis", p_value);
	}

	public void AnimSetMongo(bool p_value)
	{
		animationController.SetValue("PushMongo", p_value);
	}

	public void AnimSetNoComply(bool p_value)
	{
		animationController.SetValue("NoComply", p_value);
	}

	public void AnimSetNollie(float p_value)
	{
		animationController.SetValue("Nollie", p_value);
		animationController.SetNollieSteezeIK(p_value);
	}

	public void AnimSetNoseManual(bool p_value, float p_manualAxis)
	{
		float single;
		if (!p_value)
		{
			AnimSetManualAxis(p_manualAxis);
			animationController.SetValue("Manual", p_value);
			animationController.SetValue("NoseManual", p_value);
		}
		else
		{
			_boardAngleToGround = Vector3.Angle(Instance.boardController.boardTransform.up, Instance.GetGroundNormal());
			if (p_manualAxis > 0.1f)
			{
				single = _boardAngleToGround;
			}
			else
			{
				single = (p_manualAxis < -0.1f ? -_boardAngleToGround : _boardAngleToGround);
			}
			_boardAngleToGround = single;
			_boardAngleToGround = Mathf.Clamp(_boardAngleToGround, -30f, 30f);
			_boardAngleToGround /= 30f;
			AnimSetManualAxis(_boardAngleToGround);
			animationController.SetValue("Manual", p_value);
		}
		Manualling = p_value;
	}

	public void AnimSetPopStrength(float p_value)
	{
		animationController.SetValue("PopStrength", p_value);
	}

	public void AnimSetPush(bool p_value)
	{
		animationController.SetValue("Push", p_value);
	}

	public void AnimSetRollOff(bool p_value)
	{
		animationController.SetValue("RollOff", p_value);
	}

	public void AnimSetScoop(float p_value)
	{
		animationController.SetValue("ScoopAxis", p_value);
	}

	public void AnimSetSetupBlend(float p_value)
	{
		animationController.SetValue("SetupBlend", p_value);
	}

	public void AnimSetSwitch(float p_animSwitch)
	{
		animationController.SetValue("Switch", p_animSwitch);
	}

	public void AnimSetTurn(float p_value)
	{
		float single = Mathd.DampSpring((float)((GetBoardBackwards() ? 1 : -1)) * Instance.boardController.localXAccel / 5.5f - _turnAnimAmount, _turnVel, turnAnimSpring, turnAnimDamp);
		_turnVel += single;
		_turnAnimAmount += _turnVel;
		animationController.SetValue("Turn", (IsAnimSwitch ? -_turnAnimAmount : _turnAnimAmount));
	}

	public void AnimSetupTransition(bool p_value)
	{
		animationController.SetValue("Setup", p_value);
	}

	public void AnimSetWindUp(float p_value)
	{
		animationController.SetValue("WindUp", p_value);
	}

	public void ApplyWeightOnBoard()
	{
		if (boardController.TurnTarget == 0f)
		{
			Vector3 vector3 = boardController.boardRigidbody.velocity;
			boardController.boardRigidbody.AddForce((-skaterController.skaterTransform.up * 80f) * impactBoardDownForce, ForceMode.Force);
			if (boardController.boardRigidbody.velocity.magnitude < vector3.magnitude)
			{
				Rigidbody rigidbody = boardController.boardRigidbody;
				Vector3 vector31 = boardController.boardRigidbody.velocity;
				rigidbody.velocity = vector31.normalized * vector3.magnitude;
			}
		}
	}

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		if (!(_instance != null) || !(_instance != this))
		{
			_instance = this;
			return;
		}
		Destroy(gameObject);
	}

	public Vector3 BoardToTargetVector()
	{
		return boardController.boardTransform.position - boardController.boardTargetPosition.position;
	}

	public void Brake(float p_force)
	{
		Vector3 vector3 = boardController.boardRigidbody.velocity;
		boardController.boardRigidbody.velocity = vector3.normalized * (vector3.magnitude - p_force * Time.deltaTime);
	}

	public void CacheRidingTransforms()
	{
		_lastRidingPosition = skaterController.skaterTransform.position;
		_ridingCameraForward = cameraController._actualCam.forward;
		_ridingPlayerForward = skaterController.skaterTransform.forward;
	}

	public void CameraLookAtPlayer()
	{
		cameraController.LookAtPlayer();
	}

	public void CancelRespawnInvoke()
	{
		CancelInvoke("DoBail");
	}

	private void CanManual()
	{
		playerSM.OnCanManualSM();
	}

	public bool CanNollieOutOfGrind()
	{
		return boardController.triggerManager.canNollie;
	}

	public bool CanOllieOutOfGrind()
	{
		return boardController.triggerManager.canOllie;
	}

	public void CorrectVelocity()
	{
		skaterController.CorrectVelocity();
	}

	public void CrossFadeAnimation(string p_animName, float p_transitionDuration)
	{
		animationController.CrossFadeAnimation(p_animName, p_transitionDuration);
	}

	public void DebugAugmentedAngles(Vector2 p_pos, bool p_isRight)
	{
		Quaternion quaternion;
		if (p_isRight)
		{
			_rightAugScale = rightAugmentedVector.rectTransform.localScale;
			_rightAugScale.y = Mathf.Clamp(p_pos.magnitude, 0f, 1f);
			rightAugmentedVector.rectTransform.localScale = _rightAugScale;
			quaternion = rightAugmentedVector.rectTransform.rotation;
			_rightAugRot = quaternion.eulerAngles;
			_rightAugRot.z = Vector2.SignedAngle(-Vector2.up, p_pos);
			rightAugmentedVector.rectTransform.rotation = Quaternion.Euler(_rightAugRot);
			return;
		}
		_leftAugScale = leftAugmentedVector.rectTransform.localScale;
		_leftAugScale.y = Mathf.Clamp(p_pos.magnitude, 0f, 1f);
		leftAugmentedVector.rectTransform.localScale = _leftAugScale;
		quaternion = leftAugmentedVector.rectTransform.rotation;
		_leftAugRot = quaternion.eulerAngles;
		_leftAugRot.z = Vector2.SignedAngle(-Vector2.up, p_pos);
		leftAugmentedVector.rectTransform.rotation = Quaternion.Euler(_leftAugRot);
	}

	public void DebugPopStick(bool p_canPop, bool p_isRight)
	{
		if (p_isRight)
		{
			rightRawVector.color = (p_canPop ? Color.red : Color.green);
			return;
		}
		leftRawVector.color = (p_canPop ? Color.red : Color.green);
	}

	public void DebugRawAngles(Vector2 p_pos, bool p_isRight)
	{
		Quaternion quaternion;
		if (p_isRight)
		{
			_rightRawScale = rightRawVector.rectTransform.localScale;
			_rightRawScale.y = Mathf.Clamp(p_pos.magnitude, 0f, 1f);
			rightRawVector.rectTransform.localScale = _rightRawScale;
			quaternion = rightRawVector.rectTransform.rotation;
			_rightRawRot = quaternion.eulerAngles;
			_rightRawRot.z = Vector2.SignedAngle(-Vector2.up, p_pos);
			rightRawVector.rectTransform.rotation = Quaternion.Euler(_rightRawRot);
			return;
		}
		_leftRawScale = leftRawVector.rectTransform.localScale;
		_leftRawScale.y = Mathf.Clamp(p_pos.magnitude, 0f, 1f);
		leftRawVector.rectTransform.localScale = _leftRawScale;
		quaternion = leftRawVector.rectTransform.rotation;
		_leftRawRot = quaternion.eulerAngles;
		_leftRawRot.z = Vector2.SignedAngle(-Vector2.up, p_pos);
		leftRawVector.rectTransform.rotation = Quaternion.Euler(_leftRawRot);
	}

	public float DistanceToBoardTarget()
	{
		return Vector3.Distance(boardController.boardTransform.position, boardController.boardTargetPosition.position);
	}

	public void DoBail()
	{
		respawn.DoRespawn();
	}

	public void DoBailDelay()
	{
		Invoke("DoBail", 2.5f);
	}

	private void DoInvert(StickInput p_popStick, ref float r_invertVel, bool p_forwardLoad)
	{
		float pPopStick = invertMult * p_popStick.rawInput.radialVel;
		if (pPopStick > r_invertVel && pPopStick > 0.1f)
		{
			GetBoardBackwards();
			Debug.LogWarning("changed");
			Debug.Log(string.Concat(new object[] { "newraw:", p_popStick.rawInput.radialVel, " new?:", pPopStick, "   firstvel:", boardController.firstVel }));
		}
	}

	public void DoKick(bool p_forwardLoad, float strength)
	{
		float single = (float)((GetBoardBackwards() ? 1 : -1));
		float single1 = (float)((p_forwardLoad ? -1 : 1));
		boardController.RotateWithPop(single * single1 * strength, false);
	}

	private void FixedUpdate()
	{
		playerSM.FixedUpdateSM();
	}

	public void FixTargetNormal()
	{
		boardController.trajectory.PredictedGroundNormal = Vector3.up;
	}

	public void FlipTrickRotation()
	{
		boardController.Rotate(false, true);
	}

	public void ForceBail()
	{
		skaterController.respawn.bail.OnBailed();
		playerSM.OnBailedSM();
	}

	public void ForcePivotForwardRotation(float p_value)
	{
		boardController.ForcePivotForwardRotation(p_value);
	}

	public float GetAngleToAugment(Vector2 p_pos, bool p_isRight)
	{
		if (p_pos.magnitude < 0.1f)
		{
			return 0f;
		}
		return -Vector2.SignedAngle(GetUpVectorToAugment(p_isRight), p_pos);
	}

	public bool GetAnimReleased()
	{
		return animationController.skaterAnim.GetBool("Released");
	}

	public bool GetBoardBackwards()
	{
		return boardController.IsBoardBackwards;
	}

	public float GetBrakeForce()
	{
		return skaterController.breakForce;
	}

	public float GetCoMDisplacement(float p_timeStep)
	{
		return _comDisplacement.GetDisplacement(p_timeStep);
	}

	public float GetDisplacementSum()
	{
		return _comDisplacement.sum;
	}

	public float GetFlipSpeed()
	{
		return boardController.thirdVel;
	}

	public float GetForwardSpeed()
	{
		return boardController.firstVel;
	}

	public Vector3 GetGrindContactPosition()
	{
		return boardController.triggerManager.grindContactSplinePosition.position;
	}

	public Vector3 GetGrindDirection()
	{
		return boardController.triggerManager.grindDirection;
	}

	public Vector3 GetGrindRight()
	{
		return boardController.triggerManager.grindRight;
	}

	public Vector3 GetGrindUp()
	{
		return boardController.triggerManager.grindUp;
	}

	public Vector3 GetGroundNormal()
	{
		return boardController.GroundNormal;
	}

	public float GetKneeBendWeight()
	{
		return ikController.GetKneeBendWeight();
	}

	public float GetLastTweakAxis()
	{
		return _toeTweakAxis / 2f;
	}

	public Vector3 GetLerpedGroundNormal()
	{
		return boardController.LerpedGroundNormal;
	}

	public float GetNollie(bool p_isRight)
	{
		switch (SettingsManager.Instance.controlType)
		{
			case SettingsManager.ControlType.Same:
			{
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					if (p_isRight)
					{
						return 0f;
					}
					return 1f;
				}
				if (p_isRight)
				{
					return 1f;
				}
				return 0f;
			}
			case SettingsManager.ControlType.Swap:
			{
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					if (!Instance.IsSwitch)
					{
						if (p_isRight)
						{
							return 0f;
						}
						return 1f;
					}
					if (p_isRight)
					{
						return 0f;
					}
					return 1f;
				}
				if (!Instance.IsSwitch)
				{
					if (p_isRight)
					{
						return 1f;
					}
					return 0f;
				}
				if (p_isRight)
				{
					return 1f;
				}
				return 0f;
			}
			case SettingsManager.ControlType.Simple:
			{
				if (!Instance.IsSwitch)
				{
					if (p_isRight)
					{
						return 0f;
					}
					return 1f;
				}
				if (p_isRight)
				{
					return 0f;
				}
				return 1f;
			}
		}
		return 0f;
	}

	public bool GetPopped()
	{
		return playerSM.PoppedSM();
	}

	public float GetPopStrength()
	{
		return animationController.skaterAnim.GetFloat("PopStrength");
	}

	public float GetPushForce()
	{
		return skaterController.pushForce;
	}

	public float GetScoopSpeed()
	{
		return boardController.secondVel;
	}

	public Vector2 GetUpVectorToAugment(bool p_isRight)
	{
		switch (SettingsManager.Instance.controlType)
		{
			case SettingsManager.ControlType.Same:
			{
				if (!p_isRight)
				{
					_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? Vector2.up : -Vector2.up);
					break;
				}
				else
				{
					_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -Vector2.up : Vector2.up);
					break;
				}
			}
			case SettingsManager.ControlType.Swap:
			{
				if (Instance.IsSwitch)
				{
					if (!p_isRight)
					{
						_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? Vector2.up : -Vector2.up);
						break;
					}
					else
					{
						_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -Vector2.up : Vector2.up);
						break;
					}
				}
				else if (!p_isRight)
				{
					_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -Vector2.up : Vector2.up);
					break;
				}
				else
				{
					_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? Vector2.up : -Vector2.up);
					break;
				}
			}
			case SettingsManager.ControlType.Simple:
			{
				if (Instance.IsSwitch)
				{
					if (!p_isRight)
					{
						_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? Vector2.up : Vector2.up);
						break;
					}
					else
					{
						_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -Vector2.up : -Vector2.up);
						break;
					}
				}
				else if (!p_isRight)
				{
					_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? Vector2.up : Vector2.up);
					break;
				}
				else
				{
					_tempAugmentedUp = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -Vector2.up : -Vector2.up);
					break;
				}
			}
		}
		return _tempAugmentedUp;
	}

	public float GetWindUp()
	{
		return inputController.GetWindUp();
	}

	public void GrindRotateBoard(float p_horizontal, float p_vertical)
	{
		newRot = (Quaternion.AngleAxis(p_vertical, (!GetBoardBackwards() ? boardController.boardTransform.right : -boardController.boardTransform.right)) * Quaternion.AngleAxis(p_horizontal, skaterController.skaterTransform.up)) * skaterController.skaterTransform.rotation;
		boardOffsetRoot.rotation = newRot;
	}

	public void GrindRotatePlayerHorizontal(float p_value)
	{
		if (!Mathd.Vector3IsInfinityOrNan(GetGrindContactPosition()))
		{
			playerRotationReference.RotateAround(GetGrindContactPosition(), boardController.triggerManager.playerOffset.up, p_value);
		}
	}

	public void Impact()
	{
		SetBoardToMaster();
		Instance.SetTurningMode(InputController.TurningMode.Grounded);
		AnimOllieTransition(false);
		AnimSetupTransition(false);
	}

	public bool IsBacksideGrind()
	{
		bool flag = false;
		float single = Vector3.SignedAngle(Vector3.ProjectOnPlane(skaterController.skaterTransform.position - _lastRidingPosition, GetGrindUp()), GetGrindDirection(), GetGrindUp());
		if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
		{
			if (single >= 0f)
			{
				flag = (IsSwitch ? false : true);
			}
			else
			{
				flag = (!IsSwitch ? false : true);
			}
		}
		else if (single >= 0f)
		{
			flag = (!IsSwitch ? false : true);
		}
		else
		{
			flag = (IsSwitch ? false : true);
		}
		return flag;
	}

	public bool IsCurrentAnimationPlaying(string p_name)
	{
		AnimatorStateInfo currentAnimatorStateInfo = animationController.skaterAnim.GetCurrentAnimatorStateInfo(0);
		return currentAnimatorStateInfo.IsName(p_name);
	}

	public bool IsCurrentGrindMetal()
	{
		if (SoundManager.Instance.deckSounds.grindState != DeckSounds.GrindState.metal)
		{
			return false;
		}
		return true;
	}

	public bool IsGrounded()
	{
		return boardController.Grounded;
	}

	public bool IsRightSideOfGrind()
	{
		if (Vector3.SignedAngle(Vector3.ProjectOnPlane(skaterController.skaterTransform.position - _lastRidingPosition, GetGrindUp()), GetGrindDirection(), GetGrindUp()) > 0f)
		{
			return true;
		}
		return false;
	}

	private void LerpFlipAxis()
	{
		_flipAxis = Mathf.Lerp(_flipAxis, _flipAxisTarget, Time.deltaTime * 15f);
		animationController.SetValue("FlipAxis", _flipAxis);
	}

	public void LimitAngularVelocity(float _maxY)
	{
		boardController.LimitAngularVelocity(_maxY);
	}

	public void LockAngularVelocity(Quaternion p_rot)
	{
		boardController.LockAngularVelocity(p_rot);
	}

	public void LogVelY()
	{
		Debug.LogError(string.Concat("xxxxxxxxxx p_pop aft1: ", skaterController.skaterRigidbody.velocity.y));
	}

	public void ManualRotation(bool p_manual, float p_manualAxis, float p_secondaryAxis, float p_swivel)
	{
		boardController.SetManualAngularVelocity(p_manual, p_manualAxis, p_secondaryAxis, p_swivel);
	}

	public void MoveCameraToPlayer()
	{
		cameraController.MoveCameraToPlayer();
	}

	private void OnDestroy()
	{
		if (playerSM != null)
		{
			playerSM.StopSM();
			playerSM = null;
		}
	}

	public void OnEndImpact()
	{
		playerSM.OnEndImpactSM();
	}

	public void OnEnterSetupState()
	{
		_isInSetupState = true;
	}

	public void OnExitSetupState()
	{
		_isInSetupState = false;
	}

	private void OnFlipStickReset()
	{
	}

	public void OnFlipStickUpdate(ref bool p_p_flipDetected, ref bool p_potentialFlip, ref Vector2 p_initialFlipDir, ref int p_p_flipFrameCount, ref int p_p_flipFrameMax, ref float p_toeAxis, ref float p_p_flipVel, ref float p_popVel, ref float p_popDir, ref float p_flip, StickInput p_flipStick, bool p_releaseBoard, bool p_isSettingUp, ref float p_invertVel, float p_augmentedAngle, bool popRotationDone, bool p_forwardLoad, ref float p_flipWindowTimer)
	{
		float single;
		float single1;
		if (p_p_flipDetected)
		{
			if (p_flip == 0f)
			{
				single = 0f;
			}
			else
			{
				single = (p_flip > 0f ? 1f : -1f);
			}
			AnimSetFlip(single);
			AnimRelease(true);
			if (playerSM.PoppedSM())
			{
				SetLeftIKLerpTarget(1f);
				SetRightIKLerpTarget(1f);
			}
			if (p_toeAxis == 0f)
			{
				single1 = 0f;
			}
			else
			{
				single1 = (p_toeAxis > 0f ? 1f : -1f);
			}
			float single2 = single1;
			float popToeVel = boneMult * p_flipStick.PopToeVel.y;
			float single3 = (float)((p_forwardLoad ? -1 : 1));
			float popToeSpeed = flipMult * p_flipStick.PopToeSpeed;
			if ((Mathf.Sign(p_flipStick.ToeAxisVel) == Mathf.Sign(p_flip) || !playerSM.PoppedSM()) && Mathf.Abs(popToeSpeed) > Mathf.Abs(p_p_flipVel))
			{
				p_p_flipVel = popToeSpeed;
				p_flipWindowTimer = 0f;
			}
			if (Mathf.Abs(p_invertVel) == 0f || Mathf.Sign(popToeVel) == Mathf.Sign(1f) && Mathf.Abs(popToeVel) > Mathf.Abs(p_invertVel))
			{
				p_invertVel = single3 * popToeVel;
			}
			if (0 == 0 && !playerSM.PoppedSM())
			{
				p_flipWindowTimer += Time.deltaTime;
				if (p_flipWindowTimer >= 0.3f)
				{
					p_p_flipVel = 0f;
					p_invertVel = 0f;
					p_p_flipDetected = false;
					AnimRelease(false);
					AnimSetFlip(0f);
					AnimForceFlipValue(0f);
					p_flipWindowTimer = 0f;
				}
			}
			SetFlipSpeed(Mathf.Clamp(p_p_flipVel, -4000f, 4000f) * single2);
		}
		else
		{
			float popToeSpeed1 = flipMult * p_flipStick.PopToeSpeed;
			if (popToeSpeed1 > flipThreshold)
			{
				float single4 = Vector3.Angle(p_flipStick.PopToeVector, Vector2.up);
				if (single4 < 150f && single4 > 15f && p_flipStick.PopToeVector.magnitude > flipStickDeadZone && Vector2.Angle(p_flipStick.PopToeVel, p_flipStick.PopToeVector - Vector2.zero) < 90f)
				{
					p_initialFlipDir = p_flipStick.PopToeVector;
					p_toeAxis = p_flipStick.FlipDir;
					p_flip = p_flipStick.ToeAxis;
					float forwardDir = p_flipStick.ForwardDir;
					if (forwardDir <= 0.2f)
					{
						forwardDir += 0.2f;
					}
					p_popDir = Mathf.Clamp(forwardDir, 0f, 1f);
					p_p_flipVel = popToeSpeed1;
					p_flipWindowTimer = 0f;
					p_p_flipDetected = true;
					playerSM.PoppedSM();
					return;
				}
			}
		}
	}

	public void OnImpact()
	{
		boardController.boardRigidbody.angularVelocity = Vector3.zero;
		boardController.backTruckRigidbody.angularVelocity = Vector3.zero;
		boardController.frontTruckRigidbody.angularVelocity = Vector3.zero;
	}

	public void OnInAir(float p_animTimeToImpact)
	{
		Vector3 vector3 = Vector3.zero;
		float single = boardController.trajectory.CalculateTrajectory(skaterController.skaterTransform.position - (Vector3.up * 0.9765f), skaterController.skaterRigidbody.velocity, 50f, out vector3);
		float single1 = Mathf.Clamp(p_animTimeToImpact / single, 0.01f, 1f);
		animationController.ScaleAnimSpeed(single1);
	}

	public void OnManualEnter(bool rightFirst)
	{
		if (rightFirst)
		{
			playerSM.OnManualEnterSM(inputController.RightStick, inputController.LeftStick);
			return;
		}
		playerSM.OnManualEnterSM(inputController.LeftStick, inputController.RightStick);
	}

	public void OnManualUpdate(StickInput stick)
	{
		bool flag;
		if (stick.HoldingManual)
		{
			UpdateManual(stick.IsRightStick);
			if ((stick.rawInput.pos.magnitude <= 0.9f || stick.rawInput.avgSpeedLastUpdate >= 2f) && (stick.ManualAxis >= 0.1f && stick.ManualAxis <= 0.95f || stick.rawInput.avgSpeedLastUpdate >= 10f))
			{
				stick.ManualFrameCount = 0;
			}
			else
			{
				StickInput manualFrameCount = stick;
				manualFrameCount.ManualFrameCount = manualFrameCount.ManualFrameCount + 1;
				if (stick.ManualFrameCount >= 7)
				{
					playerSM.OnManualExitSM();
					AnimSetManual(false, Mathf.Lerp(AnimGetManualAxis(), 0f, Time.deltaTime * 10f));
					stick.HoldingManual = false;
					stick.ManualFrameCount = 0;
					return;
				}
			}
		}
		else
		{
			if (boardController.Grounded)
			{
				flag = (Mathf.Abs(stick.rawInput.pos.x) >= 0.6f || stick.ManualAxis <= 0.2f || stick.ManualAxis >= 0.85f ? false : stick.rawInput.avgSpeedLastUpdate < 7f);
			}
			else
			{
				flag = (Mathf.Abs(stick.rawInput.pos.x) >= 0.1f || stick.rawInput.pos.magnitude >= 0.85f || stick.ManualAxis <= 0.4f || stick.ManualAxis >= 0.95f ? false : stick.rawInput.avgSpeedLastUpdate < 2f);
			}
			if (!flag)
			{
				stick.ManualFrameCount = 0;
				return;
			}
			StickInput stickInput = stick;
			stickInput.ManualFrameCount = stickInput.ManualFrameCount + 1;
			if (stick.ManualFrameCount >= (boardController.Grounded ? 16 : 2))
			{
				OnManualEnter(stick.IsRightStick);
				stick.HoldingManual = true;
				stick.ManualFrameCount = 0;
				return;
			}
		}
	}

	public void OnNextState()
	{
		playerSM.OnNextStateSM();
	}

	public void OnNoseManualEnter(bool rightFirst)
	{
		if (rightFirst)
		{
			playerSM.OnNoseManualEnterSM(inputController.RightStick, inputController.LeftStick);
			return;
		}
		playerSM.OnNoseManualEnterSM(inputController.LeftStick, inputController.RightStick);
	}

	public void OnNoseManualUpdate(StickInput stick)
	{
		bool flag;
		if (stick.HoldingNoseManual)
		{
			UpdateNoseManual(stick.IsRightStick);
			if ((stick.rawInput.pos.magnitude <= 0.9f || stick.rawInput.avgSpeedLastUpdate >= 2f) && (stick.NoseManualAxis >= 0.1f && stick.NoseManualAxis <= 0.95f || stick.rawInput.avgSpeedLastUpdate >= 10f))
			{
				stick.NoseManualFrameCount = 0;
			}
			else
			{
				StickInput noseManualFrameCount = stick;
				noseManualFrameCount.NoseManualFrameCount = noseManualFrameCount.NoseManualFrameCount + 1;
				if (stick.NoseManualFrameCount >= 7)
				{
					AnimSetNoseManual(false, Instance.AnimGetManualAxis());
					playerSM.OnNoseManualExitSM();
					stick.HoldingNoseManual = false;
					stick.NoseManualFrameCount = 0;
					return;
				}
			}
		}
		else
		{
			if (boardController.Grounded)
			{
				flag = (Mathf.Abs(stick.rawInput.pos.x) >= 0.6f || stick.NoseManualAxis <= 0.2f || stick.NoseManualAxis >= 0.85f ? false : stick.rawInput.avgSpeedLastUpdate < 7f);
			}
			else
			{
				flag = (Mathf.Abs(stick.rawInput.pos.x) >= 0.1f || stick.rawInput.pos.magnitude >= 0.85f || stick.NoseManualAxis <= 0.4f || stick.NoseManualAxis >= 0.95f ? false : stick.rawInput.avgSpeedLastUpdate < 2f);
			}
			if (!flag)
			{
				stick.NoseManualFrameCount = 0;
				return;
			}
			StickInput stickInput = stick;
			stickInput.NoseManualFrameCount = stickInput.NoseManualFrameCount + 1;
			if (stick.NoseManualFrameCount >= (boardController.Grounded ? 16 : 2))
			{
				OnNoseManualEnter(stick.IsRightStick);
				stick.HoldingNoseManual = true;
				stick.NoseManualFrameCount = 0;
				return;
			}
		}
	}

	public void OnPop(float p_pop, float p_scoop)
	{
		VelocityOnPop = boardController.boardRigidbody.velocity;
		Instance.SetTurningMode(InputController.TurningMode.InAir);
		SetSkaterToMaster();
		Vector3 pPop = skaterController.skaterTransform.up * p_pop;
		Vector3 vector3 = skaterController.skaterRigidbody.velocity + pPop;
		Vector3.Angle(cameraController._actualCam.forward, vector3);
		Vector3 vector31 = skaterController.PredictLanding(pPop);
		skaterController.skaterRigidbody.AddForce(pPop, ForceMode.Impulse);
		skaterController.skaterRigidbody.AddForce(vector31, ForceMode.VelocityChange);
		SoundManager.Instance.PlayPopSound(p_scoop);
		comController.popForce = pPop;
	}

	public void OnPop(float p_pop, float p_scoop, Vector3 p_popOutDir)
	{
		VelocityOnPop = boardController.boardRigidbody.velocity;
		Instance.SetTurningMode(InputController.TurningMode.InAir);
		SetSkaterToMaster();
		Vector3 pPopOutDir = (skaterController.skaterTransform.up + p_popOutDir) * p_pop;
		skaterController.PredictLanding(pPopOutDir);
		skaterController.skaterRigidbody.AddForce(pPopOutDir, ForceMode.Impulse);
		SoundManager.Instance.PlayPopSound(p_scoop);
		comController.popForce = pPopOutDir;
	}

	public void OnPopStartCheck(bool p_canPop, StickInput p_popStick, ref PlayerController.SetupDir _setupDir, bool p_forwardLoad, float p_popThreshold, ref float r_invertVel, float p_augmentedAngle, ref float r_popVel)
	{
		if (p_canPop)
		{
			if (p_popStick.AugmentedPopToeSpeed <= p_popThreshold || p_popStick.AugmentedPopToeVel.y >= 0f)
			{
				SetupDirection(p_popStick, ref _setupDir, p_augmentedAngle);
			}
			else
			{
				SetupDirection(p_popStick, ref _setupDir, p_augmentedAngle);
				float single = (float)((p_forwardLoad ? -1 : 1));
				if (Mathf.Abs(p_popStick.AugmentedToeAxisVel) < olliexVelThreshold && p_popStick.AugmentedPopToeVel.y < -5f)
				{
					SetPopValues(false, 0f, 0f, 0f, ref r_popVel);
				}
				if (p_popStick.AugmentedToeAxisVel >= p_popThreshold)
				{
					if ((int)_setupDir != 1)
					{
						Mathf.Clamp(single * shuvMult * p_popStick.AugmentedToeAxisVel, -shuvMax, shuvMax);
						SetPopValues(true, single * 1f, single * shuvMult * p_popStick.AugmentedToeAxisVel, 1f, ref r_popVel);
					}
					else
					{
						SetPopValues(true, doubleShuvMult, single * doubleShuvMult * p_popStick.AugmentedToeAxisVel, 1f, ref r_popVel);
					}
				}
				if (p_popStick.AugmentedToeAxisVel <= -p_popThreshold)
				{
					if ((int)_setupDir == 2)
					{
						SetPopValues(true, -doubleShuvMult, single * doubleShuvMult * p_popStick.AugmentedToeAxisVel, 1f, ref r_popVel);
						return;
					}
					Mathf.Clamp(single * shuvMult * p_popStick.AugmentedToeAxisVel, -shuvMax, shuvMax);
					SetPopValues(true, single * -1f, single * shuvMult * p_popStick.AugmentedToeAxisVel, 1f, ref r_popVel);
					return;
				}
			}
		}
	}

	public void OnPopStickUpdate(float p_threshold, bool p_canPop, StickInput p_popStick, ref float r_popVel, float p_popThreshold, bool p_forwardLoad, ref PlayerController.SetupDir _setupDir, ref float r_invertVel, float p_augmentedAngle)
	{
		if (p_popStick.AugmentedPopToeSpeed > p_popThreshold && p_popStick.AugmentedPopToeVel.y < 0f)
		{
			float single = (float)((p_forwardLoad ? -1 : 1));
			float pPopStick = invertMult * p_popStick.rawInput.radialVel;
			if (pPopStick > r_invertVel && pPopStick > 0.1f)
			{
				GetBoardBackwards();
			}
			float augmentedToeAxisVel = single * shuvMult * p_popStick.AugmentedToeAxisVel;
			if (Mathf.Abs(r_popVel) < 0.1f || Mathf.Sign(augmentedToeAxisVel) == Mathf.Sign(r_popVel) && Mathf.Abs(augmentedToeAxisVel) > Mathf.Abs(r_popVel))
			{
				if (p_popStick.AugmentedToeAxisVel > 0f)
				{
					int num = (int)_setupDir;
				}
				if (p_popStick.AugmentedToeAxisVel < 0f)
				{
					int num1 = (int)_setupDir;
				}
				augmentedToeAxisVel = Mathf.Clamp(augmentedToeAxisVel, -shuvMax, shuvMax);
				SetScoopSpeed(augmentedToeAxisVel);
				r_popVel = augmentedToeAxisVel;
			}
		}
	}

	public void PhysicsRotation(float p_force, float p_damper)
	{
		boardController.PhysicsRotation(p_force, p_damper);
	}

	public Vector3 PlayerForward()
	{
		return skaterController.skaterTransform.forward;
	}

	public void ReduceImpactBounce()
	{
		boardController.ReduceImpactBounce();
	}

	public void RemoveBoardAngularVelocity()
	{
		boardController.boardRigidbody.angularVelocity = Vector3.zero;
	}

	public void RemoveTurnTorque(float p_value, InputController.TurningMode p_turningMode)
	{
		switch (p_turningMode)
		{
			case InputController.TurningMode.Grounded:
			{
				boardController.RemoveTurnTorqueLinear();
				return;
			}
			case InputController.TurningMode.PreWind:
			{
				boardController.RemoveTurnTorque(0.5f);
				return;
			}
			case InputController.TurningMode.InAir:
			{
				skaterController.RemoveTurnTorque(p_value);
				return;
			}
			case InputController.TurningMode.FastLeft:
			{
				skaterController.RemoveTurnTorque(0.95f);
				return;
			}
			case InputController.TurningMode.FastRight:
			{
				skaterController.RemoveTurnTorque(0.95f);
				return;
			}
			case InputController.TurningMode.Manual:
			{
				boardController.RemoveTurnTorqueLinear();
				return;
			}
			default:
			{
				return;
			}
		}
	}

	public void ResetAfterGrinds()
	{
		AnimForceFlipValue(0f);
		AnimForceScoopValue(0f);
		AnimSetFlip(0f);
		AnimSetScoop(0f);
		AnimCaught(false);
	}

	public void ResetAllAnimations()
	{
		animationController.ScaleAnimSpeed(1f);
		AnimCaught(false);
		AnimForceFlipValue(0f);
		AnimForceScoopValue(0f);
		AnimSetFlip(0f);
		AnimSetScoop(0f);
		AnimRelease(false);
	}

	public void ResetAllAnimationsExceptSpeed()
	{
		AnimCaught(false);
		AnimForceFlipValue(0f);
		AnimForceScoopValue(0f);
		AnimSetFlip(0f);
		AnimSetScoop(0f);
		AnimRelease(false);
		AnimSetupTransition(false);
		AnimOllieTransition(false);
	}

	public void ResetAllExceptSetup()
	{
		animationController.ScaleAnimSpeed(1f);
		AnimCaught(false);
		AnimForceFlipValue(0f);
		AnimForceScoopValue(0f);
		AnimSetFlip(0f);
		AnimSetScoop(0f);
		AnimRelease(false);
		AnimOllieTransition(false);
	}

	public void ResetAnimationsAfterImpact()
	{
		animationController.ScaleAnimSpeed(1f);
		AnimCaught(false);
		AnimRelease(false);
		AnimSetupTransition(false);
		AnimOllieTransition(false);
	}

	public void ResetBackTruckCenterOfMass()
	{
		boardController.backTruckRigidbody.ResetCenterOfMass();
	}

	public void ResetBoardCenterOfMass()
	{
		boardController.boardRigidbody.ResetCenterOfMass();
	}

	public void ResetFrontTruckCenterOfMass()
	{
		boardController.frontTruckRigidbody.ResetCenterOfMass();
	}

	public void ResetIKOffsets()
	{
		ikController.ResetIKOffsets();
	}

	public void ResetPIDRotationValues()
	{
		boardController.ResetPIDRotationValues();
	}

	public void RotateToCatchRotation()
	{
		boardController.CatchRotation();
	}

	public void ScaleDisplacementCurve(float p_skaterHeight)
	{
		_comDisplacement.ScaleDisplacementCurve(p_skaterHeight);
	}

	private void SetBackFootAxis(float p_backForwardAxis, float p_backToeAxis)
	{
		_backFootForwardAxis = p_backForwardAxis;
		_backFootToeAxis = p_backToeAxis;
	}

	public void SetBackPivotRotation(float p_frontToeAxis)
	{
		boardController.SetBackPivotRotation(p_frontToeAxis);
	}

	public void SetBackTruckCenterOfMass(Vector3 p_position)
	{
		boardController.backTruckRigidbody.centerOfMass = p_position;
	}

	public void SetBoardBackwards()
	{
		boardController.SetBoardBackwards();
	}

	public void SetBoardCenterOfMass(Vector3 p_position)
	{
		boardController.boardRigidbody.centerOfMass = p_position;
		CenterOfMass.position = boardController.boardTransform.TransformPoint(boardController.boardRigidbody.centerOfMass);
	}

	public void SetBoardTargetPosition(float p_frontMagnitudeMinusBackMagnitude)
	{
		boardController.SetBoardTargetPosition(p_frontMagnitudeMinusBackMagnitude);
	}

	public void SetBoardToMaster()
	{
		movementMaster = PlayerController.MovementMaster.Board;
		skaterController.skaterRigidbody.useGravity = false;
		skaterController.skaterRigidbody.velocity = Vector3.zero;
	}

	public void SetCatchForwardRotation()
	{
		boardController.SetCatchForwardRotation();
	}

	public void SetFlipSpeed(float p_value)
	{
		boardController.thirdVel = (boardController.IsBoardBackwards ? -p_value : p_value) * 1.2f;
	}

	public void SetForwardSpeed(float p_value)
	{
		boardController.firstVel = (boardController.IsBoardBackwards ? -p_value : p_value);
	}

	private void SetFrontFootAxis(float p_frontForwardAxis, float p_frontToeAxis)
	{
		_frontFootForwardAxis = p_frontForwardAxis;
		_frontFootToeAxis = p_frontToeAxis;
	}

	public void SetFrontPivotRotation(float p_backToeAxis)
	{
		boardController.SetFrontPivotRotation(p_backToeAxis);
	}

	public void SetFrontTruckCenterOfMass(Vector3 p_position)
	{
		boardController.frontTruckRigidbody.centerOfMass = p_position;
	}

	public void SetGrindPIDRotationValues()
	{
		boardController.SetGrindPIDRotationValues();
	}

	public void SetGrindTweakAxis(float p_value)
	{
		animationController.SetGrindTweakValue(p_value);
	}

	public void SetIKOnOff(float p_value)
	{
		ikController.OnOffIK(p_value);
	}

	public void SetIKRigidboardKinematicFalse()
	{
		ikController.SetIKRigidbodyKinematic(false);
	}

	public void SetIKRigidbodyKinematic(bool p_value)
	{
		ikController.SetIKRigidbodyKinematic(p_value);
	}

	public void SetIKRigidbodyKinematicNextFrame()
	{
		DHTools.Instance.InvokeNextFrame(new DHTools.Function(SetIKRigidboardKinematicFalse));
	}

	public void SetInAirFootPlacement(float p_toeAxis, float p_forwardAxis, bool p_front)
	{
		if (p_front)
		{
			_frontMagnitude.x = p_toeAxis;
			_frontMagnitude.y = p_forwardAxis;
			animationController.SetValue("FrontToeAxis", p_toeAxis);
			animationController.SetValue("FrontForwardAxis", p_forwardAxis);
			animationController.SetSteezeValue("FrontToeAxis", p_toeAxis);
			animationController.SetSteezeValue("FrontForwardAxis", p_forwardAxis);
			SetFrontFootAxis(p_forwardAxis, p_toeAxis);
			return;
		}
		_backMagnitude.x = p_toeAxis;
		_backMagnitude.y = p_forwardAxis;
		animationController.SetValue("BackToeAxis", p_toeAxis);
		animationController.SetValue("BackForwardAxis", p_forwardAxis);
		animationController.SetSteezeValue("BackToeAxis", p_toeAxis);
		animationController.SetSteezeValue("BackForwardAxis", p_forwardAxis);
		SetBackFootAxis(p_forwardAxis, p_toeAxis);
	}

	public void SetKneeBendWeight(float p_value)
	{
		ikController.SetKneeBendWeight(p_value);
	}

	public void SetLeftIKLerpTarget(float p_value)
	{
		ikController.SetLeftLerpTarget(p_value, p_value);
	}

	public void SetLeftIKLerpTarget(float p_pos, float p_rot)
	{
		ikController.SetLeftLerpTarget(p_pos, p_rot);
	}

	public void SetLeftIKOffset(float p_toeAxis, float p_forwardDir, float p_popDir, bool p_isPopStick, bool p_lockHorizontal, bool p_popping)
	{
		ikController.SetLeftIKOffset(p_toeAxis, p_forwardDir, p_popDir, p_isPopStick, p_lockHorizontal, p_popping);
	}

	public void SetLeftIKRotationWeight(float p_value)
	{
		ikController.SetLeftIKRotationWeight(p_value);
	}

	public void SetLeftIKWeight(float p_value)
	{
		ikController.LeftIKWeight(p_value);
	}

	public void SetLeftSteezeWeight(float p_value)
	{
		ikController.SetLeftSteezeWeight(p_value);
	}

	public void SetManual(bool p_value)
	{
		Manualling = p_value;
	}

	public void SetManualStrength(float p_value)
	{
		animationController.SetValue("ManualStrength", p_value);
	}

	public void SetMaxSteeze(float p_value)
	{
		ikController.SetMaxSteeze(p_value);
	}

	public void SetMaxSteezeLeft(float p_value)
	{
		ikController.SetMaxSteezeLeft(p_value);
	}

	public void SetMaxSteezeRight(float p_value)
	{
		ikController.SetMaxSteezeRight(p_value);
	}

	public void SetPivotForwardRotation(float p_leftForwardAxisPlusRightForwardAxis, float p_speed)
	{
		boardController.SetPivotForwardRotation(p_leftForwardAxisPlusRightForwardAxis, p_speed);
	}

	public void SetPivotSideRotation(float p_leftToeAxisMinusRightToeAxis)
	{
		boardController.SetPivotSideRotation(p_leftToeAxisMinusRightToeAxis);
	}

	public void SetPopValues(bool p_animReleased, float p_animScoopSpeed, float p_scoopSpeed, float p_popValue, ref float r_popVel)
	{
		if (Mathf.Sign(p_animScoopSpeed) != Mathf.Sign(p_scoopSpeed))
		{
			p_animScoopSpeed *= -1f;
		}
		r_popVel = p_scoopSpeed;
		ResetAfterGrinds();
		AnimRelease(p_animReleased);
		AnimSetScoop(p_animScoopSpeed);
		SetTargetToMaster();
		SetScoopSpeed(p_scoopSpeed);
		SetLeftIKLerpTarget(p_popValue);
		SetRightIKLerpTarget(p_popValue);
		playerSM.OnNextStateSM();
	}

	public void SetPuppetMasterMode(BehaviourPuppet.NormalMode p_mode)
	{
		skaterController.SetPuppetMode(p_mode);
	}

	public void SetPushForce(float p_value)
	{
		skaterController.pushForce = p_value;
	}

	public void SetRightIKLerpTarget(float p_value)
	{
		ikController.SetRightLerpTarget(p_value, p_value);
	}

	public void SetRightIKLerpTarget(float p_pos, float p_rot)
	{
		ikController.SetRightLerpTarget(p_pos, p_rot);
	}

	public void SetRightIKOffset(float p_toeAxis, float p_forwardDir, float p_popDir, bool p_isPopStick, bool p_lockHorizontal, bool p_popping)
	{
		ikController.SetRightIKOffset(p_toeAxis, p_forwardDir, p_popDir, p_isPopStick, p_lockHorizontal, p_popping);
	}

	public void SetRightIKRotationWeight(float p_value)
	{
		ikController.SetRightIKRotationWeight(p_value);
	}

	public void SetRightIKWeight(float p_value)
	{
		ikController.RightIKWeight(p_value);
	}

	public void SetRightSteezeWeight(float p_value)
	{
		ikController.SetRightSteezeWeight(p_value);
	}

	public void SetScoopSpeed(float p_value)
	{
		boardController.secondVel = -p_value;
	}

	public void SetSkaterToMaster()
	{
		movementMaster = PlayerController.MovementMaster.Skater;
		skaterController.skaterRigidbody.useGravity = true;
	}

	public void SetTargetToMaster()
	{
		skaterController.skaterRigidbody.angularVelocity = boardController.boardRigidbody.angularVelocity;
		skaterController.skaterRigidbody.velocity = boardController.boardRigidbody.velocity;
		boardController.boardRigidbody.angularVelocity = Vector3.zero;
		movementMaster = PlayerController.MovementMaster.Target;
	}

	public void SetTurningMode(InputController.TurningMode p_turningMode)
	{
		inputController.turningMode = p_turningMode;
	}

	public void SetTurnMultiplier(float p_value)
	{
		inputController.TriggerMultiplier = p_value;
	}

	private void SetTweakAxis()
	{
		float animatorSpeed = 1f / animationController.GetAnimatorSpeed();
		_forwardTweakAxis = Mathf.MoveTowards(_forwardTweakAxis, _frontFootForwardAxis + _backFootForwardAxis, Time.deltaTime * animatorSpeed * 10f);
		_toeTweakAxis = Mathf.MoveTowards(_toeTweakAxis, _frontFootToeAxis + -_backFootToeAxis, Time.deltaTime * animatorSpeed * 10f);
		animationController.SetTweakValues(Mathf.Clamp(_forwardTweakAxis / 2f, -1f, 1f), _toeTweakAxis / 2f);
		animationController.SetTweakMagnitude(_frontMagnitude.magnitude, _backMagnitude.magnitude);
	}

	public void SetupDirection(StickInput p_popStick, ref PlayerController.SetupDir _setupDir)
	{
		_stickAngle = Vector2.SignedAngle(p_popStick.AugmentedPopToeVector, Vector2.up);
		if ((int)_setupDir == 0)
		{
			if (_stickAngle < -4f)
			{
				_setupDir = PlayerController.SetupDir.Left;
				return;
			}
			if (_stickAngle > 4f)
			{
				_setupDir = PlayerController.SetupDir.Right;
			}
		}
	}

	public void SetupDirection(StickInput p_popStick, ref PlayerController.SetupDir _setupDir, float p_augmentedAngle)
	{
		if (p_augmentedAngle == 0f)
		{
			_stickAngle = Vector2.SignedAngle(p_popStick.AugmentedPopToeVector, Vector2.up);
			if ((int)_setupDir == 0)
			{
				if (_stickAngle < -4f)
				{
					_setupDir = PlayerController.SetupDir.Left;
					return;
				}
				if (_stickAngle > 4f)
				{
					_setupDir = PlayerController.SetupDir.Right;
				}
			}
		}
	}

	public void SkaterRotation(bool p_canRotate, bool p_manualling)
	{
		skaterController.UpdateSkaterRotation(p_canRotate, p_manualling);
	}

	public void SkaterRotation(Quaternion p_rot)
	{
		skaterController.UpdateSkaterRotation(true, p_rot);
		skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
	}

	public void SnapRotation()
	{
		boardController.SnapRotation();
	}

	public void SnapRotation(bool p_value)
	{
		boardController.SnapRotation(p_value);
	}

	public void SnapRotation(float p_value)
	{
		boardController.SnapRotation(p_value);
	}

	private void Start()
	{
		playerSM = new PlayerStateMachine(gameObject);
		playerSM.StartSM();
	}

	public void TurnLeft(float p_value, InputController.TurningMode p_turningMode)
	{
		switch (p_turningMode)
		{
			case InputController.TurningMode.Grounded:
			{
				boardController.AddTurnTorque(-p_value);
				skaterController.AddTurnTorque(-p_value * torsoTorqueMult);
				return;
			}
			case InputController.TurningMode.PreWind:
			{
				boardController.AddTurnTorque(-(p_value / 5f));
				return;
			}
			case InputController.TurningMode.InAir:
			{
				skaterController.AddTurnTorque(-p_value);
				return;
			}
			case InputController.TurningMode.FastLeft:
			{
				if (SettingsManager.Instance.stance != SettingsManager.Stance.Regular)
				{
					skaterController.AddTurnTorque(-p_value);
					return;
				}
				skaterController.AddTurnTorque(-p_value, true);
				return;
			}
			case InputController.TurningMode.FastRight:
			{
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					skaterController.AddTurnTorque(-p_value);
					return;
				}
				skaterController.AddTurnTorque(-p_value, true);
				return;
			}
			case InputController.TurningMode.Manual:
			{
				boardController.AddTurnTorqueManuals(-p_value);
				return;
			}
			default:
			{
				return;
			}
		}
	}

	public void TurnRight(float p_value, InputController.TurningMode p_turningMode)
	{
		switch (p_turningMode)
		{
			case InputController.TurningMode.Grounded:
			{
				boardController.AddTurnTorque(p_value);
				skaterController.AddTurnTorque(p_value * torsoTorqueMult);
				return;
			}
			case InputController.TurningMode.PreWind:
			{
				boardController.AddTurnTorque(p_value / 5f);
				return;
			}
			case InputController.TurningMode.InAir:
			{
				skaterController.AddTurnTorque(p_value);
				return;
			}
			case InputController.TurningMode.FastLeft:
			{
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					skaterController.AddTurnTorque(p_value);
					return;
				}
				skaterController.AddTurnTorque(p_value, true);
				return;
			}
			case InputController.TurningMode.FastRight:
			{
				if (SettingsManager.Instance.stance != SettingsManager.Stance.Regular)
				{
					skaterController.AddTurnTorque(p_value);
					return;
				}
				skaterController.AddTurnTorque(p_value, true);
				return;
			}
			case InputController.TurningMode.Manual:
			{
				boardController.AddTurnTorqueManuals(p_value);
				return;
			}
			default:
			{
				return;
			}
		}
	}

	public bool TwoWheelsDown()
	{
		return boardController.TwoDown;
	}

	private void Update()
	{
		playerSM.UpdateSM();
		if (_flipAxisTarget != 0f)
		{
			LerpFlipAxis();
		}
		SetTweakAxis();
	}

	public void UpdateBoardPosition()
	{
		boardController.UpdateBoardPosition();
	}

	public void UpdateManual(bool rightFirst)
	{
		if (rightFirst)
		{
			playerSM.OnManualUpdateSM(inputController.RightStick, inputController.LeftStick);
			return;
		}
		playerSM.OnManualUpdateSM(inputController.LeftStick, inputController.RightStick);
	}

	public void UpdateNoseManual(bool rightFirst)
	{
		if (rightFirst)
		{
			playerSM.OnNoseManualUpdateSM(inputController.RightStick, inputController.LeftStick);
			return;
		}
		playerSM.OnNoseManualUpdateSM(inputController.LeftStick, inputController.RightStick);
	}

	public void UpdateSkaterDuringPop()
	{
		skaterController.UpdatePositionDuringPop();
	}

	public void UpdateSkaterPosition()
	{
		skaterController.UpdatePositions();
	}

	public enum MovementMaster
	{
		Board,
		Target,
		Skater
	}

	public enum SetupDir
	{
		Middle,
		Left,
		Right
	}
}