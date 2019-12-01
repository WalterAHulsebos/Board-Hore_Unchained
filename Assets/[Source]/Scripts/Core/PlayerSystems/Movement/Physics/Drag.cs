using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.Extensions;

namespace Core.PlayerSystems.Movement
{
    public class Drag : VehicleBehaviour
    {
        #region Variables
        
        [SerializeField] private float 
            linearDrag = 0.1f,
            freeWheelDrag = 1f,
            brakingDrag = 2f,
            angularDrag = 3f;
        
        [SerializeField] private bool 
            linearDragCheck = true,
            brakingDragCheck = false,
            freeWheelDragCheck = true;

        #endregion

        #region Methods
        
        protected override void Start()
        {
            base.Start();
            
            _vehicle.rigidbody.angularDrag = angularDrag;
        }

        private void Update()
        {
            UpdateDrag(
                _vehicle.rigidbody,
                _vehicle.wheelsData.anyGrounded,
                _vehicle.InputData,
                _vehicle.SpeedData
            );
        }

        private void UpdateDrag(Rigidbody rb, bool grounded, PlayerInputs input, VehicleSpeed speedData)
        {
            linearDragCheck = input.accelerationInput.Abs() < 0.05 || grounded;
            float __linearDragToAdd = linearDragCheck ? linearDrag : 0;

            
            brakingDragCheck = (input.accelerationInput < 0) && speedData.forwardSpeed > 0;
            float __brakingDragToAdd = brakingDragCheck ? brakingDrag : 0;
            

            freeWheelDragCheck = input.accelerationInput.Abs() < 0.02f && grounded;
            float __freeWheelDragToAdd = freeWheelDragCheck ? freeWheelDrag : 0;
            

            rb.drag = __linearDragToAdd + __brakingDragToAdd + __freeWheelDragToAdd;
        }
        
        #endregion
    }
}