using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    [System.Serializable]
    public class LateralFriction : VehicleBehaviour
    {
        public AnimationCurve slideFrictionCurve;
        [Range(0, 1)] public float baseTireStickiness;
        [Space] public float currentTireStickiness;
        [Space] public float slidingFrictionRatio;
        public float slidingFrictionForceAmount;
        public float slidingFrictionToForwardSpeedAmount;
        
        private void LateUpdate()
        {
            CalculateLateralFriction(_vehicle.SpeedData);
        }

        private void FixedUpdate()
        {
            ApplyLateralFriction(_vehicle.wheelData.grounded, _vehicle.rigidbody);
        }

        private void CalculateLateralFriction(VehicleSpeed speedData)
        {
            float __slideFrictionRatio = 0;

            if (Math.Abs(speedData.sideSpeed + speedData.forwardSpeed) > 0.01f)
                __slideFrictionRatio = Mathf.Clamp01(Mathf.Abs(speedData.sideSpeed) /
                                                   (Mathf.Abs(speedData.sideSpeed) + speedData.forwardSpeed));

            slidingFrictionRatio = slideFrictionCurve.Evaluate(__slideFrictionRatio);

            //TODO: Factor in surface normal - will make car more slippery non-horizontal surfaces

            slidingFrictionForceAmount = slidingFrictionRatio * -speedData.sideSpeed * currentTireStickiness;
        }

        private void ApplyLateralFriction(bool grounded, Rigidbody rb)
        {
            if (!grounded) return;

            //Stops sideways sliding 
            rb.AddForce(slidingFrictionForceAmount * rb.transform.right, ForceMode.Impulse);
            currentTireStickiness = baseTireStickiness;
        }
    }
}