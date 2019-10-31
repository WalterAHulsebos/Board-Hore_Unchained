//-----------------------------------------------------------------------
// <copyright file="AutomatedValidationHook.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [Toggle("EnabledProp", CollapseOthersOnExpand = false)]
    [HideReferenceObjectPicker]
    public class AutomatedValidationHook
    {
        [ShowInInspector]
        private bool EnabledProp
        {
            get => Enabled;

            set
            {
                if (value != Enabled)
                {
                    Enabled = value;
                    OnEnabledChanged();
                }
            }
        }

        [HideInInspector]
        public bool Enabled;

        public bool FinishValidationOnFailures;

        [HideInInspector]
        public IValidationHook Hook;

        // If people ever want the ability to add multiple validations we can easily let them. But lets start out simple.
        //[ListDrawerSettings(Expanded = true, DraggableItems = false, CustomAddFunction = "CreateAutomatedValidation")]
        [HideInInspector]
        public List<AutomatedValidation> Validations = new List<AutomatedValidation>();

        [ShowInInspector]
        [HideLabel, InfoBox("Note that opening the validator window will stop the project from entering play mode", "ShowPlayModeWarning", InfoMessageType = InfoMessageType.Warning)]
        private AutomatedValidation Validation
        {
            get
            {
                if (Validations == null || Validations.Count == 0)
                    Validations = new List<AutomatedValidation>() { new AutomatedValidation() };

                return Validations[0];
            }
            set
            {
                if (Validations == null || Validations.Count == 0)
                    Validations = new List<AutomatedValidation>() { new AutomatedValidation() };

                Validations[0] = value;
            }
        }

        private bool ShowPlayModeWarning()
        {
            return Enabled && Hook is OnPlayValidationHook && (Validation.HasActionFlag(AutomatedValidation.Action.OpenValidatorIfError) || Validation.HasActionFlag(AutomatedValidation.Action.OpenValidatorIfWarning));
        }

        public string Name => Hook.Name;

        public AutomatedValidationHook(IValidationHook hook)
        {
            Hook = hook;
        }

        public void SetupHook()
        {
            Hook.Hook(OnHookExecuting);
        }

        public void Unhook()
        {
            Hook.Unhook(OnHookExecuting);
        }

        private void OnEnabledChanged()
        {
            Hook.Unhook(OnHookExecuting);

            if (Enabled)
            {
                Hook.Hook(OnHookExecuting);
            }
        }

        public void OnHookExecuting()
        {
            ValidationRunner runner = new ValidationRunner();

            bool stopTriggeringEvent = false;

            try
            {
                foreach (AutomatedValidation validation in Validations)
                {
                    bool openValidatorWindow = false;
                    IValidationProfile actuallyFailingProfile = null;

                    try
                    {
                        foreach (IValidationProfile profile in validation.ProfilesToRun)
                        {
                            foreach (ValidationProfileResult result in profile.Validate(runner))
                            {
                                if (GUIHelper.DisplaySmartUpdatingCancellableProgressBar("Executing Validation Hook: " + Name + " (Profile: " + profile.Name + ")", result.Name, result.Progress))
                                {
                                    // Cancel validation
                                    return;
                                }

                                foreach (ValidationResult subResult in result.Results)
                                {
                                    bool couldExitFromFailure = false;

                                    if (subResult.ResultType == ValidationResultType.Error)
                                    {
                                        if (validation.HasActionFlag(AutomatedValidation.Action.LogError))
                                        {
                                            UnityEngine.Object source = result.GetSource() as UnityEngine.Object;
                                            Debug.LogError("Validation error on object '" + source + "', path '" + subResult.GetFullPath() + "': " + subResult.Message, source);
                                        }

                                        if (validation.HasActionFlag(AutomatedValidation.Action.OpenValidatorIfError))
                                        {
                                            openValidatorWindow = true;
                                            couldExitFromFailure = true;
                                        }

                                        if (validation.HasActionFlag(AutomatedValidation.Action.StopHookEventOnError))
                                        {
                                            stopTriggeringEvent = true;
                                            couldExitFromFailure = true;
                                        }
                                    }
                                    else if (subResult.ResultType == ValidationResultType.Warning)
                                    {
                                        if (validation.HasActionFlag(AutomatedValidation.Action.LogWarning))
                                        {
                                            UnityEngine.Object source = result.GetSource() as UnityEngine.Object;
                                            Debug.LogWarning("Validation warning on object '" + source + "', path '" + subResult.GetFullPath() + "': " + subResult.Message, source);
                                        }

                                        if (validation.HasActionFlag(AutomatedValidation.Action.OpenValidatorIfWarning))
                                        {
                                            openValidatorWindow = true;
                                            couldExitFromFailure = true;
                                        }

                                        if (validation.HasActionFlag(AutomatedValidation.Action.StopHookEventOnWarning))
                                        {
                                            stopTriggeringEvent = true;
                                            couldExitFromFailure = true;
                                        }
                                    }

                                    if (couldExitFromFailure)
                                    {
                                        actuallyFailingProfile = profile;

                                        if (!FinishValidationOnFailures)
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (openValidatorWindow)
                        {                           
                            if (Hook is OnPlayValidationHook)
                            {
                                stopTriggeringEvent = true;
                            }   

                            OpenValidator(validation.ProfilesToRun, actuallyFailingProfile);
                        }

                        if (stopTriggeringEvent)
                        {
                            Hook.StopTriggeringEvent();
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void OpenValidator(List<IValidationProfile> profilesToRun, IValidationProfile actuallyFailingProfile)
        {
            IValidationProfile profile;

            if (profilesToRun.Count == 0)
            {
                return;
            }
            else if (profilesToRun.Count == 1)
            {
                profile = profilesToRun[0];
            }
            else if (profilesToRun.All(n => n is ValidationProfileAsset))
            {
                profile = new ValidationCollectionProfile()
                {
                    Name = "Failed '" + Name + "' hook profiles",
                    Description = "These are the profiles that failed when the hook was executed",
                    Profiles = profilesToRun.Cast<ValidationProfileAsset>().ToArray()
                };
            }
            else
            {
                profile = actuallyFailingProfile;
            }

            if (profile != null)
            {
                ValidationProfileManagerWindow.OpenProjectValidatorWithProfile(profile, true);
            }
        }

        public override string ToString()
        {
            return Hook.Name;
        }
    }
}