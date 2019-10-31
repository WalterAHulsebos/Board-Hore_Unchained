using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if ODIN_INSPECTOR
using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;
#endif

namespace CommonGames.Utilities
{
    public abstract class Multiton<T> : MonoBehaviour where T : Multiton<T>
    {
        protected static int Length;

        ///<summary> Static reference to all the Instances of T</summary>
        private static Dictionary<Type, object> InstancesDictionary { get; set; } = new Dictionary<Type, object>();

        public static List<T> Instances
        {
            get
            {
                InstancesDictionary.TryGetValue(typeof(T), out object __instances);
                return (List<T>)__instances;
            }
            set
            {
                InstancesDictionary.TryGetValue(typeof(T), out object __instances);
                __instances = value;
            }
        }
        
        public static int IndexFromInstance(Multiton<T> instance) => InstancesDictionary.Values.ToList().IndexOf(instance);

        ///<summary> Gets whether Instances of this Multiton exist.</summary>
        public static bool InstanceExists
        {
            get
            {
                bool __typeExists = InstancesDictionary.TryGetValue(typeof(T), out object __instances);

                if (!__typeExists) return false;
                
                return ((List<T>)__instances).Count > 0;
            }
        }

        ///<summary> OnEnable method to associate Multiton to their Instance.</summary>
        protected virtual void OnEnable()
        {
            bool __typeExists = InstancesDictionary.TryGetValue(typeof(T), out object instances);
            
            if (!__typeExists)
            {
                instances = new List<T>(Length);
                InstancesDictionary.Add(typeof(T), instances);
            }

            ((List<T>)instances)?.Add((T)this);
        }

        ///<summary> OnDisable method to clear Multiton association.</summary>
        protected virtual void OnDisable()
        {
            InstancesDictionary.TryGetValue(typeof(T), out object __instances);
            ((List<T>)__instances)?.Remove((T)this);
        }
    }
}