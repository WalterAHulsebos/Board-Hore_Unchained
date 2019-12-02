using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using TSD.uTireSettings;

[CustomEditor(typeof(uTireGlobalSettings))]
public class uTireGlobalSettingsEditor : Editor 
{
	TSD.uTireEditor.uTireGlobalGUI GlobalGUI;

	public override void OnInspectorGUI()
	{
		GlobalGUI = TSD.uTireEditor.uTireGlobalGUI.Instance; 
		uTireGlobalSettings t = (uTireGlobalSettings)target;

		//EditorGUILayout.HelpBox("")

		GUILayout.Label("3D Collision Settings");
		GUILayout.Space(6f);
		EditorGUILayout.BeginHorizontal();
		t.phyicsLayermask = EditorGUILayout.MaskField(new GUIContent("Physics Interaction Layermask", "Raycast target layer(s), for performance reasons you should only include layers that you actually use for collision detection"), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(t.phyicsLayermask), InternalEditorUtility.layers);
		EditorGUILayout.EndHorizontal();

		t.animationSpeedOnCollision = EditorGUILayout.Slider( new GUIContent( "Animation speed On Collision", "Reaction speed to the environment when we hit something. 35 usually works well."),  t.animationSpeedOnCollision, 0f, 250f);
		t.animationSpeedOnNoCollision = EditorGUILayout.Slider(new GUIContent("Animation speed On No Collision", "Interpolation speed back to original state. 75 usually works well."), t.animationSpeedOnNoCollision, 0f, 250f);

		GUILayout.Space(16f);
		GUILayout.Label("Simple settings");
		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("KPH", GlobalGUI.ToggleStyle(t.GetMeasurement() == TSD.uTireRuntime.Measurement.KPH ? true : false, TSD.uTireEditor.uTireGlobalGUI.buttonSize.big))) { t.SetMeasurement(TSD.uTireRuntime.Measurement.KPH); }
		if (GUILayout.Button("MPH", GlobalGUI.ToggleStyle(t.GetMeasurement() == TSD.uTireRuntime.Measurement.MPH ? true : false, TSD.uTireEditor.uTireGlobalGUI.buttonSize.big))) { t.SetMeasurement(TSD.uTireRuntime.Measurement.MPH); }
		EditorGUILayout.EndHorizontal();

		
		GUILayout.Label(string.Format("Default sideways slide Min[{0} {2}] Max[{1} {2}]", t.GetSlideMinMax().min.ToString("0"), t.GetSlideMinMax().max.ToString("0"), t.GetMeasurement().ToString()));
		EditorGUILayout.MinMaxSlider(ref t.GetSlideMinMax().min, ref t.GetSlideMinMax().max, 0f, 300f);

		EditorUtility.SetDirty(t);
	}
}
