using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.CGTK;

namespace Core.PlayerSystems.Movement
{
    public partial class Wheels
    {
     
        [Serializable]
        public class Wheel
        {
            public NWH.WheelController3D.WheelController wheelController;
            public bool steer;
            public bool power;

            [HideInInspector] public bool isGrounded = false;
            [HideInInspector] public WheelHit groundData = new WheelHit();
        }
    
        [Serializable]
        public class WheelsData
        {
            public Wheels.Wheel[] wheels = new Wheel[4];
            
            [HideInInspector] public bool anyGrounded = default;
            
            [HideInInspector] public int numberOfGroundedWheels = default;
            
            [HideInInspector] public Vector3 averageWheelSurfaceNormal = default;

            /*
            public WheelsData(
                Wheel[] wheels, 
                bool grounded = default, 
                int numberOfGroundedWheels = default, 
                Vector3 averageWheelSurfaceNormal = default)
            {
                this.wheels = wheels;
                
                if(!(this.grounded != default && grounded == default))
                {
                    this.grounded = default;
                }
                
                if(!(this.numberOfGroundedWheels != default && numberOfGroundedWheels == default))
                {
                    this.numberOfGroundedWheels = default;
                }
                
                if(!(this.averageWheelSurfaceNormal != default && averageWheelSurfaceNormal == default))
                {
                    this.averageWheelSurfaceNormal = default;
                }
            }
            */

            public WheelsData() { }
            
            public WheelsData(Wheel[] wheels)
            {
                this.wheels = wheels;
            }
        }
        
    }   
}