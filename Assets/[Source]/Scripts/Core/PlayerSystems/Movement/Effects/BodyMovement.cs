using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    public class BodyMovement : VehicleBehaviour
    {
        public Transform modelBase;
        public Vector3 modelBaseOffset;

        public BodyAxisMovement roll;
        public BodyAxisMovement pitch;

        private void Update()
        {
            /*
            if(_vehicle.mayMove <= 0)
            {
                UpdateCarBodyTransform();
            }
            */
            
            UpdateCarBodyTransform();
        }

        private void UpdateCarBodyTransform()
        {
            Transform __transform = transform;
            __transform.position = _vehicle.rigidbody.position;
            __transform.rotation = _vehicle.rigidbody.rotation;

            if(_vehicle.mayMove) return;

            roll.currentAngle = Mathf.Lerp(roll.currentAngle, roll.inputMaxAngle * _vehicle.InputData.steeringInput,Time.deltaTime * 10);

            float __currentBodyRoll = (roll.currentAngle - _vehicle.SpeedData.SideSpeedPercent * roll.speedMaxAngle) * _vehicle.SpeedData.SpeedPercent;

            float __currentBodyPitch = pitch.currentAngle + (Mathf.Clamp01(_vehicle.SpeedData.ForwardSpeedPercent) * pitch.speedMaxAngle);

            modelBase.rotation = _vehicle.rigidbody.rotation * Quaternion.Euler(__currentBodyPitch, 0, __currentBodyRoll);
        }
    }

    [System.Serializable]
    public class BodyAxisMovement
    {
        public float currentAngle;
        public float inputMaxAngle;
        public float speedMaxAngle;
    }
}