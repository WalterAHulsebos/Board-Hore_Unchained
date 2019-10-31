using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#endif

using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;
using CommonGames.Utilities.CGTK;

namespace Core.PlayerSystems.Movement.Abilities
{
    public class JumpAbility : BaseAbility
    {
        #region Variables

        [SerializeField] private float heightMin = 1f, heightMax = 15f;
        
        [SerializeField] private float maxChargeDuration = 1f;

        [ProgressBar(0f, 100f)]
        [HideLabel, ShowInInspector]
        private float CurrentCharge => _currentCharge;

        [SerializeField] private float _currentCharge = 0f;
        
        private bool 
            _holdingJump,
            _stoppedHoldingJump;

        private List<Action> _actions;

        #endregion

        #region Methods

        private void Awake()
        {
            abilityAction.started += __StartedJump;
            abilityAction.canceled += __StoppedJump;


            void __StartedJump(InputAction.CallbackContext ctx)
            {
                if(_vehicle.mayMove > 0) return;
                
                _holdingJump = true;
            }
            void __StoppedJump(InputAction.CallbackContext ctx)
            {
                _holdingJump = false;
                _stoppedHoldingJump = true;
                    
                if(_vehicle.mayMove > 0) return;
                    
                DoAbility();
            }
        }

        private void Update()
        {
            if(!_holdingJump) return;
            
            _currentCharge += (100f / maxChargeDuration) * Time.deltaTime;

            _currentCharge.Clamp(0f, 100f);
        }

        private void FixedUpdate()
        {
            //DoAbility();
        }

        public override void CheckInput()
        {
            
        }

        public override void DoAbility()
        {
            //if (!_holdingJump) return;
            //_holdingJump = false;

            _stoppedHoldingJump = false;
            
            //if (!_vehicle.wheelData.grounded) return;

            Rigidbody __rigidbody = _vehicle.rigidbody;

            float __force = Mathf.Lerp(heightMin, heightMax, _currentCharge / 100f);
            
            __rigidbody.AddForceAtPosition(__force * __rigidbody.transform.up, __rigidbody.position, ForceMode.Impulse);

            _currentCharge = default;

        }

        #endregion

    }
}