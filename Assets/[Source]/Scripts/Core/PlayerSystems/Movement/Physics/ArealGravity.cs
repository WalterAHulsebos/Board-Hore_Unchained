using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.Extensions;

namespace Core.PlayerSystems.Movement
{
    public class ArealGravity : VehicleBehaviour
    {
        #region Variables

        [SerializeField] private float airGravityMultiplier = 2f;

        private float _defaultGravity = default;
        
        #endregion

        #region Methods
        
        protected override void Start()
        {
            base.Start();

            _defaultGravity = UnityEngine.Physics.gravity.y;
        }

        private void FixedUpdate()
        {
            if(_vehicle.Grounded)
            {
                UnityEngine.Physics.gravity = new Vector3(0, _defaultGravity, 0);
            }
            else
            {
                UnityEngine.Physics.gravity = new Vector3(0, _defaultGravity * airGravityMultiplier, 0);
            }
        }

        #endregion
    }
}