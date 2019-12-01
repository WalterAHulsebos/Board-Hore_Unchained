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
using CommonGames.Utilities.Helpers;
using CommonGames.Utilities.CGTK;
using CommonGames.Utilities.Helpers;

namespace Core.PlayerSystems.Movement.Abilities
{
    public class JumpAbility : BaseAbility
    {
        #region Variables

        [SerializeField] private float heightMin = 1f, heightMax = 15f;

        [SerializeField] private float jumpTime = 0.5f;
        
        [SerializeField] private float maxChargeDuration = 1f;

        [ProgressBar(0f, 100f)]
        [HideLabel, ShowInInspector]
        private float CurrentCharge { get; set; } = 0f;

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

        public override void AbilityUpdate()
        {
            base.AbilityUpdate();

            if(!_holdingJump) return;
            
            CurrentCharge += (100f / maxChargeDuration) * Time.deltaTime;

            CurrentCharge.Clamp(0f, 100f);
        }

        public void CheckInput()
        {
            
        }

        public override void DoAbility()
        {
            //if (!_holdingJump) return;
            //_holdingJump = false;

            _stoppedHoldingJump = false;
            
            //if (!_vehicle.wheelsData.grounded) return;

            Rigidbody __rigidbody = _vehicle.rigidbody;

            float __jumpHeight = Mathf.Lerp(heightMin, heightMax, CurrentCharge / 100f);

            float __jumpForce = SuvatHelper.GetInitialVelocityNoA(s_displacement: __jumpHeight, v_finalVelocity: 0, t_time: jumpTime);
            
            __rigidbody.AddForceAtPosition(__jumpForce * __rigidbody.transform.up, __rigidbody.position, ForceMode.Impulse);

            CurrentCharge = default;
        }

        #endregion

    }
}