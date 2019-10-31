using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;
using CommonGames.Utilities.CGTK;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using JetBrains.Annotations;

namespace Core.PlayerSystems.Movement
{
    public class VehicleCore : Singleton<VehicleCore>
    {
        #region Variables
        
        [OdinSerialize] public PlayerInputs InputData { get; set; }
        [OdinSerialize] public VehicleSpeed SpeedData { get; set; }
        
        public WheelData wheelData;

        [HideInInspector] public new Rigidbody rigidbody;
        
        [HideInInspector] public Vector3 averageColliderSurfaceNormal;
        
        public static event Action<VehicleCore> LeavingGround_Event = vehicleCore => { };
        public static event Action<VehicleCore> Landing_Event = vehicleCore => { };
        
        [PublicAPI]
        [HideInInspector] public CGLock mayMove = new CGLock(0);
        
        private bool _prevGroundedState;
        
        #endregion

        private void Start()
        {
            rigidbody = GetComponentInChildren<Rigidbody>();
        }

        private void Update()
        {
            switch (_prevGroundedState)
            {
                case false when wheelData.grounded:
                    Landing_Event(obj: this);
                    break;
                case true when !wheelData.grounded:
                    LeavingGround_Event(obj: this);
                    break;
            }

            _prevGroundedState = wheelData.grounded;
        }

        [PublicAPI]
        public void SetPosition(in Vector3 position)
        {
            transform.position = position;
        }
        
        
        [PublicAPI]
        public void SetPositionAndRotation(in Vector3 position, in Quaternion rotation = default)
        {
            transform.SetPositionAndRotation(position: position, rotation: rotation);
        }
    }
}