//-----------------------------------------------------------------------
// <copyright file="ValidationProfileAsset.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor.Validation;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public abstract class ValidationProfileAsset : ScriptableObject, IValidationProfile
    {
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public abstract IEnumerable<IValidationProfile> GetNestedValidationProfiles();
        public abstract Texture GetProfileIcon();
        public abstract object GetSource(ValidationProfileResult entry);
        public abstract IEnumerable<ValidationProfileResult> Validate(ValidationRunner runner);

        public abstract IValidationProfile GetWrappedProfile();
    }

    public abstract class ValidationProfileAsset<T> : ValidationProfileAsset
        where T : IValidationProfile
    {
        [HideLabel]
        public T Profile;

        public override string Name
        {
            get => Profile == null ? "" : Profile.Name;
            set { if (Profile != null) Profile.Name = value; }
        }

        public override string Description
        {
            get => Profile == null ? "" : Profile.Description;
            set { if (Profile != null) Profile.Description = value; }
        }

        public override IEnumerable<IValidationProfile> GetNestedValidationProfiles()
        {
            if (Profile == null) yield break;

            foreach (IValidationProfile profile in Profile.GetNestedValidationProfiles())
                yield return profile;
        }

        public override Texture GetProfileIcon()
        {
            return Profile == null ? null : Profile.GetProfileIcon();
        }

        public override object GetSource(ValidationProfileResult entry)
        {
            return Profile == null ? null : Profile.GetSource(entry);
        }

        public override IEnumerable<ValidationProfileResult> Validate(ValidationRunner runner)
        {
            if (Profile == null) yield break;

            foreach (ValidationProfileResult result in Profile.Validate(runner))
                yield return result;
        }

        public override IValidationProfile GetWrappedProfile()
        {
            return Profile;
        }
    }
}