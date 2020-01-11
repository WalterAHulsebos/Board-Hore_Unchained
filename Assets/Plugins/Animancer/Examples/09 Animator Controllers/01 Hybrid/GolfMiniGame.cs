// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.Examples.StateMachines.Brains;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers
{
    /// <summary>
    /// A <see cref="CreatureState"/> which allows the player to play golf using the
    /// <see cref="AnimationEvents.GolfHitController"/> script.
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Hybrid - Golf Mini Game")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers/GolfMiniGame")]
    public sealed class GolfMiniGame : CreatureState
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimationEvents.GolfHitController _GolfHitController;
        [SerializeField] private Transform _GolfClub;
        [SerializeField] private Transform _ExitPoint;
        [SerializeField] private GameObject _RegularControls;
        [SerializeField] private GameObject _GolfControls;

        private Vector3 _GolfClubStartPosition;
        private Quaternion _GolfClubStartRotation;

        /************************************************************************************************************************/

        private void Awake()
        {
            _GolfClubStartPosition = _GolfClub.localPosition;
            _GolfClubStartRotation = _GolfClub.localRotation;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// When a <see cref="Creature"/> enters this trigger, try to make it enter this state.
        /// </summary>
        private void OnTriggerEnter(Collider collider)
        {
            if (enabled)
                return;

            Creature = collider.GetComponent<Creature>();
            if (Creature != null)
                Creature.StateMachine.TrySetState(this);
        }

        /************************************************************************************************************************/

        private void OnEnable()
        {
            // When this state is entered, disable the Creature's movement and move them next to the golf ball.
            Creature.Rigidbody.velocity = Vector3.zero;
            Creature.Rigidbody.isKinematic = true;
            Creature.Rigidbody.position = _GolfHitController.transform.position;
            Creature.Animancer.transform.rotation = _GolfHitController.transform.rotation;
            Creature.Brain.enabled = false;

            // Put the GolfClub in their hand, specifically as a child of the "Holder.R" object which is positioned
            // correctly for holding objects.
            var rightHand = Creature.Animancer.Animator.GetBoneTransform(HumanBodyBones.RightHand);
            rightHand = rightHand.Find("Holder.R");
            _GolfClub.parent = rightHand;
            _GolfClub.localPosition = Vector3.zero;
            _GolfClub.localRotation = Quaternion.identity;

            // Activate the GolfHitController. This state doesn't actually do anything, it simply occupies the
            // creature's state machine while allowing the GolfHitController to do whatever it wants.
            _GolfHitController.gameObject.SetActive(true);

            // Swap the displayed controls.
            _RegularControls.SetActive(false);
            _GolfControls.SetActive(true);
        }

        /************************************************************************************************************************/

        /// <remarks>
        /// Usually we could use OnDisable for anything we need to do when a state is exited, but it also gets called
        /// when unloading the scene, which means some of our references might be null.
        /// </remarks>
        public override void OnExitState()
        {
            base.OnExitState();

            // Basically just undo everything OnEnable did.

            _GolfHitController.gameObject.SetActive(false);
            _RegularControls.SetActive(true);
            _GolfControls.SetActive(false);

            _GolfClub.parent = transform;
            _GolfClub.localPosition = _GolfClubStartPosition;
            _GolfClub.localRotation = _GolfClubStartRotation;

            Creature.Rigidbody.position = _ExitPoint.position;
            Creature.Rigidbody.rotation = _ExitPoint.rotation;
            Creature.Rigidbody.isKinematic = false;
            Creature.Brain.enabled = true;
        }

        /************************************************************************************************************************/
    }
}
