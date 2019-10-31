using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.CGTK;

namespace Core.PlayerSystems.Movement
{
    [System.Serializable]
    public class Wheels : VehicleBehaviour
    {
        private Dictionary<Transform, WheelHitData> _mapWheelToLastHitCache = new Dictionary<Transform, WheelHitData>();

        private class WheelHitData
        {
            public bool IsGrounded;
            public RaycastHit GroundData;
        }
        
        public float wheelHeight = 0.5f;
        public LayerMask groundCheckLayer = 1 << 1;
        public WheelData wheelData;


        protected override void Start()
        {
            base.Start();
            
            foreach (Transform __wheel in wheelData.physicsWheelPoints)
            {
                _mapWheelToLastHitCache[__wheel] = new WheelHitData();
            }
        }

        private void Update()
        {
            UpdateWheelStates();
            _vehicle.wheelData = wheelData;
        }

        private void UpdateWheelStates()
        {
            Vector3 __surfaceNormal = Vector3.zero;

            wheelData.numberOfGroundedWheels = 0;
            wheelData.grounded = false;

            foreach (Transform __wheel in wheelData.physicsWheelPoints)
            {
                Ray __ray = new Ray(__wheel.position, -__wheel.transform.up);
                
                bool __grounded = Physics.Raycast(__ray, out RaycastHit __hit, wheelHeight, groundCheckLayer);
                
                CGDebug.DrawRay(__ray).Color(Color.yellow);

                WheelHitData __wheelHitData = _mapWheelToLastHitCache[__wheel];

                __wheelHitData.IsGrounded = __grounded;
                __wheelHitData.GroundData = __hit;

                if (!__grounded) continue;

                wheelData.grounded = true;
                wheelData.numberOfGroundedWheels += 1;

                __surfaceNormal += __hit.normal;
            }

            wheelData.averageWheelSurfaceNormal = __surfaceNormal.normalized;
        }
    }

    [Serializable]
    public class WheelData
    {
        public Transform[] physicsWheelPoints;
        public bool grounded;
        public int numberOfGroundedWheels;
        public Vector3 averageWheelSurfaceNormal;
    }
}