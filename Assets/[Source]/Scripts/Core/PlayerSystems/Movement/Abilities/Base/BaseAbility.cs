using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Core.PlayerSystems.Movement.Abilities
{
    public abstract class BaseAbility : VehicleBehaviour, IAbility
    {
        #region Variables
        
        public InputAction abilityAction;

        public float DeltaTime { get; set; }
        public float FixedDeltaTime { get; set; }

        #endregion

        #region Methods

        #region Unity Event Functions

        protected virtual void OnEnable() => OnAbilityEnable();
        protected virtual void OnDisable() => OnAbilityDisable();

        protected override void Start()
        {
            base.Start();
            
            Initialize();
        }

        protected virtual void Update() => AbilityUpdate();
        protected virtual void FixedUpdate() => AbilityFixedUpdate();

        #endregion
        
        public virtual void OnAbilityEnable()
        {
            abilityAction.Enable();
        }
        public virtual void OnAbilityDisable()
        {
            abilityAction.Disable();
        }

        public virtual void Initialize() { }

        public virtual void AbilityUpdate() => DeltaTime = Time.deltaTime;
        public virtual void AbilityFixedUpdate() => FixedDeltaTime = Time.fixedDeltaTime;

        public abstract void DoAbility();

        #endregion

    }
}