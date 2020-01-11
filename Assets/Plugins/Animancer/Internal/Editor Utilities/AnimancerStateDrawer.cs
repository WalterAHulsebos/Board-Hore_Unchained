// Animancer // Copyright 2019 Kybernetik //

#if UNITY_EDITOR

#pragma warning disable IDE0041 // Use 'is null' check.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] Draws the Inspector GUI for an <see cref="AnimancerState"/>.</summary>
    public class AnimancerStateDrawer<T> : AnimancerNodeDrawer<T> where T : AnimancerState
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Constructs a new <see cref="AnimancerStateDrawer{T}"/> to manage the Inspector GUI for the 'target'.
        /// </summary>
        public AnimancerStateDrawer(T target)
        {
            Target = target;
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="GUIStyle"/> used for the area encompassing this drawer. Null.</summary>
        protected override GUIStyle RegionStyle { get { return null; } }

        /************************************************************************************************************************/

        /// <summary>Draws a foldout arrow to expand/collapse the state details.</summary>
        protected override void DoFoldoutGUI(Rect area, out string label)
        {
            var key = Target.Key;

            var isAssetUsedAsKey = key == null || ReferenceEquals(key, Target.MainObject);

            float foldoutWidth;
            if (isAssetUsedAsKey)
            {
                foldoutWidth = EditorGUI.indentLevel * AnimancerEditorUtilities.IndentSize;
                label = "";
            }
            else
            {
                foldoutWidth = EditorGUIUtility.labelWidth;

                if (key is string)
                    label = "\"" + key + "\"";
                else
                    label = key.ToString();
            }

            area.xMin -= 2;
            area.width = foldoutWidth;

            IsExpanded = EditorGUI.Foldout(area, IsExpanded, GUIContent.none, !isAssetUsedAsKey);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Draws the state's main label: an <see cref="Object"/> field if it has a
        /// <see cref="AnimancerState.MainObject"/>, otherwise just a simple text label.
        /// <para></para>
        /// Also shows a bar to indicate its progress.
        /// </summary>
        protected override void DoLabelGUI(Rect area, string label)
        {
            HandleLabelClick(area);

            AnimancerEditorUtilities.DoWeightLabelGUI(ref area, Target.Weight);

            if (!ReferenceEquals(Target.MainObject, null))
            {
                EditorGUI.BeginChangeCheck();

                var newObject = EditorGUI.ObjectField(area, label, Target.MainObject, typeof(Object), false);

                if (EditorGUI.EndChangeCheck())
                    Target.MainObject = newObject;
            }
            else
            {
                EditorGUI.LabelField(area, label, Target.ToString());
            }

            // Highlight a section of the label based on the time like a loading bar.
            if (Target.HasLength && (Target.IsPlaying || Target.Time != 0))
            {
                var color = GUI.color;

                // Green = Playing, Yelow = Paused.
                GUI.color = Target.IsPlaying ? new Color(0.25f, 1, 0.25f, 0.25f) : new Color(1, 1, 0.25f, 0.25f);

                area.xMin += AnimancerEditorUtilities.IndentSize;
                area.width -= 18;

                float length;
                var wrappedTime = GetWrappedTime(out length);
                if (length > 0)
                    area.width *= Mathf.Clamp01(wrappedTime / length);

                GUI.Box(area, GUIContent.none);

                GUI.color = color;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Handles Ctrl + Click on the label to CrossFade the animation.
        /// <para></para>
        /// If Shift is also held, the effect will be queued until after the previous animation finishes.
        /// </summary>
        private void HandleLabelClick(Rect area)
        {
            var currentEvent = Event.current;
            if (currentEvent.type != EventType.MouseUp ||
                !currentEvent.control ||
                !area.Contains(currentEvent.mousePosition))
                return;

            currentEvent.Use();

            if (currentEvent.shift)
            {
                AnimationQueue.CrossFadeQueued(Target);
                return;
            }

            AnimationQueue.ClearQueue(Target.Layer);

            if (Target.Root.IsGraphPlaying)
            {
                var fadeDuration = Target.CalculateEditorFadeDuration(AnimancerPlayable.DefaultFadeDuration);
                Target.Root.CrossFade(Target, fadeDuration);
            }
            else
            {
                Target.Root.Play(Target);
                Target.Root.Evaluate();
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Manages the playing of animations in sequence.
        /// </summary>
        private sealed class AnimationQueue
        {
            /************************************************************************************************************************/

            private static readonly Dictionary<AnimancerLayer, AnimationQueue>
                PlayableToQueue = new Dictionary<AnimancerLayer, AnimationQueue>();

            private readonly List<AnimancerState>
                Queue = new List<AnimancerState>();

            /************************************************************************************************************************/

            private AnimationQueue() { }

            public static void CrossFadeQueued(AnimancerState state)
            {
                CleanUp();

                var layer = state.Layer;

                // If the layer has no current state, just play the animation immediately.
                if (!layer.CurrentState.IsValid() || layer.CurrentState.Weight == 0)
                {
                    var fadeDuration = state.CalculateEditorFadeDuration(AnimancerPlayable.DefaultFadeDuration);
                    layer.CrossFade(state, fadeDuration);
                    return;
                }

                AnimationQueue queue;
                if (!PlayableToQueue.TryGetValue(layer, out queue))
                {
                    queue = new AnimationQueue();
                    PlayableToQueue.Add(layer, queue);
                }

                queue.Queue.Add(state);

                layer.CurrentState.OnEnd -= queue.PlayNext;
                layer.CurrentState.OnEnd += queue.PlayNext;
            }

            /************************************************************************************************************************/

            public static void ClearQueue(AnimancerLayer layer)
            {
                PlayableToQueue.Remove(layer);
            }

            /************************************************************************************************************************/

            private static readonly List<AnimancerLayer>
                OldQueues = new List<AnimancerLayer>();

            /// <summary>
            /// Clear out any playables that have been destroyed.
            /// </summary>
            private static void CleanUp()
            {
                OldQueues.Clear();

                foreach (var layer in PlayableToQueue.Keys)
                {
                    if (!layer.IsValid)
                        OldQueues.Add(layer);
                }

                for (int i = 0; i < OldQueues.Count; i++)
                {
                    PlayableToQueue.Remove(OldQueues[i]);
                }
            }

            /************************************************************************************************************************/

            private void PlayNext()
            {
                if (Queue.Count == 0)
                    return;

                var state = Queue[0];
                Queue.RemoveAt(0);
                if (!state.IsValid())
                {
                    PlayNext();
                    return;
                }

                var fadeDuration = state.CalculateEditorFadeDuration(AnimancerPlayable.DefaultFadeDuration);
                state.Layer.CrossFade(state, fadeDuration);
                state.OnEnd = PlayNext;
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Gets the current <see cref="AnimancerState.Time"/>.
        /// If the state is looping, the value is modulo by the <see cref="AnimancerState.Length"/>.
        /// </summary>
        private float GetWrappedTime(out float length)
        {
            var time = DisplayTime;
            length = DisplayLength;

            var wrappedTime = time;

            if (Target.IsLooping)
            {
                wrappedTime %= length;
                if (wrappedTime < 0)
                    wrappedTime += length;
                else if (wrappedTime == 0 && time != 0)
                    wrappedTime = length;
            }

            return wrappedTime;
        }

        /// <summary>Returns the <see cref="AnimancerState.Time"/>.</summary>
        protected virtual float DisplayTime { get { return Target.Time; } }

        /// <summary>Returns the <see cref="AnimancerState.Length"/>.</summary>
        protected virtual float DisplayLength { get { return Target.Length; } }

        /************************************************************************************************************************/

        /// <summary> Draws the details of the target state in the GUI.</summary>
        protected override void DoDetailsGUI(IAnimancerComponent owner)
        {
            if (!IsExpanded)
                return;

            EditorGUI.indentLevel++;
            DoTimeSliderGUI();
            DoNodeDetailsGUI();
            DoOnEndGUI();
            EditorGUI.indentLevel--;
        }

        /************************************************************************************************************************/

        /// <summary>Draws a slider for controlling the current <see cref="AnimancerState.Time"/>.</summary>
        private void DoTimeSliderGUI()
        {
            if (!Target.HasLength)
                return;

            float length;
            var time = GetWrappedTime(out length);

            if (length == 0)
                return;

            var area = AnimancerEditorUtilities.GetLayoutRect(true);

            var normalized = DoNormalizedTimeToggle(ref area);

            string label;
            float max;
            if (normalized)
            {
                label = "Normalized Time";
                time /= length;
                max = 1;
            }
            else
            {
                label = "Time";
                max = length;
            }

            DoLoopCounterGUI(ref area, length);

            EditorGUI.BeginChangeCheck();
            label = AnimancerEditorUtilities.BeginTightLabel(label);
            time = EditorGUI.Slider(area, label, time, 0, max);
            AnimancerEditorUtilities.EndTightLabel();
            if (AnimancerEditorUtilities.TryUseClickEvent(area, 2))
                time = 0;
            if (EditorGUI.EndChangeCheck())
            {
                SetTime(normalized, time);
                Target.Root.Evaluate();
            }
        }

        /// <summary>
        /// Sets the <see cref="AnimancerState.Time"/> or <see cref="AnimancerState.NormalizedTime"/>.
        /// </summary>
        protected virtual void SetTime(bool normalized, float time)
        {
            if (normalized)
                Target.NormalizedTime = time;
            else
                Target.Time = time;
        }

        /************************************************************************************************************************/

        private static readonly BoolPref UseNormalizedTimeSliders = new BoolPref("UseNormalizedTimeSliders", false);

        private static float _UseNormalizedTimeSlidersWidth;

        private bool DoNormalizedTimeToggle(ref Rect area)
        {
            var content = AnimancerEditorUtilities.TempContent("N");
            var style = AnimancerEditorUtilities.GUIStyles.MiniButton;

            if (_UseNormalizedTimeSlidersWidth == 0)
                _UseNormalizedTimeSlidersWidth = style.CalculateWidth(content);

            var toggleArea = AnimancerEditorUtilities.StealFromRight(ref area, _UseNormalizedTimeSlidersWidth);

            UseNormalizedTimeSliders.Value = GUI.Toggle(toggleArea, UseNormalizedTimeSliders, content, style);
            return UseNormalizedTimeSliders;
        }

        /************************************************************************************************************************/

        private void DoLoopCounterGUI(ref Rect area, float length)
        {
            var loops = Mathf.Abs((int)(DisplayTime / length));
            var label = "x" + loops;
            var style = GUI.skin.label;

            var width = style.CalculateWidth(label);

            var labelArea = AnimancerEditorUtilities.StealFromRight(ref area, width);

            GUI.Label(labelArea, label);
        }

        /************************************************************************************************************************/

        private void DoOnEndGUI()
        {
            if (Target.OnEnd == null)
                return;

            var area = AnimancerEditorUtilities.GetLayoutRect(true);

            EditorGUI.LabelField(area, "OnEnd: " + Target.OnEnd.Method);
        }

        /************************************************************************************************************************/
        #region Context Menu
        /************************************************************************************************************************/

        /// <summary>
        /// Checks if the current event is a context menu click within the 'clickArea' and opens a context menu with various
        /// functions for the <see cref="AnimancerNodeDrawer{T}.Target"/>.
        /// </summary>
        protected override void PopulateContextMenu(GenericMenu menu)
        {
            menu.AddDisabledItem(new GUIContent(DetailsPrefix + "State: " + Target.ToString()));

            var key = Target.Key;

            if (key != null)
                menu.AddDisabledItem(new GUIContent(DetailsPrefix + "Key: " + key));

            AnimancerEditorUtilities.AddMenuItem(menu, "Play",
                !Target.IsPlaying || Target.Weight != 1,
                () => Target.Root.Play(Target));

            AnimancerEditorUtilities.AddFadeFunction(menu, "Cross Fade (Ctrl + Click)",
                Target.Weight != 1,
                Target, (duration) => Target.Root.CrossFade(Target, duration));

            AnimancerEditorUtilities.AddFadeFunction(menu, "Cross Fade Queued (Ctrl + Shift + Click)",
                Target.Weight != 1,
                Target, (duration) => AnimationQueue.CrossFadeQueued(Target));

            AddContextMenuFunctions(menu);

            menu.AddItem(new GUIContent(DetailsPrefix + "Log Details"), false,
                () => Debug.Log("AnimancerState: " + Target.GetDescription(true)));

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Destroy State"), false, () => Target.Dispose());

            menu.AddSeparator("");
            AnimancerEditorUtilities.AddDocumentationLink(menu, "State Documentation", "/docs/manual/states");
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Adds the details of this state to the menu.
        /// By default, that means a single item showing the path of the <see cref="AnimancerState.MainObject"/>.
        /// </summary>
        protected virtual void AddContextMenuFunctions(GenericMenu menu)
        {
            var length = DisplayLength;
            if (!float.IsNaN(length))
                menu.AddDisabledItem(new GUIContent(DetailsPrefix + "Length: " + length));

            menu.AddDisabledItem(new GUIContent(DetailsPrefix + "Playable Path: " + Target.GetPath()));

            var mainAsset = Target.MainObject;
            if (mainAsset != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(mainAsset);
                if (assetPath != null)
                    menu.AddDisabledItem(new GUIContent(DetailsPrefix + "Asset Path: " + assetPath.Replace("/", "->")));
            }

            if (Target.OnEnd != null)
            {
                const string OnEndPrefix = "On End/";

                var label = OnEndPrefix +
                    (Target.OnEnd.Target != null ? ("Target: " + Target.OnEnd.Target) : "Target: null");

                var targetObject = Target.OnEnd.Target as Object;
                AnimancerEditorUtilities.AddMenuItem(menu, label,
                    targetObject != null,
                    () => Selection.activeObject = targetObject);

                menu.AddDisabledItem(new GUIContent(OnEndPrefix + "Declaring Type: " + Target.OnEnd.Method.DeclaringType.FullName));
                menu.AddDisabledItem(new GUIContent(OnEndPrefix + "Method: " + Target.OnEnd.Method));

                menu.AddItem(new GUIContent(OnEndPrefix + "Clear"), false, () => Target.OnEnd = null);
                menu.AddItem(new GUIContent(OnEndPrefix + "Invoke"), false, () => Target.OnEnd());
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif

