using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

namespace Core.PlayerSystems.Movement.Abilities
{
    public class DriftAbility : BaseAbility
    {
        #region Variables
        
        [SerializeField] private AnimationCurve driftCurve = AnimationCurve.Linear(timeStart: 0, valueStart: 0, timeEnd: 1, valueEnd: 1);

        [SerializeField] private float
            driftInDuration = 0.5f,
            driftOutDuration = 1f;

        private float 
            _currentDriftFactor,
            _currentDriftTime;
        
        private bool 
            _driftInput,
            _isDrifting;

        private LateralFriction _lateralFriction;
        
        #endregion

        #region Methods

        #region Unity Event Functions

        private void Awake()
        {
            abilityAction.started +=
                ctx =>
                {
                    _driftInput = true;
                };
            abilityAction.canceled +=
                ctx =>
                {
                    _driftInput = false;
                };
        }

        public override void Initialize()
        {
            _lateralFriction = _vehicle.GetComponentInChildren<LateralFriction>();
        }

        #endregion

        public override void AbilityUpdate()
        {
            base.AbilityUpdate();
            
            if(!_vehicle.mayMove) return;

            CheckInput();
            Drift();
        }

        public void CheckInput()
        {
            if (_driftInput && _vehicle.wheelsData.anyGrounded)
            {
                _isDrifting = true;
            }
            else
            {
                _isDrifting = false;
            }
        }

        private void Drift()
        {
            if (_isDrifting)
            {
                _currentDriftTime += Time.deltaTime * 1 / driftInDuration;
            }
            else if (_currentDriftTime > 0)
            {
                _currentDriftTime -= Time.deltaTime * 1 / driftOutDuration;
            }

            _currentDriftTime = Mathf.Clamp01(_currentDriftTime);
            _currentDriftFactor = driftCurve.Evaluate(_currentDriftTime);

            _lateralFriction.currentTireStickiness = _lateralFriction.baseTireStickiness * _currentDriftFactor;
        }

        public override void DoAbility()
        {
            bool __belowBaseTireStickiness =
                (_lateralFriction.currentTireStickiness < _lateralFriction.baseTireStickiness);

            if (__belowBaseTireStickiness &&
                !_isDrifting &&
                _vehicle.wheelsData.anyGrounded
            )
            {
                //This is to try recover some lost speed while drifting
                _vehicle.rigidbody.AddForce(
                    Mathf.Abs(_lateralFriction.slidingFrictionForceAmount) * _vehicle.rigidbody.transform.forward,
                    ForceMode.Acceleration);
            }
        }
        
        #endregion
        
    }
}