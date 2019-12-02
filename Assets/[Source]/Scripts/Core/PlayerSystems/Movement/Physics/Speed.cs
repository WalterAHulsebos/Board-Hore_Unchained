using System;
using UnityEngine;

using CommonGames.Utilities.Extensions;

namespace Core.PlayerSystems.Movement
{
    public partial class Speed : VehicleBehaviour
    {
        [Tooltip("Maximum speed in units per second.")]
        [SerializeField] private float maxSpeed = 30;
        
        private float _currentTimeValue;
        private float _nextTimeValue;
        private float _nextVelocityMagnitude;

        private Rigidbody _rigidbody;

        protected override void Start()
        {
            base.Start();

            _rigidbody = _vehicle.rigidbody;
        }

        private void Update() => CalculateSpeedData(_vehicle.SpeedData);

        private void CalculateSpeedData(VehicleSpeed speedData)
        {
            speedData.sideSpeed = Vector3.Dot(_rigidbody.transform.right, _rigidbody.velocity);
            speedData.forwardSpeed = Vector3.Dot(_rigidbody.transform.forward, _rigidbody.velocity);
            speedData.speed = _rigidbody.velocity.magnitude;
        }
    }
}