using System;
using System.Collections;
using System.Reflection;

using UnityEngine;

using JetBrains.Annotations;

namespace CommonGames.Utilities.Extensions
{
    using Random = System.Random;
    
    public static partial class Constants
    {
        /// <summary> Lowercase letters from a to z </summary>
        [PublicAPI]
        public const string LOWERCASE_LETTERS = "abcdefghijklmnopqrstuvwxyz";
        
        /// <summary> Uppercase letters from A to Z </summary>
        [PublicAPI]
        public const string UPPERCASE_LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        /// <summary> All <see cref="LOWERCASE_LETTERS"/> and <see cref="UPPERCASE_LETTERS"/>. </summary>
        [PublicAPI]
        public const string LETTERS = LOWERCASE_LETTERS + UPPERCASE_LETTERS;
        
        /// <summary> Digits from 0 to 9 </summary>
        [PublicAPI]
        public const string DIGITS = "0123456789";
        
        /// <summary> All <see cref="LETTERS"/> and <see cref="DIGITS"/> </summary>
        [PublicAPI]
        public const string ALPHANUMERICS = LETTERS + DIGITS;
        
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        public static Random RANDOM { get; } = new Random();
        
        [PublicAPI]
        public sealed class CoroutineHandlerClass : EnsuredSingleton<CoroutineHandlerClass>{}
        
        [PublicAPI]
        public static CoroutineHandlerClass CoroutineHandler => CoroutineHandlerClass.Instance;

        /// <summary> The default wait time for WaitForSeconds' </summary>
        [PublicAPI]
        public const float DEFAULT_TOGGLE_TIME = 0f;
        
        [PublicAPI]
        public static readonly WaitForSeconds DefaultWait = new WaitForSeconds(DEFAULT_TOGGLE_TIME);
        
        
        ///// <summary> Square root of 0.5 </summary>
        //[PublicAPI]
        //public const float SQRT_05 = 0.7071067811865475244f;
        
        /// <summary> Square root of 2. </summary>
        [PublicAPI]
        public const float SQRT_2 = 1.4142135623730950488f;
        
        /// <summary> Square root of 5. </summary>
        [PublicAPI]
        public const float SQRT_5 = 2.2360679774997896964f;
        
        /// <summary> Golden angle in radians. </summary>
        [PublicAPI]
        public const float GOLDEN_ANGLE = Mathf.PI * (3 - SQRT_5);

    }
}
