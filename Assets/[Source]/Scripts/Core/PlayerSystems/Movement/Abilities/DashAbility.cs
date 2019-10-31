using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#endif

using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;
using CommonGames.Utilities.CGTK;

namespace Core.PlayerSystems.Movement.Abilities
{
    public class DashAbility : BaseAbility
    {
        #region Variables
        
        [SerializeField] private float dashForce = 30f;

        //public AnimationCurve BoostCurve;
        [SerializeField] private float boostRechargeRate = 0.5f;

        private Acceleration _acceleration;

        private bool _readyForDash = false;

        public bool IsBoosting { get; private set; }

        #endregion
        
        #region Methods
        
        private void Awake()
        {
            abilityAction.started +=
                ctx =>
                {
                    _readyForDash = true;
                };
            abilityAction.canceled +=
                ctx =>
                {
                    //_readyForDash = true;
                    //DoAbility();
                };
        }

        protected override void Start()
        {
            base.Start();
            
            _acceleration = GetComponentInChildren<Acceleration>();
        }

        private void Update()
        {
            //UpdateAbility();
        }

        private void FixedUpdate()
        {
            if(!_vehicle.mayMove) return;
            
            if(!_readyForDash) return;
            
            DoAbility();
        }

        public override void CheckInput() { }

        public override void DoAbility()
        {
            if (!_vehicle.wheelData.grounded) return;
            
            _readyForDash = false;
            
            Rigidbody __rigidbody = _vehicle.rigidbody;
            Transform __transform = __rigidbody.transform;
            
            Vector3 __force = Time.fixedDeltaTime * dashForce * __transform.right;
            
            __rigidbody.AddForceAtPosition(3f * __transform.up, __rigidbody.position, ForceMode.Impulse);
            __rigidbody.AddForce(__force, ForceMode.Impulse);
        }

        
        #endregion
    }
}
