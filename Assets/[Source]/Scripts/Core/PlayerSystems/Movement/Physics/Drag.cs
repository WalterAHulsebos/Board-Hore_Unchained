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
            aerialDrag = 0.5f,
            angularDrag = 3f;
        
        private bool 
            linearDragCheck,
            brakingDragCheck,
            freeWheelDragCheck;

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
            float __combinedDrag = 0f;
            
            linearDragCheck = (input.accelerationInput.Abs() < 0.05) || grounded;
            
            //If We're braking
            brakingDragCheck = (input.accelerationInput < 0) && speedData.forwardSpeed > 0;
            
            //If not giving any input.
            freeWheelDragCheck = input.accelerationInput.Abs() < 0.02f && grounded;
            
            __combinedDrag += linearDragCheck ? linearDrag : 0;
            __combinedDrag += brakingDragCheck ? brakingDrag : 0;
            __combinedDrag += freeWheelDragCheck ? freeWheelDrag : 0;
            __combinedDrag += !grounded ? aerialDrag : 0;

            rb.drag = __combinedDrag;
        }
        
        #endregion
    }
}