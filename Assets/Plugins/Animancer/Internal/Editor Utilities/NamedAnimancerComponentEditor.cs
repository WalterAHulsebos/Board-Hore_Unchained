// Animancer // Copyright 2019 Kybernetik //

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only]
    /// A custom Inspector for <see cref="NamedAnimancerComponent"/>s.
    /// </summary>
    [CustomEditor(typeof(NamedAnimancerComponent), true), CanEditMultipleObjects]
    public class NamedAnimancerComponentEditor : AnimancerComponentEditor
    {
        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Draws any custom GUI for the 'property'. The return value indicates whether the GUI should replace the
        /// regular call to <see cref="EditorGUILayout.PropertyField"/> or not.
        /// </summary>
        protected override bool DoOverridePropertyGUI(string name, SerializedProperty property, GUIContent label)
        {
            switch (name)
            {
                case "_PlayAutomatically":
                    if (ShouldShowAnimationFields())
                        DoDefaultAnimationField(property);
                    return true;

                case "_Animations":
                    if (ShouldShowAnimationFields())
                    {
                        //DoDefaultAnimationField(property);
                        DoAnimationsField(property, label);
                    }
                    return true;

                default:
                    return base.DoOverridePropertyGUI(name, property, label);
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The <see cref="NamedAnimancerComponent.PlayAutomatically"/> and
        /// <see cref="NamedAnimancerComponent.Animations"/> fields are only used on startup, so we don't need to show
        /// them in Play Mode after the object is already enabled.
        /// </summary>
        private bool ShouldShowAnimationFields()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return true;

            for (int i = 0; i < Targets.Length; i++)
            {
                if (!Targets[i].enabled)
                    return true;
            }

            return false;
        }

        /************************************************************************************************************************/

        private void DoDefaultAnimationField(SerializedProperty playAutomatically)
        {
            var area = AnimancerEditorUtilities.GetLayoutRect();

            var playAutomaticallyWidth = EditorGUIUtility.labelWidth + GUI.skin.toggle.normal.background.width;
            var playAutomaticallyArea = AnimancerEditorUtilities.StealFromLeft(ref area, playAutomaticallyWidth);
            var label = AnimancerEditorUtilities.TempContent(playAutomatically);
            EditorGUI.PropertyField(playAutomaticallyArea, playAutomatically, label);

            SerializedProperty firstElement;
            AnimationClip clip;

            var animations = serializedObject.FindProperty("_Animations");
            if (animations.arraySize > 0)
            {
                firstElement = animations.GetArrayElementAtIndex(0);
                clip = (AnimationClip)firstElement.objectReferenceValue;
                EditorGUI.BeginProperty(area, null, firstElement);
            }
            else
            {
                firstElement = null;
                clip = null;
                EditorGUI.BeginProperty(area, null, animations);
            }

            EditorGUI.BeginChangeCheck();

            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            clip = (AnimationClip)EditorGUI.ObjectField(area, GUIContent.none, clip, typeof(AnimationClip), true);

            EditorGUI.indentLevel = indentLevel;

            if (EditorGUI.EndChangeCheck())
            {
                if (clip != null)
                {
                    if (firstElement == null)
                    {
                        animations.arraySize = 1;
                        firstElement = animations.GetArrayElementAtIndex(0);
                    }

                    firstElement.objectReferenceValue = clip;
                }
                else
                {
                    if (firstElement == null || animations.arraySize == 1)
                        animations.arraySize = 0;
                    else
                        firstElement.objectReferenceValue = clip;
                }
            }

            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private ReorderableList _Animations;
        private static int _RemoveAnimationIndex;

        private void DoAnimationsField(SerializedProperty property, GUIContent label)
        {
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing - 1);

            if (_Animations == null)
            {
                _Animations = new ReorderableList(property.serializedObject, property.Copy())
                {
                    drawHeaderCallback = DrawAnimationsHeader,
                    drawElementCallback = DrawAnimationElement,
                    elementHeight = EditorGUIUtility.singleLineHeight,
                    onRemoveCallback = RemoveSelectedElement,
                };
            }

            _RemoveAnimationIndex = -1;

            GUILayout.BeginVertical();
            _Animations.DoLayoutList();
            GUILayout.EndVertical();

            if (_RemoveAnimationIndex >= 0)
            {
                property.DeleteArrayElementAtIndex(_RemoveAnimationIndex);
            }

            AnimancerEditorUtilities.HandleDragAndDropAnimations(GUILayoutUtility.GetLastRect(), (clip) =>
            {
                var index = property.arraySize;
                property.arraySize = index + 1;
                var element = property.GetArrayElementAtIndex(index);
                element.objectReferenceValue = clip;
            });
        }

        /************************************************************************************************************************/

        private SerializedProperty _AnimationsArraySize;

        private void DrawAnimationsHeader(Rect area)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth -= 6;

            area.width += 5;

            var property = _Animations.serializedProperty;
            var label = AnimancerEditorUtilities.TempContent(property);
            EditorGUI.BeginProperty(area, label, property);

            if (_AnimationsArraySize == null)
            {
                _AnimationsArraySize = property.Copy();
                _AnimationsArraySize.Next(true);
                _AnimationsArraySize.Next(true);
            }

            EditorGUI.PropertyField(area, _AnimationsArraySize, label);

            EditorGUI.EndProperty();

            EditorGUIUtility.labelWidth = labelWidth;
        }

        /************************************************************************************************************************/

        private static readonly HashSet<Object>
            PreviousAnimations = new HashSet<Object>();

        private void DrawAnimationElement(Rect area, int index, bool isActive, bool isFocused)
        {
            if (index == 0)
                PreviousAnimations.Clear();

            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth -= 20;

            var element = _Animations.serializedProperty.GetArrayElementAtIndex(index);

            var color = GUI.color;
            var animation = element.objectReferenceValue;
            if (animation == null || PreviousAnimations.Contains(animation))
                GUI.color = AnimancerEditorUtilities.WarningFieldColor;
            else
                PreviousAnimations.Add(animation);

            EditorGUI.BeginChangeCheck();
            EditorGUI.ObjectField(area, element, GUIContent.none);

            if (EditorGUI.EndChangeCheck() && element.objectReferenceValue == null)
                _RemoveAnimationIndex = index;

            GUI.color = color;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        /************************************************************************************************************************/

        private static void RemoveSelectedElement(ReorderableList list)
        {
            var property = list.serializedProperty;
            var element = property.GetArrayElementAtIndex(list.index);

            // Deleting a non-null element sets it to null, so we make sure it's null to actually remove it.
            if (element.objectReferenceValue != null)
                element.objectReferenceValue = null;

            property.DeleteArrayElementAtIndex(list.index);

            if (list.index >= property.arraySize - 1)
                list.index = property.arraySize - 1;
        }

        /************************************************************************************************************************/
        #region Menu Items
        /************************************************************************************************************************/

        /// <summary>The start of all <see cref="NamedAnimancerComponent"/> context menu items.</summary>
        public new const string MenuItemPrefix = "CONTEXT/NamedAnimancerComponent/";

        /************************************************************************************************************************/

        [MenuItem(MenuItemPrefix + "Play Animations in Sequence", validate = true)]
        [MenuItem(MenuItemPrefix + "Cross Fade Animations in Sequence", validate = true)]
        private static bool IsPlaying()
        {
            return EditorApplication.isPlaying;
        }

        /************************************************************************************************************************/

        /// <summary>Starts <see cref="NamedAnimancerComponent.PlayAnimationsInSequence"/> as a coroutine.</summary>
        [MenuItem(MenuItemPrefix + "Play Animations in Sequence", priority = MenuItemPriority)]
        private static void PlayAnimationsInSequence(MenuCommand command)
        {
            var animancer = command.context as NamedAnimancerComponent;
            animancer.StartCoroutine(animancer.PlayAnimationsInSequence());
        }

        /// <summary>Starts <see cref="NamedAnimancerComponent.CrossFadeAnimationsInSequence"/> as a coroutine.</summary>
        [MenuItem(MenuItemPrefix + "Cross Fade Animations in Sequence", priority = MenuItemPriority)]
        private static void CrossFadeAnimationsInSequence(MenuCommand command)
        {
            var animancer = command.context as NamedAnimancerComponent;
            animancer.StartCoroutine(animancer.CrossFadeAnimationsInSequence());
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif
