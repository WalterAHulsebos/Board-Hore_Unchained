using System;

using UnityEngine;
	
using JetBrains.Annotations;

#if ODIN_INSPECTOR
using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour; 
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CommonGames.Utilities
{

    /// <typeparam name="T"> Type of the Singleton. </typeparam>
    public abstract class EditorSingleton<T> : CGMonoBehaviour
        where T : EditorSingleton<T>
    {
        #region Variables

        private static T _instance = null;
        /// <summary> The static reference to the Instance </summary>
        [PublicAPI]
        public static T Instance 
        { 
            get => _instance = _instance ? _instance : FindObjectOfType<T>();
            protected set => _instance = value; 
        }

        /// <summary> Gets whether an Instance of this singleton exists </summary>
        [PublicAPI]
        public static bool InstanceExists => (_instance != null);
		
        #endregion

        #region Methods

        /// <summary> This is only in the Singleton so OnDestroy is called??</summary>
        protected virtual void OnAwake() { }
        
        /// <summary> OnEnable method to associate Singleton with Instance </summary>
        protected virtual void OnEnable()
        {
            Debug.Log("THE EDITOR SINGLETON HAS BEEN ENABLED");

            if(InstanceExists)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = (T)this;
            }
        }

        /// <summary> OnDestroy method to clear Singleton association </summary>
        protected void OnDestroy()
        {
            //base.OnDestroy();
            
            Debug.Log("THE EDITOR SINGLETON HAS BEEN DESTROYED");
            
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        /// <summary> Reset method to associate Singleton with Instance IN EDITOR. </summary>
        protected override void OnEditorEnable()
        {
            OnAnyDestroyedInEditor_Event += OnAnyEditorDestroyed;

            Debug.Log("EDITOR ENABLE");
            
            OnEnable();
        }
        protected override void OnEditorDisable() { }

        protected override void OnEditorDestroy()
        {
            if(Instance == this)
            {
                Instance = null;
            }
        }

        protected static void OnAnyEditorDestroyed()
        {
            Debug.Log("ANY EDITOR DISABLE");
            
            Instance = FindObjectOfType<T>();
        }

        #endregion
    }
}