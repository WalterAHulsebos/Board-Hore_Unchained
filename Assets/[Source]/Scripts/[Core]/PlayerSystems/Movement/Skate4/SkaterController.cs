using FSMHelper;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using System;
using UnityEngine;
using VacuumBreather;

public class SkaterController : MonoBehaviour
{
	[Header("Components")]
	public Respawn respawn;

	public PuppetMaster puppetMaster;

	public BehaviourPuppet behaviourPuppet;

	public FullBodyBipedIK finalIk;

	[Header("Transforms")]
	public Transform skaterTransform;

	public Transform animBoardTargetTransform;

	public Transform animBoardFrontWheelTransform;

	public Transform animBoardBackWheelTransform;

	public Transform animBoardBackwardsTargetTransform;

	public Transform physicsBoardTransform;

	public Transform physicsBoardBackwardsTransform;

	public Transform upVectorTransform;

	public Transform regsLeftKneeGuide;

	public Transform regsRightKneeGuide;

	public Transform goofyLeftKneeGuide;

	public Transform goofyRightKneeGuide;

	[Header("Rigidbodies")]
	public Rigidbody skaterRigidbody;

	[Header("Colliders")]
	public CapsuleCollider leftFootCollider;

	public CapsuleCollider rightFootCollider;

	[Header("Variables")]
	[Range(1f, 10f)]
	public float pushForce = 8f;

	[Range(1f, 10f)]
	public float breakForce = 3f;

	private Quaternion _currentRotationTarget;

	private Vector3 _currentForwardTarget;

	private float _duration;

	private float _startTime;

	private Vector3 _startUpVector = Vector3.up;

	private Quaternion _startRotation = Quaternion.identity;

	private Quaternion _newUp = Quaternion.identity;

	private Vector3 _boardTargetVel;

	private Vector3 _boardTargetLastPos;

	private bool _landingPrediction;

	private float _animSwitch;

	private readonly PidQuaternionController _pidController = new PidQuaternionController(8f, 0f, 0.05f);

	public float Kp;

	public float Ki;

	public float Kd;

	public float rotSmooth;

	public float maxCrouchAtAngle;

	public float crouchSmooth;

	private float _crouchAmount;

	public Vector3 pushBrakeForce;

	public float totalSystemMass;

	private Vector3 _angularVelocity;

	public Rigidbody leanProxy;

	public Vector3 comboAccelLerp;

	public float maxComboAccelLerp;

	public Vector3 BoardTargetVel => _boardTargetVel;

	public float crouchAmount => _crouchAmount;

	public Transform LowestWheelTransform
	{
		get
		{
			if ((skaterRigidbody.position - animBoardFrontWheelTransform.position).magnitude > (skaterRigidbody.position - animBoardBackWheelTransform.position).magnitude)
			{
				return animBoardFrontWheelTransform;
			}
			return animBoardBackWheelTransform;
		}
	}

	public SkaterController()
	{
	}

	public void AddCollisionOffset()
	{
		if (!Mathd.Vector3IsInfinityOrNan(PlayerController.Instance.BoardToTargetVector()))
		{
			Transform targetVector = skaterTransform;
			targetVector.position = targetVector.position + PlayerController.Instance.BoardToTargetVector();
		}
	}

	public void AddTurnTorque(float p_value)
	{
		p_value /= 10f;
		skaterRigidbody.AddTorque(skaterTransform.up * p_value, ForceMode.VelocityChange);
		leanProxy.AddTorque(skaterTransform.up * p_value, ForceMode.VelocityChange);
	}

	public void AddTurnTorque(float p_value, bool p_fast)
	{
		p_value = Mathf.Lerp(p_value / 10f, p_value, Mathf.Abs(p_value));
		skaterRigidbody.AddTorque(skaterTransform.up * p_value, ForceMode.VelocityChange);
	}

	public void AddUpwardDisplacement(float p_timeStep)
	{
		if (!Mathd.Vector3IsInfinityOrNan(GetBoardDisplacement()))
		{
			Vector3 coMDisplacement = skaterTransform.up * PlayerController.Instance.GetCoMDisplacement(p_timeStep);
			Transform transforms = skaterTransform;
			transforms.position = transforms.position + coMDisplacement;
		}
	}

	private void Awake()
	{
		float single = 0f;
		Rigidbody[] componentsInChildren = PlayerController.Instance.gameObject.GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			single += componentsInChildren[i].mass;
		}
		totalSystemMass = single;
	}

	public void CorrectVelocity()
	{
		float single = Vector3.Angle(Vector3.ProjectOnPlane(skaterRigidbody.velocity, PlayerController.Instance.GetGroundNormal()), Vector3.up);
		float single1 = Vector3.Angle(PlayerController.Instance.GetGroundNormal(), Vector3.up);
		if (PlayerController.Instance.IsGrounded() && single > 45f && single < 80f && single1 > 10f && single1 < 85f)
		{
			skaterRigidbody.velocity = Vector3.ProjectOnPlane(skaterRigidbody.velocity, PlayerController.Instance.GetGroundNormal());
		}
	}

	private void FixedUpdate()
	{
		_currentRotationTarget = UpdateTargetRotation();
		_currentForwardTarget = UpdateTargetForward();
		UpdateBoardTargetVel();
	}

	public Vector3 GetBoardDisplacement()
	{
		return PlayerController.Instance.boardController.boardTransform.position - PlayerController.Instance.boardController.boardTargetPosition.position;
	}

	private void InAirRotation(float p_slerp)
	{
		Quaternion quaternion = Quaternion.Slerp(_startRotation, _newUp, p_slerp);
		upVectorTransform.rotation = quaternion;
		Quaternion rotation = Quaternion.FromToRotation(skaterTransform.up, upVectorTransform.up);
		rotation *= skaterRigidbody.rotation;
		skaterRigidbody.rotation = rotation;
		leanProxy.rotation = rotation;
		comboAccelLerp = skaterTransform.up;
	}

	private void InAirRotationLogic()
	{
		InAirRotation(Mathf.Clamp((Time.time - _startTime) / _duration, 0f, 1f));
	}

	private void OnAnimatorIK(int p_layerIndex)
	{
		PlayerController.Instance.playerSM.OnAnimatorUpdateSM();
	}

	public Vector3 PredictLanding(Vector3 p_popForce)
	{
		p_popForce = skaterRigidbody.velocity + p_popForce;
		_startTime = Time.time;
		Vector3 vector3 = Vector3.zero;
		_duration = PlayerController.Instance.boardController.trajectory.CalculateTrajectory(skaterTransform.position - (Vector3.up * 0.9765f), p_popForce, 50f, out vector3);
		_startRotation = skaterRigidbody.rotation;
		_startUpVector = skaterTransform.up;
		_landingPrediction = true;
		_newUp = Quaternion.FromToRotation(_startUpVector, PlayerController.Instance.boardController.trajectory.PredictedGroundNormal);
		_newUp *= skaterRigidbody.rotation;
		CancelInvoke("PreLandingEvent");
		Invoke("PreLandingEvent", _duration - 0.3f);
		return vector3;
	}

	private void PreLandingEvent()
	{
		PlayerController.Instance.playerSM.OnPreLandingEventSM();
	}

	public void RemoveTurnTorque(float p_value)
	{
		Vector3 pValue = skaterTransform.InverseTransformDirection(skaterRigidbody.angularVelocity);
		pValue.y *= p_value;
		skaterRigidbody.angularVelocity = skaterTransform.TransformDirection(pValue);
	}

	public void ResetRotationLerps()
	{
		leanProxy.rotation = skaterTransform.rotation;
		comboAccelLerp = skaterTransform.up;
	}

	public void SetPuppetMode(BehaviourPuppet.NormalMode p_mode)
	{
		behaviourPuppet.masterProps.normalMode = p_mode;
	}

	private void UdpateSkater()
	{
		if (!Mathd.Vector3IsInfinityOrNan(GetBoardDisplacement()))
		{
			Transform boardDisplacement = skaterTransform;
			boardDisplacement.position = boardDisplacement.position + GetBoardDisplacement();
			skaterRigidbody.velocity = PlayerController.Instance.boardController.boardRigidbody.velocity;
		}
	}

	private void Update()
	{
		_animSwitch = Mathf.Lerp(_animSwitch, (PlayerController.Instance.IsSwitch ? 1f : 0f), Time.deltaTime * 10f);
		PlayerController.Instance.AnimSetSwitch(_animSwitch);
	}

	private void UpdateBoardTargetVel()
	{
		_boardTargetVel = (animBoardTargetTransform.position - _boardTargetLastPos) / Time.deltaTime;
		_boardTargetLastPos = animBoardTargetTransform.position;
	}

	public void UpdatePositionDuringPop()
	{
		_landingPrediction = false;
		_duration = 0f;
		_startTime = 0f;
	}

	public void UpdatePositions()
	{
		if (PlayerController.Instance.movementMaster == PlayerController.MovementMaster.Skater)
		{
			InAirRotationLogic();
			return;
		}
		if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Target)
		{
			UdpateSkater();
		}
		_landingPrediction = false;
		_duration = 0f;
		_startTime = 0f;
	}

	public void UpdateRidingPositionsCOMTempWallie()
	{
	}

	public void UpdateSkaterPosFromComPos()
	{
		Vector3 vector3 = PlayerController.Instance.skaterController.skaterTransform.InverseTransformPoint(PlayerController.Instance.boardController.boardTransform.position);
		Vector3 vector31 = skaterTransform.InverseTransformPoint(PlayerController.Instance.comController.transform.position);
		if (vector31.y - vector3.y < 0.4f)
		{
			vector31 = new Vector3(vector31.x, vector3.y + 0.4f, vector31.z);
		}
		Vector3 vector32 = new Vector3(0f, vector31.y, 0f);
		skaterTransform.position = skaterTransform.TransformPoint(vector32);
		skaterRigidbody.velocity = PlayerController.Instance.comController.COMRigidbody.velocity;
	}

	public void UpdateSkaterRotation(bool canRotate, bool manualling)
	{
		if (canRotate && PlayerController.Instance.IsGrounded())
		{
			Vector3 vector3 = pushBrakeForce / totalSystemMass;
			float instance = 1f - PlayerController.Instance.boardController.xzRot / maxCrouchAtAngle;
			_crouchAmount = Mathf.SmoothStep(_crouchAmount, instance, Time.fixedDeltaTime * crouchSmooth);
			Vector3 vector31 = -Physics.gravity;
			Vector3 instance1 = PlayerController.Instance.boardController.acceleration - vector3;
			Vector3 vector32 = vector31 + instance1;
			comboAccelLerp = Vector3.RotateTowards(comboAccelLerp, vector32, 0.0174532924f * maxComboAccelLerp * Time.deltaTime, 5f * Time.deltaTime);
			Debug.DrawRay(skaterTransform.position, vector32 * 0.1f, Color.red);
			Debug.DrawRay(skaterTransform.position, skaterTransform.up, Color.green);
			Debug.DrawRay(skaterTransform.position, comboAccelLerp * 0.1f, Color.blue);
			_pidController.Kp = Kp;
			_pidController.Ki = Ki;
			_pidController.Kd = Kd;
			Vector3 vector33 = (!PlayerController.Instance.GetBoardBackwards() ? physicsBoardTransform.forward : -physicsBoardTransform.forward);
			if (manualling)
			{
				vector33 = Vector3.ProjectOnPlane(vector33, PlayerController.Instance.GetGroundNormal());
			}
			Quaternion quaternion = Quaternion.LookRotation(vector33, comboAccelLerp);
			Vector3 vector34 = _pidController.ComputeRequiredAngularAcceleration(skaterRigidbody.rotation, quaternion, leanProxy.angularVelocity, Time.fixedDeltaTime);
			leanProxy.AddTorque(vector34, ForceMode.Acceleration);
			skaterRigidbody.rotation = leanProxy.rotation;
		}
	}

	public void UpdateSkaterRotation(bool p_canRotate, Quaternion p_rot)
	{
		if (p_canRotate)
		{
			skaterTransform.rotation = Quaternion.Slerp(skaterTransform.rotation, p_rot, Time.fixedDeltaTime * 50f);
		}
	}

	public void UpdateSkaterRotationOLD(bool canRotate)
	{
		if (canRotate)
		{
			skaterTransform.rotation = Quaternion.Slerp(skaterTransform.rotation, _currentRotationTarget, Time.fixedDeltaTime * 10f);
			Quaternion rotation = Quaternion.FromToRotation(skaterTransform.forward, Vector3.ProjectOnPlane(_currentForwardTarget, skaterTransform.up)) * skaterTransform.rotation;
			skaterTransform.rotation = rotation;
		}
	}

	private Vector3 UpdateTargetForward()
	{
		if (!PlayerController.Instance.boardController.IsBoardBackwards)
		{
			return physicsBoardTransform.forward;
		}
		return physicsBoardBackwardsTransform.forward;
	}

	private Quaternion UpdateTargetRotation()
	{
		if (!PlayerController.Instance.boardController.IsBoardBackwards)
		{
			return physicsBoardTransform.rotation;
		}
		return physicsBoardBackwardsTransform.rotation;
	}
}