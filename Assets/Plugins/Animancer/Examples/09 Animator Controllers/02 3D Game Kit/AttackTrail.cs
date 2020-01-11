// Animancer // NOT Copyright 2019 Kybernetik //
// This script was copied from the 3D Game Kit with a few modifications.
// The original name was TimeEffect.

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System.Collections;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>
    /// A copy of the TimeEffect class in the 3D Game Kit, given a more descriptive name and cleaned up a bit. We can't
    /// just use that class because the Animancer assembly can't reference the 3D Game Kit assembly (because it isn't
    /// required to use the rest of Animancer).
    /// </summary>
    [AddComponentMenu("Animancer/Examples/Game Kit - Attack Trail")]
    [HelpURL(Strings.APIDocumentationURL + ".Examples.AnimatorControllers.GameKit/AttackTrail")]
    public sealed class AttackTrail : MonoBehaviour
    {
        /************************************************************************************************************************/

        /// <remarks>
        /// Was called `staffLight`, but there's no reason this script should be in any way specific to a staff.
        /// </remarks>
        [SerializeField] private Light _Light;

        /// <remarks>
        /// Was a non-serialized field that got retrieved by Awake, but this is more flexible and efficient.
        /// Note that this is a Legacy <see cref="Animation"/> component, not a Mecanim <see cref="Animator"/>.
        /// </remarks>
        [SerializeField] private Animation _Animation;

        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <remarks>
        /// Was an Awake method, but now that <see cref="_Animation"/> is serialized, we can just get it when this
        /// component is first added and still allow the user to assign any reference they want in the Inspector.
        /// </remarks>
        private void Reset()
        {
            _Light = Editor.AnimancerEditorUtilities.GetComponentInHierarchy<Light>(gameObject);
            _Animation = Editor.AnimancerEditorUtilities.GetComponentInHierarchy<Animation>(gameObject);
            gameObject.SetActive(false);
        }
#endif

        /************************************************************************************************************************/

        public void Activate()
        {
            gameObject.SetActive(true);
            _Light.enabled = true;

            // No point in null checking here because DisableAtEndOfAnimation wouldn't work without it anyway.
            //if (_Animation)
            _Animation.Play();

            StartCoroutine(DisableAtEndOfAnimation());
        }

        /************************************************************************************************************************/

        private IEnumerator DisableAtEndOfAnimation()
        {
            yield return new WaitForSeconds(_Animation.clip.length);

            gameObject.SetActive(false);
            _Light.enabled = false;
        }

        /************************************************************************************************************************/
    }
}
