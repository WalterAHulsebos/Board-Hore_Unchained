using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Core.PlayerSystems.Movement.Abilities
{
    public abstract class BaseScriptableAbility : ScriptableObject, IAbility, IActorIndex
    {
        #region Variables
        
        public InputAction abilityAction;
        
        protected VehicleCore _vehicle;

        public int VehicleIndex { get; set; }
        
        public float DeltaTime { get; set; }
        public float FixedDeltaTime { get; set; }

        #endregion

        #region Methods

        #region Event Functions

        public virtual void OnEnable() => OnAbilityEnable();
        public virtual void OnDisable() => OnAbilityDisable();

        #endregion
        
        public virtual void OnAbilityEnable() => abilityAction.Enable();
        public virtual void OnAbilityDisable() => abilityAction.Disable();

        public virtual void Initialize()
        {
            _vehicle = VehicleCore.Instances[VehicleIndex];
        }

        public void AbilityUpdate()
        {
            //We're setting this assuming you call this properly from some MonoBehaviour's Update()
            DeltaTime = Time.deltaTime;
        }
        public void AbilityFixedUpdate()
        {
            //We're setting this assuming you call this properly from some MonoBehaviour's FixedUpdate()
            FixedDeltaTime = Time.fixedDeltaTime;
        }
        
        public abstract void DoAbility();
        
        #endregion
    }
}