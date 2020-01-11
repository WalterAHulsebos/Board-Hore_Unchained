// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.Locomotion
{
    /// <summary>
    /// Demonstrates how to use Root Motion for some animations but not others.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Locomotion - Root Motion")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.Locomotion/RootMotion")]
    public sealed class RootMotion : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <summary>
        /// A <see cref="ClipState.Serializable"/> with an <see cref="ApplyRootMotion"/> toggle.
        /// </summary>
        [Serializable]
        public class MotionClip : ClipState.Serializable
        {
            /************************************************************************************************************************/

            [SerializeField, Tooltip("Determines if root motion should be enabled when this animation plays")]
            private bool _ApplyRootMotion;

            /// <summary>
            /// Determines if root motion should be enabled when this animation plays.
            /// </summary>
            public bool ApplyRootMotion
            {
                get { return _ApplyRootMotion; }
                set { _ApplyRootMotion = value; }
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private float _MaxDistance;
        [SerializeField] private MotionClip[] _Animations;

        private Vector3 _Start;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            _Start = transform.position;
            Play(0);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Plays the animation at the specified 'index'.
        /// </summary>
        /// <remarks>
        /// This method is called by UI Buttons.
        /// </remarks>
        public void Play(int index)
        {
            Play(_Animations[index]);
        }

        /// <summary>
        /// Plays the specified <see cref="MotionClip"/> and sets <see cref="Animator.applyRootMotion"/> according to
        /// its <see cref="MotionClip.ApplyRootMotion"/>.
        /// </summary>
        public void Play(MotionClip motion)
        {
            _Animancer.Transition(motion);
            _Animancer.Animator.applyRootMotion = motion.ApplyRootMotion;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Teleports this object back to its starting location if it moves too far.
        /// </summary>
        private void FixedUpdate()
        {
            if (Vector3.Distance(_Start, transform.position) > _MaxDistance)
                transform.position = _Start;
        }

        /************************************************************************************************************************/
    }
}
