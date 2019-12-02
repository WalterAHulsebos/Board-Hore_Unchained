using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TSD.uTireRuntime;
using TSD.uTireSettings;

namespace TSD.uTireEditor
{
	[CustomEditor(typeof(uTireWorldSpaceBehaviour)), CanEditMultipleObjects]
	public class uTireWorldSpaceCollisionEditor : Editor
	{

		internal static bool showDebugRays { get; private set; }

		Renderer _renderer;
		Renderer renderer { get
			{
				if(_renderer == null)
				{
					_renderer = myTarget.GetComponent<Renderer>();
				}
				return _renderer;
			}
		}

		Mesh _mesh;
		Mesh mesh { get
			{
				if(_mesh == null)
				{
					_mesh = myTarget.GetComponent<MeshFilter>().sharedMesh;
				}
				return _mesh;
			}
		}

		Material _material;
		Material material
		{
			get
			{
				if(_material == null)
				{
					_material = renderer.sharedMaterial;
				}
				return _material;
			}
		}

		uTireWorldSpaceBehaviour _myTarget;
		uTireWorldSpaceBehaviour myTarget
		{
			get
			{
				if (_myTarget == null)
				{
					_myTarget = (uTireWorldSpaceBehaviour)target;
					if (_myTarget.editorData == null)
					{
						_myTarget.editorData = new EditorData();
						_myTarget.editorData.Init(_myTarget.GetComponent<MeshRenderer>().bounds);
					}
				}
				return _myTarget;
			}
		}
		int textWidth = 32;

		string[] tabs = new string[]{ "Raycast", "Material", "Misc" };
		int selectedTab = 0;

		static bool sceneViewEditors = true;

	//	bool3 positionOffsetAxis = new bool3();
		bool modifierPressed;

		Texture logo;
		Texture logoText;
		public override void OnInspectorGUI()
		{
			uTireGlobalGUI.Instance.drawLogo(logo, logoText);
		}

		private void OnSceneGUI()
		{

			Event e = Event.current;
			switch (e.type)
			{
				case EventType.KeyDown:
					{
						if (Event.current.keyCode == (KeyCode.LeftControl))
						{
							modifierPressed = true;
						}
						break;
					}
				case EventType.KeyUp:
					{
						if (Event.current.keyCode == (KeyCode.LeftControl))
						{
							modifierPressed = false;
						}
						break;
					}
			}

			//	uTireWorldSpaceBehaviour myTarget = (uTireWorldSpaceBehaviour)target;
			Handles.BeginGUI();
			GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height - 75, 300, 25));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Toggle Debug View"))
				uTireWorldSpaceBehaviour.drawDebug = !uTireWorldSpaceBehaviour.drawDebug;
				SceneView.RepaintAll();
			if (GUILayout.Button("Toggle Scene View controls"))
				sceneViewEditors = !sceneViewEditors;
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			SceneView.RepaintAll();
			if (sceneViewEditors)
			{

				Rect controlsRect = new Rect(20, 20, Mathf.Max( Screen.width / 4, 250), 460);

				drawBackBox(controlsRect);
				GUILayout.BeginArea(controlsRect);

				selectedTab = GUILayout.Toolbar(selectedTab, tabs);
				switch (selectedTab)
				{
					case 0:
						tabRaySettings();
						break;
					case 1:
						tabMaterialSettings();
						break;
					case 2:
						tabMiscSettings();
						break;
					default:
						break;
				}

				GUILayout.EndArea();
			}

			displayErrors();
			Handles.EndGUI();

			if(GUI.changed)
			{
				SceneView.RepaintAll();
			}
		}

		string[] toolbarPositionOffset = new string[] { "X", "Y", "Z" };
		int selectionToolbarPositionOffset = 0;

		private void OnEnable()
		{
			selectionToolbarPositionOffset = getSelected(myTarget.offsetPos);
			logo = (Texture)Resources.Load("uTire_Logo_text_bare");
			logoText = (Texture)Resources.Load("uTire_Logo_text_3DCOLLISION");
		}

		void displayErrors()
		{
			if(uTireGlobalSettings.Instance.phyicsLayermask == 0)
			{
				Rect rect = new Rect(Screen.width / 2f - 100, 50, 200, 90);
				GUI.backgroundColor = Color.red;
				GUILayout.BeginArea(rect);
				GUILayout.Box("NO PHYSICS LAYER SELECTED");
				if(GUILayout.Button("Open GlobalSettings", uTireGlobalGUI.Instance.hugeBtn))
				{
					UnityEditor.Selection.activeObject = uTireSettings.uTireGlobalSettings.Instance;
				}
				GUILayout.EndArea();
			//	GUI.Label(new Rect(Screen.width / 2f, 50, 150, 30), "NO PHYSICS LAYER SELECTED");
			}
		}

		void tabRaySettings()
		{
			GUILayout.Label(new GUIContent("Ray Count", "How many rays do you need in each ring"));
			GUILayout.BeginHorizontal(); 
			myTarget.rayCountEditor = GUILayout.HorizontalSlider(myTarget.rayCountEditor, 1, 64);
			myTarget.rayCountEditor = EditorGUILayout.FloatField(myTarget.rayCountEditor, GUILayout.Width(textWidth));
			GUILayout.EndHorizontal();

			GUILayout.Label(new GUIContent("Ray Ring Count","How many raycast do you need horizontally - 3 is usually more than enough for proper 3d collision detection"));
			GUILayout.BeginHorizontal();
			myTarget.rayRowCountEditor = Mathf.Round(GUILayout.HorizontalSlider(myTarget.rayRowCountEditor, 1, 5));
			myTarget.rayRowCountEditor = Mathf.Round(EditorGUILayout.FloatField(myTarget.rayRowCountEditor, GUILayout.Width(textWidth)));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			
			GUILayout.Label( new GUIContent("Side axis", "Select the axis that's considered the side of your mesh(this is used to determine radius and ray length limits)"), GUILayout.Width(75) );
			EditorGUI.BeginChangeCheck();
			myTarget.editorData.selectedMeshSide = GUILayout.Toolbar(myTarget.editorData.selectedMeshSide, toolbarPositionOffset);
			
			GUILayout.EndHorizontal();
			float meshSideSize = getV3Component(myTarget.editorData.selectedMeshSide, mesh.bounds.extents);

			if (EditorGUI.EndChangeCheck())
			{
				myTarget.minRadius = (meshSideSize - myTarget.radius) * .5f;
			}


			GUILayout.Label( new GUIContent("Ray Length", "Set it to just a bit longer than the mesh itself. The script will look for collisions up to this distance."));
			GUILayout.BeginHorizontal();
			myTarget.rayLength = GUILayout.HorizontalSlider(myTarget.rayLength, 0f, meshSideSize);
			myTarget.rayLength = EditorGUILayout.FloatField(myTarget.rayLength, GUILayout.Width(textWidth));
			GUILayout.EndHorizontal();

			GUILayout.Label( new GUIContent("Radius", "Set this to just a bit bigger than the rim(otherwise it might poke through it)"));
			GUILayout.BeginHorizontal();
			myTarget.radius = GUILayout.HorizontalSlider(myTarget.radius, 0f, meshSideSize );
			myTarget.radius = EditorGUILayout.FloatField(myTarget.radius, GUILayout.Width(textWidth));
			GUILayout.EndHorizontal();

			GUILayout.Label(new GUIContent("Min Radius", "If the detected hit is closer than this the hit point will be clamped - so it won't be inside the mesh(if a hit ends up *in* the mesh then it will inflate the mesh isntead). Just a bit bigger than the center can be a good starting point."));
			GUILayout.BeginHorizontal();
			myTarget.minRadius = GUILayout.HorizontalSlider(myTarget.minRadius, (meshSideSize - myTarget.radius) * .5f, myTarget.rayLength);
			myTarget.minRadius = EditorGUILayout.FloatField(myTarget.minRadius, GUILayout.Width(textWidth));
			GUILayout.EndHorizontal();


			GUILayout.BeginHorizontal();
			GUILayout.Label(new GUIContent("Offset Axis", "Look at the tire from the front, if you use multiple rings this value will offset them"), GUILayout.Width(75));
			selectionToolbarPositionOffset = GUILayout.Toolbar(selectionToolbarPositionOffset, toolbarPositionOffset);
			GUILayout.EndHorizontal();
			Vector2 posOff = getData(selectionToolbarPositionOffset, myTarget.offsetPos, mesh.bounds.extents);

			GUILayout.Label(new GUIContent("Ring position offset", "When using multiple rings you can offset their position with this value horizontally."));
			GUILayout.BeginHorizontal();
		//	myTarget.offsetPos =  new Vector3(0, 0, GUILayout.HorizontalSlider(myTarget.offsetPos.z, 0, renderer.bounds.extents.z));
			myTarget.offsetPos = setV3(selectionToolbarPositionOffset, GUILayout.HorizontalSlider(posOff.x, 0, posOff.y));
		//	myTarget.offsetPos = setV3(selectionToolbarPositionOffset, EditorGUILayout.FloatField(posOff.x, GUILayout.Width(textWidth)));
			GUILayout.EndHorizontal();

			GUILayout.Label(new GUIContent("Rotation offset", "If you use multiple rings you can rotate them to better fit the sides - ideally it should be at a ~45 degrees on the sides, but depends on your particular mesh"));
			GUILayout.BeginHorizontal();
			myTarget.castDirectionMax = new Vector3(GUILayout.HorizontalSlider(myTarget.castDirectionMax.x, -1, 1), 1, 0);
			myTarget.castDirectionMax = new Vector3(EditorGUILayout.FloatField(myTarget.castDirectionMax.x, GUILayout.Width(textWidth)), 1, 0);
			GUILayout.EndHorizontal();


			GUILayout.Space(8);
			GUILayout.Label("Optimization");

			GUILayout.Label( new GUIContent("Angle","Angle range you want the rays to check" ));
			GUILayout.BeginHorizontal();
			myTarget.angle = GUILayout.HorizontalSlider(myTarget.angle, 0f, 360f);
			myTarget.angle = EditorGUILayout.FloatField(myTarget.angle, GUILayout.Width(textWidth));
			GUILayout.EndHorizontal();

			GUILayout.Label( new GUIContent("Angle Offset", "Set the direction of the cast rays"));
			GUILayout.BeginHorizontal();
			myTarget.offsetAngle = GUILayout.HorizontalSlider(myTarget.offsetAngle, 0f, 360f);
			myTarget.offsetAngle = EditorGUILayout.FloatField(myTarget.offsetAngle, GUILayout.Width(textWidth));
			//	GUILayout.Label(myTarget.offsetAngle.ToString(), GUILayout.Width(textWidth));
			GUILayout.EndHorizontal();

			bool groupActive = modifierPressed;
			EditorGUI.BeginDisabledGroup(!groupActive);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button( new GUIContent( "Set as Default", "Save settings as default. Hold down left control to enable the button.")))
			{
				saveRaySettingsToGlobalSettings();
			}
			if (GUILayout.Button(new GUIContent("Load from Default", "Load default settings. Hold down left control to enable the button.")))
			{
				loadGlobalSettings();
			}

			GUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
		}

		float getV3Component(int index, Vector3 data)
		{
			float result;
			switch (index)
			{
				case 0:
					result =data.x;
					break;
				case 1:
					result = data.y;
					break;
				case 2:
					result = data.z;
					break;
				default:
					result = -1f;
					break;
			}
			return result;
		}

		Vector3 setV3(int index, float data)
		{
			Vector3 result;
			switch (index)
			{
				case 0:
					result = new Vector3(data, 0, 0);
					break;
				case 1:
					result = new Vector3(0, data, 0);
					break;
				case 2:
					result = new Vector3(0, 0, data);
					break;
				default:
					result = Vector3.zero;
					break;
			}
			return result;
		}

		void tabMaterialSettings()
		{
			float meshSideSize = getV3Component(myTarget.editorData.selectedMeshSide, mesh.bounds.extents);

			if(GUILayout.Button("Open Material Editor", uTireGlobalGUI.Instance.biggerBtn))
			{
				uTireMaterialWindow.Init();
				uTireMaterialWindow window = (uTireMaterialWindow)EditorWindow.GetWindow(typeof(uTireMaterialWindow));
				window.changeSelection(myTarget.gameObject);
			}

			EditorGUILayout.HelpBox("These settings are raycastHit count dependent - meaning if you change the amount of rays you'll have to modify these settings to get similar results", MessageType.Info);

			GUILayout.Label( new GUIContent("Strength", "How much should a single hit affect each vertex"));
			GUILayout.BeginHorizontal();
			myTarget.matStrength = GUILayout.HorizontalSlider(myTarget.matStrength, 0, 1);
			myTarget.matStrength = EditorGUILayout.FloatField(myTarget.matStrength, GUILayout.Width(textWidth));
		//	material.SetFloat("_distanceCheckStrength", GUILayout.HorizontalSlider(material.GetFloat("_distanceCheckStrength"), 0, 1));
		//	material.SetFloat("_distanceCheckStrength", EditorGUILayout.FloatField(material.GetFloat("_distanceCheckStrength"), GUILayout.Width(textWidth)));
			GUILayout.EndHorizontal();

			GUILayout.Label(new GUIContent("Distance diminish modifier", "How fast should each hit lose strength - think of it as offset"));
			GUILayout.BeginHorizontal();
			myTarget.matDistance = GUILayout.HorizontalSlider(myTarget.matDistance, 0, meshSideSize * 16f);
			myTarget.matDistance = EditorGUILayout.FloatField(myTarget.matDistance, GUILayout.Width(textWidth));
		//	material.SetFloat("_distance", GUILayout.HorizontalSlider(material.GetFloat("_distance"), 0, meshSideSize * 16f));
		//	material.SetFloat("_distance", EditorGUILayout.FloatField(material.GetFloat("_distance"), GUILayout.Width(textWidth)));
			GUILayout.EndHorizontal();

			GUILayout.Label(new GUIContent("Strength mask contrast", "Contrast amount - I mean, I'm trying here but what description should I give to this?"));
			GUILayout.BeginHorizontal();
			material.SetFloat("_powExp", GUILayout.HorizontalSlider(material.GetFloat("_powExp"), 0.01f, 1f));
			material.SetFloat("_powExp", EditorGUILayout.FloatField(material.GetFloat("_powExp"), GUILayout.Width(textWidth)));
			GUILayout.EndHorizontal();

			if(material.GetVector("_distortionLimits").magnitude == 0)
			{
				EditorGUILayout.HelpBox("Distortion limit set to (0,0,0,), no distortion will be applied", MessageType.Warning);
			}
			GUILayout.Label(new GUIContent("Distortion limit", "Limits the amount of overall distortion in each axis."));
			GUILayout.BeginHorizontal();
			Vector3 distortionLimit = material.GetVector("_distortionLimits");
			distortionLimit = new Vector3(EditorGUILayout.FloatField(distortionLimit.x), EditorGUILayout.FloatField(distortionLimit.y), EditorGUILayout.FloatField(distortionLimit.z));
			material.SetVector("_distortionLimits", distortionLimit);
			GUILayout.EndHorizontal();

			GUILayout.Label(new GUIContent("Fake distortion multiplier", "Usually(0,0,0) is fine, but this value(masked by the actual hits) is added on top of the deformation, this can help achieve those sexy side bulges\n ( ͡° ͜ʖ ͡°)"));
			GUILayout.BeginHorizontal();
			Vector3 fakeMultiplier = material.GetVector("_3dCollisionFakeMultiplier");
			fakeMultiplier = new Vector3( GUILayout.HorizontalSlider(fakeMultiplier.x, 0f, mesh.bounds.size.x), GUILayout.HorizontalSlider(fakeMultiplier.y, 0f, mesh.bounds.size.y), GUILayout.HorizontalSlider(fakeMultiplier.z, 0f, mesh.bounds.size.z));
			material.SetVector("_3dCollisionFakeMultiplier", fakeMultiplier);
			GUILayout.EndHorizontal();
		}

		void tabMiscSettings()
		{
			GUILayout.Label(new GUIContent("Center offset", "When the wheel mesh isn't centralized you can add an offset here. Make sur the origin as at the pivot of the wheel."));
			GUILayout.BeginHorizontal();
			Vector3 min = mesh.bounds.center - mesh.bounds.extents;
			Vector3 max = mesh.bounds.extents + mesh.bounds.center;
			myTarget.originOffset = new Vector3(
				EditorGUILayout.Slider(myTarget.originOffset.x, min.x, max.x), 
				EditorGUILayout.Slider(myTarget.originOffset.y, min.y, max.y), 
				EditorGUILayout.Slider(myTarget.originOffset.z, min.z, max.z));
			GUILayout.EndHorizontal();

			GUILayout.Label("Align to center");
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("X")) { myTarget.originOffset.x = Mathf.Lerp(min.x, max.x, .5f); }
			if (GUILayout.Button("Y")) { myTarget.originOffset.y = Mathf.Lerp(min.y, max.y, .5f); }
			if (GUILayout.Button("Z")) { myTarget.originOffset.z = Mathf.Lerp(min.z, max.z, .5f); }
			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();
			if(GUILayout.Button(new GUIContent("Open global settings"), uTireGlobalGUI.Instance.biggerBtn))
			{
				Selection.activeObject = uTireGlobalSettings.Instance;
			}
			
		}

		void saveRaySettingsToGlobalSettings()
		{
			uTireGlobalSettings.Instance.rayCount		= myTarget.rayCount;
			uTireGlobalSettings.Instance.rayRingCount	= myTarget.rayRowCount;
			uTireGlobalSettings.Instance.rayAngle		= myTarget.angle;
			uTireGlobalSettings.Instance.rayAngleOffset = myTarget.offsetAngle;
		}

		void loadGlobalSettings()
		{
			myTarget.rayCount		= uTireGlobalSettings.Instance.rayCount;
			myTarget.rayRowCount	= uTireGlobalSettings.Instance.rayRingCount;
			myTarget.angle			= uTireGlobalSettings.Instance.rayAngle;
			myTarget.offsetAngle	= uTireGlobalSettings.Instance.rayAngleOffset;
		}

		void drawBackBox(Rect rect, bool toggle = true, float verticalOffset = 0f)
		{
			Rect lastRect = rect;
			Rect outlineRect = new Rect(rect.x - 5, lastRect.y - 5, lastRect.width +10, lastRect.height + 10);

			if (toggle) { GUI.Box(outlineRect, "", uTireGlobalGUI.Instance.ToggleButtonStyleToggled); }// ToggleButtonStyleToggled); }
			else { GUI.Box(outlineRect, ""); }
		}

		//might want to rename this 
		Vector2 getData(int _selection, Vector3 firstDataSet, Vector3 secondDataSet)
		{
			Vector2 result;
			switch (_selection)
			{
				case 0:
					result = new Vector2(firstDataSet.x, secondDataSet.x);
					break;
				case 1:
					result = new Vector2(firstDataSet.y, secondDataSet.y);
					break;
				case 2:
					result = new Vector2(firstDataSet.z, secondDataSet.z);
					break;
				default:
					result = Vector2.zero;
					break;
			}
			return result;
		}

		int getSelected(Vector3 dataSet)
		{
			int result;
			if(dataSet.magnitude == 0)
			{
				var arr = new float[] { mesh.bounds.extents.x, mesh.bounds.extents.y, mesh.bounds.extents.z };
				result = arr.Select((axis, index) => new { axis, index }).First(element => element.axis == Mathf.Min(arr)).index;
			}
			else
			{
				if (dataSet.x != 0) { result = 0; }
				else if (dataSet.y != 0) { result = 1; }
				else { result = 2; }
			}
			return result;
		}

		class bool3
		{
			public bool x
			{
				get;private set;
			}
			public bool y
			{
				get; private set;

			}
			bool _z;
			public bool z
			{
				get; private set;
			}

			public void setX(bool state)
			{
				x = state;
				y = !x; z = !x;
			}
			public void setY(bool state)
			{
				y = state;
				x = !y; z = !y;
			}

			public void setZ(bool state)
			{
				z = state;
				x = !z; y = !z;
			}
		}
	}
}