using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Sirenix.OdinInspector;

namespace Core.PlayerSystems.Movement
{
    public class VehicleInput : VehicleBehaviour
    {
        #region Variables
        
        public InputAction moveAction;

        private Vector2 _moveInput;
        
        #endregion

        #region Methods

        public void OnEnable()
        {
            moveAction.Enable();
        }

        public void OnDisable()
        {
            moveAction.Disable();
        }

        protected override void Start()
        {
            base.Start();
            _vehicle.InputData = new PlayerInputs();
        }
        
        private void Update() 
            => HandleInputs();

        private void HandleInputs()
        {
            _moveInput = moveAction.ReadValue<Vector2>();
            
            //Forward/Reverse
            _vehicle.InputData.accelerationInput = _moveInput.y;

            //Steering
            _vehicle.InputData.steeringInput = _moveInput.x;
        }
        
        #endregion
    }

    [Serializable]
    public class PlayerInputs
    {
        public float accelerationInput;
        public float steeringInput;
    }
}
