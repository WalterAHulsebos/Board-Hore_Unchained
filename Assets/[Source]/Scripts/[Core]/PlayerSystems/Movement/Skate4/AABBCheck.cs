using System;
using UnityEngine;

public class AABBCheck : MonoBehaviour
{
	private Rigidbody _board;

	private Rigidbody _backTruck;

	private Rigidbody _frontTruck;

	private Vector3 _lastBoardPos = Vector3.zero;

	private Vector3 _lastBoardLocalPos = Vector3.zero;

	private Vector3 _lastBoardVelocity = Vector3.zero;

	private Vector3 _lastBoardAngularVelocity = Vector3.zero;

	private Vector3 _lastBackTruckPos = Vector3.zero;

	private Vector3 _lastBackTruckLocalPos = Vector3.zero;

	private Vector3 _lastBackTruckVelocity = Vector3.zero;

	private Vector3 _lastBackTruckAngularVelocity = Vector3.zero;

	private Vector3 _lastFrontTruckPos = Vector3.zero;

	private Vector3 _lastFrontTruckLocalPos = Vector3.zero;

	private Vector3 _lastFrontTruckVelocity = Vector3.zero;

	private Vector3 _lastFrontTruckAngularVelocity = Vector3.zero;

	public AABBCheck()
	{
	}

	private void FixedUpdate()
	{
	}

	private void LateUpdate()
	{
	}

	private void NaNCheck()
	{
		if (!Mathd.Vector3IsInfinityOrNan(_board.position))
		{
			_lastBoardPos = _board.position;
		}
		else
		{
			Debug.LogError("Board Position is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_board.transform.localPosition))
		{
			_lastBoardLocalPos = _board.transform.localPosition;
		}
		else
		{
			Debug.LogError("Board Local Position is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_board.velocity))
		{
			_lastBoardVelocity = _board.velocity;
		}
		else
		{
			Debug.LogError("Board Velocity is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_board.angularVelocity))
		{
			_lastBoardAngularVelocity = _board.angularVelocity;
		}
		else
		{
			Debug.LogError("Board Angular Velocity is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_backTruck.position))
		{
			_lastBackTruckPos = _backTruck.position;
		}
		else
		{
			Debug.LogError("Back Truck Position is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_backTruck.transform.localPosition))
		{
			_lastBackTruckLocalPos = _backTruck.transform.localPosition;
		}
		else
		{
			Debug.LogError("Back Truck Local Position is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_backTruck.velocity))
		{
			_lastBackTruckVelocity = _backTruck.velocity;
		}
		else
		{
			Debug.LogError("Back Truck Velocity is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_backTruck.angularVelocity))
		{
			_lastBackTruckAngularVelocity = _backTruck.angularVelocity;
		}
		else
		{
			Debug.LogError("Back Truck Angular Velocity is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_frontTruck.position))
		{
			_lastFrontTruckPos = _frontTruck.position;
		}
		else
		{
			Debug.LogError("Front Truck Position is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_frontTruck.transform.localPosition))
		{
			_lastFrontTruckLocalPos = _frontTruck.transform.localPosition;
		}
		else
		{
			Debug.LogError("Front Truck Local Position is NaN");
			RestorLastValues();
		}
		if (!Mathd.Vector3IsInfinityOrNan(_frontTruck.velocity))
		{
			_lastFrontTruckVelocity = _frontTruck.velocity;
		}
		else
		{
			Debug.LogError("Front Truck Velocity is NaN");
			RestorLastValues();
		}
		if (Mathd.Vector3IsInfinityOrNan(_frontTruck.angularVelocity))
		{
			Debug.LogError("Front Truck Angular Velocity is NaN");
			RestorLastValues();
			return;
		}
		_lastFrontTruckAngularVelocity = _frontTruck.angularVelocity;
	}

	private void RestorLastValues()
	{
		Debug.LogError("Restored Values");
		_board.position = _lastBoardPos;
		_board.transform.localPosition = _lastBoardLocalPos;
		_board.velocity = _lastBoardVelocity;
		_board.angularVelocity = _lastBoardAngularVelocity;
		_backTruck.position = _lastBackTruckPos;
		_backTruck.transform.localPosition = _lastBackTruckLocalPos;
		_backTruck.velocity = _lastBackTruckVelocity;
		_backTruck.angularVelocity = _lastBackTruckAngularVelocity;
		_frontTruck.position = _lastFrontTruckPos;
		_frontTruck.transform.localPosition = _lastFrontTruckLocalPos;
		_frontTruck.velocity = _lastFrontTruckVelocity;
		_frontTruck.angularVelocity = _lastFrontTruckAngularVelocity;
	}

	private void Start()
	{
		_board = PlayerController.Instance.boardController.boardRigidbody;
		_backTruck = PlayerController.Instance.boardController.backTruckRigidbody;
		_frontTruck = PlayerController.Instance.boardController.frontTruckRigidbody;
	}

	private void Update()
	{
	}
}