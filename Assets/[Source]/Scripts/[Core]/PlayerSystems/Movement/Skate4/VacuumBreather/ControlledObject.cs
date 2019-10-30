using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VacuumBreather
{
	public class ControlledObject : MonoBehaviour
	{
		private readonly PidQuaternionController _pidController = new PidQuaternionController(8f, 0f, 0.05f);

		private Transform _currentTransform;

		private Rigidbody _objectRigidbody;

		public float Kp;

		public float Ki;

		public float Kd;

		public Quaternion DesiredOrientation
		{
			get;
			set;
		}

		public ControlledObject()
		{
		}

		private void Awake()
		{
			_currentTransform = transform;
			_objectRigidbody = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			Quaternion desiredOrientation = DesiredOrientation;
			if (_currentTransform == null || _objectRigidbody == null)
			{
				return;
			}
			_pidController.Kp = Kp;
			_pidController.Ki = Ki;
			_pidController.Kd = Kd;
			Vector3 vector3 = _pidController.ComputeRequiredAngularAcceleration(_currentTransform.rotation, DesiredOrientation, _objectRigidbody.angularVelocity, Time.fixedDeltaTime * 0.25f);
			_objectRigidbody.AddTorque(vector3, ForceMode.Acceleration);
		}
	}
}