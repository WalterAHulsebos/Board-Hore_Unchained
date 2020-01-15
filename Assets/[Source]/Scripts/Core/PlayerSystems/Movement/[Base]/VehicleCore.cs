using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonGames.Utilities.CustomTypes;
using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;
using CommonGames.Utilities.Helpers;
using CommonGames.Utilities.CGTK;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using JetBrains.Annotations;

namespace Core.PlayerSystems.Movement
{
    public class VehicleCore : IndexedMultiton<VehicleCore>
    {
        #region Variables

        #region Serialized
        
        [ReadOnly]
        [NonSerialized]
        [OdinSerialize] public Wheels.WheelsData wheelsData = new Wheels.WheelsData();
        
        [ReadOnly]
        [OdinSerialize] public PlayerInputs InputData { get; set; }
        
        [ReadOnly]
        [OdinSerialize] public VehicleSpeed SpeedData { get; set; } = new VehicleSpeed();

        [ReadOnly]
        [LabelText("Grounded")]
        [SerializeField] private bool groundedDebug = false;

        [PublicAPI]
        public bool Grounded => wheelsData.anyGrounded;

        #endregion
        
        #region Non-Serialized
        
        //[HideInInspector]
        [ReadOnly]
        public new Rigidbody rigidbody;
        
        [HideInInspector] public Vector3 averageColliderSurfaceNormal;

        public event Action 
            LeavingGround_Event,
            Landing_Event,
            Accelerating_Event,
            Decelerating_Event,
            Idle_Event,
            Cruise_Event;
        
        [PublicAPI]
        [HideInInspector] public CGLock mayMove = new CGLock(0);
        
        private bool _prevGroundedState;
        
        #endregion
        
        #endregion

        #region Methods

        protected override void OnValidate()
        {
            base.OnValidate();
            
            rigidbody = GetComponentInChildren<Rigidbody>();

            wheelsData = GetComponentInChildren<Wheels>().wheelsData;
            
            SpeedData = new VehicleSpeed();
        }
        
        private void Awake()
        {
            rigidbody = GetComponentInChildren<Rigidbody>();
            
            SpeedData = new VehicleSpeed();
        }

        private void Start()
        {
            rigidbody = GetComponentInChildren<Rigidbody>();

            
            InitializeVehicleBehaviours();
        }

        private void Update()
        {
            if(PrefabCheckHelper.CheckIfPrefab(this)) return;
            if(!Application.isPlaying) return;
            
            groundedDebug = Grounded;
            
            switch (_prevGroundedState)
            {
                case false when !Grounded:
                    //Landing_Event?.Invoke(obj: this);
                    Landing_Event?.Invoke();
                    break;
                case true when Grounded:
                    //LeavingGround_Event?.Invoke(obj: this);
                    LeavingGround_Event?.Invoke();
                    break;
            }

            _prevGroundedState = Grounded;
        }

        public void InvokeAccelerate() => Accelerating_Event?.Invoke();
        public void InvokeDecelerate() => Decelerating_Event?.Invoke();
        public void InvokeIdle() => Idle_Event?.Invoke();
        public void InvokeCruise() => Cruise_Event?.Invoke();
        
        private void InitializeVehicleBehaviours()
        {
            VehicleBehaviour[] __components = GetComponentsInChildren<VehicleBehaviour>();

            foreach(VehicleBehaviour __component in __components)
            {
                __component.VehicleIndex = this.Index;
                __component.SetupReferences();
            }
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