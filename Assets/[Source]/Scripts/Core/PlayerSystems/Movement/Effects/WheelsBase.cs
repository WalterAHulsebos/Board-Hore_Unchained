using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.PlayerSystems.Movement
{

    public class WheelsBase : VehicleBehaviour
    {

        [SerializeField] private GameObject wheelPrefab;
        [SerializeField] private Transform[] wheelAttachPoints; //Different to the placement of physics wheels
        [SerializeField] private List<WheelVisuals> wheelVisuals = new List<WheelVisuals>();

        protected override void Start()
        {
            base.Start();
            
            InitWheels();
        }
        
        private void LateUpdate()
        {
            foreach (WheelVisuals __wheelVisual in wheelVisuals)
            {
                __wheelVisual.ProcessWheelVisuals(_vehicle.InputData, _vehicle.SpeedData);
            }
        }

        private void InitWheels()
        {
            foreach (Transform __wheelAttachPoint in wheelAttachPoints)
            {
                WheelVisuals __wheelVisual = Instantiate(wheelPrefab, __wheelAttachPoint.position, __wheelAttachPoint.rotation, __wheelAttachPoint).GetComponent<WheelVisuals>();
                
                __wheelVisual.SetUpWheel(_vehicle.rigidbody);
                
                wheelVisuals.Add(__wheelVisual);
            }
        }
    }
}