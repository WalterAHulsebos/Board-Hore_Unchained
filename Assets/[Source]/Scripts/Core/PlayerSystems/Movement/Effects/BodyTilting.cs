using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    public class BodyTilting : VehicleBehaviour
    {
        #region Variables

        [SerializeField] private Transform modelBase;

        [SerializeField] private BodyAxisMovement roll;
        [SerializeField] private BodyAxisMovement pitch;

        #endregion

        #region Methods

        private void Update()
        {
            if(_vehicle.mayMove) return;
            
            UpdateCarBodyTilt();
        }

        private void UpdateCarBodyTilt()
        {
            roll.currentAngle = Mathf.Lerp(roll.currentAngle, roll.inputMaxAngle * _vehicle.InputData.steeringInput,Time.deltaTime * 10);

            float __currentBodyRoll = (roll.currentAngle - _vehicle.SpeedData.SideSpeedPercent * roll.speedMaxAngle) * _vehicle.SpeedData.SpeedPercent;

            float __currentBodyPitch = pitch.currentAngle + (Mathf.Clamp01(_vehicle.SpeedData.ForwardSpeedPercent) * pitch.speedMaxAngle);

            modelBase.rotation = _vehicle.rigidbody.rotation * Quaternion.Euler(__currentBodyPitch, 0, __currentBodyRoll);
        }
        
        #endregion
    }
}