// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using System;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    [AddComponentMenu("Animancer/Examples/Game Kit - Attack State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/AttackState")]
    public sealed class AttackState : CreatureState
    {
        /************************************************************************************************************************/

        /// <summary>
        /// A <see cref="ClipState.Serializable"/> with an <see cref="EndNormalizedTime"/> to end the animation early
        /// and an <see cref="AttackTrail"/> to show during the animation.
        /// </summary>
        [Serializable]
        public class AttackClip : ClipState.Serializable
        {
            /************************************************************************************************************************/

            [SerializeField]
            [Tooltip("The normalized time when this attack will begin transitioning back to idle")]
            [Range(0, 1)]
            private float _EndNormalizedTime = 0.75f;

            /// <summary>
            /// The <see cref="AnimancerState.NormalizedTime"/> when this attack will begin transitioning back to idle.
            /// </summary>
            public float EndNormalizedTime
            {
                get { return _EndNormalizedTime; }
            }

            /************************************************************************************************************************/

            [SerializeField]
            private AttackTrail _Trail;

            /// <summary>
            /// This method is called during <see cref="AnimancerComponent.Transition(IAnimancerTransition, int)"/>,
            /// so we use it to activate the trail at the same time without needing to expose the trail publicly.
            /// </summary>
            public override void Apply(AnimancerState state)
            {
                base.Apply(state);
                _Trail.Activate();

                // If we wanted the ability to tweak the speed of the attack animation, we would also need to pass that
                // speed onto the trail so it gets shown correctly in relation to the character.
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        [SerializeField] private float _TurnSpeed = 400;
        [SerializeField] private RandomAudioPlayer _WeaponAudio;
        [SerializeField] private RandomAudioPlayer _EmoteAudio;
        [SerializeField] private AttackClip[] _Animations;

        private int _AttackIndex = int.MaxValue;
        private AttackClip _Attack;

        /************************************************************************************************************************/

        public override bool CanEnterState(CreatureState previousState)
        {
            return Creature.CharacterController.isGrounded;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Start at the beginning of the sequence by default, but if the previous attack hasn't faded out yet then
        /// perform the next attack instead.
        /// </summary>
        private void OnEnable()
        {
            if (_AttackIndex >= _Animations.Length - 1 ||
                _Animations[_AttackIndex].State.Weight == 0)
            {
                _AttackIndex = 0;
            }
            else
            {
                _AttackIndex++;
            }

            _Attack = _Animations[_AttackIndex];
            Animancer.Transition(_Attack);
            _WeaponAudio.PlayRandomClip();
            _EmoteAudio.PlayRandomClip();
            Creature.ForwardSpeed = 0;
        }

        /************************************************************************************************************************/

        public override bool FullMovementControl { get { return false; } }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            if (Creature.CheckMotionState())
                return;

            Creature.TurnTowards(Creature.Brain.Movement, _TurnSpeed);
        }

        /************************************************************************************************************************/

        public override bool CanExitState(CreatureState nextState)
        {
            return _Attack.State.NormalizedTime >= _Attack.EndNormalizedTime;
        }

        /************************************************************************************************************************/
    }
}
