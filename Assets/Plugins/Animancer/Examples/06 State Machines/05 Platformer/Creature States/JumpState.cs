// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// A <see cref="CreatureState"/> that plays a jump animation and applies some upwards force.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Platformer - Jump State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Platformer/JumpState")]
    public sealed class JumpState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField]
        private AnimationClip _Animation;

        [SerializeField]
        private float _Force;

        private AnimancerState _AnimancerState;

        /************************************************************************************************************************/

        public override float MovementSpeed
        {
            get { return Creature.Idle.MovementSpeed; }
        }

        /************************************************************************************************************************/

        public override bool CanEnterState(CreatureState previousState)
        {
            return Creature.GroundDetector.IsGrounded;
        }

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Creature.Rigidbody.velocity += new Vector2(0, _Force);

            _AnimancerState = Creature.Animancer.Play(_Animation);
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            // Wait until we are grounded and the animation has finished, then return to idle.
            if (Creature.GroundDetector.IsGrounded && _AnimancerState.NormalizedTime > 1)
                Creature.Idle.ForceEnterState();
        }

        /************************************************************************************************************************/
    }
}
