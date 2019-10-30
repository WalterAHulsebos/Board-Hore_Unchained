using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using UnityEditor;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Fiftytwo.Wiring
{
    [CanEditMultipleObjects]
    [CustomEditor( typeof( StoppableDelay ) )]
    public class StoppableDelayEditor : Editor
    {
        SerializedProperty _timeUnit;
        SerializedProperty _random;
        SerializedProperty _interval;
        SerializedProperty _interval2;
        SerializedProperty _outputEvent;

        void OnEnable()
        {
            _timeUnit = serializedObject.FindProperty( "_timeUnit" );
            _random = serializedObject.FindProperty( "_random" );
            _interval = serializedObject.FindProperty( "_interval" );
            _interval2 = serializedObject.FindProperty( "_interval2" );
            _outputEvent = serializedObject.FindProperty( "_outputEvent" );
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField( _timeUnit );

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField( _random );
            EditorGUILayout.PropertyField( _interval );

            if( _random.boolValue )
                EditorGUILayout.PropertyField( _interval2 );

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField( _outputEvent );

            serializedObject.ApplyModifiedProperties();
        }
    }
}
