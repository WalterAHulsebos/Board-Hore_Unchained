// Animancer // Copyright 2019 Kybernetik //

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] Draws the Inspector GUI for an <see cref="AnimancerNode"/>.</summary>
    public interface IAnimancerNodeDrawer
    {
        /// <summary>Draws the details and controls for the target node in the Inspector.</summary>
        void DoGUI(IAnimancerComponent owner);
    }

    /************************************************************************************************************************/

    /// <summary>[Editor-Only] Draws the Inspector GUI for an <see cref="AnimancerNode"/>.</summary>
    public abstract class AnimancerNodeDrawer<T> : IAnimancerNodeDrawer where T : AnimancerNode
    {
        /************************************************************************************************************************/

        /// <summary>The node being managed.</summary>
        public T Target { get; protected set; }

        /// <summary>If true, the details of the <see cref="Target"/> will be expanded in the Inspector.</summary>
        public bool IsExpanded
        {
            get { return Target._IsInspectorExpanded; }
            protected set { Target._IsInspectorExpanded = value; }
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="GUIStyle"/> used for the area encompassing this drawer.</summary>
        protected abstract GUIStyle RegionStyle { get; }

        /************************************************************************************************************************/

        /// <summary>Draws the details and controls for the target <see cref="Target"/> in the Inspector.</summary>
        public virtual void DoGUI(IAnimancerComponent owner)
        {
            AnimancerEditorUtilities.BeginVerticalBox(RegionStyle);
            {
                DoHeaderGUI();
                DoDetailsGUI(owner);
            }
            AnimancerEditorUtilities.EndVerticalBox(RegionStyle);

            CheckContextMenu(GUILayoutUtility.GetLastRect());
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Draws the name and other details of the <see cref="Target"/> in the GUI.
        /// </summary>
        protected virtual void DoHeaderGUI()
        {
            var area = AnimancerEditorUtilities.GetLayoutRect(true);

            string label;
            DoFoldoutGUI(area, out label);
            DoLabelGUI(area, label);
        }

        /// <summary>Draws a foldout arrow to expand/collapse the node details.</summary>
        protected abstract void DoFoldoutGUI(Rect area, out string label);

        /// <summary>
        /// Draws the node's main label: an <see cref="Object"/> field if it has a
        /// <see cref="AnimancerState.MainObject"/>, otherwise just a simple text label.
        /// </summary>
        protected abstract void DoLabelGUI(Rect area, string label);

        /// <summary> Draws the details of the <see cref="Target"/> in the GUI.</summary>
        protected abstract void DoDetailsGUI(IAnimancerComponent owner);

        /************************************************************************************************************************/

        /// <summary>
        /// Draws controls for <see cref="AnimancerNode.IsPlaying"/>, <see cref="AnimancerNode.Speed"/>, and
        /// <see cref="AnimancerNode.Weight"/>.
        /// </summary>
        protected void DoNodeDetailsGUI()
        {
            var labelWidth = EditorGUIUtility.labelWidth;

            var area = AnimancerEditorUtilities.GetLayoutRect(true);

            var right = area.xMax;

            // Is Playing.
            var label = AnimancerEditorUtilities.BeginTightLabel("Is Playing");
            area.width = EditorGUIUtility.labelWidth + 16;
            Target.IsPlaying = EditorGUI.Toggle(area, label, Target.IsPlaying);
            AnimancerEditorUtilities.EndTightLabel();

            area.x += area.width;
            area.xMax = right;

            float speedWidth, weightWidth;
            Rect speedRect, weightRect;
            AnimancerEditorUtilities.SplitHorizontally(area, "Speed", "Weight", out speedWidth, out weightWidth, out speedRect, out weightRect);

            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Speed.
            EditorGUIUtility.labelWidth = speedWidth;
            EditorGUI.BeginChangeCheck();
            var speed = EditorGUI.FloatField(speedRect, "Speed", Target.Speed);
            if (EditorGUI.EndChangeCheck())
                Target.Speed = speed;
            if (AnimancerEditorUtilities.TryUseClickEvent(speedRect, 2))
                Target.Speed = Target.Speed != 1 ? 1 : 0;

            // Weight.
            EditorGUIUtility.labelWidth = weightWidth;
            EditorGUI.BeginChangeCheck();
            var weight = EditorGUI.FloatField(weightRect, "Weight", Target.Weight);
            if (EditorGUI.EndChangeCheck())
                Target.Weight = weight;
            if (AnimancerEditorUtilities.TryUseClickEvent(weightRect, 2))
                Target.Weight = Target.Weight != 1 ? 1 : 0;

            EditorGUI.indentLevel = indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;

            DoFadeDetailsGUI();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Draws controls for <see cref="AnimancerNode.FadeSpeed"/> and <see cref="AnimancerNode.TargetWeight"/>.
        /// </summary>
        private void DoFadeDetailsGUI()
        {
            var area = AnimancerEditorUtilities.GetLayoutRect(true);
            area = EditorGUI.IndentedRect(area);

            var speedLabel = AnimancerEditorUtilities.GetNarrowText("Fade Speed");
            var targetLabel = AnimancerEditorUtilities.GetNarrowText("Target Weight");

            float speedWidth, weightWidth;
            Rect speedRect, weightRect;
            AnimancerEditorUtilities.SplitHorizontally(area, speedLabel, targetLabel,
                out speedWidth, out weightWidth, out speedRect, out weightRect);

            var labelWidth = EditorGUIUtility.labelWidth;
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.BeginChangeCheck();

            // Fade Speed.
            EditorGUIUtility.labelWidth = speedWidth;
            Target.FadeSpeed = EditorGUI.DelayedFloatField(speedRect, speedLabel, Target.FadeSpeed);
            if (AnimancerEditorUtilities.TryUseClickEvent(speedRect, 2))
            {
                Target.FadeSpeed = Target.FadeSpeed != 0 ?
                    0 :
                    Mathf.Abs(Target.Weight - Target.TargetWeight) / AnimancerPlayable.DefaultFadeDuration;
            }

            // Target Weight.
            EditorGUIUtility.labelWidth = weightWidth;
            Target.TargetWeight = EditorGUI.FloatField(weightRect, targetLabel, Target.TargetWeight);
            if (AnimancerEditorUtilities.TryUseClickEvent(weightRect, 2))
            {
                if (Target.TargetWeight != Target.Weight)
                    Target.TargetWeight = Target.Weight;
                else if (Target.TargetWeight != 1)
                    Target.TargetWeight = 1;
                else
                    Target.TargetWeight = 0;
            }

            if (EditorGUI.EndChangeCheck() && Target.FadeSpeed != 0)
                Target.StartFade(Target.TargetWeight, 1 / Target.FadeSpeed);

            EditorGUI.indentLevel = indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        /************************************************************************************************************************/
        #region Context Menu
        /************************************************************************************************************************/

        /// <summary>
        /// The menu label prefix used for details about the <see cref="Target"/>.
        /// </summary>
        protected const string DetailsPrefix = "Details/";

        /// <summary>
        /// Checks if the current event is a context menu click within the 'clickArea' and opens a context menu with various
        /// functions for the <see cref="Target"/>.
        /// </summary>
        protected void CheckContextMenu(Rect clickArea)
        {
            if (!AnimancerEditorUtilities.TryUseClickEvent(clickArea, 1))
                return;

            var menu = new GenericMenu();
            PopulateContextMenu(menu);
            menu.ShowAsContext();
        }

        /// <summary>[Editor-Only] Adds functions relevant to the <see cref="Target"/>.</summary>
        protected abstract void PopulateContextMenu(GenericMenu menu);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif

