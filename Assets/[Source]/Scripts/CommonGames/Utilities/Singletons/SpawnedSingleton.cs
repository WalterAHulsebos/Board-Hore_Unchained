namespace CommonGames.Utilities
{
    using UnityEngine;
    
    using UnityEditor;
    
    using JetBrains.Annotations;

    #if ODIN_INSPECTOR
    using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour; 
    #endif

    //Why don't virtual statics exist?....
    /// <typeparam name="T">Type of the Singleton</typeparam>
    public abstract class SpawnedSingleton<T> : MonoBehaviour where T : SpawnedSingleton<T>
    {
        //private static readonly object obj = new object();
        
        private static GameObject SpawnedSingletonGameObject = null;
        
        private static T _instance = null;
        
        [PublicAPI]
        public static T Instance
        {
            get
            {
                #if UNITY_EDITOR
                if(!EditorApplication.isPlaying) return null; //Don't do anything while in edit mode only.
                #endif
                
                if(_beingDestroyed)
                {
                    Debug.LogWarning("SpawnedSingleton being accessed while being destroyed!");
                    return null;
                }
                
                if(_instance != null)
                {
                    Debug.Log("Instance is not null!");
                    
                    return _instance;
                }

                if(SpawnedSingletonGameObject == null) return null;
                
                _instance = SpawnedSingletonGameObject.AddComponent<T>();

                return _instance;

            }
                    
            protected set => _instance = value;
        }
        
        private static bool _beingDestroyed = false;

        protected virtual void Awake()
        {
            #if UNITY_EDITOR
            if(!EditorApplication.isPlaying) return; //Don't do anything while in edit mode only.
            #endif
            
            if(SpawnedSingletonGameObject == null)
            {
                SpawnedSingletonGameObject = new GameObject(name: "SpawnedSingletonGameObject");
            }

            if (Instance != null) //For the Instance
            {
                Debug.Log("Instance is not null, destroying Instance");

                Destroy(this);
            }
            else
            {
                Instance = (T)this;   
            }
        }
        
        private void OnDestroy()
        {
            _beingDestroyed = true;

            if(Instance == null) return;
            
            Debug.Log("Somehow Instance isn't null when it's being destroyed");
            Instance = null;
        }
    }
}