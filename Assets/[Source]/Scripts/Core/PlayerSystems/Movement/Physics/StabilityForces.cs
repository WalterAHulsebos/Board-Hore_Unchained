using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    [Serializable]
    public class StabilityForces : VehicleBehaviour
    {
        public float linearStabilityForce = 500;
        public float angularStabilityForce = 1500;

        private void FixedUpdate()
        {
            ApplyLinearStabilityForces(
                _vehicle.rigidbody,
                _vehicle.wheelData.physicsWheelPoints,
                _vehicle.wheelData.grounded,
                _vehicle.wheelData.numberOfGroundedWheels
            );

            ApplyAngularStabilityForces(
                _vehicle.rigidbody,
                _vehicle.averageColliderSurfaceNormal,
                _vehicle.wheelData.grounded
            );
        }

        private void ApplyLinearStabilityForces(Rigidbody rigidbody, Transform[] physicsWheelPoints, bool grounded,
            int numberOfGroundedWheels)
        {
            if (!(linearStabilityForce > 0) || !grounded || numberOfGroundedWheels >= 3) return;
            
            Vector3 __downwardForce = linearStabilityForce * Time.fixedDeltaTime * Vector3.down;
            foreach (Transform __wheel in physicsWheelPoints)
            {
                rigidbody.AddForceAtPosition(__downwardForce, __wheel.position, ForceMode.Acceleration);
            }
        }

        private void ApplyAngularStabilityForces(Rigidbody rigidbody, Vector3 averageColliderSurfaceNormal, bool grounded)
        {
            if (averageColliderSurfaceNormal == Vector3.zero || grounded) return;
            
            //Gets the angle in order to determine the direction the vehicle needs to roll
            Transform __transform = rigidbody.transform;
            Vector3 __forward = __transform.forward;
            float __angle = Vector3.SignedAngle(__transform.up, averageColliderSurfaceNormal, __forward);

            //Angular stability only uses roll - Using multiple axis becomes unpredictable 
            Vector3 __torqueAmount = Mathf.Sign(__angle) * Time.fixedDeltaTime * angularStabilityForce * __forward;

            rigidbody.AddTorque(__torqueAmount, ForceMode.Acceleration);
        }
    }
}