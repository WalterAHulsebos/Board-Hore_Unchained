//-----------------------------------------------------------------------
// <copyright file="ValidationProfileTree.cs" company="Sirenix IVS">
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
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ValidationProfileTree : OdinMenuTree
    {
        private Dictionary<object, OdinMenuItem> childMenuItemLookup;
        public static Color red = new Color(0.787f, 0.133f, 0.133f, 1f);
        public static Color orange = new Color(0.934f, 0.66f, 0.172f, 1f);

        public OdinMenuItem GetMenuItemForObject(object obj)
        {
            OdinMenuItem result;
            childMenuItemLookup.TryGetValue(obj, out result);
            return result;
        }

        public ValidationProfileTree()
        {
            Config.DrawSearchToolbar = true;
            Config.AutoHandleKeyboardNavigation = true;
            Config.UseCachedExpandedStates = false;

            DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
            childMenuItemLookup = new Dictionary<object, OdinMenuItem>();

            Selection.SelectionConfirmed += (x) =>
            {
                OdinMenuItem sel = x.FirstOrDefault();
                if (sel != null && sel.Value is ValidationProfileResult)
                {
                    ValidationProfileResult result = sel.Value as ValidationProfileResult;
                    if (result != null)
                    {
                        UnityEngine.Object source = result.GetSource() as UnityEngine.Object;
                        GUIHelper.SelectObject(source);
                    }
                }
            };
        }

        public void AddProfileRecursive(IValidationProfile profile, OdinMenuItem menuItem = null)
        {
            menuItem = menuItem ?? RootMenuItem;

            OdinMenuItem newMenuItem = new OdinMenuItem(this, profile.Name, profile)
            {
                Icon = profile.GetProfileIcon()
            };

            childMenuItemLookup[profile] = newMenuItem;

            if (profile is ValidationProfileAsset)
            {
                IValidationProfile wrappedProfile = (profile as ValidationProfileAsset).GetWrappedProfile();
                childMenuItemLookup[wrappedProfile] = newMenuItem;
            }

            menuItem.ChildMenuItems.Add(newMenuItem);

            foreach (IValidationProfile childProfile in profile.GetNestedValidationProfiles())
            {
                AddProfileRecursive(childProfile, newMenuItem);
            }

            if (menuItem == RootMenuItem)
            {
                EnumerateTree().ForEach(x => x.Toggled = true);
                UpdateMenuTree();
            }
        }

        public void BuildTree(object obj)
        {
            OdinMenuItem selected = Selection.FirstOrDefault();
            if (selected != null && selected.Value != obj)
            {
                OdinMenuItem menuItem = EnumerateTree().FirstOrDefault(n => n.Value == obj);
                if (menuItem != null)
                {
                    menuItem.Select();
                }
            }
        }

        public void Draw()
        {
            DrawMenuTree();
        }

        public void AddResultToTree(ValidationProfileResult result)
        {
            if (result.Results == null)
            {
                return;
            }

            if (result.Results.Any(x => x.ResultType != ValidationResultType.Valid))
            {
                OdinMenuItem menuItem = new OdinMenuItem(this, result.Name, result);

                Scene scene = default(Scene);
                if (result.Source as UnityEngine.Object)
                {
                    Component component = result.Source as Component;
                    GameObject go = result.Source as GameObject;
                    if (component)
                    {
                        go = component.gameObject;
                    }

                    if (go)
                    {
                        scene = go.scene;
                    }
                }

                childMenuItemLookup[result] = menuItem;

                if (result.Profile != null && scene.IsValid() && !childMenuItemLookup.ContainsKey(scene.path) && childMenuItemLookup.ContainsKey(result.Profile))
                {
                    OdinMenuItem sceneItem = new OdinMenuItem(this, scene.name, scene.path);
                    sceneItem.IconGetter = () => EditorIcons.UnityLogo;
                    sceneItem.Toggled = true;
                    childMenuItemLookup.Add(scene.path, sceneItem);
                    childMenuItemLookup[result.Profile].ChildMenuItems.Add(sceneItem);
                }

                if (scene.IsValid() && childMenuItemLookup.ContainsKey(scene.path))
                {
                    childMenuItemLookup[scene.path].ChildMenuItems.Add(menuItem);
                }
                else if (result.Profile == null || !childMenuItemLookup.ContainsKey(result.Profile))
                {
                    MenuItems.Add(menuItem);
                }
                else
                {
                    childMenuItemLookup[result.Profile].ChildMenuItems.Add(menuItem);
                }

                if (result.Source != null)
                {
                    Component component = result.Source as UnityEngine.Component;
                    if (component)
                    {
                        menuItem.Icon = GUIHelper.GetAssetThumbnail(component.gameObject, null, false);
                    }
                    else
                    {
                        menuItem.Icon = GUIHelper.GetAssetThumbnail(result.Source as UnityEngine.Object, result.Source.GetType(), false);
                    }
                }
                else
                {
                    menuItem.Icon = EditorIcons.Transparent.Active;
                }
            }
        }

        public void CleanProfile(IValidationProfile profile)
        {
            OdinMenuItem menuItem;
            if (childMenuItemLookup.TryGetValue(profile, out menuItem))
            {
                List<OdinMenuItem> allProfileMenuItems = menuItem.GetChildMenuItemsRecursive(true).Where(x => x.Value is IValidationProfile).ToList();

                foreach (OdinMenuItem pi in allProfileMenuItems)
                {
                    pi.ChildMenuItems.RemoveAll(x => !(x.Value is IValidationProfile));
                }
            }

            MarkDirty();
            RebuildChildMenuItemLookup();
        }

        private void RebuildChildMenuItemLookup()
        {
            childMenuItemLookup.Clear();

            foreach (OdinMenuItem item in EnumerateTree())
            {
                childMenuItemLookup[item.Value] = item;

                if (item.Value is ValidationProfileAsset)
                {
                    IValidationProfile wrappedProfile = (item.Value as ValidationProfileAsset).GetWrappedProfile();
                    childMenuItemLookup[wrappedProfile] = item;
                }
            }
        }

        public void AddErrorAndWarningIcons()
        {
            Dictionary<OdinMenuItem, int> errorCount = new Dictionary<OdinMenuItem, int>();
            Dictionary<OdinMenuItem, int> warningCount = new Dictionary<OdinMenuItem, int>();
            int maxECount = 0;
            int maxWCount = 0;

            foreach (OdinMenuItem mi in EnumerateTree())
            {
                ValidationProfileResult result = mi.Value as ValidationProfileResult;
                if (result == null || result.Results == null || result.Results.Count == 0) continue;

                int ec = result.Results.Count(x => x.ResultType == ValidationResultType.Error);
                int wc = result.Results.Count(x => x.ResultType == ValidationResultType.Warning);

                foreach (OdinMenuItem mm in mi.GetParentMenuItemsRecursive(true))
                {
                    if (!errorCount.ContainsKey(mm)) errorCount[mm] = 0;
                    if (!warningCount.ContainsKey(mm)) warningCount[mm] = 0;
                    maxECount = Math.Max(ec, errorCount[mm] += ec);
                    maxWCount = Math.Max(wc, warningCount[mm] += wc);
                }
            }

            //var wStyle = new GUIStyle("sv_label_5");
            //var eStyle = new GUIStyle("sv_label_6");
            //wStyle.alignment = TextAnchor.MiddleCenter;
            //eStyle.alignment = TextAnchor.MiddleCenter;

            float eCountWidth = SirenixGUIStyles.LeftAlignedWhiteMiniLabel.CalcSize(new GUIContent(maxECount + " ")).x;
            float wCountWidth = SirenixGUIStyles.LeftAlignedWhiteMiniLabel.CalcSize(new GUIContent(maxWCount + " ")).x;

            wCountWidth = eCountWidth = Mathf.Max(eCountWidth, wCountWidth);

            foreach (OdinMenuItem mi in EnumerateTree())
            {
                if (!errorCount.ContainsKey(mi)) errorCount[mi] = 0;
                if (!warningCount.ContainsKey(mi)) warningCount[mi] = 0;

                int ec = errorCount[mi];
                int wc = warningCount[mi];
                GUIContent ecl = new GUIContent(ec + "");
                GUIContent wcl = new GUIContent(wc + "");

                mi.OnDrawItem += (m) =>
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        Rect rect = m.Rect.Padding(10, 5);
                        rect.height += 1;
                        Rect errorRect = rect.AlignRight(eCountWidth);
                        Rect warningRect = errorRect.SubX(wCountWidth - 1);
                        warningRect.width = wCountWidth;

                        bool hasErrors = ec > 0;
                        if (hasErrors)
                        {
                            SirenixEditorGUI.DrawSolidRect(errorRect, red);
                            //SirenixEditorGUI.DrawBorders(errorRect, 1);
                            errorRect.y -= 1;
                            errorRect.x -= 1;
                            GUI.Label(errorRect, ecl, SirenixGUIStyles.CenteredWhiteMiniLabel);
                        }

                        bool hasWarnings = wc > 0;
                        if (hasWarnings)
                        {
                            warningRect.x -= 1;
                            SirenixEditorGUI.DrawSolidRect(warningRect, orange);
                            //SirenixEditorGUI.DrawBorders(warningRect, 1);
                            warningRect.y -= 1;
                            GUI.Label(warningRect, wcl, SirenixGUIStyles.CenteredBlackMiniLabel);
                        }
                    }
                };
            }
        }
    }
}
