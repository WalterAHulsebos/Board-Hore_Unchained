// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>
    /// A centralised group of references to the common parts of a creature and a state machine for their actions.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Platformer - Creature")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Platformer/Creature")]
    public sealed class Creature : MonoBehaviour, IAnimancerClipSource
    {
        /************************************************************************************************************************/

        [SerializeField]
        private AnimancerComponent _Animancer;
        public AnimancerComponent Animancer { get { return _Animancer; } }

        [SerializeField]
        private SpriteRenderer _Renderer;
        public SpriteRenderer Renderer { get { return _Renderer; } }

        [SerializeField]
        private CreatureBrain _Brain;
        public CreatureBrain Brain { get { return _Brain; } }

        [SerializeField]
        private Rigidbody2D _Rigidbody;
        public Rigidbody2D Rigidbody { get { return _Rigidbody; } }

        [SerializeField]
        private GroundDetector _GroundDetector;
        public GroundDetector GroundDetector { get { return _GroundDetector; } }

        [SerializeField]
        private Health _Health;
        public Health Health { get { return _Health; } }

        [SerializeField]
        private CreatureState _Idle;
        public CreatureState Idle { get { return _Idle; } }

        // Stats.
        // Mana.
        // Pathfinding.
        // Etc.
        // Anything common to most creatures.

        /************************************************************************************************************************/

        public FSM.StateMachine<CreatureState> StateMachine { get; private set; }

        /// <summary>
        /// Forces the <see cref="StateMachine"/> to return to the <see cref="Idle"/> state.
        /// </summary>
        public Action ForceEnterIdleState { get; private set; }

        /************************************************************************************************************************/

        private void Awake()
        {
            ForceEnterIdleState = () => StateMachine.ForceSetState(_Idle);

            StateMachine = new FSM.StateMachine<CreatureState>(_Idle);
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            var speed = StateMachine.CurrentState.MovementSpeed * _Brain.MovementDirection;
            _Rigidbody.velocity = new Vector2(speed, _Rigidbody.velocity.y);

            // The sprites face right by default, so flip the X axis when moving left.
            if (speed != 0)
                _Renderer.flipX = _Brain.MovementDirection < 0;
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Displays the current state at the bottom of the Inspector.
        /// </summary>
        /// <remarks>
        /// Inspector Gadgets Pro allows you to easily customise the Inspector without writing a full custom Inspector
        /// class by simply adding a method with this name. Without Inspector Gadgets, this method will do nothing.
        /// It can be purchased from https://assetstore.unity.com/packages/tools/gui/inspector-gadgets-pro-83013
        /// </remarks>
        private void AfterInspectorGUI()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                GUI.enabled = false;
                UnityEditor.EditorGUILayout.ObjectField("Current State", StateMachine.CurrentState, typeof(CreatureState), true);
            }
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
