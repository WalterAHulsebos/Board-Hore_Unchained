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

        #region Serialized
        
        [ReadOnly]
        [NonSerialized]
        [OdinSerialize] public Wheels.WheelsData wheelsData;
        
        [ReadOnly]
        [OdinSerialize] public PlayerInputs InputData { get; set; }
        
        [ReadOnly]
        [OdinSerialize] public VehicleSpeed SpeedData { get; set; }

        [ReadOnly]
        [LabelText("Grounded")]
        [SerializeField] private bool groundedDebug = false;

        [PublicAPI]
        public bool Grounded => wheelsData.anyGrounded;

        #endregion
        
        #region Non-Serialized
        
        [HideInInspector] public new Rigidbody rigidbody;
        
        [HideInInspector] public Vector3 averageColliderSurfaceNormal;

        public static event Action<VehicleCore> LeavingGround_Event = vehicleCore => { };
        public static event Action<VehicleCore> Landing_Event = vehicleCore => { };
        
        [PublicAPI]
        [HideInInspector] public CGLock mayMove = new CGLock(0);
        
        private bool _prevGroundedState;
        
        #endregion
        
        #endregion

        #region Methods

        private void Start()
        {
            rigidbody = GetComponentInChildren<Rigidbody>();
        }

        private void Update()
        {
            groundedDebug = Grounded;
            
            switch (_prevGroundedState)
            {
                case false when wheelsData.anyGrounded:
                    Landing_Event(obj: this);
                    break;
                case true when !wheelsData.anyGrounded:
                    LeavingGround_Event(obj: this);
                    break;
            }

            _prevGroundedState = wheelsData.anyGrounded;
        }

        [PublicAPI]
        public void SetPosition(in Vector3 position)
        {
            transform.position = position;
        }
        
        [PublicAPI]
        public void SetRotation(in Quaternion rotation)
        {
            transform.rotation = rotation;
        }
        
        [PublicAPI]
        public void SetPositionAndRotation(in Vector3 position, in Quaternion rotation = default)
        {
            transform.SetPositionAndRotation(position: position, rotation: rotation);
        }
        
        #endregion
    }
}