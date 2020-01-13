// Animancer // Copyright 2019 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Internal]
    /// A custom Inspector for an <see cref="AnimancerLayer"/> which sorts and exposes some of its internal values.
    /// </summary>
    public sealed class AnimancerLayerDrawer : AnimancerNodeDrawer<AnimancerLayer>
    {
        /************************************************************************************************************************/

        /// <summary>The states in the target layer which have non-zero <see cref="AnimancerNode.Weight"/>.</summary>
        public readonly List<AnimancerState> ActiveStates = new List<AnimancerState>();

        /// <summary>The states in the target layer which have zero <see cref="AnimancerNode.Weight"/>.</summary>
        public readonly List<AnimancerState> InactiveStates = new List<AnimancerState>();

        /************************************************************************************************************************/

        /// <summary>The <see cref="GUIStyle"/> used for the area encompassing this drawer. <see cref="GUISkin.box"/>.</summary>
        protected override GUIStyle RegionStyle { get { return GUI.skin.box; } }

        /************************************************************************************************************************/
        #region Gathering
        /************************************************************************************************************************/

        /// <summary>
        /// Initialises an editor in the list for each layer in the 'animancer'.
        /// <para></para>
        /// The 'count' indicates the number of elements actually being used. Spare elements are kept in the list in
        /// case they need to be used again later.
        /// </summary>
        internal static void GatherLayerEditors(AnimancerPlayable animancer, List<AnimancerLayerDrawer> editors, out int count)
        {
            count = animancer.LayerCount;
            for (int i = 0; i < count; i++)
            {
                AnimancerLayerDrawer editor;
                if (editors.Count <= i)
                {
                    editor = new AnimancerLayerDrawer();
                    editors.Add(editor);
                }
                else
                {
                    editor = editors[i];
                }

                editor.GatherStates(animancer.GetLayer(i));
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Sets the target 'layer' and sorts its states and their keys into the active/inactive lists.
        /// </summary>
        private void GatherStates(AnimancerLayer layer)
        {
            Target = layer;

            ActiveStates.Clear();
            InactiveStates.Clear();

            foreach (var state in layer)
            {
                if (HideInactiveStates && state.Weight == 0)
                    continue;

                if (!SeparateActiveFromInactiveStates || state.Weight != 0)
                {
                    ActiveStates.Add(state);
                }
                else
                {
                    InactiveStates.Add(state);
                }
            }

            SortAndGatherKeys(ActiveStates);
            SortAndGatherKeys(InactiveStates);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Sorts any entries that use another state as their key to come right after that state.
        /// See <see cref="AnimancerPlayable.CrossFadeFromStart(AnimancerState, float)"/>.
        /// </summary>
        private static void SortAndGatherKeys(List<AnimancerState> states)
        {
            var count = states.Count;
            if (count == 0)
                return;

            if (SortStatesByName)
            {
                states.Sort((x, y) =>
                {
                    if (x.MainObject == null)
                        return y.MainObject == null ? 0 : 1;
                    else if (y.MainObject == null)
                        return -1;

                    return x.MainObject.name.CompareTo(y.MainObject.name);
                });
            }

            // Sort any states that use another state as their key to be right after the key.
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                var key = state.Key;

                var keyState = key as AnimancerState;
                if (keyState == null)
                    continue;

                var keyStateIndex = states.IndexOf(keyState);
                if (keyStateIndex < 0 || keyStateIndex + 1 == i)
                    continue;

                states.RemoveAt(i);

                if (keyStateIndex < i)
                    keyStateIndex++;

                states.Insert(keyStateIndex, state);

                i--;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary>The number of pixels of indentation required to fit the foldout arrow.</summary>
        const float FoldoutIndent = 12;

        /// <summary>Draws a foldout arrow to expand/collapse the state details.</summary>
        protected override void DoFoldoutGUI(Rect area, out string label)
        {
            label = Target.IsAdditive ? "Additive" : "Override";
            if (Target._Mask != null)
                label = string.Concat(label, " (", Target._Mask.name, ")");

            area.xMin += FoldoutIndent;
            IsExpanded = EditorGUI.Foldout(area, IsExpanded, GUIContent.none, true);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Draws the layer's name and weight.
        /// </summary>
        protected override void DoLabelGUI(Rect area, string label)
        {
            area.xMin += FoldoutIndent;

            AnimancerEditorUtilities.DoWeightLabelGUI(ref area, Target.Weight);

            EditorGUIUtility.labelWidth -= FoldoutIndent;
            EditorGUI.LabelField(area, Target.Name, label);
            EditorGUIUtility.labelWidth += FoldoutIndent;
        }

        /************************************************************************************************************************/

        /// <summary> Draws the details of the target state in the GUI.</summary>
        protected override void DoDetailsGUI(IAnimancerComponent owner)
        {
            if (IsExpanded)
            {
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                GUILayout.Space(FoldoutIndent);
                GUILayout.BeginVertical();

                DoLayerDetailsGUI();
                DoNodeDetailsGUI();

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            DoStatesGUI(owner);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Draws controls for <see cref="AnimancerLayer.IsAdditive"/> and <see cref="AnimancerLayer._Mask"/>.
        /// </summary>
        private void DoLayerDetailsGUI()
        {
            var area = AnimancerEditorUtilities.GetLayoutRect(true);
            area = EditorGUI.IndentedRect(area);

            var labelWidth = EditorGUIUtility.labelWidth;
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var additiveLabel = AnimancerEditorUtilities.GetNarrowText("Is Additive");
            var additiveLabelWidth = GUI.skin.label.CalculateWidth(additiveLabel);
            var additiveWidth = GUI.skin.toggle.CalculateWidth(additiveLabel);

            var maskRect = AnimancerEditorUtilities.StealFromRight(ref area, area.width - additiveWidth);

            // Additive.
            EditorGUIUtility.labelWidth = additiveLabelWidth;
            Target.IsAdditive = EditorGUI.Toggle(area, additiveLabel, Target.IsAdditive);

            // Mask.
            var maskLabel = AnimancerEditorUtilities.TempContent("Mask");
            EditorGUIUtility.labelWidth = GUI.skin.label.CalculateWidth(maskLabel);
            EditorGUI.BeginChangeCheck();
            Target._Mask = (AvatarMask)EditorGUI.ObjectField(maskRect, maskLabel, Target._Mask, typeof(AvatarMask), false);
            if (EditorGUI.EndChangeCheck())
                Target.SetMask(Target._Mask);

            EditorGUI.indentLevel = indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        /************************************************************************************************************************/

        private void DoStatesGUI(IAnimancerComponent owner)
        {
            if (HideInactiveStates)
            {
                DoStatesGUI("Active States", ActiveStates, owner);
            }
            else if (SeparateActiveFromInactiveStates)
            {
                DoStatesGUI("Active States", ActiveStates, owner);
                DoStatesGUI("Inactive States", InactiveStates, owner);
            }
            else
            {
                DoStatesGUI("States", ActiveStates, owner);
            }

            if (Target.PortIndex == 0 &&
                Target.Weight != 0 &&
                !Target.IsAdditive &&
                !Mathf.Approximately(Target.GetTotalWeight(), 1))
            {
                EditorGUILayout.HelpBox(
                    "The total Weight of all states in this layer does not equal 1, which will likely give undesirable results." +
                    " Click here for more information.",
                    MessageType.Warning);

                if (AnimancerEditorUtilities.TryUseClickEventInLastRect())
                    EditorUtility.OpenWithDefaultApp(Strings.DocsURLs.Fading);
            }
        }

        /************************************************************************************************************************/

        /// <summary>Draws all 'states' in the given list.</summary>
        private void DoStatesGUI(string label, List<AnimancerState> states, IAnimancerComponent owner)
        {
            var area = AnimancerEditorUtilities.GetLayoutRect();

            var width = GUI.skin.label.CalculateWidth("Weight");
            GUI.Label(AnimancerEditorUtilities.StealFromRight(ref area, width), "Weight");

            EditorGUI.LabelField(area, label, states.Count.ToString());

            EditorGUI.indentLevel++;
            for (int i = 0; i < states.Count; i++)
            {
                DoStateGUI(states[i], owner);
            }
            EditorGUI.indentLevel--;
        }

        /************************************************************************************************************************/

        /// <summary>Cached Inspectors that have already been created for states.</summary>
        private readonly Dictionary<AnimancerState, IAnimancerNodeDrawer>
            StateInspectors = new Dictionary<AnimancerState, IAnimancerNodeDrawer>();

        /// <summary>Draws the Inspector for the given 'state'.</summary>
        private void DoStateGUI(AnimancerState state, IAnimancerComponent owner)
        {
            IAnimancerNodeDrawer Inspector;
            if (!StateInspectors.TryGetValue(state, out Inspector))
            {
                Inspector = state.GetDrawer();
                StateInspectors.Add(state, Inspector);
            }

            Inspector.DoGUI(owner);
            DoChildStatesGUI(state, owner);
        }

        /************************************************************************************************************************/

        /// <summary>Draws all child states of the 'state'.</summary>
        private void DoChildStatesGUI(AnimancerState state, IAnimancerComponent owner)
        {
            EditorGUI.indentLevel++;

            foreach (var child in state)
            {
                if (child == null)
                    continue;

                DoStateGUI(child, owner);
            }

            EditorGUI.indentLevel--;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Draws the details and controls for the target <see cref="AnimancerNodeDrawer{T}.Target"/> in the Inspector.
        /// </summary>
        public override void DoGUI(IAnimancerComponent owner)
        {
            base.DoGUI(owner);

            var area = GUILayoutUtility.GetLastRect();
            HandleDragAndDropAnimations(area, owner, Target.PortIndex);
        }

        /// <summary>
        /// If <see cref="AnimationClip"/>s or <see cref="IAnimationClipSource"/>s are dropped inside the 'dropArea',
        /// this method creates a new state in the 'target' for each animation.
        /// </summary>
        public static void HandleDragAndDropAnimations(Rect dropArea, IAnimancerComponent target, int layerIndex)
        {
            AnimancerEditorUtilities.HandleDragAndDropAnimations(dropArea, (clip) =>
            {
                target.GetOrCreateState(clip, layerIndex);
            });
        }

        /************************************************************************************************************************/
        #region Context Menu
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Adds functions relevant to the <see cref="AnimancerNodeDrawer{T}.Target"/>.</summary>
        protected override void PopulateContextMenu(GenericMenu menu)
        {
            menu.AddDisabledItem(new GUIContent(Target.ToString()));

            AnimancerEditorUtilities.AddMenuItem(menu, "Stop",
                HasAnyStates((state) => state.IsPlaying || state.Weight != 0),
                () => Target.Stop());

            AnimancerEditorUtilities.AddFadeFunction(menu, "Fade In",
                Target.PortIndex > 0 && Target.Weight != 1, Target,
                (duration) => Target.StartFade(1, duration));
            AnimancerEditorUtilities.AddFadeFunction(menu, "Fade Out",
                Target.PortIndex > 0 && Target.Weight != 0, Target,
                (duration) => Target.StartFade(0, duration));

            menu.AddItem(new GUIContent("Inverse Kinematics/Apply Animator IK"),
                Target.ApplyAnimatorIK,
                () => Target.ApplyAnimatorIK = !Target.ApplyAnimatorIK);
            menu.AddItem(new GUIContent("Inverse Kinematics/Default Apply Animator IK"),
                Target.DefaultApplyAnimatorIK,
                () => Target.DefaultApplyAnimatorIK = !Target.DefaultApplyAnimatorIK);
            menu.AddItem(new GUIContent("Inverse Kinematics/Apply Foot IK"),
                Target.ApplyFootIK,
                () => Target.ApplyFootIK = !Target.ApplyFootIK);
            menu.AddItem(new GUIContent("Inverse Kinematics/Default Apply Foot IK"),
                Target.DefaultApplyFootIK,
                () => Target.DefaultApplyFootIK = !Target.DefaultApplyFootIK);

            menu.AddSeparator("");

            AnimancerEditorUtilities.AddMenuItem(menu, "Destroy States",
                ActiveStates.Count > 0 || InactiveStates.Count > 0,
                () => Target.DestroyStates());

            AnimancerEditorUtilities.AddMenuItem(menu, "Add Layer",
                Target.Root.LayerCount < AnimancerPlayable.maxLayerCount,
                () => Target.Root.LayerCount++);
            AnimancerEditorUtilities.AddMenuItem(menu, "Remove Layer",
                Target.Root.LayerCount > 0,
                () => Target.Root.LayerCount--);

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Keep Weightless Playables Connected"),
                Target.Root.KeepChildrenConnected,
                () => Target.Root.KeepChildrenConnected = !Target.Root.KeepChildrenConnected);

            AddPrefFunctions(menu);

            menu.AddSeparator("");

            AnimancerEditorUtilities.AddDocumentationLink(menu, "Layer Documentation", "/docs/manual/blending/layers");
            AddPlayableGraphVisualizerFunction(menu);

            menu.ShowAsContext();
        }

        /************************************************************************************************************************/

        private bool HasAnyStates(Func<AnimancerState, bool> condition)
        {
            foreach (var state in Target)
            {
                if (condition(state))
                    return true;
            }

            return false;
        }

        /************************************************************************************************************************/

        private void AddPlayableGraphVisualizerFunction(GenericMenu menu)
        {
            var type = Type.GetType("GraphVisualizer.PlayableGraphVisualizerWindow," +
                " Unity.PlayableGraphVisualizer.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

            AnimancerEditorUtilities.AddMenuItem(menu, "Playable Graph Visualizer", type != null, () =>
            {
                var window = EditorWindow.GetWindow(type);

                var field = type.GetField("m_CurrentGraph",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                if (field != null)
                    field.SetValue(window, Target.Root._Graph);
            });
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Prefs
        /************************************************************************************************************************/

        internal const string PrefPrefix = "Display Options/";
        private static readonly BoolPref
            SortStatesByName = new BoolPref(PrefPrefix + "Sort By Name", true),
            HideInactiveStates = new BoolPref(PrefPrefix + "Hide Inactive", false),
            SeparateActiveFromInactiveStates = new BoolPref(PrefPrefix + "Separate Active From Inactive", false);
        internal static readonly BoolPref
            ShowUpdatingNodes = new BoolPref(PrefPrefix + "Show Dirty Nodes", false);

        /************************************************************************************************************************/

        private static void AddPrefFunctions(GenericMenu menu)
        {
            SortStatesByName.AddToggleFunction(menu);
            HideInactiveStates.AddToggleFunction(menu);
            SeparateActiveFromInactiveStates.AddToggleFunction(menu);
            ShowUpdatingNodes.AddToggleFunction(menu);
            AnimancerPlayable.RepaintConstantlyInEditMode.AddToggleFunction(menu);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif
