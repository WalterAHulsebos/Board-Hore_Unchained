//-----------------------------------------------------------------------
// <copyright file="OdinValidationConfig.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Serialization;
    using Sirenix.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    internal class OdinValidatorConfigAttribute : GlobalConfigAttribute
    {
        public OdinValidatorConfigAttribute(string configPath) : base(SirenixAssetPaths.SirenixPluginPath + configPath) { }
    }

    [OdinValidatorConfig("Odin Validator/Editor/Config/"), ShowOdinSerializedPropertiesInInspector]
    public class OdinValidationConfig : GlobalConfig<OdinValidationConfig>, ISerializationCallbackReceiver, IOverridesSerializationFormat
    {
        [InitializeOnLoadMethod]
        private static void InitHooks()
        {
            UnityEditorEventUtility.DelayAction(() =>
            {
                foreach (AutomatedValidationHook item in Instance.Hooks)
                {
                    if (item.Enabled)
                    {
                        item.SetupHook();
                    }
                }
            });
        }

        [Required]
        [HideReferenceObjectPicker]
        [AssetSelector(Filter = "t:ValidationProfileAsset", DrawDropdownForListElements = false)]
        public List<IValidationProfile> MainValidationProfiles;

        [ValueDropdown("GetAvailableHooks", DrawDropdownForListElements = false, DropdownWidth = 250), HideReferenceObjectPicker]
        [ListDrawerSettings(ListElementLabelName = "Name", Expanded = false, DraggableItems = false)]
        public List<AutomatedValidationHook> Hooks = new List<AutomatedValidationHook>();

        private IEnumerable GetAvailableHooks()
        {
            HashSet<Type> notTheseHooks = new HashSet<Type>();

            if (Hooks != null)
                notTheseHooks.AddRange(Hooks.Where(n => n.Hook != null).Select(n => n.Hook.GetType()));

            List<Type> availableHookTypes = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                .Where(type => !type.IsAbstract && !type.IsInterface && !notTheseHooks.Contains(type) && typeof(IValidationHook).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null).ToList();

            return availableHookTypes
                .Select(type => new AutomatedValidationHook((IValidationHook)Activator.CreateInstance(type)))
                .Select(x => new ValueDropdownItem(x.Hook.Name, x));
        }

        #region OdinSerialization

        [SerializeField, HideInInspector]
        private SerializationData serializationData;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref serializationData);

            if (Hooks == null)
            {
                Hooks = new List<AutomatedValidationHook>();
            }
            else
            {
                Hooks.RemoveAll(n => n.Hook == null);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref serializationData);
        }

        DataFormat IOverridesSerializationFormat.GetFormatToSerializeAs(bool isPlayer)
        {
            return DataFormat.Nodes;
        }

        #endregion

        [Button(ButtonSizes.Medium), HorizontalGroup(0.5f, Order = -2), PropertyOrder(-20)]
        private void CreateNewProfile()
        {
            ValidationProfileSOCreator.ShowDialog<ValidationProfileAsset>(SirenixAssetPaths.SirenixPluginPath + "Odin Validator/Editor/Config/", newProfile =>
            {
                if (newProfile != null)
                {
                    MainValidationProfiles.Add(newProfile);
                    InspectorUtilities.RegisterUnityObjectDirty(this);
                }
            });
        }

        [Button("Reset Default Profiles", ButtonSizes.Medium), HorizontalGroup(0.5f)]
        public void ResetMainProfilesToDefault()
        {
            ResetData(false);
        }

        protected override void OnConfigAutoCreated()
        {
            ResetData(true);
        }

        private TAsset GetOrCreateValidationProfileSubAsset<TAsset, TProfile>(TProfile newProfile, bool overridePreExistingProfile)
            where TAsset : ValidationProfileAsset<TProfile>
            where TProfile : IValidationProfile
        {
            string path = AssetDatabase.GetAssetPath(this);
            TAsset asset = AssetDatabase.LoadAllAssetsAtPath(path).OfType<TAsset>().FirstOrDefault(a => a.name == newProfile.Name);

            if (asset == null)
            {
                asset = CreateInstance<TAsset>();
                asset.Profile = newProfile;
                asset.name = newProfile.Name;
                AssetDatabase.AddObjectToAsset(asset, this);
                AssetDatabase.SaveAssets();
            }
            else if (overridePreExistingProfile)
            {
                asset.Profile = newProfile;
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }

        private void ResetData(bool overridePreExistingProfileAssets)
        {
            AssetValidationProfileAsset scanAllAssets = GetOrCreateValidationProfileSubAsset<AssetValidationProfileAsset, AssetValidationProfile>(new AssetValidationProfile()
            {
                Name = "Scan All Assets",
                Description = "Scans all prefabs and scriptable objects in the project",
                AssetPaths = new string[] { "Assets" },
                SearchFilters = new string[] { "t:Prefab", "t:ScriptableObject" }
            }, overridePreExistingProfileAssets);

            SceneValidationProfileAsset scanAllScenes = GetOrCreateValidationProfileSubAsset<SceneValidationProfileAsset, SceneValidationProfile>(new SceneValidationProfile()
            {
                Name = "Scan All Scenes",
                Description = "Scans all scenes in the project",
                ScenePaths = new string[] { "Assets" }
            }, overridePreExistingProfileAssets);

            ValidationCollectionProfileAsset scanEntireProject = GetOrCreateValidationProfileSubAsset<ValidationCollectionProfileAsset, ValidationCollectionProfile>(new ValidationCollectionProfile()
            {
                Name = "Scan Entire Project",
                Description = "Scans all prefabs, scriptable objects and scenes in the project",
                Profiles = new ValidationProfileAsset[] { scanAllAssets, scanAllScenes }
            }, overridePreExistingProfileAssets);

            SceneValidationProfileAsset scanOpenScenes = GetOrCreateValidationProfileSubAsset<SceneValidationProfileAsset, SceneValidationProfile>(new SceneValidationProfile()
            {
                Name = "Scan Open Scenes",
                Description = "Scans all open scenes, without going through scene asset dependencies.",
                IncludeOpenScenes = true,
            }, overridePreExistingProfileAssets);

            SceneValidationProfileAsset scanScenesFromBuildOptions = GetOrCreateValidationProfileSubAsset<SceneValidationProfileAsset, SceneValidationProfile>(new SceneValidationProfile()
            {
                Name = "Scan Scenes From Build Options",
                Description = "Scans all scenes from build options, including scene asset dependencies.",
                IncludeScenesFromBuildOptions = true,
                IncludeAssetDependencies = true,
            }, overridePreExistingProfileAssets);

            AutomatedValidationHook onPlayHook = new AutomatedValidationHook(new OnPlayValidationHook())
            {
                Enabled = false,
                Validations = new List<AutomatedValidation>()
                {
                    new AutomatedValidation()
                    {
                        Actions = AutomatedValidation.Action.OpenValidatorIfError | AutomatedValidation.Action.OpenValidatorIfWarning,
                        ProfilesToRun = new List<IValidationProfile>() { scanOpenScenes }
                    }
                }
            };

            AutomatedValidationHook onBuild = new AutomatedValidationHook(new OnBuildValidationHook())
            {
                Enabled = false,
                Validations = new List<AutomatedValidation>()
                {
                    new AutomatedValidation()
                    {
                        Actions = AutomatedValidation.Action.OpenValidatorIfError | AutomatedValidation.Action.OpenValidatorIfWarning,
                        ProfilesToRun = new List<IValidationProfile>() { scanScenesFromBuildOptions }
                    }
                }
            };

            AutomatedValidationHook onStartup = new AutomatedValidationHook(new OnProjectStartupValidationHook())
            {
                Enabled = false,
                Validations = new List<AutomatedValidation>()
                {
                    new AutomatedValidation()
                    {
                        Actions = AutomatedValidation.Action.OpenValidatorIfError | AutomatedValidation.Action.OpenValidatorIfWarning,
                        ProfilesToRun = new List<IValidationProfile>() { scanEntireProject }
                    }
                }
            };

            MainValidationProfiles = new List<IValidationProfile>()
            {
                scanEntireProject,
                scanAllAssets,
                scanAllScenes,
                scanOpenScenes,
                scanScenesFromBuildOptions,
            };

            Hooks = new List<AutomatedValidationHook>()
            {
                onPlayHook,
                onBuild,
                onStartup
            };
        }
    }
}