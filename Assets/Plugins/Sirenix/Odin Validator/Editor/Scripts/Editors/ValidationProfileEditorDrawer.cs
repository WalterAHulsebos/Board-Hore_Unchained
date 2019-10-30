//-----------------------------------------------------------------------
// <copyright file="ValidationProfileEditorDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class ValidationProfileEditorDrawer : OdinValueDrawer<ValidationProfileEditor>
    {
        private LocalPersistentContext<float> menuTreeWidth;
        private List<ResizableColumn> columns;
        private Vector2 scrollPos;
        private ValidationRunner runner;
        private ValidationProfileEditor editor;
        private IValidationProfile profile;
        private InspectorProperty sourceProperty;
        private ValidationProfileTree validationProfileTree;
        private ValidationOverview overview;
        private LocalPersistentContext<bool> overviewToggle;
        private float overviewHeight = 300;

        protected override void Initialize()
        {
            menuTreeWidth = this.GetPersistentValue<float>("menuTreeWidth", 380);
            columns = new List<ResizableColumn>() { ResizableColumn.FlexibleColumn(menuTreeWidth.Value, 80), ResizableColumn.DynamicColumn(0, 200) };
            runner = new ValidationRunner();
            overview = new ValidationOverview();
            editor = ValueEntry.SmartValue;
            profile = editor.Profile;
            sourceProperty = Property.Children["selectedSourceTarget"];
            validationProfileTree = new ValidationProfileTree();
            overviewToggle = this.GetPersistentValue<bool>("overviewToggle", true);

            validationProfileTree.Selection.SelectionChanged += (x) =>
            {
                if (x == SelectionChangedType.ItemAdded)
                {
                    object value = validationProfileTree.Selection.SelectedValue;
                    ValidationProfileResult result = value as ValidationProfileResult;
                    if (result != null)
                    {
                        editor.SetTarget(result.GetSource());

                    }
                    else
                    {
                        editor.SetTarget(value);
                    }
                }
            };

            overview.OnProfileResultSelected += result =>
            {
                OdinMenuItem mi = validationProfileTree.GetMenuItemForObject(result);
                mi.Select();
                object source = result.GetSource();
                editor.SetTarget(source);
            };

            validationProfileTree.AddProfileRecursive(ValueEntry.SmartValue.Profile);

            OdinMenuTree.ActiveMenuTree = validationProfileTree;

            if (editor.ScanProfileImmediatelyWhenOpening)
            {
                editor.ScanProfileImmediatelyWhenOpening = false;
                ScanProfile(editor.Profile);
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Save menuTreeWidth.
            menuTreeWidth.Value = columns[0].ColWidth;

            // Bottom Slide Toggle Bits:
            Rect overviewSlideRect = new Rect();
            Rect toggleOverviewBtnRect = new Rect();

            Rect topRect;
            GUILayout.BeginHorizontal(GUILayoutOptions.ExpandHeight());
            {
                topRect = GUIHelper.GetCurrentLayoutRect();
                GUITableUtilities.ResizeColumns(topRect, columns);

                // Bottom Slide Toggle Bits:
                // The bottom slide-rect toggle needs to be drawn above, but is placed below.
                overviewSlideRect = topRect.AlignBottom(4).AddY(4);
                overviewSlideRect.width += 4;
                toggleOverviewBtnRect = overviewSlideRect.AlignCenter(100).AlignBottom(14);
                EditorGUIUtility.AddCursorRect(toggleOverviewBtnRect, MouseCursor.Arrow);
                if (SirenixEditorGUI.IconButton(toggleOverviewBtnRect.AddY(-2), overviewToggle.Value ? EditorIcons.TriangleDown : EditorIcons.TriangleUp))
                {
                    overviewToggle.Value = !overviewToggle.Value;
                }

                if (overviewToggle.Value)
                {
                    overviewHeight -= SirenixEditorGUI.SlideRect(overviewSlideRect.SetXMax(toggleOverviewBtnRect.xMin), MouseCursor.SplitResizeUpDown).y;
                    overviewHeight -= SirenixEditorGUI.SlideRect(overviewSlideRect.SetXMin(toggleOverviewBtnRect.xMax), MouseCursor.SplitResizeUpDown).y;
                }

                // Left menu tree
                GUILayout.BeginVertical(GUILayoutOptions.Width(columns[0].ColWidth).ExpandHeight());
                {
                    EditorGUI.DrawRect(GUIHelper.GetCurrentLayoutRect(), SirenixGUIStyles.EditorWindowBackgroundColor);
                    validationProfileTree.Draw();
                }
                GUILayout.EndVertical();

                // Draw selected
                GUILayout.BeginVertical();
                {
                    DrawTopBarButtons();
                    DrawSelectedTests();
                }
                GUILayout.EndVertical();
                GUITableUtilities.DrawColumnHeaderSeperators(topRect, columns, SirenixGUIStyles.BorderColor);
            }
            GUILayout.EndHorizontal();

            // Bottom Slide Toggle Bits:
            if (overviewToggle.Value)
            {
                GUILayoutUtility.GetRect(0, 4); // Slide Area.
            }

            EditorGUI.DrawRect(overviewSlideRect, SirenixGUIStyles.BorderColor);
            EditorGUI.DrawRect(toggleOverviewBtnRect.AddY(-overviewSlideRect.height), SirenixGUIStyles.BorderColor);
            SirenixEditorGUI.IconButton(toggleOverviewBtnRect.AddY(-2), overviewToggle.Value ? EditorIcons.TriangleDown : EditorIcons.TriangleUp);

            // Overview
            if (overviewToggle.Value)
            {
                GUILayout.BeginVertical(GUILayout.Height(overviewHeight));
                {
                    overview.DrawOverview();
                }
                GUILayout.EndVertical();

                if (Event.current.type == EventType.Repaint)
                {
                    overviewHeight = Mathf.Max(50, overviewHeight);
                    float height = GUIHelper.CurrentWindow.position.height - overviewSlideRect.yMax;
                    overviewHeight = Mathf.Min(overviewHeight, height);
                }
            }
        }

        public void ScanProfile(IValidationProfile profile)
        {
            EditorApplication.delayCall += () =>
            {
                List<ValidationProfileResult> results = new List<ValidationProfileResult>();
                validationProfileTree.CleanProfile(profile);

                try
                {
                    foreach (ValidationProfileResult result in profile.Validate(runner))
                    {
                        validationProfileTree.AddResultToTree(result);
                        results.Add(result);

                        if (GUIHelper.DisplaySmartUpdatingCancellableProgressBar(result.Profile.Name, result.Name, result.Progress))
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                    overview.ProfileResults = results;
                    overview.Update();
                    validationProfileTree.MarkDirty();
                    validationProfileTree.UpdateMenuTree();
                    validationProfileTree.AddErrorAndWarningIcons();
                }
            };
        }

        private void DrawSelectedTests()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUIStyle.none);
            GUILayout.BeginVertical(SirenixGUIStyles.ContentPadding);
            GUIHelper.PushLabelWidth(columns.Last().ColWidth * 0.33f);
            sourceProperty.Draw(null);
            GUIHelper.PopLabelWidth();
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void DrawTopBarButtons()
        {
            string btnName = "Run " + profile.Name;
            float width = GUI.skin.button.CalcSize(GUIHelper.TempContent(btnName)).x + 10;
            Rect rect = GUIHelper.GetCurrentLayoutRect().AlignRight(width);
            rect.x -= 5;
            rect.y -= 26;
            rect.height = 18;

            GUIHelper.PushColor(Color.green);
            if (GUI.Button(rect, btnName))
            {
                ScanProfile(profile);
            }
            GUIHelper.PopColor();

            object selectedValue = validationProfileTree.Selection.SelectedValue;

            if (selectedValue is ValidationProfileResult)
            {
                ValidationProfileResult result = selectedValue as ValidationProfileResult;
                if (result != null)
                {
                    // Draw top bar buttons
                    Object source = result.GetSource() as UnityEngine.Object;
                    if (source != null)
                    {
                        rect.x -= 100;
                        rect.width = 90;
                        if (GUI.Button(rect, "Select Object", SirenixGUIStyles.ButtonRight))
                        {
                            GUIHelper.SelectObject(source);
                            GUIHelper.PingObject(source);
                        }
                        rect.x -= 80;
                        rect.width = 80;
                        if (GUI.Button(rect, "Ping Object", SirenixGUIStyles.ButtonLeft))
                        {
                            GUIHelper.PingObject(source);
                        }
                    }
                }
            }
        }
    }
}
