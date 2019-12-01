using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Core.PlayerSystems.Movement
{

    public interface IAbility
    {
        #region Methods

        /// <summary> Call when Enabling the Ability</summary>
        void OnAbilityEnable();
        /// <summary> Call when Disabling the Ability</summary>
        void OnAbilityDisable();

        
        /// <summary> Call when the Ability is Enabled, similar to Start() as here you should be able to assume everything is ready. </summary>
        void Initialize();

        /// <summary> Call when updating the Ability. </summary>
        ///<remarks> If you're going to use this call this from a MonoBehaviour's Update() function. </remarks>  
        void AbilityUpdate();
        
        /// <summary> Call when updating the Ability in a fixed time loop. </summary>
        ///<remarks> If you're going to use this call this from a MonoBehaviour's FixedUpdate() function. </remarks>
        void AbilityFixedUpdate();
        
        float DeltaTime { get; set; }
        float FixedDeltaTime { get; set; }
        
        void DoAbility();

        #endregion
    }
}
