using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    public class ColliderData : VehicleBehaviour
    {
        public static event Action<Collision> EOnCollision = collisionPosition => { };
        
        private void OnCollisionEnter(Collision other)
        {
            EOnCollision(other);
        }

        private void OnCollisionStay(Collision collision)
        {
            Vector3 __surfaceNormalSum = Vector3.zero;
            
            for (int __i = 0; __i < collision.contactCount; __i++)
            {
                __surfaceNormalSum += collision.contacts[__i].normal;
            }

            _vehicle.averageColliderSurfaceNormal = __surfaceNormalSum.normalized;
        }

        private void OnCollisionExit(Collision collision)
        {
            _vehicle.averageColliderSurfaceNormal = Vector3.zero;
        }
    }
}