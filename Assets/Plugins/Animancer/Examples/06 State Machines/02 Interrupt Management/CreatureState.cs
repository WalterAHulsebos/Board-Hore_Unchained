// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.InterruptManagement
{
    /// <summary>
    /// A state for a <see cref="Creature"/> which plays an animation and uses a <see cref="Priority"/>
    /// enum to determine which other states can interrupt it.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Interrupt Management - Creature State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.InterruptManagement/CreatureState")]
    public sealed class CreatureState : StateBehaviour<CreatureState>, IAnimancerClipSource
    {
        /************************************************************************************************************************/

        /// <summary>Levels of importance.</summary>
        public enum Priority
        {
            Low,// Could specify "Low = 0," if we want to be explicit.
            Medium,// Medium = 1,
            High,// High = 2,
        }

        /************************************************************************************************************************/

        [SerializeField] private Creature _Creature;
        [SerializeField] private AnimationClip _Animation;
        [SerializeField] private Priority _Priority;

        /************************************************************************************************************************/

        public AnimancerComponent Animancer { get { return _Creature.Animancer; } }

        /************************************************************************************************************************/

        private void OnEnable()
        {
            var state = Animancer.CrossFade(_Animation);
            if (!_Animation.isLooping)
                state.OnEnd = _Creature.ForceIdleState;
        }

        /************************************************************************************************************************/

        public override bool CanExitState(CreatureState nextState)
        {
            return nextState._Priority >= _Priority;
        }

        /************************************************************************************************************************/
    }
}
