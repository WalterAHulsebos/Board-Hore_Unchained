// Animancer // Copyright 2019 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] A welcome screen for Animancer.</summary>
    /// <remarks>Automatic selection is handled by <see cref="ShowReadMeOnStartup"/>.</remarks>
    //[CreateAssetMenu(menuName = "Animancer/Read Me", order = AnimancerComponent.AssetMenuOrder + 4)]
    internal sealed class ReadMe : ScriptableObject
    {
        /************************************************************************************************************************/

        /// <summary>The icon to display at the top of the Inspector.</summary>
        public Texture2D icon;

        /// <summary>The directory containing the example scenes.</summary>
        public DefaultAsset examplesFolder;

        /// <summary>If true, <see cref="ShowReadMeOnStartup"/> won't automatically select this asset.</summary>
        public bool dontShowOnStartup;

        /************************************************************************************************************************/

        [CustomEditor(typeof(ReadMe))]
        private sealed class Editor : UnityEditor.Editor
        {
            /************************************************************************************************************************/

            /// <summary>
            /// The release ID of this Animancer version.
            /// </summary>
            /// <remarks>
            /// <list type="bullet">
            ///   <item>[1] = v1.0: 2018-05-02.</item>
            ///   <item>[2] = v1.1: 2018-05-29.</item>
            ///   <item>[3] = v1.2: 2018-08-14.</item>
            ///   <item>[4] = v1.3: 2018-09-12.</item>
            ///   <item>[5] = v2.0: 2018-10-08.</item>
            ///   <item>[6] = v3.0: 2019-05-27.</item>
            ///   <item>[7] = v3.1: 2019-08-12.</item>
            /// </list>
            /// </remarks>
            private const int ReleaseNumber = 7;

            /// <summary>The display name of this Animancer version.</summary>
            private const string VersionName = "v3.1";

            /// <summary>The end of the URL for the change log of this Animancer version.</summary>
            private const string ChangeLogSuffix = "v3-1";

            /// <summary>The key used to save the release number.</summary>
            private const string ReleaseNumberPrefKey = "Animancer.ReleaseNumber";

            /************************************************************************************************************************/

            [NonSerialized] private int _PreviousVersion;

            /************************************************************************************************************************/

            private void OnEnable()
            {
                _PreviousVersion = PlayerPrefs.GetInt(ReleaseNumberPrefKey, -1);
                if (_PreviousVersion < 0)
                    _PreviousVersion = EditorPrefs.GetInt(ReleaseNumberPrefKey, -1);// Animancer v2.0 used EditorPrefs.
            }

            /************************************************************************************************************************/

            protected override void OnHeaderGUI()
            {
                var readme = (ReadMe)target;

                GUILayout.BeginHorizontal("In BigTitle");
                {
                    const string Title = "Animancer Pro\n" + VersionName;
                    var title = AnimancerEditorUtilities.TempContent(Title, null, false);
                    var iconWidth = GUIStyles.Title.CalcHeight(title, EditorGUIUtility.currentViewWidth);
                    GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
                    GUILayout.Label(title, GUIStyles.Title);
                }
                GUILayout.EndHorizontal();
            }

            /************************************************************************************************************************/

            public override void OnInspectorGUI()
            {
                var target = (ReadMe)this.target;

                DoWarnings();

                DoHeadingLink("Documentation", Strings.DocumentationURL);

                DoSpace();

                DoHeadingLink("Examples", Strings.DocumentationURL + "/docs/examples");
                EditorGUILayout.ObjectField(target.examplesFolder, typeof(DefaultAsset), false);

                DoSpace();

                DoHeadingLink("Change Log", Strings.DocsURLs.ChangeLogPrefix + ChangeLogSuffix);

                DoSpace();

                DoHeadingLink("Forum", "https://forum.unity.com/threads/566452");

                DoSpace();

                DoHeadingLink("Support", "mailto:" + Strings.DeveloperEmail + "?subject=Animancer", Strings.DeveloperEmail);

                DoSpace();

                DoHeadingLink("Feedback Survey", "https://forms.gle/43ZyiaWhGWmiTyL98");
                GUILayout.Label("Once you have tried Animancer, please give some feedback via this anonymous survey." +
                    " It should only take a few minutes.", EditorStyles.wordWrappedLabel);

                DoSpace();

                EditorGUI.BeginChangeCheck();
                target.dontShowOnStartup = GUILayout.Toggle(target.dontShowOnStartup, "Don't show this Read Me on startup");
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                    if (target.dontShowOnStartup)
                        PlayerPrefs.SetInt(ReleaseNumberPrefKey, ReleaseNumber);
                }
            }

            /************************************************************************************************************************/

            private void DoSpace()
            {
                GUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);
            }

            /************************************************************************************************************************/

            private void DoWarnings()
            {
                // Upgraded from any older version.
                if (_PreviousVersion >= 0 && _PreviousVersion < ReleaseNumber)
                {
                    DoSpace();

                    var directory = AssetDatabase.GetAssetPath(target);
                    directory = Path.GetDirectoryName(directory);
                    EditorGUILayout.HelpBox(
                        "You must fully delete any old version of Animancer before importing a new version." +
                        " You can ignore this message if you have already done so." +
                        " Otherwise click here to delete '" + directory + "' then import Animancer again.",
                        MessageType.Warning);
                    CheckDeleteAnimancer(directory);

                    // Upgraded from before v2.0.
                    if (_PreviousVersion < 4)
                    {
                        DoSpace();

                        EditorGUILayout.HelpBox("It seems you have just upgraded from an earlier version of Animancer" +
                            " (before v2.0) so you will need to restart Unity before you can use it.",
                            MessageType.Warning);
                    }

                    DoSpace();
                }
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Asks if the user wants to delete the root Animancer folder and does so if they confirm.
            /// </summary>
            private void CheckDeleteAnimancer(string directory)
            {
                if (!AnimancerEditorUtilities.TryUseClickEventInLastRect())
                    return;

                if (!AssetDatabase.IsValidFolder(directory))
                {
                    Debug.Log(directory + " doesn't exist." +
                        " You must have moved Animancer somewhere else so you will need to delete it manually.");
                    return;
                }

                if (!EditorUtility.DisplayDialog("Delete Animancer?",
                    "Would you like to delete " + directory + "?" +
                    "\n\nYou will then need to reimport Animancer manually.",
                    "Delete", "Cancel"))
                    return;

                AssetDatabase.DeleteAsset(directory);
            }

            /************************************************************************************************************************/

            private bool DoHeadingLink(string heading, string url, string displayURL = null)
            {
                if (DoLinkLabel(heading, GUIStyles.Heading))
                    Application.OpenURL(url);

                bool clicked;

                if (displayURL == null)
                    displayURL = url;

                var content = AnimancerEditorUtilities.TempContent(displayURL,
                    "Click to copy this link to the clipboard");

                if (GUILayout.Button(content, GUIStyles.Body))
                {
                    GUIUtility.systemCopyBuffer = displayURL;
                    Debug.Log("Copied '" + displayURL + "' to the clipboard.");
                    clicked = true;
                }
                else clicked = false;

                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Text);

                return clicked;
            }

            /************************************************************************************************************************/

            private bool DoLinkLabel(string label, GUIStyle style)
            {
                var content = AnimancerEditorUtilities.TempContent(label);
                var area = GUILayoutUtility.GetRect(content, style);

                Handles.BeginGUI();
                Handles.color = style.normal.textColor;
                Handles.DrawLine(new Vector3(area.xMin, area.yMax), new Vector3(area.xMax, area.yMax));
                Handles.color = Color.white;
                Handles.EndGUI();

                EditorGUIUtility.AddCursorRect(area, MouseCursor.Link);

                return GUI.Button(area, content, style);
            }

            /************************************************************************************************************************/

            private static class GUIStyles
            {
                /************************************************************************************************************************/

                public static readonly GUIStyle Title = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 26,
                };

                public static readonly GUIStyle Heading = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 18,
                    stretchWidth = false,
                };

                public static readonly GUIStyle Body = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 14,
                };

                /************************************************************************************************************************/

                static GUIStyles()
                {
                    Heading.normal.textColor = new Color32(0x00, 0x78, 0xDA, 0xFF);
                }

                /************************************************************************************************************************/
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
    }
}

#endif

