using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{
    ///<summary> Handles Body tilt (Leaning in the direction of movement) </summary>
    public class BodyMovement : VehicleBehaviour
    {
        [SerializeField] private Transform modelBase;
        //public Vector3 modelBaseOffset;

        private void Update()
        {
            /*
            if(_vehicle.mayMove <= 0)
            {
                UpdateCarBodyTransform();
            }
            */
            
            UpdateCarBodyTransform();
        }
        
        private void FixedUpdate()
        {
            /*
            if(_vehicle.mayMove <= 0)
            {
                UpdateCarBodyTransform();
            }
            */
            
            UpdateCarBodyTransform();
        }

        private void UpdateCarBodyTransform()
        {
            Transform __transform = transform;
            __transform.position = _vehicle.rigidbody.position;
            __transform.rotation = _vehicle.rigidbody.rotation;
        }
    }

    [Serializable]
    public class BodyAxisMovement
    {
        public float currentAngle;
        public float inputMaxAngle;
        public float speedMaxAngle;
    }
}