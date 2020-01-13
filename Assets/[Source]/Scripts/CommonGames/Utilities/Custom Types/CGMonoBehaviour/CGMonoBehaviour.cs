using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#region ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endregion

public abstract class CGMonoBehaviour : MonoBehaviour
{
    //#if UNITY_EDITOR

    public static event Action OnAnyDestroyedInEditor_Event;
    
    private bool _isInitialized = false;
    
    protected void Reset()
    {
        if(this._isInitialized) return;
        
        #if UNITY_EDITOR
        
        if(EditorApplication.isPlaying) return;
        
        this.OnEditorEnable();
        
        #endif
    }

    #if UNITY_EDITOR
    
    protected abstract void OnEditorEnable();
    protected abstract void OnEditorDisable();
    protected abstract void OnEditorDestroy();
    
    #endif
    
    [CustomEditor(typeof(CGMonoBehaviour), editorForChildClasses: true)]
    public class CGMonoBehaviourEditor : 
        #if Odin_Editor
        Editor
        #else
        OdinEditor
        #endif
    {
        private bool _runningInEditor = true;
        
        #if ODIN_INSPECTOR
        protected override void OnEnable()
        {
            base.OnEnable();
        #else
        protected virtual void OnEnable()
        {
        #endif
        
            _runningInEditor = Application.isEditor && !Application.isPlaying;
        }

        
        #if ODIN_INSPECTOR
        protected override void OnDisable()
        {
            base.OnDisable();
        #else
        protected virtual void OnDisable()
        {
        #endif

            if(!_runningInEditor) return;
            
            try
            {
                (target as CGMonoBehaviour)?.OnEditorDestroy();
                
                OnAnyDestroyedInEditor_Event?.Invoke();
            }
            catch
            {
                Debug.Log("SOMETHING WENT WRONG");
            }
        }
    }

    //#endif
}