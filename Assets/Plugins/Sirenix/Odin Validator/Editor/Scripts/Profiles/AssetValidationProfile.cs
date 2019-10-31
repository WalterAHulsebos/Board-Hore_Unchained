//-----------------------------------------------------------------------
// <copyright file="AssetValidationProfile.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;

    [Serializable]
    public class AssetValidationProfile : ValidationProfile
    {
        [FolderPath]
        public string[] SearchFilters = new string[] { "t:Prefab", "t:ScriptableObject" };

        [FolderPath]
        [PropertyTooltip("Add asset directoris or individual file paths.")]
        public string[] AssetPaths = new string[0];

        [Required]
        [PropertyTooltip("Add folders or files by reference.")]
        public UnityEngine.Object[] AssetReferences = new UnityEngine.Object[0];

        [FolderPath]
        [PropertyTooltip("Exclude asset directoris or individual file paths.")]
        public string[] ExcludeAssetPaths = new string[0];

        [Required]
        [PropertyTooltip("Exclude folders or files by reference.")]
        public UnityEngine.Object[] ExcludeAssetReferences = new UnityEngine.Object[0];

        public IEnumerable<string> GetAllAssetsToValidate()
        {
            if (AssetPaths == null) yield break;
            List<string> allAssetPaths = AssetPaths.ToList();
            List<string> allExcludeAssetPaths = ExcludeAssetPaths.ToList();
            foreach (UnityEngine.Object item in ExcludeAssetReferences.Where(x => x))
            {
                string path = AssetDatabase.GetAssetPath(item);
                if (!string.IsNullOrEmpty(path))
                {
                    if (Directory.Exists(path))
                    {
                        allAssetPaths.Add(path);
                    }
                    else if(File.Exists(path))
                    {
                        allExcludeAssetPaths.Add(path);
                    }
                }
            }

            HashSet<string> excludeMap = new HashSet<string>();

            // Exclude assets:
            string[] excludeDirectories = allExcludeAssetPaths.Select(x => x.Trim('/'))
                  .Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x))
                  .ToArray();

            List<string> excludeAssetPaths = allExcludeAssetPaths.Where(x => File.Exists(x)).ToList();

            excludeMap.AddRange(excludeAssetPaths);
            if (excludeDirectories.Length > 0)
            {
                List<string> guids = SearchFilters.SelectMany(x => AssetDatabase.FindAssets(x, excludeDirectories)).ToList();
                List<string> assets = guids.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
                excludeMap.AddRange(assets);
            }

            // Add assets:
            string[] addDirectories = allAssetPaths.Select(x => x.Trim('/')).Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x)).ToArray();
            List<string> addAssetPaths = allAssetPaths.Where(x => File.Exists(x)).ToList();

            if (addDirectories.Length > 0)
            {
                List<string> guids = SearchFilters.SelectMany(x => AssetDatabase.FindAssets(x, addDirectories)).ToList();
                List<string> assets = guids.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();

                foreach (string asset in assets)
                {
                    if (excludeMap.Add(asset))
                    {
                        yield return asset;
                    }
                }
            }

            foreach (string asset in addAssetPaths)
            {
                if (excludeMap.Add(asset))
                {
                    yield return asset;
                }
            }
        }

        public override IEnumerable<ValidationProfileResult> Validate(ValidationRunner runner)
        {
            List<string> assetPaths = GetAllAssetsToValidate().ToList();

            float step = 1f / assetPaths.Count;
            for (int i = 0; i < assetPaths.Count; i++)
            {
                string path = assetPaths[i];
                float progress = step * i;
                UnityEngine.Object[] assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(path);

                foreach (UnityEngine.Object asset in assetsAtPath)
                {
                    List<ValidationResult> results = null;

                    runner.ValidateUnityObjectRecursively(asset, ref results);

                    yield return new ValidationProfileResult()
                    {
                        Profile = this,
                        Progress = progress,
                        Name = Path.GetFileName(path),
                        Source = asset,
                        Results = results,
                        SourceRecoveryData = asset,
                    };
                }
            }
        }
    }
}