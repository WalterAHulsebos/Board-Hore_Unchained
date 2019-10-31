using Dreamteck.Splines;
using FSMHelper;
using System;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
	public GrindDetection grindDetection;

	public GrindCollisions boardCollision;

	public GrindCollisions backTruckCollision;

	public GrindCollisions frontTruckCollision;

	public Transform grindContactSplinePosition;

	public Transform grindOffset;

	public SplineComputer spline;

	public SplineComputer currentSpline;

	public Transform playerOffset;

	public Vector3 grindContactPoint = Vector3.zero;

	public bool canOllie;

	public bool canNollie;

	public bool enteredFromRight;

	public bool swapped;

	public TriggerManager.SideEnteredGrind sideEnteredGrind = TriggerManager.SideEnteredGrind.Center;

	public bool grindContact;

	[SerializeField]
	private bool _isColliding;

	public bool wasColliding;

	[SerializeField]
	private GrindTrigger[] _grindTriggers = new GrindTrigger[5];

	public bool[] activeGrinds = new bool[5];

	public Vector3 grindDirection;

	public Vector3 grindUp;

	public Vector3 grindRight;

	public float corrrectiveForce = 10f;

	[SerializeField]
	private Transform _tailLimit;

	[SerializeField]
	private Transform _backTruckLeftLimit;

	[SerializeField]
	private Transform _backTruckRightLimit;

	[SerializeField]
	private Transform _frontTruckLeftLimit;

	[SerializeField]
	private Transform _frontTruckRightLimit;

	[SerializeField]
	private Transform _noseLimit;

	public bool[] _contactLimitIsRight = new bool[6];

	private float _stallTimer;

	private float _maxStallTime = 0.3f;

	[SerializeField]
	private bool _boardCollision;

	[SerializeField]
	private bool _frontTruckCollision;

	[SerializeField]
	private bool _backTruckCollision;

	private bool _collidingThisFrame;

	private float _grindAngle;

	public bool IsColliding
	{
		get => _isColliding;
		set => _isColliding = value;
	}

	public TriggerManager()
	{
	}

	private void DetectCollisionPoint()
	{
		if (boardCollision.isColliding || backTruckCollision.isColliding || frontTruckCollision.isColliding)
		{
			_grindAngle = Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.forward, grindUp), grindDirection);
			if (_grindAngle > 70f && _grindAngle < 110f)
			{
				if (boardCollision.isColliding)
				{
					grindContactPoint = boardCollision.lastCollision;
					grindContact = true;
				}
				else if (frontTruckCollision.isColliding)
				{
					grindContactPoint = frontTruckCollision.lastCollision;
					grindContact = true;
				}
				else if (backTruckCollision.isColliding)
				{
					grindContactPoint = backTruckCollision.lastCollision;
					grindContact = true;
				}
			}
			else if (!backTruckCollision.isColliding && !frontTruckCollision.isColliding)
			{
				grindContactPoint = boardCollision.lastCollision;
				grindContact = true;
			}
			else if (backTruckCollision.isColliding && frontTruckCollision.isColliding)
			{
				grindContactPoint = (backTruckCollision.lastCollision + frontTruckCollision.lastCollision) / 2f;
				grindContact = true;
			}
			else if (frontTruckCollision.isColliding)
			{
				grindContactPoint = frontTruckCollision.lastCollision;
				grindContact = true;
			}
			else if (backTruckCollision.isColliding)
			{
				grindContactPoint = backTruckCollision.lastCollision;
				grindContact = true;
			}
		}
		else
		{
			grindContact = false;
		}
		if (!boardCollision.isColliding)
		{
			_boardCollision = false;
		}
		else
		{
			_boardCollision = true;
		}
		if (!frontTruckCollision.isColliding)
		{
			_frontTruckCollision = false;
		}
		else
		{
			_frontTruckCollision = true;
		}
		if (backTruckCollision.isColliding)
		{
			_backTruckCollision = true;
			return;
		}
		_backTruckCollision = false;
	}

	public void GrindTriggerCheck()
	{
		_collidingThisFrame = false;
		for (int i = 0; i < 5; i++)
		{
			if (_grindTriggers[i].Colliding)
			{
				grindDirection = _grindTriggers[i].GrindDirection;
				grindUp = _grindTriggers[i].GrindUp;
				grindRight = _grindTriggers[i].GrindRight;
				wasColliding = false;
				_collidingThisFrame = true;
				activeGrinds[i] = true;
			}
		}
		if (!_collidingThisFrame)
		{
			for (int j = 0; j < 5; j++)
			{
				if (_grindTriggers[j].WasColliding)
				{
					wasColliding = false;
					_collidingThisFrame = true;
				}
			}
		}
		if (wasColliding && !IsColliding && !_collidingThisFrame)
		{
			for (int k = 0; k < (int)activeGrinds.Length; k++)
			{
				activeGrinds[k] = false;
			}
			canOllie = false;
			canNollie = false;
			PlayerController.Instance.playerSM.OnGrindEndedSM();
			swapped = false;
			wasColliding = false;
		}
		if (!IsColliding && _collidingThisFrame)
		{
			PlayerController.Instance.playerSM.OnGrindDetectedSM();
			PlayerController.Instance.playerSM.OnGrindStaySM();
			PlayerController.Instance.playerSM.SetSplineSM(spline);
		}
		if (IsColliding && _collidingThisFrame)
		{
			PlayerController.Instance.playerSM.OnGrindStaySM();
			if (PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude >= 0.2f)
			{
				_stallTimer = 0f;
			}
			else
			{
				_stallTimer += Time.deltaTime;
				if (_stallTimer > _maxStallTime)
				{
					PlayerController.Instance.ForceBail();
				}
			}
		}
		if (IsColliding && !_collidingThisFrame)
		{
			wasColliding = true;
		}
		IsColliding = _collidingThisFrame;
		DetectCollisionPoint();
		if (IsColliding)
		{
			grindDetection.DetectGrind(activeGrinds[0], activeGrinds[1], activeGrinds[2], activeGrinds[3], activeGrinds[4], grindUp, grindDirection, grindRight, ref canOllie, ref canNollie, _backTruckCollision, _frontTruckCollision, _boardCollision);
		}
	}

	public enum SideEnteredGrind
	{
		Left,
		Right,
		Center
	}
}