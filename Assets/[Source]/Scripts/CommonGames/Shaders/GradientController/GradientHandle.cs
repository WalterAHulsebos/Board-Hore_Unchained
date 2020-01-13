
/*
==========================================
    Copyright (c) 2017 Dynamic_Static,
        Patrick Purcell
    Licensed under the MIT license
    http://opensource.org/licenses/MIT
==========================================
*/

#if ODIN_INSPECTOR

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;

#endif

namespace Dynamic_Static
{
    using UnityEngine;

    /// <summary>
    /// Provides color control for one point in a GradientController.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public sealed class GradientHandle
        : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private Color color = Color.white;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets or sets this GradientHandle's Color.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        [OnValueChanged(nameof(ChangeTransform))]
        public Transform handleTransform;

        [OdinSerialize]
        public Transform Transform => transform;
        
        public Transform HandleTransform
        {
            get
            {
                handleTransform = this.transform;
                
                return handleTransform;   
            }
            set
            {
                this.transform.SetPositionAndRotation(value.position, value.rotation);
                
                handleTransform = value;
            }
        }

        private void ChangeTransform()
        {
            HandleTransform = handleTransform;
        }

        #endregion

        #if UNITY_EDITOR
        #region ON DRAW GIZMOS
        private void OnDrawGizmos()
        {
            var gizmosColor = Gizmos.color;
            Gizmos.color = Color.clear;
            Gizmos.DrawCube(transform.position, transform.localScale);
            Gizmos.color = gizmosColor;
        }
        #endregion
        #endif
    }
} // namespace Dynamic_Static
