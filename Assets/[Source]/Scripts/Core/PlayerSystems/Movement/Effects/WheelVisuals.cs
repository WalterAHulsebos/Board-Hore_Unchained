using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    public class WheelVisuals : VehicleBehaviour
    {
        private Rigidbody _carRb;
        private Transform _attachPoint;
        private PlayerInputs _input;
        private VehicleSpeed _vehicleSpeed;
        public bool grounded;

        public LayerMask groundLayer;

        [Header("Wheel Dimensions")] public WheelPosition wheelPosition;
        public float wheelRadius;
        public float suspensionMaxHeight;
        public float frontWheelTurnMaxAngle = 30;

        [Header("Effects")] private TrailRenderer _tyreTrail;
        private ParticleSystem _dustParticles;
        private bool _isDustParticlesNull;
        private bool _isTyreTrailNull;

        protected override void Start()
        {
            base.Start();
            
            _attachPoint = transform.parent;

            _tyreTrail = GetComponentInChildren<TrailRenderer>();
            _dustParticles = GetComponentInChildren<ParticleSystem>();

            _isTyreTrailNull = _tyreTrail == null;
            _isDustParticlesNull = _dustParticles == null;
        }

        public void SetUpWheel(Rigidbody carRb)
        {
            _carRb = carRb;
            wheelPosition = transform.parent.localPosition.z > 0 ? WheelPosition.Front : WheelPosition.Back;
        }

        public void ProcessWheelVisuals(PlayerInputs input, VehicleSpeed vehicleSpeed)
        {
            _input = input;
            _vehicleSpeed = vehicleSpeed;
            UpdateWheelPosition();
            UpdateWheelRotations();
            UpdateWheelParticles();
            UpdateWheelTrails();
        }

        private void UpdateWheelPosition()
        {
            RaycastHit __hit;

            grounded = Physics.Raycast(_attachPoint.position, -_attachPoint.up, out __hit, suspensionMaxHeight);

            float __wheelExtension = suspensionMaxHeight;

            if (grounded)
            {
                __wheelExtension = __hit.distance;
            }

            transform.position = _attachPoint.position + ((__wheelExtension - wheelRadius) * -_attachPoint.up);
        }

        private void UpdateWheelRotations()
        {
            //In case tyre model is flipped
            int __rotationDirection = transform.parent.localScale.x > 0 ? 1 : -1;

            //Front wheel steering - Checks if the wheel is in front of the cars center of mass
            if (wheelPosition == WheelPosition.Front)
                transform.localRotation = Quaternion.Euler(0,
                    (_input.steeringInput * frontWheelTurnMaxAngle * __rotationDirection), 0);

            //Converts linear speed into angular speed and applies it to the wheels
            float __wheelCircumference = 2 * Mathf.PI * (wheelRadius / 2);
            float __angularVelocity = _vehicleSpeed.forwardSpeed / __wheelCircumference /** Time.deltaTime*/;
            //transform.GetChild(0).Rotate(0, angularVelocity, 0);
            transform.GetChild(0).Rotate(__angularVelocity, 0, 0);
        }

        private void UpdateWheelParticles()
        {
            if (_isDustParticlesNull)
                return;

            bool __isBackWheel = (wheelPosition == WheelPosition.Back);
            bool __isGoingFastEnough = (_vehicleSpeed.ForwardSpeedPercent > 0.5f &&
                                        _vehicleSpeed.forwardSpeed > _vehicleSpeed.sideSpeed);

            if (__isBackWheel &&
                __isGoingFastEnough &&
                grounded)
            {
                if (!_dustParticles.isPlaying)
                {
                    _dustParticles.Play();
                }

                return;
            }

            _dustParticles.Stop();
        }

        private void UpdateWheelTrails()
        {
            if (_isTyreTrailNull)
                return;

            bool __overSideSpeedThreshold = false;

            switch (wheelPosition)
            {
                case WheelPosition.Front:
                    __overSideSpeedThreshold =
                        Mathf.Abs(_vehicleSpeed.sideSpeed) > Mathf.Abs(_vehicleSpeed.forwardSpeed / 2);
                    break;
                case WheelPosition.Back:
                    __overSideSpeedThreshold =
                        Mathf.Abs(_vehicleSpeed.sideSpeed) > 5 /*Mathf.Abs(_vehicleSpeed.ForwardSpeed / 2)*/;
                    break;
            }

            _tyreTrail.transform.position = transform.position + wheelRadius * -_attachPoint.up;
            _tyreTrail.emitting = __overSideSpeedThreshold && _vehicleSpeed.forwardSpeed > 1 && grounded;
        }

    }

    public enum WheelPosition
    {
        Front,
        Back,
        Other
    }
}