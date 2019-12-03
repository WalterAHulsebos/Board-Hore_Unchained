namespace TSD
{
	namespace uTireEditor
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;
		using UnityEditor;
		using uTireRuntime;

		/// <summary>
		/// Custom inspector for uTire Manager
		/// </summary>
		[CustomEditor(typeof(uTireManager))]
		public class uTireBehaviourEditor : Editor
		{
			HOEditorUndoManager undoManager;

			uTireGlobalGUI GUIInstance;
			Texture logo;

			uTireManager myTarget;

			void OnEnable()
			{
				GUIInstance = uTireGlobalGUI.Instance;
				logo = (Texture)Resources.Load("uTire_Logo_286");

				myTarget = (uTireManager)target;
				undoManager = new HOEditorUndoManager(myTarget, "uTireManager");
			}

			public override void OnInspectorGUI()
			{

				undoManager.CheckUndo(myTarget);

				GUIInstance.drawLogo(logo);
				
				GUILayout.BeginVertical();
				GUILayout.Space(16f);
				if (GUILayout.Button("Open Manager Window", GUIInstance.hugeBtn))
				{
					uTireManagerWindow.Init();
				}
				GUILayout.Space(16f);
				GUILayout.EndVertical();
				
				drawIndicators(myTarget);
				undoManager.CheckDirty();
			}


			void drawIndicators(uTireManager m_target)
			{
				GUILayout.BeginHorizontal();
				float fixedWidth = (EditorGUIUtility.currentViewWidth) * .5f - 12f;
				
				GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(fixedWidth), GUILayout.Height(GUIInstance.biggerBtn.fixedHeight) };
				if (GUILayout.Button("Dynamic Visuals", GUIInstance.ToggleStyle(m_target.updateMaterial), options)) { m_target.updateMaterial = !m_target.updateMaterial; }
				if (GUILayout.Button("Dynamic Wheel Colliders", GUIInstance.ToggleStyle(m_target.updateWheelColliderRadius), options)) { m_target.updateWheelColliderRadius = !m_target.updateWheelColliderRadius; }
				GUILayout.EndHorizontal();
			}
		}

	}
}
