//-----------------------------------------------------------------------
// <copyright file="ValidationProfile.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinValidator.Editor
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public interface IValidationProfile
    {
        string Name { get; set; }
        string Description { get; set; }

        object GetSource(ValidationProfileResult entry);
        IEnumerable<IValidationProfile> GetNestedValidationProfiles();
        IEnumerable<ValidationProfileResult> Validate(ValidationRunner runner);
        Texture GetProfileIcon();
    }

    public abstract class ValidationProfile : IValidationProfile
    {
        [SerializeField]
        private string name;

        [SerializeField, TextArea(1, 5)]
        private string description;

        public string Name { get => name;
            set => name = value;
        }

        public string Description { get => description;
            set => description = value;
        }

        public virtual object GetSource(ValidationProfileResult entry)
        {
            return entry.Source;
        }

        public virtual IEnumerable<IValidationProfile> GetNestedValidationProfiles()
        {
            yield break;
        }

        public abstract IEnumerable<ValidationProfileResult> Validate(ValidationRunner runner);

        public virtual Texture GetProfileIcon()
        {
            return GUIHelper.GetAssetThumbnail(null, typeof(ScriptableObject), false);
        }
    }

    public class ValidationProfileResult
    {
        public IValidationProfile Profile;
        public float Progress;
        public string Name;
        public object Source;
        public List<ValidationResult> Results;
        public object SourceRecoveryData;

        public object GetSource()
        {
            if (Profile == null) return null;
            if (Results == null) return null;
            return Profile.GetSource(this);
        }
    }

    public class ValidationProfileAttributeProcessor<T> : OdinAttributeProcessor<T>
        where T : ValidationProfile
    {
        public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member)
        {
            return true;
        }

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.DeclaringType != typeof(ValidationProfile))
            {
                attributes.Add(new HideInTablesAttribute());
            }
        }
    }
}