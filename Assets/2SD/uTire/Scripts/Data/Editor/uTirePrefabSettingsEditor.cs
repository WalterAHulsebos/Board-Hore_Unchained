using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TSD.uTireSettings;

[CustomEditor(typeof(uTirePrefabSettings))]
public class uTirePrefabSettingsEditor : Editor
{

	TSD.uTireEditor.uTireGlobalGUI GlobalGUI;

	[SerializeField]
	int wheelCount = 1;
	bool showFlatness;

	void OnEnable()
	{
		uTirePrefabSettings t = (uTirePrefabSettings)target;
		wheelCount = t.tirePressure.Count;
	}

	public override void OnInspectorGUI()
	{
		GlobalGUI = TSD.uTireEditor.uTireGlobalGUI.Instance; 
		uTirePrefabSettings t = (uTirePrefabSettings)target;

		EditorGUILayout.HelpBox("This prefab will be used to access the data, if you provided it to the uTire Manager when registering the vehicles it will read these values", MessageType.Info);
		if(t.Prefab == null)
		{
			EditorGUILayout.HelpBox("No Prefab was specified, you won't be able to access the data!", MessageType.Error);
		}
		t.Prefab = (GameObject)EditorGUILayout.ObjectField(t.Prefab, typeof(GameObject), false);
		resizeTireArray(t);

		if (GUILayout.Button(showFlatness ? "Hide Tire Pressure Settings" : "Show Tire Pressure Settings", GlobalGUI.ToggleStyle(showFlatness))) { showFlatness = !showFlatness; }
		if(showFlatness)
		{
			drawPressure(t);
		}

		EditorGUILayout.HelpBox("Wheel radius multiplier when the tire is completely deflated(or when compressed)", MessageType.Info);
		t.tireRadiusMultiplier = EditorGUILayout.Slider("Radius multiplier when flat", t.tireRadiusMultiplier, 0.1f, 1f);

		EditorGUILayout.HelpBox("How much can the Wheel rotate around its (local) axis?", MessageType.Info);
		t.maxSteeringAngle = EditorGUILayout.Slider("Maximum Steering Angle", t.maxSteeringAngle, 0f, 90f);


		EditorGUILayout.HelpBox("Sideways slide - the tire will slide along the X axis when the vehicle is moving above min speed. You can leave it at 0 if you want to use the global settings.", MessageType.Info);
		GUILayout.Label(string.Format("Speed until sideways slide will start Min[{0}] Max[{1}]", t.slideMinMaxOverride.min.ToString("0.00"), t.slideMinMaxOverride.max.ToString("0.00")));
		EditorGUILayout.MinMaxSlider(ref t.slideMinMaxOverride.min, ref t.slideMinMaxOverride.max, 0, 300);
	}

	void drawPressure(uTirePrefabSettings _target)
	{
		EditorGUILayout.Separator();
		wheelCount = Mathf.RoundToInt(EditorGUILayout.Slider("Wheel Count", wheelCount, 1, 18));

		EditorGUILayout.Separator();
		_target.tirePressureMultiplier = EditorGUILayout.Slider("Global tire pressure multiplier", _target.tirePressureMultiplier, 0f, 2f);
		EditorGUILayout.Separator();

		EditorGUI.indentLevel++;
		for (int i = 0; i < _target.tirePressure.Count; i++)
		{
			_target.tirePressure[i] = EditorGUILayout.Slider(string.Format("Tire[{0}]", i.ToString()), _target.tirePressure[i], 0f, 1f);
		}
		EditorGUI.indentLevel--;
	}

	void resizeTireArray(uTirePrefabSettings _target)
	{
		if (_target.tirePressure.Count != wheelCount)
		{
			if (_target.tirePressure.Count < wheelCount)
			{
				List<float> extendedList = new List<float>(wheelCount - _target.tirePressure.Count);
				for (int i = 0; i < wheelCount - _target.tirePressure.Count; i++)
				{
					extendedList.Add(1f);
				}
				_target.tirePressure.AddRange(extendedList);
			}
			else
			{
				_target.tirePressure.RemoveAt(_target.tirePressure.Count - 1);
			}
		}
	}
}
