using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;
using CommonGames.Utilities.CGTK;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using JetBrains.Annotations;

namespace Core.PlayerSystems.Movement
{
    [Serializable]
    public class VehicleSpeed
    {
        public float 
            speed,
            forwardSpeed,
            sideSpeed,
            topSpeed;

        [PublicAPI]
        public float ForwardSpeedPercent => (forwardSpeed.Abs() / topSpeed).Abs();
            
        [PublicAPI]
        public float SideSpeedPercent => Mathf.Abs(sideSpeed / topSpeed);
            
        [PublicAPI]
        public float SpeedPercent => Mathf.Abs(speed / topSpeed);

        [PublicAPI]
        public VehicleSpeed(float topSpeed)
        {
            this.topSpeed = topSpeed;
        }
    }

}