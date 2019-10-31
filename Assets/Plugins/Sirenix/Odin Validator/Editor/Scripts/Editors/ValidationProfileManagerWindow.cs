//-----------------------------------------------------------------------
// <copyright file="ValidationProfileManagerWindow.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class ValidationProfileManagerWindow : OdinEditorWindow
    {
        private SlidePageNavigationHelper<object> pager;

        [MenuItem("Tools/Odin Project Validator")]
        public static void OpenProjectValidator()
        {
            ValidationProfileManagerWindow window = GetWindow<ValidationProfileManagerWindow>();
            window.Show();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(870, 700);
            window.pager = new SlidePageNavigationHelper<object>();
            window.pager.PushPage(new ValidationProfileManagerOverview(window.pager), "Overview");
        }

        internal class ValidationProfileEditorWrapper
        {
#pragma warning disable 0414 // Remove unread private members
            [DisableContextMenu, ShowInInspector, HideReferenceObjectPicker]
            private ValidationProfileEditor validationProfileEditor;
#pragma warning restore 0414 // Remove unread private members

            public ValidationProfileEditorWrapper(ValidationProfileEditor validationProfileEditor)
            {
                this.validationProfileEditor = validationProfileEditor;
            }
        }

        public static void OpenProjectValidatorWithProfile(IValidationProfile profile, bool scanProfileImmediately = false)
        {
            ValidationProfileManagerWindow window = GetWindow<ValidationProfileManagerWindow>();
            window.Show();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(670, 700);
            window.pager = new SlidePageNavigationHelper<object>();
            window.pager.PushPage(new ValidationProfileManagerOverview(window.pager), "Overview");

            ValidationProfileEditor editor = new ValidationProfileEditor(profile);
            editor.ScanProfileImmediatelyWhenOpening = scanProfileImmediately;
            window.pager.PushPage(new ValidationProfileEditorWrapper(editor), profile.Name);
        }

        protected override void Initialize()
        {
            WindowPadding = new Vector4(0, 0, 0, 0);
            if (pager == null)
            {
                pager = new SlidePageNavigationHelper<object>();
                pager.PushPage(new ValidationProfileManagerOverview(pager), "Overview");
            }
        }

        protected override void DrawEditors()
        {
            SirenixEditorGUI.DrawSolidRect(new Rect(0, 0, position.width, position.height), SirenixGUIStyles.DarkEditorBackground);

            // Draw top pager:
            Rect rect = GUIHelper.GetCurrentLayoutRect().AlignTop(34);
            SirenixEditorGUI.DrawSolidRect(rect, SirenixGUIStyles.EditorWindowBackgroundColor);
            SirenixEditorGUI.DrawBorders(rect, 0, 0, 0, 1);
            pager.DrawPageNavigation(rect.AlignCenterY(20).HorizontalPadding(10));

            // Draw pages:
            pager.BeginGroup();
            int i = 0;
            foreach (SlidePageNavigationHelper<object>.Page page in pager.EnumeratePages)
            {
                if (page.BeginPage())
                {
                    GUILayout.BeginVertical(GUILayoutOptions.ExpandHeight(true));
                    GUILayout.Space(30);
                    DrawEditor(i);
                    GUILayout.EndVertical();
                }
                page.EndPage();
                i++;
            }
            pager.EndGroup();
        }

        protected override IEnumerable<object> GetTargets()
        {
            return pager.EnumeratePages.Select(x => x.Value);
        }
    }
}
