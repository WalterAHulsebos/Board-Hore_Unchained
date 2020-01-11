// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Creatures
{
    /// <summary>
    /// A state for a <see cref="Creature"/> which simply plays an animation.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Creatures - Creature State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Creatures/CreatureState")]
    public sealed class CreatureState : StateBehaviour<CreatureState>, IAnimancerClipSource
    {
        /************************************************************************************************************************/

        [SerializeField] private Creature _Creature;
        [SerializeField] private AnimationClip _Animation;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            var state = _Creature.Animancer.CrossFade(_Animation);
            if (!_Animation.isLooping)
                state.OnEnd = _Creature.ForceIdleState;
        }

        /************************************************************************************************************************/

        public AnimancerComponent Animancer { get { return _Creature.Animancer; } }

        /************************************************************************************************************************/
    }
}
