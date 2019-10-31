using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour; 
#endif

namespace Core.PlayerSystems.Movement
{
    public class VehicleBehaviour : MonoBehaviour
    {
        protected VehicleCore _vehicle;

        protected virtual void Start()
        {
            _vehicle = VehicleCore.Instance;
        }
    }
}
