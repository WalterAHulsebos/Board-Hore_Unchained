// Animancer // Copyright 2019 Kybernetik //

#if UNITY_EDITOR

using System.Text;
using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only]
    /// A custom Inspector for <see cref="AnimancerComponent"/>s.
    /// </summary>
    [CustomEditor(typeof(AnimancerComponent), true), CanEditMultipleObjects]
    public class AnimancerComponentEditor : AnimancerPlayableEditor
    {
        /************************************************************************************************************************/

        /// <summary>The priority of all context menu items added by this class.</summary>
        public const int MenuItemPriority = 2000;

        /// <summary>The start of all <see cref="AnimancerComponent"/> context menu items.</summary>
        public const string MenuItemPrefix = "CONTEXT/AnimancerComponent/";

        /************************************************************************************************************************/

        /// <summary>Returns <see cref="AnimancerPlayable.IsGraphPlaying"/>.</summary>
        [MenuItem(MenuItemPrefix + "Pause Graph", validate = true)]
        private static bool ValidatePauseGraph(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            return
                animancer.IsPlayableInitialised &&
                animancer.Playable.IsGraphPlaying;
        }

        /// <summary>Calls <see cref="AnimancerPlayable.PauseGraph()"/>.</summary>
        [MenuItem(MenuItemPrefix + "Pause Graph", priority = MenuItemPriority)]
        private static void PauseGraph(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            animancer.Playable.PauseGraph();
        }

        /************************************************************************************************************************/

        /// <summary>Returns !<see cref="AnimancerPlayable.IsGraphPlaying"/>.</summary>
        [MenuItem(MenuItemPrefix + "Unpause Graph", validate = true)]
        private static bool ValidatePlayGraph(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            return
                animancer.IsPlayableInitialised &&
                !animancer.Playable.IsGraphPlaying;
        }

        /// <summary>Calls <see cref="AnimancerPlayable.UnpauseGraph()"/>.</summary>
        [MenuItem(MenuItemPrefix + "Unpause Graph", priority = MenuItemPriority)]
        private static void PlayGraph(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            animancer.Playable.UnpauseGraph();
            animancer.Evaluate();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns <see cref="AnimancerComponent.IsPlayableInitialised"/>.
        /// </summary>
        [MenuItem(MenuItemPrefix + "Stop All Animations", validate = true)]
        private static bool ValidateStop(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            return animancer.IsPlayableInitialised;
        }

        /// <summary>Calls <see cref="AnimancerComponent.Stop()"/>.</summary>
        [MenuItem(MenuItemPrefix + "Stop All Animations", priority = MenuItemPriority)]
        private static void Stop(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            animancer.Stop();
            animancer.Evaluate();
        }

        /************************************************************************************************************************/

        /// <summary>Logs a description of all states currently in the <see cref="AnimancerComponent.Playable"/>.</summary>
        [MenuItem(MenuItemPrefix + "Log Description of States", priority = MenuItemPriority)]
        private static void LogDescriptionOfStates(MenuCommand command)
        {
            var animancer = command.context as AnimancerComponent;
            var message = new StringBuilder();
            message.Append(animancer.ToString());
            if (animancer.IsPlayableInitialised)
            {
                message.Append(":\n");
                animancer.Playable.AppendDescription(message);
            }
            else
            {
                message.Append(": Playable is not initialised.");
            }

            AnimancerEditorUtilities.AppendNonCriticalIssues(message);

            Debug.Log(message, animancer);
        }

        /************************************************************************************************************************/
    }
}

#endif

