// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.Brains
{
    /// <summary>
    /// Base class for the various states a <see cref="Brains.Creature"/> can be in and actions they can perform.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Brains - Creature State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.StateMachines.Brains/CreatureState")]
    public abstract class CreatureState : StateBehaviour<CreatureState>,
        IOwnedState<CreatureState>, IAnimancerClipSource
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Creature _Creature;

        /// <summary>The <see cref="Brains.Creature"/> that owns this state.</summary>
        public Creature Creature
        {
            get { return _Creature; }
            protected set { _Creature = value; }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _Creature = Editor.AnimancerEditorUtilities.GetComponentInHierarchy<Creature>(gameObject);
        }
#endif

        /************************************************************************************************************************/

        public AnimancerComponent Animancer { get { return _Creature.Animancer; } }

        /************************************************************************************************************************/

        public StateMachine<CreatureState> OwnerStateMachine { get { return _Creature.StateMachine; } }

        /************************************************************************************************************************/
    }
}
