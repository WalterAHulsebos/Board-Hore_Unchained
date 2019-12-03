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

using JetBrains.Annotations;

using Core.Environmental.Railings;

namespace Core.PlayerSystems.Movement.Abilities
{
    public class GrindAbility : BaseAbility
    {
        #region Variables
        
        [HideInInspector]
        public TubeGenerator bar;
        
        //public EndOfPathInstruction endOfPathInstruction;
        [SerializeField] private float 
            speed = 17.5f, 
            bailForce = 175.0f, 
            bailUpPercentage = 0.2f, 
            
            offsetFromBar = 0.5f;

        //[ReadOnly]
        // ReSharper disable once InconsistentNaming
        [OdinSerialize] private readonly float CooldownTime = 0.25f;


        private float _distanceTravelled;

        private Matrix4x4? _worldMatrix = null, _localMatrix = null;
        
        private List<FrenetFrame> _frenetFrames = null;

        private bool _onGrindBar = false;
        
        //for example 10m/s on a bar of 100m would be 0.1/s, because 10 is 10% of 100.
        private float _calculatedSpeed = 0f;

        private bool _readyToBail = false;

        private float
            _timeOfLastBail = 0f;

        #endregion

        #region Methods

        private void Awake()
        {
            abilityAction.started +=
                ctx => { _readyToBail = true; };
            
            abilityAction.canceled +=
                ctx => { _readyToBail = false; };
        }

        public override void DoAbility() { }

        public override void AbilityUpdate()
        {
            base.AbilityUpdate();

            if(!_onGrindBar) return;
            
            if(_readyToBail)
            {
                BailFromBar();
                return;
            }
            
            FollowBar();
        }

        [PublicAPI]
        public void AttachToBar(TubeGenerator bar)
        {
            //If the time since last bail is less than CoolDownTime, return. 
            float __timeSinceLastBail = (Time.time - _timeOfLastBail).Abs();
            if(__timeSinceLastBail < CooldownTime) return;

            if(_onGrindBar) return;

            this.bar = bar;
            
            Transform __barTransform = bar.transform;
            
            _worldMatrix = __barTransform.WorldMatrix();
            _localMatrix = __barTransform.LocalMatrix();

            Vector3 __relativePositionToTube = _vehicle.rigidbody.transform.GetRelativePosition(relativeTo: __barTransform);

            float __startPoint = bar.GetStartPoint(position: __relativePositionToTube);

            _distanceTravelled = __startPoint;
            
            _calculatedSpeed = (speed / bar.Length);

            _vehicle.mayMove++;

            ToggleRigidBody();
            StopSecondaryMovement();

            _onGrindBar = true;
        }
        
        [PublicAPI]
        public void BailFromBar()
        {
            _onGrindBar = false;
            
            ToggleRigidBody();

            _distanceTravelled = 0f;

            _timeOfLastBail = Time.time;

            Rigidbody __rigidbody = _vehicle.rigidbody;

            Transform __rigidbodyTransform = __rigidbody.transform;
            Vector3 __direction = Vector3.SlerpUnclamped(a: __rigidbodyTransform.forward, b: __rigidbodyTransform.up, t: bailUpPercentage);

            __rigidbody.AddForceAtPosition(force: __direction.SetVectorLength(bailForce), position: __rigidbody.worldCenterOfMass, mode: ForceMode.Impulse);
            
            _vehicle.mayMove--;
        }
        
        private void FollowBar()
        {
            //if(!_onGrindBar) return;

            if(_worldMatrix == null)
            {
                Debug.LogError(message: "World Matrix is null!!!!");
                return;
            }
            if(_localMatrix == null)
            {
                Debug.LogError(message: "Local Matrix is null!!!!");
                return;
            }
            
            _distanceTravelled += _calculatedSpeed * Time.deltaTime;

            float __maxPercentage = (bar.Units - 1) / bar.Units;
            
            if(_distanceTravelled >= __maxPercentage)
            //if(_distanceTravelled >= 1 || _distanceTravelled.Approximately(1))
            {
                BailFromBar();
                return;
            }

            Vector3 __posRelativeToBar = bar.Curve.GetPointAt(u: _distanceTravelled);
            
            Vector3 __tangentRelativeToBar = bar.Curve.GetTangentAt(u: _distanceTravelled);
            
            Vector3 __posRelativeToWorld = __posRelativeToBar.GetRelativePositionFrom(@from: (Matrix4x4)_worldMatrix);

            __posRelativeToWorld.y += offsetFromBar;

            _vehicle.rigidbody.transform.position = __posRelativeToWorld;
            _vehicle.rigidbody.transform.rotation = Quaternion.LookRotation(forward: __tangentRelativeToBar, upwards: Vector3.up);

            /*
            //_vehicle.rigidbody.transform.SetPositionAndRotation(
            //position: __posRelativeToWorld, 
            //rotation: Quaternion.LookRotation(__tangentRelativeToBar, Vector3.up));
            
            //_vehicle.rigidbody.MovePosition(__posRelativeToWorld);
            //_vehicle.rigidbody.MoveRotation(Quaternion.LookRotation(__tangentRelativeToBar, Vector3.up));
            */
        }

        private void StopSecondaryMovement()
        {
            Rigidbody __rigidbody = _vehicle.rigidbody;
            
            __rigidbody.ResetVelocity();
        }

        private void ToggleRigidBody()
        {
            Rigidbody __rigidbody = _vehicle.rigidbody;

            __rigidbody.constraints = __rigidbody.constraints == RigidbodyConstraints.None 
                ? RigidbodyConstraints.FreezeAll 
                : RigidbodyConstraints.None;

            __rigidbody.isKinematic = !__rigidbody.isKinematic;  

            __rigidbody.useGravity = !__rigidbody.useGravity;

            _vehicle.rigidbody = __rigidbody;
        }

        #endregion
    
    }
    
}