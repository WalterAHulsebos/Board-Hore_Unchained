//-----------------------------------------------------------------------
// <copyright file="SceneValidationProfile.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    public class SceneValidationProfile : ValidationProfile
    {
        public bool IncludeScenesFromBuildOptions;
        public bool IncludeOpenScenes;
        public bool IncludeAssetDependencies;

        [FilePath]
        [ListDrawerSettings(Expanded = true)]
        [PropertyTooltip("Reference folders containing scenes or individual scenes.")]
        public string[] ScenePaths = new string[0], ExcludeScenePaths = new string[0];

        public IEnumerable<string> GetAllScenes()
        {
            HashSet<string> excludeMap = new HashSet<string>();

            // Exclude scenes:
            if (ExcludeScenePaths != null)
            {
                string[] excludeDirectories = ExcludeScenePaths.Select(x => x.Trim('/')).Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x)).ToArray();
                IEnumerable<string> excludeScenesFiles = ExcludeScenePaths.Where(x => File.Exists(x));
                if (excludeDirectories.Length > 0)
                {
                    excludeMap.AddRange(AssetDatabase.FindAssets("t:SceneAsset", excludeDirectories).Select(x => AssetDatabase.GUIDToAssetPath(x)));
                }
                excludeMap.AddRange(excludeScenesFiles);
            }

            // Add scenes:
            if (ScenePaths != null)
            {
                string[] addDirectories = ScenePaths.Select(x => x.Trim('/')).Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x)).ToArray();
                IEnumerable<string> addSceneFiles = ScenePaths.Where(x => File.Exists(x));

                if (addDirectories.Length > 0)
                {
                    IEnumerable<string> scenes = AssetDatabase.FindAssets("t:SceneAsset", addDirectories)
                        .Select(x => AssetDatabase.GUIDToAssetPath(x));

                    foreach (string scene in scenes)
                    {
                        if (excludeMap.Add(scene))
                        {
                            yield return scene;
                        }
                    }
                }

                foreach (string scene in addSceneFiles)
                {
                    if (excludeMap.Add(scene))
                    {
                        yield return scene;
                    }
                }
            }

            if (IncludeScenesFromBuildOptions)
            {
                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {
                    if (scene.enabled && !string.IsNullOrEmpty(scene.path) && excludeMap.Add(scene.path) && File.Exists(scene.path))
                    {
                        yield return scene.path;
                    }
                }
            }

            if (IncludeOpenScenes)
            {
                SceneSetup[] setupScenes = EditorSceneManager.GetSceneManagerSetup();

                foreach (SceneSetup scene in setupScenes)
                {
                    if (!string.IsNullOrEmpty(scene.path) && excludeMap.Add(scene.path))
                    {
                        yield return scene.path;
                    }
                }
            }
        }

        public override object GetSource(ValidationProfileResult entry)
        {
            if (entry.Source as UnityEngine.Object) return entry.Source;

            SceneAddress address = (SceneAddress)entry.SourceRecoveryData; // This should work.

            bool openScene = true;

            foreach (SceneSetup setupScene in EditorSceneManager.GetSceneManagerSetup())
            {
                if (setupScene.path == address.ScenePath)
                {
                    openScene = false;

                    if (!setupScene.isActive)
                        SceneManager.SetActiveScene(SceneManager.GetSceneByPath(setupScene.path));

                    break;
                }
            }

            if (openScene)
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    return null;

                EditorSceneManager.OpenScene(address.ScenePath, OpenSceneMode.Single);
            }

            GameObject go = GetGameObjectFromIndexPath(address);

            if (go != null)
            {
                entry.Source = go;

                if (address.ComponentType != null)
                {
                    Component component = null;
                    Component[] components = go.GetComponents(address.ComponentType);

                    if (address.ComponentIndex >= 0 && address.ComponentIndex < components.Length)
                        component = components[address.ComponentIndex];

                    if (component != null)
                        entry.Source = component;
                }
            }

            return entry.Source;
        }

        private GameObject GetGameObjectFromIndexPath(SceneAddress address)
        {
            List<int> path = address.HierarchyIndexPath;

            if (path.Count == 0) return null;

            GameObject[] roots = SceneRoots(address.ScenePath).ToArray();

            if (path[0] >= roots.Length) return null;

            Transform curr = roots[path[0]].transform;

            for (int i = 1; i < path.Count; i++)
            {
                curr = curr.GetChild(path[i]);
                if (curr == null) return null;
            }

            return curr.gameObject;
        }

        private static readonly MethodInfo Scene_GetRootGameObjects_Method = typeof(Scene).GetMethod("GetRootGameObjects", Flags.InstancePublic, null, Type.EmptyTypes, null);

        public static IEnumerable<GameObject> SceneRoots(string scenePath)
        {
            Scene scene = SceneManager.GetSceneByPath(scenePath);

            if (Scene_GetRootGameObjects_Method != null && scene.IsValid())
            {
                GameObject[] roots = (GameObject[])Scene_GetRootGameObjects_Method.Invoke(scene, null);

                foreach (GameObject root in roots)
                    yield return root;
            }
            else
            {
                // Fallback; only works in Unity versions without multi-scene support
                HierarchyProperty prop = new HierarchyProperty(HierarchyType.GameObjects);
                int[] expanded = new int[0];

                while (prop.Next(expanded))
                {
                    yield return prop.pptrValue as GameObject;
                }
            }
        }

        public override IEnumerable<ValidationProfileResult> Validate(ValidationRunner runner)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                yield break;

            UnityEngine.Object[] selection = Selection.objects;
            List<string> scenesToTest = GetAllScenes().ToList();
            SceneSetup[] setup = EditorSceneManager.GetSceneManagerSetup();

            float partialProgress = 0f;
            float partialProgressStepSize = 1f / (scenesToTest.Count + (IncludeAssetDependencies ? 1 : 0));

            for (int i = 0; i < scenesToTest.Count; i++)
            {
                string scene = scenesToTest[i];

                EditorSceneManager.OpenScene(scene, OpenSceneMode.Single);

                List<Transform> gameObjectsToScan = Resources.FindObjectsOfTypeAll<Transform>()
                   .Where(x => (x.gameObject.scene.IsValid() && (x.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0))
                   //.SelectMany(x => x.GetComponents(typeof(Component)).Select(c => new { go = x.gameObject, component = c }))
                   .ToList();

                float step = 1f / gameObjectsToScan.Count;
                for (int j = 0; j < gameObjectsToScan.Count; j++)
                {
                    GameObject go = gameObjectsToScan[j].gameObject;
                    float progress = j * step * partialProgressStepSize + partialProgress;

                    {
                        List<ValidationResult> result = runner.ValidateUnityObjectRecursively(go);

                        ValidationProfileResult entry = new ValidationProfileResult()
                        {
                            Name = go.name,
                            Profile = this,
                            Source = go,
                            Results = result,
                            Progress = progress,
                            SourceRecoveryData = GetRecoveryData(go, null, scene),
                        };

                        yield return entry;
                    }

                    Component[] components = go.GetComponents<Component>();

                    for (int k = 0; k < components.Length; k++)
                    {
                        Component component = components[k];

                        if (component == null)
                        {
                            ValidationProfileResult entry = new ValidationProfileResult()
                            {
                                Name = "Missing Reference",
                                Source = go,
                                SourceRecoveryData = GetRecoveryData(go, component, scene),
                                Profile = this,
                                Progress = progress,
                                Results = new List<ValidationResult>() { new ValidationResult()
                                {
                                    Message = "Missing Reference",
                                    ResultType = ValidationResultType.Error,
                                }}
                            };

                            yield return entry;
                        }
                        else
                        {
                            List<ValidationResult> result = runner.ValidateUnityObjectRecursively(component);
                            ValidationProfileResult entry = new ValidationProfileResult()
                            {
                                Name = go.name + " - " + component.GetType().GetNiceName().SplitPascalCase(),
                                Profile = this,
                                Source = component,
                                Results = result,
                                Progress = progress,
                                SourceRecoveryData = GetRecoveryData(go, component, scene),
                            };

                            yield return entry;
                        }
                    }
                }
                partialProgress += partialProgressStepSize;
            }

            // Load a new empty scene that will be unloaded immediately, just to be sure we completely clear all changes made by the scan
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            if (setup.Length != 0)
            {
                EditorSceneManager.RestoreSceneManagerSetup(setup);
            }

            if (IncludeAssetDependencies)
            {
                UnityEngine.Object[] scenes = scenesToTest
                    .ToHashSet()
                    .Select(x => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(x))
                    .ToArray();

                UnityEngine.Object[] dep = EditorUtility.CollectDependencies(scenes);
                List<Component> components = dep.OfType<Component>().ToList();
                List<ScriptableObject> scriptableObjects = dep.OfType<ScriptableObject>().ToList();
                UnityEngine.Object[] allObjects = components.Cast<UnityEngine.Object>().Concat(scriptableObjects.Cast<UnityEngine.Object>())
                    .ToArray();

                float step = 1f / allObjects.Length;
                for (int i = 0; i < allObjects.Length; i++)
                {
                    UnityEngine.Object obj = allObjects[i];
                    float progress = i * step * partialProgressStepSize + partialProgress;
                    List<ValidationResult> result = runner.ValidateUnityObjectRecursively(obj);

                    ValidationProfileResult entry = new ValidationProfileResult()
                    {
                        Name = obj.name,
                        Profile = this,
                        Source = obj,
                        Results = result,
                        Progress = progress,
                    };

                    yield return entry;
                }
            }

            Selection.objects = selection;
        }

        private SceneAddress GetRecoveryData(GameObject go, Component co, string scene)
        {
            SceneAddress result = new SceneAddress();
            result.ScenePath = scene;
            result.HierarchyPath = GetGameObjectPath(go.transform);
            result.HierarchyIndexPath = GetGameObjectIndexPath(go.transform);

            if (co != null)
            {
                result.ComponentType = co.GetType();
                result.ComponentIndex = (go.GetComponents(co.GetType()) as IList).IndexOf(co);
            }

            return result;
        }

        private static string GetGameObjectPath(Transform transform)
        {
            string path = "";
            Transform curr = transform;

            while (curr)
            {
                if (path != "")
                    path = "/" + path;

                path = curr.name + path;
                curr = curr.parent;
            }

            return path;
        }

        private static List<int> GetGameObjectIndexPath(Transform transform)
        {
            List<int> result = new List<int>();
            Transform curr = transform;
            while (curr)
            {
                result.Add(curr.GetSiblingIndex());
                curr = curr.parent;
            }
            result.Reverse();
            return result;
        }

        public struct SceneAddress
        {
            public string ScenePath;
            public string HierarchyPath;
            public List<int> HierarchyIndexPath;
            public Type ComponentType;
            public int ComponentIndex;
        }
    }
}