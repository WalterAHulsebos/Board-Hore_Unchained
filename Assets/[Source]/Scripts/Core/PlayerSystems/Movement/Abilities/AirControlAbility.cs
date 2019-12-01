using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
//using UnityEngine.PlayerLoop;

namespace Core.PlayerSystems.Movement.Abilities
{
    public class AirControlAbility : BaseAbility
    {
        #region Variables
        
        [SerializeField] private float airControlFactor;
        [SerializeField] private Vector3 turnAxis;
        [SerializeField] private float torqueAmount;
        
        [HideInInspector] public float currentInput = 0;

        #endregion

        #region Methods

        public override void AbilityFixedUpdate()
        {
            DoAbility();
        }

        public override void AbilityUpdate()
        {
            currentInput = abilityAction.ReadValue<float>();
        }
        
        public override void DoAbility()
        {
            if (Math.Abs(currentInput) < 0.01f) return;

            if (_vehicle.wheelsData.anyGrounded || (_vehicle.averageColliderSurfaceNormal != Vector3.zero)) return;

            float __rotationTorque = currentInput * torqueAmount * Time.fixedDeltaTime * airControlFactor;
            
            _vehicle.rigidbody.AddRelativeTorque(turnAxis * __rotationTorque, ForceMode.VelocityChange);
        }
        
        #endregion
        
    }
}