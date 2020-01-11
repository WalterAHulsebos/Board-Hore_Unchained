// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>
    /// A <see cref="CreatureState"/> which plays an animation then returns to the <see cref="Creature.Idle"/> state.
    /// </summary>
    /// <remarks>
    /// Nothing in this class is specific to the concept of "respawning" so it could have a more generic name like
    /// "AnimatedState" or "BasicState".
    /// </remarks>
    [AddComponentMenu("Animancer/Examples/Game Kit - Respawn State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/RespawnState")]
    public sealed class RespawnState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private ClipState.Serializable _Animation;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Creature.Animancer.Transition(_Animation)
                .OnEnd = Creature.ForceEnterIdleState;
        }

        /************************************************************************************************************************/

        public override bool CanExitState(CreatureState nextState)
        {
            return false;
        }

        /************************************************************************************************************************/
    }
}
