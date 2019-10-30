using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.Extensions;

namespace Core.PlayerSystems.Movement
{
    [Serializable]
    public class Drag : VehicleBehaviour
    {
        [Header("Drag")] public float linearDrag;
        public float freeWheelDrag = 1f;
        public float brakingDrag = 2f;
        public float angularDrag = 3f;

        public bool linearDragCheck = true;
        public bool brakingDragCheck = false;
        public bool freeWheelDragCheck = true;

        protected override void Start()
        {
            base.Start();
            
            _vehicle.rigidbody.angularDrag = angularDrag;
        }

        private void Update()
        {
            UpdateDrag(
                _vehicle.rigidbody,
                _vehicle.wheelData.grounded,
                _vehicle.InputData,
                _vehicle.SpeedData
            );

        }

        private void UpdateDrag(Rigidbody rb, bool grounded, PlayerInputs input, VehicleSpeed speedData)
        {
            linearDragCheck = input.accelInput.Abs() < 0.05 || grounded;
            float __linearDragToAdd = linearDragCheck ? linearDrag : 0;

            brakingDragCheck = (input.accelInput < 0) && speedData.forwardSpeed > 0;
            float __brakingDragToAdd = brakingDragCheck ? brakingDrag : 0;

            freeWheelDragCheck = input.accelInput.Abs() < 0.02f && grounded;
            float __freeWheelDragToAdd = freeWheelDragCheck ? freeWheelDrag : 0;

            rb.drag = __linearDragToAdd + __brakingDragToAdd + __freeWheelDragToAdd;
        }
    }
}