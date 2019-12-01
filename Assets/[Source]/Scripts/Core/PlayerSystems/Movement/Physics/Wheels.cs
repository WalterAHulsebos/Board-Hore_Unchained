using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.CGTK;

using JetBrains.Annotations;

namespace Core.PlayerSystems.Movement
{
    [Serializable]
    public partial class Wheels : VehicleBehaviour
    {
        #region Variables
        
        //private Dictionary<Transform, WheelHitData> _mapWheelToLastHitCache = new Dictionary<Transform, WheelHitData>();

        [SerializeField] private float wheelHeight = 0.5f;
        [SerializeField] private LayerMask groundCheckLayer = 1 << 1;

        [SerializeField] private WheelsData wheelsData = new WheelsData(wheels: new Wheel[4]);
        
        #endregion

        #region Methods

        /// <summary> Gets you the wheel at Index i </summary>
        [PublicAPI]
        public Wheel this[in int i] => wheelsData.wheels[i];

        /*
        /// <summary> Guesses your wheels are assigned in order from up left to bottom right. </summary>
        [PublicAPI]
        public Wheel this[in int x, in int y] => wheelsData.wheels[x*2 + y*2];
        */

        protected override void Start()
        {
            base.Start();

            _vehicle.wheelsData = wheelsData;
        }

        private void FixedUpdate()
        {
            UpdateWheelStates();
            _vehicle.wheelsData = wheelsData;
        }

        private void UpdateWheelStates()
        {
            Vector3 __surfaceNormal = Vector3.zero;

            wheelsData.numberOfGroundedWheels = 0;
            wheelsData.anyGrounded = false;

            foreach(Wheel __wheel in wheelsData.wheels)
            {
                bool __isGrounded = __wheel.wheelController.isGrounded;
                
                __wheel.isGrounded = __isGrounded;
                //__wheel.GroundData = __hit;
                //CGDebug.DrawRay(__ray).Color(Color.yellow);
                
                if(!__isGrounded) continue;
                
                wheelsData.anyGrounded = true;
                wheelsData.numberOfGroundedWheels++;
                
                //__surfaceNormal += __hit.normal;
            }

            wheelsData.averageWheelSurfaceNormal = __surfaceNormal.normalized;
        }

        #endregion
    }
}