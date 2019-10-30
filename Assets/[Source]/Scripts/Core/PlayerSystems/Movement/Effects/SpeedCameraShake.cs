using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Core.PlayerSystems.Movement
{
    public class SpeedCameraShake : MonoBehaviour
    {
        private VehicleCore _VehicleCore;
        //private CameraSystem _cameraSystem;

        private void Awake()
        {
            _VehicleCore = transform.parent.GetComponent<VehicleCore>();
            //_cameraSystem = transform.parent.GetComponent<CameraSystem>();
        }

        private void Update()
        {
            //_cameraSystem.currentNoise.m_FrequencyGain = (_VehicleCore.speedData.SpeedPercent - 0.3f) * 15f;
            //_cameraSystem.currentNoise.m_AmplitudeGain = Mathf.Clamp01(_VehicleCore.speedData.SpeedPercent - 0.98f);
        }
    }
}