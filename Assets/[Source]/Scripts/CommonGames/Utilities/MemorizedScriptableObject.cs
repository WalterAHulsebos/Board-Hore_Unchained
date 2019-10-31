using UnityEngine;

#if ODIN_INSPECTOR
using ScriptableObject = Sirenix.OdinInspector.SerializedScriptableObject; 
#endif

/// <summary>
/// A ScriptableObject which caches and loads itself. 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MemorizedScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private const string RESOURCES_FOLDER_NAME = "MemorizedScriptableObjects";

    [SerializeField] private new string name; 
    
    private T _CachedObject;
    public T Load
    {
        get
        {
            if (_CachedObject != null) return _CachedObject;

            T obj = Resources.Load(path: $"{RESOURCES_FOLDER_NAME}/{name}") as T;
            //{typeof(T).Name}") as T;
            
            return _CachedObject = obj ?? CreateInstance<T>();
        }
    }
}
