using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Core.PlayerSystems.Movement.Abilities
{
    public abstract class BaseScriptableAbility : ScriptableObject
    {
        #region Variables
        
        public InputAction abilityAction;
        
        protected VehicleCore _vehicle;

        #endregion

        #region Methods

        protected virtual void Start()
        {
            _vehicle = VehicleCore.Instance;
        }

        protected virtual void Update()
        {
            
        }
        
        protected virtual void FixedUpdate()
        {
            
        }

        protected virtual void OnEnable()
        {
            abilityAction.Enable();
        }

        protected virtual void OnDisable()
        {
            abilityAction.Disable();
        }
        
        public abstract void CheckInput();
        public abstract void DoAbility();
        
        #endregion

    }
}