﻿using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

#region ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endregion
#endif

public abstract class CGMonoBehaviour : MonoBehaviour
{

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

    protected abstract void OnEditorEnable();
    protected abstract void OnEditorDisable();
    protected abstract void OnEditorDestroy();
    
    [CustomEditor(typeof(CGMonoBehaviour), editorForChildClasses: true)]
    public class CGMonoBehaviourEditor : 
        #if ODIN_INSPECTOR
        OdinEditor
        #else
        Editor
        #endif
    {
        private bool _runningInEditor = true;
        
        #if ODIN_INSPECTOR
        protected override void OnEnable()
        {
            base.OnEnable();
        #else
        private void OnEnable()
        {
        #endif
        
            _runningInEditor = Application.isEditor && !Application.isPlaying;
        }

        
        #if ODIN_INSPECTOR
        protected override void OnDisable()
        {
            base.OnDisable();
        #else
        private void OnDisable()
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
}