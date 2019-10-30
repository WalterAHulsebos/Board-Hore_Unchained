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
        //public InputAction lookAction;

        [Space] 
        [ShowInInspector] private float _accelerateAxis;
        [ShowInInspector] private float _brakingAxis;

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
        {
            HandleInputs();
        }

        private void HandleInputs()
        {
            _moveInput = moveAction.ReadValue<Vector2>();
            
            //Forward/Reverse
            //accelerateAxis = InputData.GetAxis("Vertical");
            _vehicle.InputData.accelInput = _moveInput.y;

            //Steering
            _vehicle.InputData.steeringInput = _moveInput.x;
        }
        
        #endregion
    }

    [Serializable]
    public class PlayerInputs
    {
        public float accelInput;
        public float steeringInput;
    }
}
