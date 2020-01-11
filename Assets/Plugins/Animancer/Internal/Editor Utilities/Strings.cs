// Animancer // Copyright 2019 Kybernetik //

namespace Animancer
{
    /// <summary>
    /// Various string constants used throughout <see cref="Animancer"/>.
    /// </summary>
    public static class Strings
    {
        /************************************************************************************************************************/

        /// <summary>The URL of the website where the Animancer documentation is hosted.</summary>
        public const string DocumentationURL = "https://kybernetikgames.github.io/animancer";

        /// <summary>The URL of the website where the Animancer API documentation is hosted.</summary>
        public const string APIDocumentationURL = DocumentationURL + "/api/Animancer";

        /// <summary>The email address which handles support for Animancer.</summary>
        public const string DeveloperEmail = "AnimancerUnityPlugin@gmail.com";

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] URLs of various documentation pages.</summary>
        public static class DocsURLs
        {
            /************************************************************************************************************************/
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member.
            /************************************************************************************************************************/

            public const string DocsURL = DocumentationURL + "/docs/";

            public const string AnimatorControllers = DocsURL + "manual/animator-controllers";

            public const string Fading = DocsURL + "manual/blending/fading";

            public const string AnimationTypes = DocsURL + "manual/inspector#animation-types";

            public const string EndEvents = DocsURL + "manual/animation-events#end-events";

            public const string UpdateModes = DocsURL + "bugs/update-modes";

            public const string ChangeLogPrefix = DocsURL + "changes/animancer-";

            /************************************************************************************************************************/
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member.
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}

