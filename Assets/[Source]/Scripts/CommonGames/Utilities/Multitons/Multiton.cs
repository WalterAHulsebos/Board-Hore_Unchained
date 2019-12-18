﻿using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.Serialization;

using System.Linq;

using UnityEngine;

using CommonGames.Utilities.Helpers;

using Sirenix.Serialization;

using JetBrains.Annotations;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;
#endif

// ReSharper disable ArgumentsStyleOther
// ReSharper disable once ArgumentsStyleNamedExpression
// ReSharper disable once CheckNamespace
namespace CommonGames.Utilities
{
    using Extensions;
    using Sirenix.Utilities;

    [ExecuteAlways]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "ArrangeThisQualifier")]
    public abstract class Multiton<T> : MonoBehaviour 
        where T : Multiton<T>
    {
        [PublicAPI]
        //[OdinSerialize]
        //[NonSerialized]
        [ListDrawerSettings(ShowIndexLabels = true)]
        public static List<T> Instances = new List<T>();
        
        [PublicAPI]
        public static int IndexFromInstance(T instance) => Instances.IndexOf(instance);

        /// <summary> Access you the vehicle at Index i </summary>
        [PublicAPI]
        public T this[in int i]
        {
            get => Instances[i];
            set => Instances[i] = value;
        }

        private bool _initialized = false;
        
        ///<summary> Associate Multiton to its Instance.</summary>
        private void Initialize()
        {
            //Debug.Log("Initialize");
            if(PrefabCheckHelper.CheckIfPrefab(this)) return;
            
            if(_initialized) return;
            _initialized = true;

            if(Instances.Contains(this)) return;

            Instances.CGAddAtLast(this as T);
        }
        
        ///<summary> Clear Multiton association.</summary>
        private void DeInitialize()
        {
            //Debug.Log("De-Initialize");
            
            //if(!Instances.Contains(this)) goto RECALCULATE; 
            
            Instances.Remove(this as T);

            this._initialized = false;

            RECALCULATE:
            RecalculateIndices();
        }
        
        #region Initialization
        
        protected virtual void Reset() => Initialize();

        protected virtual void OnValidate() => Initialize();

        //protected virtual void Awake() => Initialize(); // => Initialize();
        
        protected virtual void OnEnable() => Initialize(); // => Initialize();
        
        #endregion


        #region De-Initialization

        protected virtual void OnDisable() => DeInitialize();

        #endregion

        private static bool AllNull = true;
        
        /// <summary> Recalculates the Index of every Instance. </summary>
        protected virtual void RecalculateIndices()
        {
            //Is set to false if there's ANY Instance of this Multiton
            AllNull = true;

            for(int __index = 0; __index < Instances.Count; __index++)
            {
                T __instance = Instances[__index];
                
                //Debug.Log($"Index = {__index}, \n Instance is null? = {__instance.SafeIsUnityNull()}");

                if(__instance.SafeIsUnityNull()) continue;

                AllNull = false;

                __instance.Initialize();
            }

            //TODO: Replace hacky fix...
            if(!AllNull) return;
            
            Debug.Log("RESET!");
                
            Instances.Clear();
        }
        
    }
}