using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TSD.uTireRuntime;

namespace TSD.uTireEditor
{
	[CustomEditor(typeof(uTireWorldSpaceSynchronizer))]
	public class uTireWorldSpaceSynchronizerEditor : Editor
	{
		Texture logo;

		SerializedProperty m_templateBehavior;
		SerializedProperty m_behaviorList;

		bool canSynchronize;

		void OnEnable()
		{
			logo = (Texture)Resources.Load("uTire_Logo_286");

			m_templateBehavior = serializedObject.FindProperty("templateBehavior");
			m_behaviorList = serializedObject.FindProperty("uTireWorldSpace");
		}

		public override void OnInspectorGUI()
		{
			uTireGlobalGUI globalGUI = uTireGlobalGUI.Instance;
			uTireWorldSpaceSynchronizer t = (uTireWorldSpaceSynchronizer)target;

			globalGUI.drawLogo(logo);

			EditorGUILayout.HelpBox("Helper script to easily clone the settings from one object to another. The idea is to set one wheel up, put this on the root of your vehicle, assign the already set up wheel to the Template slot and it'll update every children to use the template data on Start().", MessageType.Info);

			EditorGUILayout.PropertyField(m_templateBehavior, new GUIContent("Template"));
			
			if(m_templateBehavior.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("Template field can't be empty", MessageType.Error);
			}
			else
			{
				EditorGUILayout.PropertyField(m_behaviorList, true);
				canSynchronize = t.uTireWorldSpace.Count != 0;
				if (!canSynchronize)
				{
					EditorGUILayout.HelpBox("If you leave the list empty, it'll look for uTireWorldSpaceBehaviour(s) in its children on Start", MessageType.Info);
				}
			}

			if(canSynchronize)
			{
				if (GUILayout.Button("Synchronize", globalGUI.hugeBtn))
				{
					t.synchronize();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		
	}
}