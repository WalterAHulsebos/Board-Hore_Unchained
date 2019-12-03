namespace TSD
{
	namespace uTireEditor
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;
		using UnityEditor;
		using System.Linq;
		using TSD.uTireEditor;
		using System;

		/// <summary>
		/// uTire integration installer window
		/// </summary>
		public class uTireInstallerWindow : EditorWindow
		{

			void OnEnable()
			{
				uTireInstallerWindow window = (uTireInstallerWindow)EditorWindow.GetWindow(typeof(uTireInstallerWindow));
				window.logo = (Texture)Resources.Load("uTire_Logo_text_bare");
				window.logoText = (Texture)Resources.Load("uTire_Logo_text_INTEGRATIONS");

				nameSpaceExists[0] = isNameSpaceExists("EVP");
				nameSpaceExists[1] = isNameSpaceExists("RCC_CarControllerV3");
				nameSpaceExists[2] = isNameSpaceExists("NWH.VehiclePhysics");
				nameSpaceExists[3] = isNameSpaceExists("NWH.WheelController3D");
			}

			bool isNameSpaceExists(string namepaceOrClass)
			{
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					foreach (var type in assembly.GetTypes())
					{
						if (type.Namespace == namepaceOrClass || type.Name == namepaceOrClass)
						{
							return true;
						}
					}
				}
				return false;
			}

			[MenuItem("Tools/TSD/Tire Deformation Integration Installer", false, 1)]
			public static void Init()
			{
				// Get existing open window or if none, make a new one:
				uTireInstallerWindow window = (uTireInstallerWindow)EditorWindow.GetWindow(typeof(uTireInstallerWindow));
				window.logo = (Texture)Resources.Load("uTire_Logo_text_bare");
				window.logoText = (Texture)Resources.Load("uTire_Logo_text_INTEGRATIONS");

				window.size.y = window.integrationsSetup.Length * 100;

				window.Show();
			}

			uTireGlobalGUI GUIInstance;
			Texture logo;
			Texture logoText;

			GUIContent[] integrationsSetup = new GUIContent[] 
			{	new GUIContent("Install EVP Integration", 
				"Will define the keywords required to use EVP integration."),
				new GUIContent("Install Realistic Car Controller v3 Integration", 
					"Will define the keywords required to use RCC integration."),
				new GUIContent("Install NWHVehiclePhysics Integration",
					"Will define the keywords required to use NWHVehiclePhysics integration. Also will automatically enable keywords required by the WheelController3D integration."),
				new GUIContent("Install Wheel Controller 3D Integration", 
					"Will define the keywords required to use WC3D integration."),
				new GUIContent("Install Light Weight Render Pipeline Shaders",
					"Will import the LWRP shaders and define the keywords required to use the LWRP."),
				new GUIContent("Install High Definition Render Pipeline Shaders",
					"Will import the HDRP shaders and define the keywords required to use the HDRP.")
					 };
			GUIContent[] integrationsRemove = new GUIContent[] 
			{ new GUIContent("Remove EVP Integration", ""),
				new GUIContent("Remove RCC v3 Integration", ""),
				new GUIContent("Remove NWHVehiclePhysics Integration", ""),
				new GUIContent("Remove WC3D Integration", ""),
				new GUIContent("Remove LWRP Integration", ""),
				new GUIContent("Remove HDRP Integration", "")
			};

			bool[] nameSpaceExists = new bool[4];

			Vector2 size = new Vector2(300, 306);
			void OnGUI()
			{
				minSize = size;
				maxSize = size;

				GUIInstance = uTireGlobalGUI.Instance;

				EditorGUILayout.Separator();
				GUIInstance.drawLogo(logo, logoText);

				EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
				//////////////////////////////////////////////
				EditorGUI.BeginDisabledGroup(!nameSpaceExists[0]);
				EditorGUILayout.Separator();
#if !TSD_INTEGRATION_EVP
				if (GUILayout.Button(integrationsSetup[0], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					registerKeywords("TSD_INTEGRATION_EVP");
				}
#else
				if (GUILayout.Button(integrationsRemove[0], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					removeKeyword("TSD_INTEGRATION_EVP");
				}
#endif
				EditorGUI.EndDisabledGroup();
				//////////////////////////////////////////////
				EditorGUI.BeginDisabledGroup(!nameSpaceExists[1]);
#if !TSD_INTEGRATION_RCC3
				if (GUILayout.Button(integrationsSetup[1], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					registerKeywords("TSD_INTEGRATION_RCC3");
				}
#else
				if (GUILayout.Button(integrationsRemove[1], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					removeKeyword("TSD_INTEGRATION_RCC3");
				}
#endif
				EditorGUI.EndDisabledGroup();
				//////////////////////////////////////////////
				EditorGUI.BeginDisabledGroup(!nameSpaceExists[2]);
#if !TSD_INTEGRATION_NWHVehiclePhysics
				if (GUILayout.Button(integrationsSetup[2], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					registerKeywords("TSD_INTEGRATION_NWHVehiclePhysics");
					registerKeywords("TSD_INTEGRATION_WC3D");
				}
#else
				if (GUILayout.Button(integrationsRemove[2], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					removeKeyword("TSD_INTEGRATION_NWHVehiclePhysics");
				}
#endif
				EditorGUI.EndDisabledGroup();
				//////////////////////////////////////////////
				EditorGUI.BeginDisabledGroup(!nameSpaceExists[3]);
#if !TSD_INTEGRATION_WC3D
				if (GUILayout.Button(integrationsSetup[3], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					registerKeywords("TSD_INTEGRATION_WC3D");
				}
#else
				if (GUILayout.Button(integrationsRemove[3], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					removeKeyword("TSD_INTEGRATION_WC3D");
				}
#endif
				EditorGUI.EndDisabledGroup();
				//////////////////////////////////////////////
#if !TSD_LWRP
				if (GUILayout.Button(integrationsSetup[4], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					registerKeywords("TSD_LWRP");
					installUnityPackage(UnityPackages.LWRP);
				}
#else
				if (GUILayout.Button(integrationsRemove[4], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					removeKeyword("TSD_LWRP");
				}
#endif
				//////////////////////////////////////////////
			//	EditorGUI.EndDisabledGroup();
				
#if !TSD_HDRP
				if (GUILayout.Button(integrationsSetup[5], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					registerKeywords("TSD_HDRP");
					installUnityPackage(UnityPackages.HDRP);
				}
#else
				if (GUILayout.Button(integrationsRemove[5], GUIInstance.biggerBtn, GUILayout.ExpandWidth(true)))
				{
					removeKeyword("TSD_HDRP");
				}
#endif
				//////////////////////////////////////////////
				EditorGUI.EndDisabledGroup(); //compile group

				if (nameSpaceExists[0] == false || nameSpaceExists[1] == false || nameSpaceExists[2] == false)
				{
					EditorGUILayout.HelpBox("Import EVP/Realistic Car Controller v3/WheelController3D first, then you can enable the integration(s)", MessageType.Info);
				}

				if (EditorApplication.isCompiling)
				{
					EditorGUILayout.HelpBox("Wait for scripts to compile", MessageType.Info);
				}

				GUIInstance.drawCloseBtn(this);

			}

			enum UnityPackages
			{
				LWRP,
				HDRP
			}

			void installUnityPackage(UnityPackages _package)
			{
				switch (_package)
				{
					case UnityPackages.LWRP:
						AssetDatabase.ImportPackage("Assets/2SD/uTire/Integrations/Packages/uTire_LWRP.unitypackage", true);
						break;
					case UnityPackages.HDRP:
						AssetDatabase.ImportPackage("Assets/2SD/uTire/Integrations/Packages/uTire_HDRP.unitypackage", true);
						break;
					default:
						break;
				}
			}

			void registerKeywords(string _symbol)
			{
				if (_symbol == "") { return; }
				string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
				List<string> allDefines = definesString.Split(';').ToList();
				if (allDefines.Contains(_symbol)) { return; }
				allDefines.Add(_symbol);

				PlayerSettings.SetScriptingDefineSymbolsForGroup(
					EditorUserBuildSettings.selectedBuildTargetGroup,
					string.Join(";", allDefines.ToArray()));
			}

			void removeKeyword(string _symbol)
			{
				if (_symbol == "") { return; }
				string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
				List<string> allDefines = definesString.Split(';').ToList();
				if (!allDefines.Contains(_symbol)) { return; }
				allDefines.Remove(_symbol);

				PlayerSettings.SetScriptingDefineSymbolsForGroup(
					EditorUserBuildSettings.selectedBuildTargetGroup,
					string.Join(";", allDefines.ToArray()));
			}
		}

	}
}