//-----------------------------------------------------------------------
// <copyright file="ValidationCollectionProfile.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector.Editor.Validation;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ValidationCollectionProfile : ValidationProfile
    {
        public ValidationProfileAsset[] Profiles;

        public override IEnumerable<IValidationProfile> GetNestedValidationProfiles()
        {
            return Profiles;
        }

        public override IEnumerable<ValidationProfileResult> Validate(ValidationRunner runner)
        {
            float partialProgress = 0f;
            float partialProgressStepSize = 1f / Profiles.Length;
            for (int i = 0; i < Profiles.Length; i++)
            {
                IValidationProfile profile = Profiles[i];
                foreach (ValidationProfileResult result in profile.Validate(runner))
                {
                    result.Progress = result.Progress * partialProgressStepSize + partialProgress;
                    yield return result;
                }

                partialProgress += partialProgressStepSize;
            }
        }
    }
}