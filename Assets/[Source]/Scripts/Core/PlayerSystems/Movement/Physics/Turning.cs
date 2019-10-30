using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    using CommonGames.Utilities.Extensions;

    public class Turning : VehicleBehaviour
    {
        public float baseTurningForce = 20f;
        public float speedFactorOffset = 0.5f;
        
        [Space] 
        
        public float currentTurningForce;
        public Vector3 currentAngularVelocity = Vector3.zero;

        protected override void Start()
        {
            base.Start();
            
            currentTurningForce = baseTurningForce;
        }

        private void Update()
        {
            currentAngularVelocity = _vehicle.rigidbody.angularVelocity;
        }

        private void FixedUpdate()
        {
            if(!_vehicle.mayMove) return;

            ApplyTurningForce(
                _vehicle.InputData,
                _vehicle.SpeedData,
                _vehicle.rigidbody,
                _vehicle.wheelData.grounded
            );
        }

        //TODO Make a version of this that works with the Vel-Time Curve
        private void ApplyTurningForce(PlayerInputs input, VehicleSpeed speedData, Rigidbody rigidbody, bool grounded)
        {
            if (input.steeringInput.Approximately(0)) return;

            if (!grounded) return;

            //Adjusts turning with speed
            float __speedFactor = Mathf.Clamp01(speedData.SpeedPercent + speedFactorOffset);
            float __rotationTorque = input.steeringInput * baseTurningForce * __speedFactor * Time.fixedDeltaTime;

            //Apply the torque to the ship's Y axis
            rigidbody.AddRelativeTorque(0f, __rotationTorque, 0f, ForceMode.VelocityChange);
        }
    }

}