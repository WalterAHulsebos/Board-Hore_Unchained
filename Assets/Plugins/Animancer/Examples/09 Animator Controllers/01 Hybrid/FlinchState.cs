// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0618 // Type or member is obsolete (for HybridAnimancerComponent in Animancer Lite).

using Animancer.Examples.StateMachines.Brains;
using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers
{
    /// <summary>
    /// A <see cref="CreatureState"/> that plays a flinch animation.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Hybrid - Flinch State")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers/FlinchState")]
    public sealed class FlinchState : CreatureState
    {
        /************************************************************************************************************************/

        private static readonly int StateName = Animator.StringToHash("Flinch");

        /************************************************************************************************************************/

        /// <summary>
        /// Unlike with Animancer, calling CrossFade (or Play) on an Animator Controller won't actually put it into
        /// that state immediately, so we need to wait for at least one frame before <see cref="FixedUpdate"/>
        /// actually starts checking if the animation is finished.
        /// </summary>
        private bool _JustStarted;

        /************************************************************************************************************************/

        private void OnEnable()
        {
            Animancer.CrossFade(StateName, 0.1f);
            _JustStarted = true;
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            Creature.Rigidbody.velocity = Vector3.zero;

            // Don't check if the animation is done on the first frame because it might not have even started playing.
            if (_JustStarted)
            {
                _JustStarted = false;
                return;
            }

            var current = Animancer.GetCurrentAnimatorStateInfo(0);
            var next = Animancer.GetNextAnimatorStateInfo(0);

            // If we are currently in the Flinch state and transitioning to something else, go to Idle.
            if (current.shortNameHash == StateName)
            {
                if (next.fullPathHash != 0)
                    Creature.Idle.ForceEnterState();
            }
            else// Or if we aren't in the flinch state and aren't transitioning to it, go to Idle as well.
            {
                if (next.shortNameHash != StateName)
                    Creature.Idle.ForceEnterState();
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The only thing that can interrupt a flinch is another flinch.
        /// </summary>
        public override bool CanExitState(CreatureState nextState)
        {
            return nextState == this;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Normally a state like this would use its Awake method to register a callback to an "OnHitReceived" event in
        /// order to trigger itself, similar to the way <see cref="StateMachines.Platformer.DieState"/> checks if the
        /// creature needs to die whenever damage is taken. That way, a creature without a Flinch/Die state won't bother
        /// checking a condition it doesn't care about (such as if an NPC can't be attacked).
        /// <para></para>
        /// But for this example we just expose this method so it can be called by a UI Button.
        /// </summary>
        public void OnTakeDamage()
        {
            Creature.StateMachine.TrySetState(this);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Normally the <see cref="Creature"/> class would have a reference to the specific type of
        /// <see cref="AnimancerComponent"/> we want, but for the sake of reusing code from the earlier example, we
        /// just use a type cast here.
        /// </summary>
        private new HybridAnimancerComponent Animancer
        {
            get { return (HybridAnimancerComponent)Creature.Animancer; }
        }

        /************************************************************************************************************************/
    }
}
