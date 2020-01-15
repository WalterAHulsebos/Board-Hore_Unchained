using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.CGTK;

using JetBrains.Annotations;

using CommonGames.Utilities.Extensions;

#if ODIN_INSPECTOR

using Sirenix.OdinInspector;

#endif

namespace Core.PlayerSystems.Movement
{
    using static Wheels;
    
    public partial class Acceleration : VehicleBehaviour
    {
        #region Variables

        [SerializeField] private float
            maxMotorTorque = 1500,
            maxBrakeTorque = 900;
            //,antiRollBarForce = 30000;
        
        //[SerializeField] private AnimationCurve velocityTimeCurve;

        [HideInInspector] public float velocity;

        [ReadOnly]
        [SerializeField] private float
            _roundedForwardSpeed = 0,
            _roundedForwardSpeedLastFrame = 0;

        private float 
            _forwardSpeed = 0, 
            _forwardSpeedLastFrame = 0; 

        private float _smoothXAxis;
        private float _xAxisVelocity;
        #endregion

        #region Methods

        private void FixedUpdate()
        {
            CalculateSpeedData(_vehicle.SpeedData);
            
            velocity = transform.InverseTransformDirection(direction: _vehicle.rigidbody.velocity).z;
            
            _smoothXAxis = Mathf.SmoothDamp(
                current: _smoothXAxis, 
                target: _vehicle.InputData.steeringInput, 
                currentVelocity: ref _xAxisVelocity, 
                smoothTime:0.12f);

            foreach(Wheel __wheel in _vehicle.wheelsData.wheels)
            {
                //__wheel.wheelController.brakeTorque = Input.GetKey(key: KeyCode.Space) ? maxBrakeTorque : 0.0f;

                __wheel.wheelController.brakeTorque = 0f;
                
                if(Mathf.Sign(velocity) < 0.1f && _vehicle.InputData.accelerationInput > 0.1f)
                {
                    __wheel.wheelController.brakeTorque = maxBrakeTorque;
                }
                
                if(!__wheel.power) continue;
                
                __wheel.wheelController.motorTorque = maxMotorTorque * _vehicle.InputData.accelerationInput;
            }
        }
        private void CalculateSpeedData(VehicleSpeed speedData)
        {
            //Debug.Log($"VEHICLE IS NULL = {_vehicle == null}");
            //Debug.Log($"RIGIDBODY IS NULL = {_vehicle.rigidbody == null}");
            //Debug.Log($"SPEEDDATA IS NULL = {_vehicle.SpeedData == null}");
            
            if(!_vehicle.Grounded) return;

            speedData.sideSpeed = Vector3.Dot(_vehicle.rigidbody.transform.right, _vehicle.rigidbody.velocity);
            _forwardSpeed = speedData.forwardSpeed = Vector3.Dot(_vehicle.rigidbody.transform.forward, _vehicle.rigidbody.velocity);
            speedData.speed = _vehicle.rigidbody.velocity.magnitude;

            _roundedForwardSpeed = Mathf.Round(speedData.forwardSpeed * 100) / 100f;
            
            //if(speedData.forwardSpeed >= 0.06)
            if(((_roundedForwardSpeed > _roundedForwardSpeedLastFrame) 
               || (_vehicle.InputData.accelerationInput >= 0.05))
                && _roundedForwardSpeed > 1)
            {
                _vehicle.InvokeAccelerate();
            }
            else if((_roundedForwardSpeed < _roundedForwardSpeedLastFrame)
                    && _roundedForwardSpeed > 1)
            {
                _vehicle.InvokeDecelerate();
            }
            else if(_roundedForwardSpeed.Approximately(_roundedForwardSpeedLastFrame) 
                    && _roundedForwardSpeed > 1)
            {
                _vehicle.InvokeCruise();
            }
            else if(_roundedForwardSpeed <= 1)
            {
                _vehicle.InvokeIdle();
            }
            else
            {
                _vehicle.InvokeIdle();
            }
            
            //Debug.Log($"<color=red> Rounded Forward Speed = {__roundedForwardSpeed} </color>");

            _forwardSpeedLastFrame = speedData.forwardSpeed;
            _roundedForwardSpeedLastFrame = Mathf.Round(speedData.forwardSpeed * 100) / 100f;
        }
        
        #endregion
    }
}