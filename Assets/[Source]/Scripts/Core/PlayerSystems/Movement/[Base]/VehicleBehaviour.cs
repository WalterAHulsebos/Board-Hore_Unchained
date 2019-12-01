using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;
using CommonGames.Utilities.Helpers;
using CommonGames.Utilities.CGTK;

#if ODIN_INSPECTOR
using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour; 
#endif

namespace Core.PlayerSystems.Movement
{
    
    /// <summary>
    /// Inherit from VehicleBehaviour if you want a MonoBehaviour with access to your Vehicle.
    /// It's basically a shorthand so I can just call all childed VehicleBehaviours from the VehicleCore on Start and send them the proper index.
    /// </summary>
    public abstract class VehicleBehaviour : MonoBehaviour, IActorIndex
    {
        #region Variables
        
        //Ja Jan, dit kan wel met een interface zodat het niet op MonoBehaviour vast zit maar dan heeft het geen Start, en ik wil dat dat automatisch gaat.
        protected VehicleCore _vehicle;
        
        /// <summary> The Index of our VehicleCore reference in it's <see cref="IndexedMultiton"/>'s List </summary>
        public int VehicleIndex { get; set; }
        
        #endregion

        #region Methods

        /// <summary> Sets the <see cref= "_vehicle"/> VehicleCore reference to this vehicle's VehicleCore </summary>
        protected virtual void Start() 
            => _vehicle = _vehicle ? _vehicle : VehicleCore.Instance;

        #endregion
    }
}