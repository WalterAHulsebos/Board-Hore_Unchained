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

namespace Core.PlayerSystems.Movement
{
    public class CameraFollowPoint : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Transform target;
        [Range(0, 30f)]
        [SerializeField] private float distance;
        [Range(0, 10f)]
        [SerializeField] private float height;
        [Range(0, 10f)]
        [SerializeField] private float targetUpOffset;
        [Range(-10f, 10f)]
        [SerializeField] private float targetForwardOffset;
        [Range(0, 50f)]
        [SerializeField] private float smoothing;
        [Range(0, 5f)]
        [SerializeField] private float angleFollowStrength;
        
        private Vector3 _targetForward;
        private float _angle;

        #endregion

        #region Methods

        private void Update()
        {
            Vector3 __prevTargetForward = _targetForward;
            Vector3 __forward = target.forward;
            
            _targetForward = Vector3.Lerp(__prevTargetForward, __forward, t: Time.deltaTime);

            Vector3 __position = target.position;
            _angle = AngleSigned(__forward, (__position - transform.position), Vector3.up);

            Vector3 __targetCamPos = __position + _targetForward * -(distance) + Vector3.up * height;

            Transform __transform = this.transform;
            __transform.position = __targetCamPos;
            __transform.LookAt(__position + Vector3.up * targetUpOffset + __forward * targetForwardOffset);
            
            __transform.rotation = Quaternion.AngleAxis(-_angle * angleFollowStrength, Vector3.up) * __transform.rotation;
        }

        /// <summary>
        /// Determine the signed angle between two vectors, with normal 'n'
        /// as the rotation axis.
        /// </summary>
        public static float AngleSigned(Vector3 vector1, Vector3 vector2, Vector3 n)
        {
            return Mathf.Atan2(
                       Vector3.Dot(n, Vector3.Cross(vector1, vector2)), 
                       Vector3.Dot(vector1, vector2)) * Mathf.Rad2Deg;
        }

        #endregion
    }
}
