using Dreamteck.Splines;
using FSMHelper;
using System;
using UnityEngine;

public class GrindTrigger : TriggerManager
{
	public TriggerManager triggerManager;

	public SplineComputer localSpline;

	[SerializeField]
	public GrindTrigger.TriggerType triggerType;

	private bool _colliding;

	private bool _wasJustColliding;

	[SerializeField]
	private Vector3 _grindDirection;

	[SerializeField]
	private Vector3 _grindUp;

	[SerializeField]
	private Vector3 _grindRightVector;

	public Vector3 closestPoint = Vector3.zero;

	private Quaternion _newUp = Quaternion.identity;

	private float _maxVelocity;

	private float _timer;

	private bool _assist;

	private double percent;

	private bool _forward;

	private float _velocityMagnitude;

	private bool _ignore;

	private SplineResult _splineResult;

	private float _steepness;

	public bool Colliding => _colliding;

	public Vector3 GrindDirection => _grindDirection;

	public Vector3 GrindRight => _grindRightVector;

	public Vector3 GrindUp => _grindUp;

	public bool WasColliding => _wasJustColliding;

	public GrindTrigger()
	{
	}

	private void CorrectVelocity(Rigidbody p_rb, float p_magnitude, bool p_addGravity)
	{
		p_magnitude = Mathf.Clamp(p_magnitude, 0f, _velocityMagnitude + _steepness);
		if (!Mathd.Vector3IsInfinityOrNan(_grindDirection) && !Mathd.IsInfinityOrNaN(p_magnitude))
		{
			if (p_addGravity)
			{
				p_rb.velocity = (_grindDirection * p_magnitude) + (Physics.gravity * Time.fixedDeltaTime);
				return;
			}
			p_rb.velocity = _grindDirection * p_magnitude;
		}
	}

	private void FixedUpdate()
	{
		_wasJustColliding = _colliding;
	}

	private void OnTriggerEnter(Collider p_other)
	{
		if (p_other.gameObject.layer == LayerMask.NameToLayer("Grindable") && (PlayerController.Instance.playerSM.CanGrindSM() || PlayerController.Instance.GetPopped() && !PlayerController.Instance.playerSM.IsGrindingSM()))
		{
			PlayerController.Instance.boardController.grindTag = p_other.gameObject.tag;
			if (!triggerManager.IsColliding)
			{
				triggerManager.spline = p_other.GetComponent<SplineComputer>();
				if (triggerManager.spline == null)
				{
					triggerManager.spline = p_other.gameObject.GetComponentInParent<SplineComputer>();
				}
			}
			if (!_colliding)
			{
				_assist = true;
				_timer = 0f;
			}
			_ignore = false;
			_colliding = true;
			if (triggerManager.spline != null)
			{
				percent = triggerManager.spline.Project((grindContact ? grindContactPoint : transform.position), 3, 0, 1);
				_splineResult = triggerManager.spline.Evaluate(percent);
				float single = Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, _splineResult.normal), _splineResult.direction);
				if (single < 89f)
				{
					_forward = true;
					_grindDirection = _splineResult.direction.normalized;
				}
				else if (single > 91f)
				{
					_forward = false;
					_grindDirection = -_splineResult.direction.normalized;
				}
				if (!VelocityCheck(_grindDirection))
				{
					PlayerController.Instance.playerSM.OnGrindEndedSM();
					_colliding = false;
					_ignore = true;
				}
				if (!_ignore)
				{
					if (PlayerController.Instance.boardController.triggerManager.IsColliding)
					{
						_grindUp = Vector3.Lerp(_grindUp, _splineResult.normal, Time.fixedDeltaTime * 4f);
						if (!_forward)
						{
							_grindRightVector = Vector3.Lerp(_grindRightVector, -_splineResult.right, Time.fixedDeltaTime * 20f);
						}
						else
						{
							_grindRightVector = Vector3.Lerp(_grindRightVector, _splineResult.right, Time.fixedDeltaTime * 20f);
						}
					}
					else
					{
						_grindUp = _splineResult.normal;
						if (!_forward)
						{
							_grindRightVector = -_splineResult.right;
						}
						else
						{
							_grindRightVector = _splineResult.right;
						}
					}
					PlayerController.Instance.boardController.GroundY = _splineResult.position.y - 0.3f;
					Vector3 vector3 = Vector3.Project(PlayerController.Instance.boardController.boardRigidbody.velocity, _grindDirection);
					_velocityMagnitude = vector3.magnitude;
					CorrectVelocity(PlayerController.Instance.boardController.boardRigidbody, _velocityMagnitude, true);
					CorrectVelocity(PlayerController.Instance.boardController.backTruckRigidbody, _velocityMagnitude, true);
					CorrectVelocity(PlayerController.Instance.boardController.frontTruckRigidbody, _velocityMagnitude, true);
					if (!Mathd.Vector3IsInfinityOrNan(_splineResult.position))
					{
						triggerManager.grindContactSplinePosition.position = _splineResult.position;
					}
					if (PlayerController.Instance.boardController.triggerManager.IsColliding)
					{
						triggerManager.grindContactSplinePosition.rotation = Quaternion.Slerp(triggerManager.grindContactSplinePosition.rotation, Quaternion.LookRotation(_grindDirection, _grindUp), Time.fixedDeltaTime * 10f);
					}
					else
					{
						triggerManager.grindContactSplinePosition.rotation = Quaternion.LookRotation(_grindDirection, _grindUp);
					}
					triggerManager.playerOffset.position = PlayerController.Instance.skaterController.skaterTransform.position;
					triggerManager.playerOffset.rotation = PlayerController.Instance.skaterController.skaterTransform.rotation;
					PlayerController.Instance.boardOffsetRoot.rotation = (PlayerController.Instance.GetBoardBackwards() ? PlayerController.Instance.skaterController.physicsBoardBackwardsTransform.rotation : PlayerController.Instance.skaterController.physicsBoardTransform.rotation);
					PlayerController.Instance.PlayerGrindRotation = PlayerController.Instance.playerRotationReference.rotation;
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Grindable"))
		{
			_colliding = false;
		}
	}

	private void OnTriggerStay(Collider p_other)
	{
		Vector3 instance;
		if (p_other.gameObject.layer == LayerMask.NameToLayer("Grindable") && PlayerController.Instance.playerSM.CanGrindSM())
		{
			if (_assist)
			{
				_timer += Time.deltaTime;
				if (_timer > 0.1f)
				{
					_assist = false;
				}
			}
			_colliding = true;
			_ignore = false;
			if (triggerManager.spline != null)
			{
				percent = triggerManager.spline.Project((grindContact ? grindContactPoint : transform.position), 3, 0, 1);
				_splineResult = triggerManager.spline.Evaluate(percent);
				if (!_forward)
				{
					_grindDirection = -_splineResult.direction.normalized;
				}
				else
				{
					_grindDirection = _splineResult.direction.normalized;
				}
				if (!VelocityCheck(_grindDirection))
				{
					PlayerController.Instance.playerSM.OnGrindEndedSM();
					_colliding = false;
					_ignore = true;
				}
				if (!_ignore)
				{
					if (Vector3.Angle(_grindDirection, Vector3.up) <= 94f)
					{
						_steepness = 0f;
					}
					else
					{
						_steepness = 5f;
					}
					if (!_forward)
					{
						_grindRightVector = Vector3.Lerp(_grindRightVector, -_splineResult.right, Time.fixedDeltaTime * 20f);
					}
					else
					{
						_grindRightVector = Vector3.Lerp(_grindRightVector, _splineResult.right, Time.fixedDeltaTime * 20f);
					}
					_grindUp = Vector3.Lerp(_grindUp, _splineResult.normal, Time.fixedDeltaTime * 4f);
					if (!Mathd.Vector3IsInfinityOrNan(_splineResult.position))
					{
						triggerManager.grindContactSplinePosition.position = _splineResult.position;
					}
					triggerManager.grindContactSplinePosition.rotation = Quaternion.Slerp(triggerManager.grindContactSplinePosition.rotation, Quaternion.LookRotation(_grindDirection, _grindUp), Time.fixedDeltaTime * 10f);
					PlayerController.Instance.boardController.GroundY = _splineResult.position.y - 0.3f;
					_newUp = Quaternion.FromToRotation(PlayerController.Instance.playerRotationReference.up, _grindUp);
					_newUp *= PlayerController.Instance.playerRotationReference.rotation;
					PlayerController.Instance.playerRotationReference.rotation = Quaternion.Slerp(PlayerController.Instance.playerRotationReference.rotation, _newUp, Time.fixedDeltaTime * 10f);
					PlayerController.Instance.PlayerGrindRotation = PlayerController.Instance.playerRotationReference.rotation;
					if (!PlayerController.Instance.GetPopped())
					{
						if (!_assist)
						{
							Rigidbody rigidbody = PlayerController.Instance.boardController.boardRigidbody;
							instance = PlayerController.Instance.boardController.boardRigidbody.velocity;
							CorrectVelocity(rigidbody, instance.magnitude, true);
							return;
						}
						Rigidbody instance1 = PlayerController.Instance.boardController.boardRigidbody;
						instance = PlayerController.Instance.boardController.boardRigidbody.velocity;
						CorrectVelocity(instance1, instance.magnitude, true);
						Rigidbody rigidbody1 = PlayerController.Instance.boardController.backTruckRigidbody;
						instance = PlayerController.Instance.boardController.backTruckRigidbody.velocity;
						CorrectVelocity(rigidbody1, instance.magnitude, true);
						Rigidbody instance2 = PlayerController.Instance.boardController.frontTruckRigidbody;
						instance = PlayerController.Instance.boardController.frontTruckRigidbody.velocity;
						CorrectVelocity(instance2, instance.magnitude, true);
						return;
					}
				}
			}
		}
		else if (p_other.gameObject.layer == LayerMask.NameToLayer("Grindable") && !PlayerController.Instance.IsGrounded() && PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude < 0.2f && !PlayerController.Instance.playerSM.IsGrindingSM())
		{
			bool flag = PlayerController.Instance.respawn.bail.bailed;
		}
	}

	private bool VelocityCheck(Vector3 _grindDirection)
	{
		Vector3 vector3 = Vector3.ProjectOnPlane(_grindDirection, Vector3.up);
		if (Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.VelocityOnPop, Vector3.up), vector3) < 60f)
		{
			return true;
		}
		return false;
	}

	public enum TriggerType
	{
		Board,
		Nose,
		Tail,
		BackTruck,
		FrontTruck
	}
}