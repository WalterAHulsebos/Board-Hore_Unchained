// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0618 // Type or member is obsolete (for ControllerStates in Animancer Lite).
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>
    /// A <see cref="CreatureState"/> which moves the creature according to their
    /// <see cref="CreatureBrain.Movement"/>.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Game Kit - Locomotion State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/LocomotionState")]
    public sealed class LocomotionState : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private Vector2ControllerState.Serializable _LocomotionBlendTree;
        [SerializeField] private ClipState.Serializable _QuickTurnLeft;
        [SerializeField] private ClipState.Serializable _QuickTurnRight;
        [SerializeField] private float _QuickTurnMoveSpeed = 2;
        [SerializeField] private float _QuickTurnAngle = 145;

        /************************************************************************************************************************/

        private void Awake()
        {
            _QuickTurnLeft.OnEnd = _QuickTurnRight.OnEnd = () => Animancer.Transition(_LocomotionBlendTree);
        }

        /************************************************************************************************************************/

        public override bool CanEnterState(CreatureState previousState)
        {
            return Creature.CharacterController.isGrounded;
        }

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Animancer.Transition(_LocomotionBlendTree);
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            if (Creature.CheckMotionState())
                return;

            Creature.UpdateSpeedControl();
            _LocomotionBlendTree.State.ParameterX = Creature.ForwardSpeed;

            // Or we could set the parameter manually:
            //_LocomotionBlendTree.State.Playable.SetFloat("Speed", Creature.ForwardSpeed);

            UpdateRotation();
            UpdateAudio();
        }

        /************************************************************************************************************************/

        private void UpdateRotation()
        {
            // If the default locomotion state isn't active we must be performing a quick turn.
            // Those animations use root motion to perform the turn so we don't want any scripted rotation during them.
            if (!_LocomotionBlendTree.State.IsActive)
                return;

            float currentAngle, targetAngle;
            if (!Creature.GetTurnAngles(Creature.Brain.Movement, out currentAngle, out targetAngle))
                return;

            // Check if we should play a quick turn animation:

            // If we are moving fast enough.
            if (Creature.ForwardSpeed > _QuickTurnMoveSpeed)
            {
                // And turning sharp enough.
                var deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
                if (Mathf.Abs(deltaAngle) > _QuickTurnAngle)
                {
                    // Determine which way we are turning.
                    var turn = deltaAngle < 0 ? _QuickTurnLeft : _QuickTurnRight;

                    // Make sure the desired turn isn't already active so we don't keep using it repeatedly.
                    if (turn.State == null || turn.State.Weight == 0)
                    {
                        Animancer.Transition(turn);

                        // Now that we are quick turning, we don't want to apply the scripted turning below.
                        return;
                    }
                }
            }

            Creature.TurnTowards(currentAngle, targetAngle, Creature.CurrentTurnSpeed);
        }

        /************************************************************************************************************************/

        [SerializeField] private RandomAudioPlayer _FootstepAudio;
        private bool _CanPlayAudio;
        private bool _IsPlayingAudio;

        /// <remarks>
        /// This is the same logic used for locomotion audio in the original PlayerController.
        /// </remarks>
        private void UpdateAudio()
        {
            float footFallCurve = _LocomotionBlendTree.State.ParameterY;

            if (footFallCurve > 0.01f && !_IsPlayingAudio && _CanPlayAudio)
            {
                _IsPlayingAudio = true;
                _CanPlayAudio = false;
                _FootstepAudio.PlayRandomClip(Creature.GroundMaterial, Creature.ForwardSpeed < 4 ? 0 : 1);
            }
            else if (_IsPlayingAudio)
            {
                _IsPlayingAudio = false;
            }
            else if (footFallCurve < 0.01f && !_CanPlayAudio)
            {
                _CanPlayAudio = true;
            }
        }

        /************************************************************************************************************************/
    }
}
