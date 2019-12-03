namespace TSD
{
	namespace uTireEditor
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;
		using UnityEditor;
		using System.Linq;
		using uTireRuntime;
		using uTireSettings;

		/// <summary>
		/// Provides access to the runtime manager and every editor
		/// </summary>
		public class uTireManagerWindow : EditorWindow
		{

			HOEditorUndoManager undoManager;

			void OnEnable()
			{
				uTireManagerWindow window = (uTireManagerWindow)EditorWindow.GetWindow(typeof(uTireManagerWindow));
				if (window.logo == null || window.logoText == null)
				{
					window.logo = (Texture)Resources.Load(logoResource);
					window.logoText = (Texture)Resources.Load(logoTextResource);
				}

				window.cacheTDReference();
				getVehicleData();

				undoManager = new HOEditorUndoManager(TDInstance, "uTireManager");
			}

			[MenuItem("Tools/TSD/uTire Manager")]
			public static void Init()
			{
				// Get existing open window or if none, make a new one:
				uTireManagerWindow window = (uTireManagerWindow)EditorWindow.GetWindow(typeof(uTireManagerWindow));
				window.Show();
			}

			static string logoResource = "uTire_Logo_text_bare";
			static string logoTextResource = "uTire_Logo_text_MANAGER";
			Texture logo;
			Texture logoText;

			uTireManager TDInstance;
			uTireGlobalGUI GUIInstance;

			bool vehiclesToggle = true;

			//////////////////////////////////////////

			Vector2 scrollPos;
			GUIContent[] contentVehicles;

			[SerializeField]
			public Vehicle selectedVehicle;

			void OnGUI()
			{
				
				GUIInstance = uTireGlobalGUI.Instance;
				///////////////////////////////////////////////////////
				//Cache, safe to comment out(as the same is done in the Init())
				//but this way we don't have to reopen the window every time we
				//change the code(what could be more fun than doing the exact
				//same thing over and over again at 3AM, amirite)
				cacheTDReference();
				///////////////////////////////////////////////////////

				///////////////////////////////////////////////////////
				//Logo
				GUIInstance.drawLogo(logo, logoText);
				///////////////////////////////////////////////////////

				///////////////////////////////////////////////////////
				//Integrations
				drawIntegrations();
				///////////////////////////////////////////////////////

				///////////////////////////////////////////////////////
				//Runtime Settings
				EditorGUILayout.Separator();
				drawSettings();
				EditorGUILayout.Separator();
				///////////////////////////////////////////////////////

				///////////////////////////////////////////////////////
				//Add vehicle Btn
				EditorGUILayout.Separator();
				drawAddVehicle();
				EditorGUILayout.Separator();
				///////////////////////////////////////////////////////

				undoManager.CheckUndo();
				///////////////////////////////////////////////////////
				//Show vehicles toggle(and list if toggle is ON)
				//if (GUILayout.Button("Show Vehicles", GUIInstance.ToggleStyle(vehiclesToggle), GUILayout.Height(32f))) { vehiclesToggle = !vehiclesToggle; }
				if (vehiclesToggle || true)
				{
					drawVehiclesList();
				}
				///////////////////////////////////////////////////////

				///////////////////////////////////////////////////////
				//Selected vehicle's modifiable stuff
				if (selectedVehicle != null && selectedVehicle.rigidBody != null)
				{
					EditorGUILayout.Separator();
					drawVehicle();
				}
				///////////////////////////////////////////////////////

				///////////////////////////////////////////////////////
				//Errors\messages(if there is any)
				EditorGUILayout.Separator();
				drawMessages();
				///////////////////////////////////////////////////////
				GUIInstance.drawCloseBtn(this);
				if (TDInstance != null) { undoManager.CheckDirty(); }
			}
			
			void drawAddVehicle()
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Add Vehicle", GUIInstance.biggerBtn))
				{
					uTireSetupDefault.Init();
					//	TireDeformationSetupWindowDefault window =(TireDeformationSetupWindowDefault) TireDeformationSetupWindowDefault.GetWindow(typeof(TireDeformationSetupWindowDefault));
				}
#if TSD_INTEGRATION_EVP
				if (GUILayout.Button("Add Vehicle(EVP)", GUIInstance.biggerBtn))
				{
					uTireSetupEVP.Init();
				}
#endif
#if TSD_INTEGRATION_RCC3
				if (GUILayout.Button("Add Vehicle(RCC v3)", GUIInstance.biggerBtn))
				{
					uTireSetupRCC.Init();
				}
#endif

#if TSD_INTEGRATION_NWHVehiclePhysics
				if (GUILayout.Button("Add Vehicle(NWH)", GUIInstance.biggerBtn))
				{
					uTireSetupNWHVehiclePhysics.Init();
				}
#endif
				EditorGUILayout.EndHorizontal();
			}

			void getVehicleData()
			{
				if (TDInstance.vehicles.Count > 0)
				{
					selectedVehicle = TDInstance.vehicles[0];
				}
				else
				{
					selectedVehicle = null;
				}
			}

			void drawVehiclesList()
			{
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUIInstance.styleEmpty, GUI.skin.verticalScrollbar, GUI.skin.window, GUILayout.MaxHeight(Mathf.Clamp((1 + TDInstance.vehicles.Count) * 30, 0, 300)));

				Vehicle vehicleToRemove = null;
				//make sure we delete the registered vehicle if the rigidbody is missing(as it was most likely removed on purpose)
				TDInstance.vehicles.RemoveAll(item => item.rigidBody.Equals(null) || item.rigidBody == null);
				foreach (var item in TDInstance.vehicles)
				{
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(item.rigidBody.name, GUIInstance.ToggleStyle(item == selectedVehicle ? true : false)))
					{
						selectedVehicle = item;
						lookUpAndStoreSelectedVehicleData();
					}
					if (GUILayout.Button("X", GUILayout.Width(Mathf.Max(Screen.width * 0.05f, 16)))) { vehicleToRemove = item; }
					EditorGUILayout.EndHorizontal();
				}

				//if we clicked on remove vehicle
				if (vehicleToRemove != null) { TDInstance.vehicles.Remove(vehicleToRemove); }

				EditorGUILayout.EndScrollView();

			}

			void drawVehicle()
			{
				EditorGUILayout.Separator();

				if (GUILayout.Button(new GUIContent("Edit Vehicle", "Change the WheelCollider/MeshRenderer/Wheel Radius"), GUIInstance.biggerBtn))
				{
					uTireSetupDefault.Init();
					uTireSetupDefault TDWindow = (uTireSetupDefault)uTireSetupDefault.GetWindow(typeof(uTireSetupDefault));
					TDWindow.loadValuesFromManager(selectedVehicle);
					TDWindow.checkIfVehicleIsRegistered();
					TSD.uTireIntegration.uTireDefault.SetActiveVehicle(selectedVehicle);
					
				}

				if (GUILayout.Button("Edit Material", GUIInstance.biggerBtn))
				{
					uTireMaterialWindow.Init();
					uTireMaterialWindow TDWindow = (uTireMaterialWindow)uTireMaterialWindow.GetWindow(typeof(uTireMaterialWindow));
					TDWindow.loadValuesFromManager(selectedVehicle.wheels[0].meshRenderer);
				}
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Save Settings", GUIInstance.ToggleStyle( showSettings == ShowSettings.save ? true : false, uTireGlobalGUI.buttonSize.big) ))
				{
					showSettings = showSettings != ShowSettings.save ? ShowSettings.save : ShowSettings.none;
				}
				if (GUILayout.Button("Load Settings", GUIInstance.ToggleStyle(showSettings == ShowSettings.load ? true : false, uTireGlobalGUI.buttonSize.big)))
				{
					showSettings = showSettings != ShowSettings.load ? ShowSettings.load : ShowSettings.none;
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Separator();

				switch (showSettings)
				{
					case ShowSettings.none:
						break;
					case ShowSettings.save:
						drawSaveSettings();
						break;
					case ShowSettings.load:
						drawLoadSettings();
						break;
					default:
						break;
				}

				EditorGUILayout.Separator();

				selectedVehicle.tirePressureMultiplier = EditorGUILayout.Slider("Tire Pressure Multiplier", selectedVehicle.tirePressureMultiplier, 0f, 2f);
				selectedVehicle.maxSteeringAngle = EditorGUILayout.Slider("Max Steering Angle", selectedVehicle.maxSteeringAngle, 0f, 90f);

				///////////////////////////////////////////////////////
				//Wheels
				drawWheels();

				EditorGUILayout.Separator();
				EditorGUILayout.HelpBox("When the vehicle's speed is between these values the tire will slide on the X axis. This is overriding the global default value which can be changed at Tools/TSD/uTire Global Settings, if you leave it at 0 the global value will be used. \nA good starting point is usually around 15-45% of the vehicle's maximum speed for min/max.", MessageType.Info);
				GUILayout.Label(string.Format("Sideways Slide Min[{0} {2}]Max[{1} {2}]", selectedVehicle.sidewaysSlideOverride.min.ToString("0"), selectedVehicle.sidewaysSlideOverride.max.ToString("0"), uTireSettings.uTireGlobalSettings.Instance.GetMeasurement().ToString()));
				EditorGUILayout.MinMaxSlider( "Sideways Slide", ref selectedVehicle.sidewaysSlideOverride.min, ref selectedVehicle.sidewaysSlideOverride.max, 0f, 300f); 
			}

			void drawWheels()
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < selectedVehicle.wheels.Length; i++)
				{
					selectedVehicle.wheels[i].tirePressure = EditorGUILayout.Slider(System.String.Format("Tire[{0}] Pressure", i), selectedVehicle.wheels[i].tirePressure, 0f, 1f);
				}
				EditorGUI.indentLevel--;
			}

			void drawSettings()
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Dynamic Visuals", GUIInstance.ToggleStyle(TDInstance.updateMaterial), GUILayout.Height(48f), GUILayout.Width(EditorGUIUtility.currentViewWidth * .5f))) { TDInstance.updateMaterial = !TDInstance.updateMaterial; }
				if (GUILayout.Button("Dynamic WheelColliders", GUIInstance.ToggleStyle(TDInstance.updateWheelColliderRadius), GUILayout.Height(48f))) { TDInstance.updateWheelColliderRadius = !TDInstance.updateWheelColliderRadius; }

				EditorGUILayout.EndHorizontal();
			}

			#region Save/load settings to file

			enum ShowSettings
			{
				none,
				save,
				load
			}
			ShowSettings showSettings = ShowSettings.none;
			Object settingsLoadPrefab;
			GameObject selectedObjectsPrefab;

			//changing when a new vehicle is selected from the list
			string saveFileName;
			string saveFilePrefix = "uTireSettings ";
			void drawSaveSettings()
			{
				string databasePath = AssetDatabase.GetAssetPath(uTireGlobalSettings.Instance);
				databasePath = databasePath.Substring(0, databasePath.LastIndexOf("/") + 1); //remove the database's filename
				string finalPath = databasePath + saveFilePrefix + saveFileName + ".asset";
				EditorGUILayout.HelpBox("You can save settings(tire pressure, deflated tire radius, maximum steering angle, etc - everything other than the references to the meshes/wheel colliders), since the file won't have references to GameObjects or WheelColliders you can use the same settings file for different vehicles", MessageType.Info);

				if (System.IO.File.Exists(finalPath))
				{
					EditorGUILayout.HelpBox("A file with the same name already exists, if you don't want to overwrite it choose a different name", MessageType.Warning);
				}

				EditorGUILayout.LabelField("File path", finalPath);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Filename");
				saveFileName = GUILayout.TextField(saveFileName);
				EditorGUILayout.EndHorizontal();

				bool canSaveFile = (saveFileName.Replace(" ", string.Empty)).Length > 0 ? true : false;
				if (!canSaveFile) { EditorGUILayout.HelpBox("Type in a name to save the settings", MessageType.Error); }
				if (selectedObjectsPrefab == null) { EditorGUILayout.HelpBox("The vehicle have to have a Prefab in order to save its settings(that's how you can access it at runtime)", MessageType.Error); }

				EditorGUI.BeginDisabledGroup(!canSaveFile || selectedObjectsPrefab == null);

				if(GUILayout.Button("Save"))
				{
					///////////////////////////////////////////////////
					//look up the necessary values needed
					///////////////////////////////////////////////////
					List<float> tirePressureList = new List<float>();
					foreach (var item in selectedVehicle.wheels)
					{
						tirePressureList.Add(item.tirePressure);
					}
					///////////////////////////////////////////////////

					saveSettingsToFile(new uTirePrefabSettings(selectedObjectsPrefab, selectedVehicle.maxSteeringAngle, selectedVehicle.tirePressureMultiplier, tirePressureList, selectedVehicle.flatTireRadiusMultiplier, selectedVehicle.sidewaysSlideOverride), finalPath);
					
					closeSettingsSaveLoad();
				}

				EditorGUI.EndDisabledGroup();
			}
			void drawLoadSettings()
			{
				EditorGUILayout.HelpBox("You can load settings(tire pressure, deflated tire radius, maximum steering angle, etc - everything other than the references to the meshes/wheel colliders)", MessageType.Info);
				settingsLoadPrefab = EditorGUILayout.ObjectField(settingsLoadPrefab, typeof(uTirePrefabSettings), false);

				EditorGUI.BeginDisabledGroup(settingsLoadPrefab == null);
				if (GUILayout.Button("Load"))
				{
					loadSettingsFromFile((uTirePrefabSettings)settingsLoadPrefab);
					closeSettingsSaveLoad();
				}
				EditorGUI.EndDisabledGroup();
			}
			/// <summary>
			/// For performance considerations we will cache the selected vehicle's Prefab(if there is one) 
			/// and also store its name so it can be used for the default name for the saved settings
			/// </summary>
			void lookUpAndStoreSelectedVehicleData()
			{
				saveFileName = selectedVehicle.rigidBody.gameObject.name;
				selectedObjectsPrefab = PrefabUtility.GetPrefabParent(selectedVehicle.rigidBody.gameObject) as GameObject;
			}

			void closeSettingsSaveLoad()
			{
				showSettings = ShowSettings.none;
			}

			void saveSettingsToFile(uTirePrefabSettings settingsObject, string path)
			{
				AssetDatabase.CreateAsset(settingsObject, path);
				AssetDatabase.SaveAssets();
				EditorUtility.FocusProjectWindow();
			}

			void loadSettingsFromFile(uTirePrefabSettings fileToLoadFrom)
			{
				selectedVehicle.SetVehicleSettings(fileToLoadFrom);
			}

			#endregion

			void drawMessages()
			{
			//	throw new System.NotImplementedException();
			}

			void drawIntegrations()
			{
				if (GUILayout.Button("Open Integrations Manager", GUIInstance.biggerBtn)) { uTireInstallerWindow.Init(); }
			}

			void cacheTDReference()
			{
				TDInstance = uTireManager.Instance;
			}
		}

	}
}