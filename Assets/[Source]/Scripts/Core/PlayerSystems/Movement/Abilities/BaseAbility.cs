using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Core.PlayerSystems.Movement.Abilities
{
    public abstract class BaseAbility : VehicleBehaviour
    {
        #region Variables
        
        public InputAction abilityAction;

        #endregion

        #region Methods

        public virtual void OnEnable()
        {
            abilityAction.Enable();
        }

        public virtual void OnDisable()
        {
            abilityAction.Disable();
        }
        
        public abstract void CheckInput();
        public abstract void DoAbility();
        
        #endregion

    }
}