// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0618 // Type or member is obsolete (for ControllerStates in Animancer Lite).
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using System;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    [AddComponentMenu("Animancer/Examples/Game Kit - Landing State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/LandingState")]
    public sealed class LandingState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private Vector2ControllerState.Serializable _SoftLanding;
        [SerializeField] private ClipState.Serializable _HardLanding;
        [SerializeField] private float _HardLandingForwardSpeed = 5;
        [SerializeField] private float _HardLandingVerticalSpeed = -10;
        [SerializeField] private float _ExitNormalizedTime = 0.75f;
        [SerializeField] private RandomAudioPlayer _ImpactAudio;
        [SerializeField] private RandomAudioPlayer _EmoteAudio;

        private bool _IsHardLanding;

        /************************************************************************************************************************/

        public override bool CanEnterState(CreatureState previousState)
        {
            return Creature.CharacterController.isGrounded;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Performs either a hard or soft landing depending on the current speed (both horizontal and vertical).
        /// </summary>
        private void OnEnable()
        {
            Creature.ForwardSpeed = Creature.DesiredForwardSpeed;

            if (Creature.VerticalSpeed <= _HardLandingVerticalSpeed &&
                Creature.ForwardSpeed >= _HardLandingForwardSpeed)
            {
                _IsHardLanding = true;
                Animancer.Transition(_HardLanding);
            }
            else
            {
                _IsHardLanding = false;
                Animancer.Transition(_SoftLanding);
                _SoftLanding.State.Parameter = new Vector2(Creature.ForwardSpeed, Creature.VerticalSpeed);
            }

            _ImpactAudio.PlayRandomClip(Creature.GroundMaterial, bankId: Creature.ForwardSpeed < 4 ? 0 : 1);
            _EmoteAudio.PlayRandomClip();
        }

        /************************************************************************************************************************/

        public override bool FullMovementControl { get { return !_IsHardLanding; } }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            if (!Creature.CharacterController.isGrounded &&
                Creature.StateMachine.TrySetState(Creature.Airborne))
                return;

            Creature.UpdateSpeedControl();

            float normalizedTime;
            if (_IsHardLanding)
            {
                normalizedTime = _HardLanding.State.NormalizedTime;
            }
            else
            {
                // We need the time of the Animator Controller state inside the Animancer.ControllerState, not the
                // time of the ControllerState itself.
                normalizedTime = _SoftLanding.State.Playable.GetCurrentAnimatorStateInfo(0).normalizedTime;

                // Update the horizontal speed but keep the initial vertical speed from when you first landed.
                _SoftLanding.State.ParameterX = Creature.ForwardSpeed;
            }

            if (normalizedTime >= _ExitNormalizedTime)
                Creature.CheckMotionState();
        }

        /************************************************************************************************************************/
    }
}
