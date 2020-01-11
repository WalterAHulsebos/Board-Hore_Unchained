// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>
    /// Base class for controlling the actions of a <see cref="Brains.Creature"/>.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Game Kit - Creature Brain")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/CreatureBrain")]
    public abstract class CreatureBrain : MonoBehaviour, IAnimancerClipSource
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Creature _Creature;
        public Creature Creature
        {
            get { return _Creature; }
            set
            {
                if (_Creature == value)
                    return;

                var oldCreature = _Creature;
                _Creature = value;

                // Make sure the old creature doesn't still reference this brain.
                if (oldCreature != null)
                    oldCreature.Brain = null;

                // Give the new creature a reference to this brain.
                if (value != null)
                    value.Brain = this;
            }
        }

        public AnimancerComponent Animancer { get { return _Creature.Animancer; } }

        /************************************************************************************************************************/

        public Vector3 Movement { get; protected set; }

        /************************************************************************************************************************/
    }
}
