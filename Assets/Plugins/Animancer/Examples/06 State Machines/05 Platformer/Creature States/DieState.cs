// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// A <see cref="CreatureState"/> that plays a die animation.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Platformer - Die State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Platformer/DieState")]
    public sealed class DieState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField]
        private AnimationClip _Animation;

        /************************************************************************************************************************/

        private void Awake()
        {
            Creature.Health.OnHealthChanged += () =>
            {
                if (Creature.Health.CurrentHealth <= 0)
                    Creature.StateMachine.ForceSetState(this);
                else if (enabled)
                    Creature.Idle.ForceEnterState();
            };
        }

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Creature.Animancer.Play(_Animation);
        }

        /************************************************************************************************************************/

        public override bool CanExitState(CreatureState nextState)
        {
            return Creature.Health.CurrentHealth > 0;
        }

        /************************************************************************************************************************/
    }
}
