﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    using CommonGames.Utilities.Extensions;

    using static Wheels;
    
    public class Steering : VehicleBehaviour
    {
        #region Variables

        [SerializeField] private float
            maxSteeringAngle = 35,
            minSteeringAngle = 20,
            
            steeringTime = 0.02f,
            angleIncrease = 0.05f;
        
        private WheelsData _wheelsData;

        private float 
            _steeringInput,
            _smoothedSteeringInput,
            _steeringVelocity;
        
        #endregion

        #region Methods
        
        protected override void Start()
        {
            base.Start();
            
            //_vehicle.SpeedData = new VehicleSpeed(velocityTimeCurve.keys[velocityTimeCurve.length - 1].value);

            _wheelsData = _vehicle.wheelsData;
        }

        private void Update()
        {
            //_steeringInput = Input.GetAxis("Horizontal");
            _steeringInput = _vehicle.InputData.steeringInput;
        }

        private void FixedUpdate()
        {
            if(!_vehicle.mayMove) return;

            _wheelsData = _vehicle.wheelsData;
            
            ApplySteeringForce();
        }

        //TODO Make a version of this that works with the Vel-Time Curve
        private void ApplySteeringForce()
        {
            //if(_vehicle.InputData.steeringInput.Approximately(0)) return;
            if(!_vehicle.Grounded) return;

            float __velocity = transform.InverseTransformDirection(this.GetComponent<Rigidbody>().velocity).z;
            
            _smoothedSteeringInput = Mathf.SmoothDamp(_smoothedSteeringInput, _steeringInput, ref _steeringVelocity, steeringTime);
            
            foreach(Wheel __wheel in _wheelsData.wheels)
            {
                if(__wheel.steer)
                {
                    __wheel.wheelController.steerAngle =
                        Mathf.Lerp(a: maxSteeringAngle, b: minSteeringAngle, t: __velocity.Abs() * angleIncrease) * _smoothedSteeringInput;
                }
            }
        }

        #endregion
        
    }

}