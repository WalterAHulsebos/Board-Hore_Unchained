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
            _wannaDrift,
            _isDrifting;

        private LateralFriction _lateralFriction;
        
        #endregion

        #region Methods
        
        private void Awake()
        {
            /*
            abilityAction.performed +=
                ctx => 
                {
                    if (ctx.interaction is SlowTapInteraction)
                    {
                        _wannaDrift = true;
                    }
                };
                */
            
            abilityAction.started +=
                ctx =>
                {
                    _wannaDrift = true;
                };
            abilityAction.canceled +=
                ctx =>
                {
                    _wannaDrift = false;
                };
        }

        protected override void Start()
        {
            base.Start();
            
            _lateralFriction = GetComponentInChildren<LateralFriction>();
        }

        private void Update()
        {
            if(!_vehicle.mayMove) return;

            CheckInput();
            UpdateAbility();
            //DoAbility();
        }

        public override void CheckInput()
        {
            if (_wannaDrift && _vehicle.wheelData.grounded)
            {
                _isDrifting = true;
            }
            else
            {
                _isDrifting = false;
            }
        }

        private void UpdateAbility()
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
                _vehicle.wheelData.grounded
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