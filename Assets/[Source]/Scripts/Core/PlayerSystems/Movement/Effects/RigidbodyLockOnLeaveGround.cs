using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{

    public class RigidbodyLockOnLeaveGround : VehicleBehaviour
    {
        [SerializeField] private float rotateSpeed = 180f;
        
        private Quaternion targetRotation = Quaternion.identity;

        protected override void Start()
        {
            base.Start();
            
            _vehicle.LeavingGround_Event += LockPhysics;
            _vehicle.Landing_Event += UnlockPhysics;
        }

        private void OnDestroy()
        {
            _vehicle.LeavingGround_Event -= LockPhysics;
            _vehicle.Landing_Event -= UnlockPhysics;
        }

        private void LockPhysics() => TogglePhysics(false);

        private void UnlockPhysics() => TogglePhysics(true);
        
        private void TogglePhysics(bool turnOff)
        {
            if(turnOff)
            {
                var forwardDirection = _vehicle.rigidbody.transform.forward;
                
                forwardDirection.y = 0;
                forwardDirection = forwardDirection.normalized;
                
                targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);  
            }
        }

        private void FixedUpdate()
        {
            if(_vehicle.Grounded) return;
            
            float step = rotateSpeed * Time.deltaTime;
                
            transform.rotation = Quaternion.RotateTowards(_vehicle.rigidbody.transform.rotation, targetRotation, step);
        }
    }
}