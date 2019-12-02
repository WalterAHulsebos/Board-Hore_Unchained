using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;
using CommonGames.Utilities.Helpers;
using CommonGames.Utilities.CGTK;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using JetBrains.Annotations;

using Physics = UnityEngine.Physics;

namespace Core.PlayerSystems.Movement
{
    public class CameraSystem : MonoBehaviour
    {
        #region Variables
        
        public Transform targetLookAt;
        public float distance;
        public float distanceMin = 3.0f;
        public float distanceMax = 10.0f;
        
        private float _mouseX = 0.0f;
        private float _mouseY = 0.0f;
        private float _startingDistance = 0.0f;
        private float _desiredDistance = 0.0f;
        
        public float xMouseSensitivity = 5.0f;
        public float yMouseSensitivity = 5.0f;
        public float mouseWheelSensitivity = 5.0f;
        public float yMinLimit = -40.0f;
        public float yMaxLimit = 80.0f;
        public float distanceSmooth = 0.05f;
        
        private float _velocityDistance = 0.0f;
        
        private Vector3 _desiredPosition = Vector3.zero;
        
        public float xSmooth = 0.05f;
        public float ySmooth = 0.1f;
        
        private float _velX = 0.0f;
        private float _velY = 0.0f;
        private float _velZ = 0.0f;
        private Vector3 _position;

        #endregion

        #region Methods

        private void Awake()
        {
            _position = transform.position;
        }

        private void Start()
        {
            distance = Mathf.Clamp(distance, distanceMin, distanceMax);
            _startingDistance = distance;
            Reset();
        }

        // Running in fixed update due to rigidbody interpolation = none
        private void FixedUpdate()
        {
            if(targetLookAt == null)
            {
                return;
            }

            HandlePlayerInput();
            CalculateDesiredPosition();
            UpdatePosition();
        }

        public void HandlePlayerInput()
        {
            float __deadZone = 0.01f; // mousewheel deadZone

            if (Input.GetMouseButton(0))
            {
                _mouseX += Input.GetAxis("Mouse X") * xMouseSensitivity;
                _mouseY -= Input.GetAxis("Mouse Y") * yMouseSensitivity;
            }

            // this is where the mouseY is limited - Helper script
            _mouseY = ClampAngle(_mouseY, yMinLimit, yMaxLimit);

            // get Mouse Wheel Input
            if (Input.GetAxis("Mouse ScrollWheel") < -__deadZone || Input.GetAxis("Mouse ScrollWheel") > __deadZone)
            {
                _desiredDistance = Mathf.Clamp(distance - (Input.GetAxis("Mouse ScrollWheel") * mouseWheelSensitivity),
                                                                                 distanceMin, distanceMax);
            }
        }

        public void CalculateDesiredPosition()
        {
            // Evaluate distance
            distance = Mathf.SmoothDamp(distance, _desiredDistance, ref _velocityDistance, distanceSmooth);

            // Calculate desired position -> Note : mouse inputs reversed to align to WorldSpace Axis
            _desiredPosition = CalculatePosition(_mouseY, _mouseX, distance);
        }

        public Vector3 CalculatePosition(float rotationX, float rotationY, float dist)
        {
            Vector3 __direction = new Vector3(0, 0, -dist);
            
            Quaternion __rotation = Quaternion.Euler(rotationX, rotationY, 0);
            
            return targetLookAt.position + (__rotation * __direction);
        }

        public void UpdatePosition()
        {
            float __posX = Mathf.SmoothDamp(_position.x, _desiredPosition.x, ref _velX, xSmooth);
            float __posY = Mathf.SmoothDamp(_position.y, _desiredPosition.y, ref _velY, ySmooth);
            float __posZ = Mathf.SmoothDamp(_position.z, _desiredPosition.z, ref _velZ, xSmooth);
            
            _position = new Vector3(__posX, __posY, __posZ);

            transform.position = _position;

            RaycastHit __hit;
            if (Physics.Raycast(_position, Vector3.down, out __hit, 0.7f))
            {
                _position = new Vector3(__posX, __hit.point.y + 0.7f, __posZ);
            }

            transform.LookAt(targetLookAt);
        }

        public void Reset()
        {
            _mouseX = 0;
            _mouseY = 10;
            distance = _startingDistance;
            _desiredDistance = distance;
        }

        public float ClampAngle(float angle, float min, float max)
        {
            while (angle < -360 || angle > 360)
            {
                if (angle < -360)
                    angle += 360;
                if (angle > 360)
                    angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }

        #endregion
    }
}
