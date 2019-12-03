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
                _vehicle.wheelsData.wheels,
                _vehicle.wheelsData.anyGrounded,
                _vehicle.wheelsData.numberOfGroundedWheels
            );

            ApplyAngularStabilityForces(
                _vehicle.rigidbody,
                _vehicle.averageColliderSurfaceNormal,
                _vehicle.wheelsData.anyGrounded
            );
        }

        private void ApplyLinearStabilityForces(Rigidbody rigidbody, Wheels.Wheel[] physicsWheelPoints, bool grounded,
            int numberOfGroundedWheels)
        {
            if (!(linearStabilityForce > 0) || !grounded || numberOfGroundedWheels >= 3) return;
            
            Vector3 __downwardForce = linearStabilityForce * Time.fixedDeltaTime * Vector3.down;
            
            foreach (Wheels.Wheel __wheel in physicsWheelPoints)
            {
                __wheel.wheelController.GetWorldPose(out Vector3 __wheelPosition, out Quaternion __wheelRotation);
                
                rigidbody.AddForceAtPosition(__downwardForce, __wheelPosition, ForceMode.Acceleration);
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