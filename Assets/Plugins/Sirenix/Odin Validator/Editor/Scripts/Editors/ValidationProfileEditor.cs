//-----------------------------------------------------------------------
// <copyright file="ValidationProfileEditor.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class ValidationProfileEditor
    {
        public bool ScanProfileImmediatelyWhenOpening;
        public IValidationProfile Profile;

        [Title("$targetTitle"), ShowIf("selectedSourceTarget", Animate = false), DisableContextMenu, InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden), SuppressInvalidAttributeError, ShowInInspector, HideReferenceObjectPicker]
        private object selectedSourceTarget;

#pragma warning disable 0414 // Remove unread private members
        private string targetTitle;
#pragma warning restore 0414 // Remove unread private members

        private string GetTargetTitle()
        {
            string result = "Validated Object";

            Object obj = selectedSourceTarget as UnityEngine.Object;

            if (obj != null)
            {
                result += ": " + obj.name;

                if (selectedSourceTarget is Component)
                {
                    Component com = selectedSourceTarget as Component;

                    if (com.gameObject.scene.IsValid())
                    {
                        result += " (" + com.gameObject.scene.name + ")";
                    }
                }
            }

            return result;
        }

        public void SetTarget(object target)
        {
            selectedSourceTarget = target;
            targetTitle = GetTargetTitle();
        }

        public ValidationProfileEditor(IValidationProfile profile)
        {
            Profile = profile;
        }

    }
}
