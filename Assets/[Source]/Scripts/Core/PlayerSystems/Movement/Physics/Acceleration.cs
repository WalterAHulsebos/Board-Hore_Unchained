using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.CGTK;

using JetBrains.Annotations;

using CommonGames.Utilities.Extensions;

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
        
        private float _smoothXAxis;
        private float _xAxisVelocity;

        private Rigidbody _rigidbody;
        private WheelsData _wheelsData;
        
        #endregion

        #region Methods

        protected override void Start()
        {
            base.Start();

            _rigidbody = _vehicle.rigidbody;
            
            //_vehicle.SpeedData = new VehicleSpeed(velocityTimeCurve.keys[velocityTimeCurve.length - 1].value);

            _wheelsData = _vehicle.wheelsData;
        }

        private void Update()
        {
            CalculateSpeedData(_vehicle.SpeedData);
            
            _wheelsData = _vehicle.wheelsData;
            
            //_accelerationToApply = GetAccelerationFromVelocityTimeCurve(velocityTimeCurve, _vehicle.InputData, _vehicle.SpeedData);
        }

        private void FixedUpdate()
        {
            velocity = transform.InverseTransformDirection(direction: GetComponent<Rigidbody>().velocity).z;
            
            _smoothXAxis = Mathf.SmoothDamp(
                current: _smoothXAxis, 
                target: _vehicle.InputData.steeringInput, 
                currentVelocity: ref _xAxisVelocity, 
                smoothTime:0.12f);

            foreach(Wheel __wheel in _wheelsData.wheels)
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
            speedData.sideSpeed = Vector3.Dot(_rigidbody.transform.right, _rigidbody.velocity);
            speedData.forwardSpeed = Vector3.Dot(_rigidbody.transform.forward, _rigidbody.velocity);
            speedData.speed = _rigidbody.velocity.magnitude;
        }
        
        #endregion
    }
}