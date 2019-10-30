//-----------------------------------------------------------------------
// <copyright file="ValidationOverview.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class ValidationOverview
    {
        private static readonly DisplayOptions[] AllDisplayOptions = Enum.GetValues(typeof(DisplayOptions)).Cast<DisplayOptions>().Where(n => n != DisplayOptions.None).ToArray();

        public string Title;
        public List<ValidationProfileResult> ProfileResults = new List<ValidationProfileResult>();
        public event Action<ValidationProfileResult> OnProfileResultSelected;
        public DisplayOptions Display = DisplayOptions.Message | DisplayOptions.Object | DisplayOptions.Category;
        public OdinMenuTree Tree;
        public DisplayOptions SortBy;
        public bool SortAscending;

        [Flags]
        public enum DisplayOptions
        {
            [HideInInspector]
            None = 0,
            Category = 1 << 0,
            Message = 1 << 1,
            Path = 1 << 2,
            Validator = 1 << 3,
            Object = 1 << 4,
            Scene = 1 << 5,
        }

        private Dictionary<DisplayOptions, ResizableColumn> columnLookup = new Dictionary<DisplayOptions, ResizableColumn>();
        private ResizableColumn[] columns;
        private static GUIStyle EnumBtnStyle;
        private DisplayOptions? nextDisplay;
        private bool shouldSort;

        public ValidationOverview()
        {
            Update();
        }

        public void ResetSortingSettings()
        {
            SortBy = DisplayOptions.None;
            SortAscending = false;
        }

        public void Update()
        {
            List<DisplayOptions> displayOptions = AllDisplayOptions.Where(option => (Display & option) == option).ToList();

            // Create/adjust columns
            if (columns == null)
            {
                columns = new ResizableColumn[displayOptions.Count];
            }
            else
            {
                Array.Resize(ref columns, displayOptions.Count);
            }

            for (int i = 0; i < displayOptions.Count; i++)
            {
                columns[i] = GetColumn(displayOptions[i]);
            }

            // Create tree
            {
                Tree = new OdinMenuTree();
                Tree.Config.AutoHandleKeyboardNavigation = true;
                Tree.Config.UseCachedExpandedStates = false;
                Tree.Config.EXPERIMENTAL_INTERNAL_DrawFlatTreeFastNoLayout = true;
                Tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;

                int itemCount = 0;

                foreach (ValidationProfileResult profileResult in ProfileResults)
                {
                    foreach (ValidationResult validationResult in profileResult.Results)
                    {
                        if (validationResult.ResultType != ValidationResultType.Error && validationResult.ResultType != ValidationResultType.Warning) continue;

                        ValidationInfoMenuItem menuItem = new ValidationInfoMenuItem(Tree, validationResult, profileResult, columns, Display, itemCount++);
                        Tree.MenuItems.Add(menuItem);
                    }
                }

                Tree.Selection.SelectionChanged += changeEvent =>
                {
                    if (changeEvent != SelectionChangedType.ItemAdded) return;

                    if (OnProfileResultSelected != null)
                    {
                        ValidationInfoMenuItem menuItem = Tree.Selection.Last() as ValidationInfoMenuItem;
                        ValidationProfileResult profileResult = menuItem.ProfileResult;

                        OnProfileResultSelected(profileResult);
                    }
                };
            }

            if (SortBy != DisplayOptions.None)
                Sort();
        }

        public void DrawOverview()
        {
            if (Event.current.type == EventType.Layout && nextDisplay != null)
            {
                Display = nextDisplay.Value;
                nextDisplay = null;
                Update();
            }

            if (Event.current.type == EventType.Layout && shouldSort)
            {
                shouldSort = false;
                Sort();
            }

            EnumBtnStyle = EnumBtnStyle ?? new GUIStyle(EditorStyles.toolbarDropDown);
            EnumBtnStyle.stretchHeight = true;
            EnumBtnStyle.fixedHeight = Tree.Config.SearchToolbarHeight;

            GUILayout.BeginHorizontal(GUILayoutOptions.ExpandHeight());
            {
                Rect rect = GUIHelper.GetCurrentLayoutRect();
                Rect columnRect = rect.AddYMin(Tree.Config.SearchToolbarHeight);

                GUILayout.BeginVertical(GUILayoutOptions.Width(rect.width).ExpandHeight());
                {
                    EditorGUI.DrawRect(columnRect.AddYMin(Tree.Config.DefaultMenuStyle.Height), SirenixGUIStyles.EditorWindowBackgroundColor);

                    GUILayout.BeginHorizontal();
                    Tree.DrawSearchToolbar();
                    Rect displayRect = GUILayoutUtility.GetRect(95, Tree.Config.SearchToolbarHeight, GUILayoutOptions.Width(95));
                    displayRect.height = Tree.Config.SearchToolbarHeight;
                    displayRect.width -= 1;

                    DisplayOptions newDisplay = EnumSelector<DisplayOptions>.DrawEnumField(displayRect, null, GUIHelper.TempContent("Data Columns"), Display, EnumBtnStyle);
                    if (newDisplay != Display)
                    {
                        nextDisplay = newDisplay;
                    }

                    GUILayout.EndHorizontal();

                    GUITableUtilities.ResizeColumns(columnRect, columns);
                    DrawColumnHeaders();
                    Tree.DrawMenuTree();
                }

                GUILayout.EndVertical();
                GUITableUtilities.DrawColumnHeaderSeperators(columnRect, columns, SirenixGUIStyles.BorderColor);

                GUILayout.Space(-5);
            }
            GUILayout.EndHorizontal();
        }

        private void Sort()
        {
            // We can't just use list.Sort(), because we want a stable sort - LINQ's OrderBy provides that
            List<OdinMenuItem> items = Tree.MenuItems.OrderBy(a => (ValidationInfoMenuItem)a, new ItemComparer(SortBy, SortAscending)).ToList();
            Tree.MenuItems.Clear();
            Tree.MenuItems.AddRange(items);
            Tree.MarkDirty();
        }

        private class ItemComparer : IComparer<ValidationInfoMenuItem>
        {
            public ItemComparer(DisplayOptions option, bool ascending)
            {
                Option = option;
                Ascending = ascending;
            }

            public DisplayOptions Option;
            public bool Ascending;

            public int Compare(ValidationInfoMenuItem a, ValidationInfoMenuItem b)
            {
                int result = DoCompare(a, b);
                return Ascending ? -result : result;
            }

            private int DoCompare(ValidationInfoMenuItem a, ValidationInfoMenuItem b)
            {
                switch (Option)
                {
                    case DisplayOptions.Message:
                        return a.ValidationResult.Message.CompareTo(b.ValidationResult.Message);
                    case DisplayOptions.Path:
                        return a.ValidationResult.GetFullPath().CompareTo(b.ValidationResult.GetFullPath());
                    case DisplayOptions.Validator:
                        return a.ValidationResult.Setup.Validator.GetType().GetNiceName().CompareTo(b.ValidationResult.Setup.Validator.GetType().GetNiceName());
                    case DisplayOptions.Object:
                        return a.ProfileResult.Name.CompareTo(b.ProfileResult.Name);
                    case DisplayOptions.Scene:
                        return a.SceneName.CompareTo(b.SceneName);
                    case DisplayOptions.Category:
                        return a.ValidationResult.ResultType.CompareTo(b.ValidationResult.ResultType);
                    default:
                        return a.OriginalItemIndex.CompareTo(b.OriginalItemIndex);
                }
            }
        }

        private void DrawColumnHeaders()
        {
            Rect columnsRect = GUILayoutUtility.GetRect(0, Tree.Config.DefaultMenuStyle.Height, GUILayoutOptions.ExpandWidth(true));

            EditorGUI.DrawRect(columnsRect, SirenixGUIStyles.DarkEditorBackground);

            //SirenixGUIStyles.Temporary.Draw(columnsRect, GUIContent.none, 0);

            int columnIndex = 0;
            float currentX = columnsRect.xMin;

            for (int i = 0; i < AllDisplayOptions.Length; i++)
            {
                DisplayOptions option = AllDisplayOptions[i];

                if ((Display & option) == option)
                {
                    float width = columns[columnIndex].ColWidth;
                    Rect rect = new Rect(currentX, columnsRect.yMin + 3, width - 0.5f, columnsRect.height);

                    rect.xMax = Math.Min(rect.xMax, columnsRect.xMax);

                    if (rect.width <= 0) break;

                    string labelText = option == DisplayOptions.Category ? "" : option.ToString();

                    if (GUI.Button(rect, GUIHelper.TempContent(labelText), SirenixGUIStyles.BoldLabel))
                    {
                        if (SortBy == option)
                        {
                            SortAscending = !SortAscending;
                        }
                        else
                        {
                            SortBy = option;
                            SortAscending = false;
                        }

                        shouldSort = true;
                    }

                    Rect iconRect = rect.AlignRight(rect.height).Padding(3).SubY(3);
                    EditorIcon icon;

                    if (SortBy != option)
                    {
                        icon = EditorIcons.TriangleRight;
                        GUIHelper.PushColor(GUI.color * 0.7f);
                    }
                    else
                    {
                        icon = SortAscending ? EditorIcons.TriangleUp : EditorIcons.TriangleDown;
                    }

                    icon.Draw(iconRect);

                    if (SortBy != option)
                    {
                        GUIHelper.PopColor();
                    }

                    currentX += width;
                    columnIndex++;
                }
            }

            SirenixEditorGUI.DrawHorizontalLineSeperator(columnsRect.xMin, columnsRect.yMax, columnsRect.width, 0.5f);
        }

        private ResizableColumn GetColumn(DisplayOptions option)
        {
            ResizableColumn result;
            if (!columnLookup.TryGetValue(option, out result))
            {
                switch (option)
                {
                    case DisplayOptions.Category:
                        result = ResizableColumn.FixedColumn(23);
                        break;
                    case DisplayOptions.Message:
                        result = ResizableColumn.DynamicColumn(250, 50);
                        break;
                    case DisplayOptions.Path:
                        result = ResizableColumn.FlexibleColumn(150, 50);
                        break;
                    case DisplayOptions.Validator:
                        result = ResizableColumn.FlexibleColumn(150, 50);
                        break;
                    case DisplayOptions.Object:
                        result = ResizableColumn.FlexibleColumn(150, 50);
                        break;
                    case DisplayOptions.Scene:
                        result = ResizableColumn.FlexibleColumn(150, 50);
                        break;
                    default:
                        result = ResizableColumn.FlexibleColumn(150, 50);
                        break;
                }

                columnLookup[option] = result;
            }

            return result;
        }

        private class ValidationInfoMenuItem : OdinMenuItem
        {
            public ValidationResult ValidationResult;
            public ValidationProfileResult ProfileResult;
            public ResizableColumn[] Columns;
            public DisplayOptions DisplayOptions;
            public int OriginalItemIndex;
            public string SceneName;

            public ValidationInfoMenuItem(OdinMenuTree tree, ValidationResult validationResult, ValidationProfileResult profileResult, ResizableColumn[] columns, DisplayOptions displayOptions, int originalItemIndex) : base(tree, "", validationResult)
            {
                ValidationResult = validationResult;
                ProfileResult = profileResult;
                Columns = columns;
                DisplayOptions = displayOptions;
                OriginalItemIndex = originalItemIndex;

                if (ProfileResult.SourceRecoveryData is SceneValidationProfile.SceneAddress)
                {
                    SceneValidationProfile.SceneAddress address = (SceneValidationProfile.SceneAddress)ProfileResult.SourceRecoveryData;
                    SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(address.ScenePath);
                    SceneName = sceneAsset != null ? sceneAsset.name : "";
                }
                else SceneName = "";

                SearchString = string.Join(" ", AllDisplayOptions.Select(x => GetDisplayString(x)).ToArray());
            }

            public override void DrawMenuItem(int indentLevel)
            {
                base.DrawMenuItem(indentLevel);

                if (!MenuItemIsBeingRendered || Event.current.type != EventType.Repaint) return;

                Rect totalRect = Rect;

                int columnIndex = 0;
                float currentX = totalRect.xMin;

                for (int i = 0; i < AllDisplayOptions.Length; i++)
                {
                    DisplayOptions option = AllDisplayOptions[i];

                    if ((DisplayOptions & option) == option)
                    {
                        float width = Columns[columnIndex].ColWidth;
                        Rect rect = new Rect(currentX, totalRect.yMin, width, totalRect.height);

                        if (option == DisplayOptions.Category)
                        {
                            rect = rect.AlignCenter(16, 16);

                            switch (ValidationResult.ResultType)
                            {
                                case ValidationResultType.Valid:
                                    GUIHelper.PushColor(Color.green);
                                    GUI.DrawTexture(rect, EditorIcons.Checkmark.Highlighted, ScaleMode.ScaleToFit);
                                    GUIHelper.PopColor();
                                    break;
                                case ValidationResultType.Error:
                                    GUI.DrawTexture(rect, EditorIcons.UnityErrorIcon, ScaleMode.ScaleToFit);
                                    break;
                                case ValidationResultType.Warning:
                                    GUI.DrawTexture(rect, EditorIcons.UnityWarningIcon, ScaleMode.ScaleToFit);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            rect.y = LabelRect.y;
                            rect.yMax = LabelRect.yMax;
                            rect.x += 5;
                            rect.width -= 10;

                            GUIStyle labelStyle = IsSelected ? Style.SelectedLabelStyle : Style.DefaultLabelStyle;
                            GUI.Label(rect, GUIHelper.TempContent(GetDisplayString(option)), labelStyle);
                        }

                        currentX += width;
                        columnIndex++;
                    }
                }
            }

            private string GetDisplayString(DisplayOptions option)
            {
                switch (option)
                {
                    case DisplayOptions.Message:
                        return ValidationResult.Message;
                    case DisplayOptions.Path:
                        {
                            string path = ValidationResult.GetFullPath();

                            if (string.IsNullOrEmpty(path) && ProfileResult.Source is UnityEngine.Object)
                            {
                                UnityEngine.Object uObj = ProfileResult.Source as UnityEngine.Object;
                                if (AssetDatabase.Contains(uObj.GetInstanceID()))
                                {
                                    path = AssetDatabase.GetAssetPath(uObj.GetInstanceID());
                                }
                            }

                            return path;
                        }
                    case DisplayOptions.Validator:
                        return ValidationResult.Setup.Validator.GetType().GetNiceName();
                    case DisplayOptions.Object:
                        return ProfileResult.Name;
                    case DisplayOptions.Scene:
                        return SceneName;
                    case DisplayOptions.Category:
                        switch (ValidationResult.ResultType)
                        {
                            case ValidationResultType.Error:
                                return "Error";
                            case ValidationResultType.Warning:
                                return "Warning";
                            case ValidationResultType.Valid:
                            case ValidationResultType.IgnoreResult:
                            default:
                                return "";
                        }
                    default:
                        return "";
                }
            }
        }
    }
}
