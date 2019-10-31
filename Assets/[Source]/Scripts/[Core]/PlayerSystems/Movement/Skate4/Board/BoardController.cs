using FSMHelper;
using System;
using UnityEngine;
using VacuumBreather;

public class BoardController : MonoBehaviour
{
	public Trajectory trajectory;

	public TriggerManager triggerManager;

	[SerializeField]
	private Transform _wheel1;

	[SerializeField]
	private Transform _wheel2;

	[SerializeField]
	private Transform _wheel3;

	[SerializeField]
	private Transform _wheel4;

	[SerializeField]
	private LayerMask _layers;

	[SerializeField]
	private RaycastHit _hit;

	public float groundNormalLerpSpeed = 20f;

	public float grindForce = 500f;

	public float grindDamp = 300f;

	public float manualForce = 50f;

	public float manualDamp = 10f;

	public float minManualAngle = 10f;

	public float maxManualAngle = 20f;

	public string grindTag;

	public bool isSliding;

	public AnimationCurve triggerCurve;

	private bool removingTorque;

	private float _groundY;

	private float _turnTarget;

	private Vector3 _groundNormal = Vector3.up;

	private Vector3 _lerpedGroundNormal = Vector3.up;

	private Vector3 _lastGroundNormal = Vector3.up;

	[SerializeField]
	private bool _isBoardBackwards;

	[SerializeField]
	private bool _released;

	private bool _grounded;

	private bool _allDown;

	private bool _twoDown;

	private bool[] _wheelsDown = new bool[4];

	[Header("Transforms")]
	public Transform boardTransform;

	public Transform leanPressurePointLeft;

	public Transform leanPressurePointRight;

	public Transform boardMesh;

	[SerializeField]
	public Transform boardControlTransform;

	[SerializeField]
	private Transform _boardTargetRotation;

	[SerializeField]
	private Transform _backwardsTargetRotation;

	public Transform boardTargetPosition;

	[SerializeField]
	private Transform _frontPivot;

	[SerializeField]
	private Transform _frontPivotCenter;

	[SerializeField]
	private Transform _backPivot;

	[SerializeField]
	private Transform _backPivotCenter;

	[SerializeField]
	private Transform _rightPivotRotationTarget;

	[SerializeField]
	private Transform _leftPivotRotationTarget;

	[SerializeField]
	private Transform _sidePivotRotation;

	[SerializeField]
	private Transform _forwardPivotRotationTarget;

	[SerializeField]
	private Transform _backwardPivotRotationTarget;

	[SerializeField]
	private Transform _catchForwardRotation;

	public Transform frontTruckCoM;

	public Transform backTruckCoM;

	[Header("Rigidbodies")]
	public Rigidbody boardRigidbody;

	public Rigidbody frontTruckRigidbody;

	public Rigidbody backTruckRigidbody;

	[Header("Truck Joints")]
	public ConfigurableJoint backTruckJoint;

	public ConfigurableJoint frontTruckJoint;

	public float popForce;

	private Vector3 _lastPos = Vector3.zero;

	private float _angle;

	private Vector3 _axis = Vector3.zero;

	private Vector3 _positionDelta = Vector3.zero;

	private Quaternion _rotationDelta = Quaternion.identity;

	public Quaternion currentRotationTarget;

	private float _firstVel;

	public float secondVel;

	public float thirdVel;

	private float _thirdVel;

	private float _firstDelta;

	private float _secondDelta;

	private float _thirdDelta;

	private Quaternion _bufferedRotation;

	private Quaternion _rotDeltaThisFrame;

	private float _bufferedFlip;

	private float _bufferedBodyRot;

	private float _rollSoundSpeed;

	private bool _bearingSoundSet;

	private Vector3 _lastInAirVelocity = Vector3.zero;

	public float wheelBase;

	public float boardLean;

	public float maxBoardLean;

	private float _theta;

	public float _targetPosition;

	private float _frontPivotLerp;

	private float _backPivotLerp;

	private float _sidePivotLerp;

	private float _forwardPivotLerp;

	private readonly PidController _pidControllerx = new PidController(8f, 0f, 0.05f);

	private readonly PidController _pidControllery = new PidController(8f, 0f, 0.05f);

	private readonly PidController _pidControllerz = new PidController(8f, 0f, 0.05f);

	public float Kp;

	public float Ki;

	public float Kd;

	[SerializeField]
	private float _catchUpRotateSpeed = 35f;

	[SerializeField]
	private float _catchRotateSpeed = 5f;

	public float _catchSignedAngle;

	private float _mag;

	private float _tempRotateSpeed;

	private float _tempUpAngle;

	private readonly PidQuaternionController _pidRotController = new PidQuaternionController(8f, 0f, 0.05f);

	public float KRp;

	public float KRi;

	public float KRd;

	public float grindKRp;

	public float grindKRi;

	public float grindKRd;

	private float _originalKRp;

	private float _originalKRi;

	private float _originalKRd;

	public float rotateSpeed = 90f;

	private Vector3 _angularTarget;

	private Vector3 _angVelTarget;

	public float maxAngularVel;

	public float onBoardMaxRollAngle;

	public Vector3 boardUpAtPopBegin;

	public float angVelMult;

	public float popBoardVelAdd;

	private float _lastuplift;

	public float popSpeed;

	private Vector3 _lastVel;

	private Vector3 _acceleration;

	public float absLocalXAccel => Mathf.Abs(boardTransform.InverseTransformDirection(acceleration).x);

	public Vector3 acceleration
	{
		get
		{
			Vector3 vector3 = boardTransform.InverseTransformDirection(_acceleration);
			vector3 = new Vector3(vector3.x * 0.25f, vector3.y, vector3.z);
			vector3 = boardTransform.TransformDirection(vector3);
			return vector3;
		}
	}

	public bool AllDown => _allDown;

	public bool AnyAxleOffGround
	{
		get
		{
			if (!_wheelsDown[0] && !_wheelsDown[1])
			{
				return true;
			}
			if (_wheelsDown[2])
			{
				return false;
			}
			return !_wheelsDown[3];
		}
	}

	public float firstVel
	{
		get => _firstVel;
		set => _firstVel = value;
	}

	public bool Grounded => _grounded;

	public Vector3 GroundNormal
	{
		get => _groundNormal;
		set => _groundNormal = value;
	}

	public float GroundY
	{
		get => _groundY;
		set => _groundY = value;
	}

	public bool IsBoardBackwards
	{
		get => _isBoardBackwards;
		set => _isBoardBackwards = value;
	}

	public Vector3 LastGroundNormal => _lastGroundNormal;

	public Vector3 LerpedGroundNormal
	{
		get => _lerpedGroundNormal;
		set => _lerpedGroundNormal = value;
	}

	public float localXAccel => boardTransform.InverseTransformDirection(acceleration).x;

	public float localXVel => boardTransform.InverseTransformDirection(boardRigidbody.velocity).x;

	public float TurnTarget
	{
		get => _turnTarget;
		set => _turnTarget = value;
	}

	public bool TwoDown => _twoDown;

	public float xacceleration => boardTransform.InverseTransformDirection(_acceleration).x;

	public float xzRot
	{
		get
		{
			float single = Mathf.Abs(Mathd.AngleBetween(0f, boardTransform.localEulerAngles.x));
			float single1 = Mathf.Abs(Mathd.AngleBetween(0f, boardTransform.localEulerAngles.z));
			return Mathf.Sqrt(single * single + single1 * single1);
		}
	}

	public float yAngVel => boardRigidbody.angularVelocity.y;

	public BoardController()
	{
	}

	public void AddPushForce(float p_value)
	{
		if (boardRigidbody.velocity.magnitude < PlayerController.Instance.topSpeed)
		{
			if (boardRigidbody.velocity.magnitude >= 0.15f)
			{
				Rigidbody rigidbody = boardRigidbody;
				Vector3 vector3 = boardRigidbody.velocity;
				rigidbody.AddForce(vector3.normalized * p_value, ForceMode.Impulse);
			}
			else if (Vector3.Angle(PlayerController.Instance.PlayerForward(), Camera.main.transform.forward) >= 90f)
			{
				boardRigidbody.AddForce((-PlayerController.Instance.PlayerForward() * p_value) * 1.4f, ForceMode.Impulse);
			}
			else
			{
				boardRigidbody.AddForce((PlayerController.Instance.PlayerForward() * p_value) * 1.4f, ForceMode.Impulse);
			}
		}
		SoundManager.Instance.PlayPushOff(0.01f);
	}

	private void AddTorqueRotation(Quaternion p_targetRot)
	{
		Mathd.DampTorqueTowards(boardRigidbody, boardRigidbody.rotation, p_targetRot, grindForce, grindDamp);
	}

	public void AddTurnTorque(float p_value)
	{
		TurnTarget = p_value;
		removingTorque = false;
		Vector3 vector3 = boardTransform.InverseTransformDirection(boardRigidbody.angularVelocity);
		vector3.y = Mathf.MoveTowards(vector3.y, 3f * p_value, Time.deltaTime * 16f);
		boardRigidbody.angularVelocity = boardTransform.TransformDirection(vector3);
	}

	public void AddTurnTorqueManuals(float p_value)
	{
		boardRigidbody.AddTorque(((boardTransform.up * p_value) * 5f) * Time.deltaTime, ForceMode.VelocityChange);
	}

	public void ApplyFriction()
	{
		if (_allDown)
		{
			Vector3 vector3 = boardTransform.InverseTransformDirection(boardRigidbody.angularVelocity);
			ref float singlePointer = ref vector3.z;
			singlePointer = singlePointer * (0.9f * Time.deltaTime * 120f);
			ref float singlePointer1 = ref vector3.x;
			singlePointer1 = singlePointer1 * (0.9f * Time.deltaTime * 120f);
			boardRigidbody.angularVelocity = boardTransform.TransformDirection(vector3);
		}
		Vector3 vector31 = boardTransform.InverseTransformDirection(boardRigidbody.velocity);
		vector31.x *= 0.3f;
		vector31.z *= 0.999f;
		boardRigidbody.velocity = boardTransform.TransformDirection(vector31);
	}

	public void ApplyOnBoardMaxRoll()
	{
		if (!Grounded)
		{
			if (Vector3.Angle(boardRigidbody.transform.up, PlayerController.Instance.skaterController.transform.up) > onBoardMaxRollAngle)
			{
				PIDRotation(currentRotationTarget);
			}
			boardRigidbody.angularVelocity = Vector3.Lerp(boardRigidbody.angularVelocity, Vector3.zero, Time.deltaTime * 50f);
		}
	}

	public void AutoCatchRotation()
	{
		CatchRotation();
	}

	private void Awake()
	{
		_lastPos = boardRigidbody.position;
		boardRigidbody.maxAngularVelocity = 150f;
		backTruckRigidbody.maxAngularVelocity = 150f;
		frontTruckRigidbody.maxAngularVelocity = 150f;
		boardRigidbody.maxDepenetrationVelocity = 1f;
		backTruckRigidbody.maxDepenetrationVelocity = 1f;
		frontTruckRigidbody.maxDepenetrationVelocity = 1f;
		boardRigidbody.solverIterations = 10;
		backTruckRigidbody.solverIterations = 10;
		frontTruckRigidbody.solverIterations = 10;
		if (trajectory == null)
		{
			trajectory = GetComponent<Trajectory>();
		}
		_originalKRp = KRp;
		_originalKRi = KRi;
		_originalKRd = KRd;
	}

	public void CacheBoardUp()
	{
		boardUpAtPopBegin = boardTransform.up;
	}

	public void CatchRotation()
	{
		Vector3 vector3 = Vector3.ProjectOnPlane(Vector3.up, boardRigidbody.transform.forward);
		_catchSignedAngle = Vector3.SignedAngle(vector3, PlayerController.Instance.skaterController.skaterTransform.up, PlayerController.Instance.skaterController.skaterTransform.right);
		PlayerController.Instance.AnimSetCatchAngle(_catchSignedAngle);
		Quaternion quaternion = Quaternion.LookRotation(_catchForwardRotation.forward, vector3);
		Quaternion.Slerp(boardRigidbody.rotation, quaternion, Time.fixedDeltaTime * _catchUpRotateSpeed);
		PIDRotation(Quaternion.Slerp(quaternion, currentRotationTarget, Time.fixedDeltaTime * _catchRotateSpeed));
	}

	public void CatchRotation(float p_mag)
	{
		_mag = Mathf.MoveTowards(_mag, p_mag, Time.deltaTime * 10f);
		Vector3 vector3 = Vector3.ProjectOnPlane(Vector3.up, boardRigidbody.transform.forward);
		_catchSignedAngle = Vector3.SignedAngle(vector3, PlayerController.Instance.skaterController.skaterTransform.up, PlayerController.Instance.skaterController.skaterTransform.right);
		PlayerController.Instance.AnimSetCatchAngle(_catchSignedAngle);
		_tempUpAngle = Vector3.Angle(boardTransform.up, PlayerController.Instance.skaterController.skaterTransform.up);
		Quaternion quaternion = Quaternion.LookRotation(_catchForwardRotation.forward, vector3);
		Quaternion quaternion1 = Quaternion.Slerp(Quaternion.Slerp(boardRigidbody.rotation, quaternion, Time.fixedDeltaTime * _catchUpRotateSpeed), currentRotationTarget, Time.fixedDeltaTime * (_tempUpAngle <= 35f || _tempUpAngle >= 90f ? _catchRotateSpeed : _catchUpRotateSpeed));
		PIDRotation(Quaternion.Slerp(quaternion1, currentRotationTarget, _mag));
	}

	public void DoBoardLean()
	{
		float single = -Mathf.Sign(Vector3.Dot(boardMesh.forward, boardRigidbody.velocity));
		float single1 = wheelBase * boardTransform.InverseTransformDirection(acceleration).x / boardTransform.InverseTransformDirection(boardRigidbody.velocity).z;
		single1 = Mathf.Clamp(single1, -1f, 1f);
		if (Mathf.Abs(boardTransform.InverseTransformDirection(boardRigidbody.velocity).z) <= 0.1f)
		{
			_theta = 0f;
		}
		else
		{
			_theta = single * 57.29578f * Mathf.Asin(single1);
		}
		_theta = Mathf.Clamp(_theta, -maxBoardLean, maxBoardLean);
	}

	private void FixedUpdate()
	{
		SetBoardControlPosition();
		if (Time.deltaTime != 0f)
		{
			_acceleration = (boardRigidbody.velocity - _lastVel) / Time.deltaTime;
		}
		_lastVel = boardRigidbody.velocity;
		SetRotationTarget();
		triggerManager.GrindTriggerCheck();
		_lastGroundNormal = GroundNormal;
		LerpedGroundNormal = Vector3.Lerp(LerpedGroundNormal, GroundNormal, Time.fixedDeltaTime * groundNormalLerpSpeed);
		_grounded = GroundCheck();
		if (PlayerController.Instance.movementMaster == PlayerController.MovementMaster.Board && _grounded && !triggerManager.IsColliding)
		{
			ApplyFriction();
		}
		_lastPos = boardRigidbody.position;
	}

	public void ForcePivotForwardRotation(float p_value)
	{
		p_value *= 0.25f;
		p_value += 0.5f;
		_forwardPivotLerp = p_value;
	}

	public int GetGrindSoundInt()
	{
		int num = 0;
		string str = grindTag;
		if (str == "Concrete")
		{
			num = 0;
		}
		else if (str == "Wood")
		{
			num = 1;
		}
		else if (str == "Metal")
		{
			num = 2;
		}
		return num;
	}

	private Vector3 GetTargetForward()
	{
		if (IsBoardBackwards)
		{
			return _backwardsTargetRotation.forward;
		}
		return _boardTargetRotation.forward;
	}

	public bool GroundCheck()
	{
		if (!Physics.Raycast(_wheel1.position, -_wheel1.up, out _hit, 0.05f, _layers))
		{
			_wheelsDown[0] = false;
		}
		else
		{
			PlayerController.Instance.boardController.GroundY = _hit.point.y;
			_groundNormal = _hit.normal;
			_wheelsDown[0] = true;
		}
		if (!Physics.Raycast(_wheel2.position, -_wheel2.up, out _hit, 0.05f, _layers))
		{
			_wheelsDown[1] = false;
		}
		else
		{
			PlayerController.Instance.boardController.GroundY = _hit.point.y;
			_groundNormal = _hit.normal;
			_wheelsDown[1] = true;
		}
		if (!Physics.Raycast(_wheel3.position, -_wheel3.up, out _hit, 0.05f, _layers))
		{
			_wheelsDown[2] = false;
		}
		else
		{
			PlayerController.Instance.boardController.GroundY = _hit.point.y;
			_groundNormal = _hit.normal;
			_wheelsDown[2] = true;
		}
		if (!Physics.Raycast(_wheel4.position, -_wheel4.up, out _hit, 0.05f, _layers))
		{
			_wheelsDown[3] = false;
		}
		else
		{
			PlayerController.Instance.boardController.GroundY = _hit.point.y;
			_groundNormal = _hit.normal;
			_wheelsDown[3] = true;
		}
		if (!_wheelsDown[0] || !_wheelsDown[1] || !_wheelsDown[2] || !_wheelsDown[3])
		{
			_allDown = false;
		}
		else
		{
			if (!_allDown)
			{
				PlayerController.Instance.playerSM.OnAllWheelsDownSM();
			}
			_allDown = true;
		}
		if ((!_wheelsDown[0] || !_wheelsDown[1]) && (!_wheelsDown[2] || !_wheelsDown[3]))
		{
			_twoDown = false;
		}
		else
		{
			_twoDown = true;
		}
		PlayerController.Instance.AnimSetAllDown(_allDown);
		if (!_wheelsDown[0] && !_wheelsDown[1] && !_wheelsDown[2] && !_wheelsDown[3])
		{
			if (_grounded)
			{
				PlayerController.Instance.animationController.SetValue("Grounded", false);
				PlayerController.Instance.playerSM.OnWheelsLeftGroundSM();
			}
			return false;
		}
		if (!_grounded)
		{
			PlayerController.Instance.animationController.SetValue("Grounded", true);
			PlayerController.Instance.playerSM.OnFirstWheelDownSM();
			if (boardRigidbody.velocity.y < 0f)
			{
				SoundManager instance = SoundManager.Instance;
				Vector3 vector3 = Vector3.ProjectOnPlane(_lastInAirVelocity, Vector3.ProjectOnPlane(_lastInAirVelocity, Vector3.up));
				instance.PlayLandingSound(vector3.magnitude);
			}
		}
		return true;
	}

	public void LeaveFlipMode()
	{
		boardRigidbody.angularVelocity = Mathd.GlobalAngularVelocityFromLocal(boardRigidbody, new Vector3(firstVel * angVelMult, secondVel * angVelMult, 0f));
	}

	public void LimitAngularVelocity(float _maxY)
	{
		Vector3 vector3 = boardTransform.InverseTransformDirection(boardRigidbody.angularVelocity);
		vector3.y = Mathf.Clamp(vector3.y, -_maxY, _maxY);
		boardRigidbody.angularVelocity = boardTransform.TransformDirection(vector3);
		Vector3 vector31 = frontTruckRigidbody.transform.InverseTransformDirection(frontTruckRigidbody.angularVelocity);
		vector31.y = Mathf.Clamp(vector31.y, -_maxY, _maxY);
		frontTruckRigidbody.angularVelocity = frontTruckRigidbody.transform.TransformDirection(vector31);
		Vector3 vector32 = backTruckRigidbody.transform.InverseTransformDirection(backTruckRigidbody.angularVelocity);
		vector32.y = Mathf.Clamp(vector32.y, -_maxY, _maxY);
		backTruckRigidbody.angularVelocity = backTruckRigidbody.transform.TransformDirection(vector32);
	}

	public void LockAngularVelocity(Quaternion p_rot)
	{
		AddTorqueRotation(p_rot);
	}

	private void ManageCapsuleCollider()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		PlayerController.Instance.playerSM.OnCollisionEnterEventSM();
	}

	private void OnCollisionExit(Collision collision)
	{
		PlayerController.Instance.playerSM.OnCollisionExitEventSM();
	}

	private void OnCollisionStay(Collision collision)
	{
		PlayerController.Instance.playerSM.OnCollisionStayEventSM();
	}

	private void PhysicsPosition()
	{
		_positionDelta = boardTargetPosition.position - boardRigidbody.position;
		Vector3 vector3 = (_positionDelta * 4000f) * Time.fixedDeltaTime;
		if (!float.IsNaN(vector3.x) && !float.IsNaN(vector3.y) && !float.IsNaN(vector3.z))
		{
			boardRigidbody.velocity = vector3;
		}
	}

	public void PhysicsRotation(float p_force, float p_damper)
	{
		PIDRotation(currentRotationTarget);
	}

	private void PIDPosition()
	{
		_pidControllerx.Kp = Kp;
		_pidControllerx.Ki = Ki;
		_pidControllerx.Kd = Kd;
		_pidControllery.Kp = Kp;
		_pidControllery.Ki = Ki;
		_pidControllery.Kd = Kd;
		_pidControllerz.Kp = Kp;
		_pidControllerz.Ki = Ki;
		_pidControllerz.Kd = Kd;
		Vector3 boardTargetVel = -(boardRigidbody.velocity - PlayerController.Instance.skaterController.BoardTargetVel) * Time.deltaTime;
		Vector3 vector3 = -(boardRigidbody.position - boardTargetPosition.position);
		Vector3 vector31 = new Vector3(_pidControllerx.ComputeOutput(vector3.x, boardTargetVel.x, Time.deltaTime), _pidControllery.ComputeOutput(vector3.y, boardTargetVel.y, Time.deltaTime), _pidControllerz.ComputeOutput(vector3.z, boardTargetVel.z, Time.deltaTime));
		if (Mathd.Vector3IsInfinityOrNan(vector31))
		{
			Debug.LogError("nan found in PID");
			return;
		}
		boardRigidbody.AddForce(vector31, ForceMode.Acceleration);
	}

	private void PIDRotation(Quaternion p_targetRot)
	{
		_pidRotController.Kp = KRp;
		_pidRotController.Ki = KRi;
		_pidRotController.Kd = KRd;
		Vector3 vector3 = _pidRotController.ComputeRequiredAngularAcceleration(boardRigidbody.transform.rotation, p_targetRot, boardRigidbody.angularVelocity, Time.deltaTime);
		Debug.DrawRay(transform.position, currentRotationTarget * Vector3.forward, Color.yellow);
		Debug.DrawRay(transform.position, boardRigidbody.transform.rotation * Vector3.forward, Color.green);
		boardRigidbody.AddTorque(vector3, ForceMode.Acceleration);
	}

	private void PredictCollision()
	{
		RaycastHit raycastHit;
		if (boardRigidbody.SweepTest(boardRigidbody.velocity.normalized, out raycastHit, (boardRigidbody.position - _lastPos).magnitude * 2.5f))
		{
			PlayerController.Instance.playerSM.OnPredictedCollisionEventSM();
		}
	}

	private void ProcessSounds()
	{
		if (_grounded)
		{
			_rollSoundSpeed = boardRigidbody.velocity.magnitude;
			SoundManager.Instance.SetRollingVolumeFromRPS(boardTransform.GetComponent<PhysicMaterial>(), _rollSoundSpeed);
			if (!_bearingSoundSet && _allDown)
			{
				SoundManager.Instance.StopBearingSound();
				_bearingSoundSet = true;
			}
		}
		if (!_allDown)
		{
			_rollSoundSpeed = Mathf.Lerp(_rollSoundSpeed, 0f, Time.deltaTime * 10f);
			SoundManager.Instance.SetRollingVolumeFromRPS(boardTransform.GetComponent<PhysicMaterial>(), _rollSoundSpeed);
			if (_bearingSoundSet)
			{
				if (_grounded)
				{
					_rollSoundSpeed *= 0.5f;
				}
				SoundManager.Instance.StartBearingSound(_rollSoundSpeed);
				_bearingSoundSet = false;
			}
		}
		if (!Grounded)
		{
			_lastInAirVelocity = boardRigidbody.velocity;
		}
	}

	public void ReduceImpactBounce()
	{
		if (removingTorque)
		{
			LimitAngularVelocity(0f);
		}
		else
		{
			LimitAngularVelocity(5f);
		}
		bool grounded = Grounded;
	}

	public void ReferenceBoardRotation()
	{
		_bufferedFlip = 0f;
	}

	public void RemoveTurnTorque(float p_value)
	{
		TurnTarget = 0f;
		Vector3 vector3 = boardTransform.InverseTransformDirection(boardRigidbody.angularVelocity);
		ref float pValue = ref vector3.y;
		pValue = pValue * (p_value * Time.deltaTime * 60f);
		boardRigidbody.angularVelocity = boardTransform.TransformDirection(vector3);
	}

	public void RemoveTurnTorqueLinear()
	{
		TurnTarget = 0f;
		removingTorque = true;
		Vector3 vector3 = boardTransform.InverseTransformDirection(boardRigidbody.angularVelocity);
		vector3.y = Mathf.MoveTowards(vector3.y, 0f, Time.deltaTime * 80f);
		boardRigidbody.angularVelocity = boardTransform.TransformDirection(vector3);
	}

	public void ResetAll()
	{
		firstVel = 0f;
		secondVel = 0f;
		thirdVel = 0f;
		_firstDelta = 0f;
		_secondDelta = 0f;
		_thirdDelta = 0f;
		_rotDeltaThisFrame = Quaternion.identity;
		_bufferedFlip = 0f;
		_bufferedBodyRot = 0f;
		_lastuplift = 0f;
	}

	public void ResetPIDRotationValues()
	{
		KRp = _originalKRp;
		KRd = _originalKRd;
		KRi = _originalKRi;
	}

	public void ResetTweakValues()
	{
		_targetPosition = 0.5f;
		_frontPivotLerp = 0.5f;
		_backPivotLerp = 0.5f;
		_sidePivotLerp = 0.5f;
		_forwardPivotLerp = 0.5f;
		_frontPivot.rotation = Quaternion.Slerp(_leftPivotRotationTarget.rotation, _rightPivotRotationTarget.rotation, _frontPivotLerp);
		_backPivot.rotation = Quaternion.Slerp(_leftPivotRotationTarget.rotation, _rightPivotRotationTarget.rotation, _backPivotLerp);
		_sidePivotRotation.rotation = Quaternion.Slerp(_leftPivotRotationTarget.rotation, _rightPivotRotationTarget.rotation, _sidePivotLerp);
		_boardTargetRotation.rotation = Quaternion.Slerp(_backwardPivotRotationTarget.rotation, _forwardPivotRotationTarget.rotation, _forwardPivotLerp);
	}

	public void Rotate(bool doPop, bool doFlip)
	{
		_firstDelta = firstVel * 500f * Time.deltaTime;
		_secondDelta = secondVel * 20f * Time.deltaTime;
		_thirdDelta = thirdVel * 20f * Time.deltaTime;
		if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
		{
			_secondDelta = -_secondDelta;
		}
		_firstDelta = Mathf.Clamp(_firstDelta, -5f, 5f);
		_secondDelta = Mathf.Clamp(_secondDelta, -6f, 6f);
		_thirdDelta = Mathf.Clamp((doFlip ? _thirdDelta : _thirdDelta * 0f), -9f, 9f);
		_rotDeltaThisFrame = Quaternion.Euler(_firstDelta, -_secondDelta, 0f);
		_bufferedRotation *= _rotDeltaThisFrame;
		Vector3 vector3 = Mathd.LocalAngularVelocity(PlayerController.Instance.skaterController.skaterRigidbody);
		Quaternion quaternion = Quaternion.AngleAxis(57.29578f * vector3.y * Time.deltaTime, PlayerController.Instance.skaterController.skaterTransform.up);
		_bufferedRotation *= quaternion;
		boardTransform.rotation = _bufferedRotation;
		_bufferedFlip += _thirdDelta;
		boardTransform.Rotate(new Vector3(0f, 0f, _bufferedFlip));
		if (doPop)
		{
			float single = popBoardVelAdd * Mathf.Abs(Mathf.Atan(0.0174532924f * _firstDelta)) - _lastuplift;
			boardRigidbody.AddForce(Vector3.up * single, ForceMode.VelocityChange);
		}
	}

	public void RotateWithPop(float _popDir, bool doFlip)
	{
		float single = _popDir * popSpeed;
		firstVel = firstVel + single;
	}

	public void SetBackPivotRotation(float p_frontToeAxis)
	{
		p_frontToeAxis *= 0.25f;
		p_frontToeAxis += 0.5f;
		_backPivotLerp = Mathf.Lerp(_backPivotLerp, p_frontToeAxis, Time.fixedDeltaTime * 20f);
		_backPivot.rotation = Quaternion.Slerp(_leftPivotRotationTarget.rotation, _rightPivotRotationTarget.rotation, _backPivotLerp);
	}

	public void SetBoardBackwards()
	{
		if (Vector3.Angle(boardTransform.forward, PlayerController.Instance.PlayerForward()) < 90f)
		{
			IsBoardBackwards = false;
			return;
		}
		IsBoardBackwards = true;
	}

	private void SetBoardControlPosition()
	{
		boardControlTransform.position = PlayerController.Instance.skaterController.animBoardTargetTransform.position;
	}

	public void SetBoardTargetPosition(float p_frontMagnitudeMinusBackMagnitude)
	{
		p_frontMagnitudeMinusBackMagnitude = Mathf.Clamp(p_frontMagnitudeMinusBackMagnitude, -1f, 1f);
		_targetPosition = p_frontMagnitudeMinusBackMagnitude;
		_targetPosition *= 0.5f;
		_targetPosition += 0.5f;
		_targetPosition = Mathf.Clamp(_targetPosition, 0f, 1f);
		boardTargetPosition.position = Vector3.Lerp(_frontPivotCenter.position, _backPivotCenter.position, _targetPosition);
	}

	public void SetCatchForwardRotation()
	{
		_catchForwardRotation.rotation = boardRigidbody.rotation;
	}

	public void SetFrontPivotRotation(float p_backToeAxis)
	{
		p_backToeAxis *= -0.25f;
		p_backToeAxis += 0.5f;
		_frontPivotLerp = Mathf.Lerp(_frontPivotLerp, p_backToeAxis, Time.fixedDeltaTime * 20f);
		_frontPivot.rotation = Quaternion.Slerp(_leftPivotRotationTarget.rotation, _rightPivotRotationTarget.rotation, _frontPivotLerp);
	}

	public void SetGrindPIDRotationValues()
	{
		KRp = grindKRp;
		KRd = grindKRd;
		KRi = grindKRi;
	}

	public void SetManualAngularVelocity(bool p_manual, float p_manualAxis, float p_secondaryAxis, float p_swivel)
	{
		float single = (Mathf.Abs(p_manualAxis) - 0.5f) * 15f + p_secondaryAxis * 10f;
		Vector3 vector3 = (!IsBoardBackwards ? boardTransform.forward : -boardTransform.forward);
		vector3 = Vector3.ProjectOnPlane(vector3, LerpedGroundNormal);
		Vector3 vector31 = Vector3.Cross(vector3, LerpedGroundNormal);
		Vector3 vector32 = Quaternion.AngleAxis(15f + single, (p_manual ? vector31 : -vector31)) * LerpedGroundNormal;
		Vector3 vector33 = Quaternion.AngleAxis(15f + single, (p_manual ? vector31 : -vector31)) * vector3;
		Vector3 vector34 = Quaternion.AngleAxis(15f + single, (p_manual ? vector31 : -vector31)) * -vector3;
		Quaternion quaternion = (!IsBoardBackwards ? Quaternion.LookRotation(vector33, vector32) : Quaternion.LookRotation(vector34, vector32));
		Mathd.DampXTorqueTowards(boardRigidbody, boardRigidbody.rotation, quaternion, manualForce, manualDamp);
	}

	public void SetPivotForwardRotation(float p_leftForwardAxisPlusRightForwardAxis, float p_speed)
	{
		p_leftForwardAxisPlusRightForwardAxis *= 0.25f;
		p_leftForwardAxisPlusRightForwardAxis += 0.5f;
		_forwardPivotLerp = Mathf.Lerp(_forwardPivotLerp, p_leftForwardAxisPlusRightForwardAxis, Time.fixedDeltaTime * p_speed);
		_boardTargetRotation.rotation = Quaternion.Slerp(_backwardPivotRotationTarget.rotation, _forwardPivotRotationTarget.rotation, _forwardPivotLerp);
	}

	public void SetPivotSideRotation(float p_leftToeAxisMinusRightToeAxis)
	{
		p_leftToeAxisMinusRightToeAxis *= 0.25f;
		p_leftToeAxisMinusRightToeAxis += 0.5f;
		_sidePivotLerp = Mathf.Lerp(_sidePivotLerp, p_leftToeAxisMinusRightToeAxis, Time.fixedDeltaTime * 20f);
		_sidePivotRotation.rotation = Quaternion.Slerp(_leftPivotRotationTarget.rotation, _rightPivotRotationTarget.rotation, _sidePivotLerp);
	}

	private void SetRotationTarget()
	{
		currentRotationTarget = (!IsBoardBackwards ? _boardTargetRotation.rotation : _backwardsTargetRotation.rotation);
	}

	private void SnapPosition()
	{
		PIDPosition();
	}

	public void SnapRotation()
	{
		PIDRotation(currentRotationTarget);
	}

	public void SnapRotation(bool p_notMovingSticks)
	{
		if (p_notMovingSticks)
		{
			AutoCatchRotation();
			return;
		}
		PIDRotation(currentRotationTarget);
	}

	public void SnapRotation(float p_mag)
	{
		CatchRotation(p_mag);
	}

	private void Update()
	{
		ProcessSounds();
	}

	public void UpdateBoardPosition()
	{
		if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Board)
		{
			PredictCollision();
			SnapPosition();
			return;
		}
		if (!AllDown)
		{
			PredictCollision();
		}
	}

	public void UpdateReferenceBoardTargetRotation()
	{
		_bufferedRotation = boardTransform.rotation;
		Vector3 vector3 = _bufferedRotation * Vector3.forward;
		_bufferedRotation.SetLookRotation(vector3, boardUpAtPopBegin);
	}
}