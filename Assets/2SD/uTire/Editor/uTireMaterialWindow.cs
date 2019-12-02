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
		using TSD.uTireSettings;

		/// <summary>
		/// Material editor window
		/// </summary>
		public class uTireMaterialWindow : EditorWindow
		{
			[MenuItem("Tools/TSD/uTire Material Editor")]
			public static void Init()
			{
				// Get existing open window or if none, make a new one:
				uTireMaterialWindow window = (uTireMaterialWindow)EditorWindow.GetWindow(typeof(uTireMaterialWindow));
				window.logo = (Texture)Resources.Load("uTire_Logo_text_bare");
				window.logoText = (Texture)Resources.Load("uTire_Logo_text_MATERIAL");
				window.Show();

				window.GUIInstance = uTireGlobalGUI.Instance;
			}

			private HOEditorUndoManager undoManager;

			Texture logo;
			Texture logoText;
			uTireGlobalGUI GUIInstance;

			List<string> validShaderVariants;

			string shaderBaseFolder = "TSD/Tire Deformation/";
			string shaderCustomAngleFolder = "TSD/Tire Deformation/CustomAngle/";

			string shaderSRPLegacy	= "Legacy";
			string shaderSRPLWRP = "LWRP";
			string shaderSRPHDRP = "HDRP";

			string shaderTess = "Tessellation";
			string shaderStandard = "Standard";
			string shaderSimple = "Simple";
		//	string shaderDebug = " Debug";

			bool isMaterialUsingTessellation { get; set; }
			bool isMaterialUsingInstancing { get; set; }

			////////////////////////////////
			//Base textures
			[SerializeField]Object albedo;
			[SerializeField]Object metallicSmoothness;
			[SerializeField]Object normal;
			////////////////////////////////

			[SerializeField]
			Object GO;
			[SerializeField]
			Object mesh;
			[SerializeField]
			Object mat;
			[SerializeField]bool mat_debugToggle;
			[SerializeField]bool mat_debugDisplacementBottomToggle;
			[SerializeField]bool mat_debugDisplacementToggle;

			static string[] materialTabs = new string[] { "Base", "Mask", "Deformation", "Misc" };
			[SerializeField]int materialTabID;

			GUIContent[] shaderBaseButtonsGUI = new GUIContent[] { new GUIContent("Base", "Works without any extra step, unless you *really* need to make the displacement look right in extereme conditions(like hoops, or rough, uneven terrain) you can use these."), new GUIContent("3D World Collisions", "With these variants the tire will try to conform to the terrain. This is a lot heavier on both the CPU and the GPU.") };
			[SerializeField] DirectionType sShaderBase;
			[SerializeField] DirectionType lastShaderBaseSelectionID;

			GUIContent[] shaderSRPButtonsGUI = new GUIContent[] { new GUIContent("Legacy", "Legacy rendering"), new GUIContent("LWRP", "Light Weight Render Pipeline"), new GUIContent("HDRP", "High Definition Render Pipeline") };
			[SerializeField] SRPType sSelectedSRP;
			[SerializeField] SRPType lastSelectedSRP;

			GUIContent[] shaderButtonsGUI = new GUIContent[] { new GUIContent("Tessellation", "PBR, Displacement and Tessellation "), new GUIContent("Standard", "PBR and Displacement"), new GUIContent("Simple", "Diffuse, Displacement") };
			[SerializeField] ShaderType sShaderType;
			[SerializeField] ShaderType lastShaderSelectionID;

			string[] debugButtons = new string[] { "Top to bottom", "Deformation Area", "Both sides", "Tire wall" };
			[SerializeField]int selectedDebugID;

			#region variables

			//////////////////////////////////////////////////////////
			//Material
			//////////////////////////////////////////////////////////
			//Base
			[SerializeField]Vector2 material_tiling = Vector2.one;
			[SerializeField]Vector2 material_offset;

			[SerializeField]float metallicMultiplier;
			[SerializeField]float AOMultiplier;
			[SerializeField]float smoothnessMultiplier;



			//Masking\Deformation
			[SerializeField]Vector2 heightMinMax = new Vector2(0, 0);
			[SerializeField]Vector2 deformationMinMax = new Vector2(0, 0);
			[SerializeField]float deformationTopMultiplier = 0f;
			[SerializeField]Vector2 sideMinMax = new Vector2(0, 0);
			[SerializeField]Vector2 wallMinMax = new Vector2(0, 0);
			[SerializeField]float wallContrast = 1f; //just a multiplier to wallMinMax.y but it's basically a contrast so 

			[SerializeField]Vector3 bottomFlatten = Vector3.zero; //(x)min, (y)softness, (z)height

			[SerializeField]Vector3 tireA = new Vector3(0, 0, 0);
			[SerializeField]Vector3 tireB = new Vector3(0, 0, 0);
			[SerializeField]float tireT;

			[SerializeField]float turnT;

			//Misc
			//Posiiton offset
			[SerializeField]Vector3 positionOffset = Vector3.zero;
			//Tessellation(if used)
			[SerializeField]Vector2 tessellationValues = new Vector2(3f, 0.5f);


			//////////////////////////////////////////////////////////
			//mesh 
			//////////////////////////////////////////////////////////
			Vector3 meshSize;
			Vector3 meshExtents;

			#endregion

			bool showShaderSelector;
			bool debugShaderSet = false;

			#region Hints

			static string[] hints = new string[] 
	{
						"Make a mask from top to bottom" + "\n"
						+ "The whiter the gradient the stronger the effect will be. Usually a nice, even gradient works best.",

						"Mask out a circular area where you don't want any deformation "
						+ "This is used for masking out the middle part where the tire is connected to the rim. Usually a fairly sharp gradient works best.",

						"Mask the sides "
						+ "Usually it looks best if you match the sides closely(but leave some space, masking about 1/5th of the tire works fine in most cases)",

						"Mask the walls "
						+ "Make sure to check with pressure on ",

						"Set the bottom"
						+"Flatten the bottom",

						"During masking it might be a good idea to disable displacement(in the Deformation/Control tab)"
	
	};

			int hintID;
			bool showHints;


			List<string> warningMessages = new List<string>();
			static string[] wMessages = new string[] 
	{ 
		"Drag in a tire MeshRenderer",
		"Drag in a tire Material",
		"Drag in a GameObject to automatically get the MeshRenderer and Material(will only check for the first renderer/material, if you have submeshes/multiple materials you have to drag them in manually - in this case you can leave the GameObject field empty, it won't do anything either way.)",
		"Material not using any Tire Deformation variant"
	};


			bool errorWrongMaterial;
			#endregion
			/*
			enum selectedShaderBase
			{
				defaultBase,
				customAngle
			}

			enum selectedShaderType
			{
				tessellation,
				standard,
				simple
			}

			enum selectedSRP
			{
				legacy,
				lw,
				hd
			}*/

			Material material { get { return (Material)mat; } }

			public void changeSelection(GameObject selectedGameObject)
			{
				GO = selectedGameObject;
				OnValidate();
				if (checkIfEverythingIsSetupProperly())
				{
					revertToDefaultValues();
					checkIfShaderUsingTessellation();
					getMaterialValues();
					getMeshSize();
				}
			}

			void Awake()
			{
				//List<string> subFolders = new List<string>() { shaderBaseFolder, shaderCustomAngleFolder };
				List<string> SRPs = new List<string>() { shaderSRPLegacy, shaderSRPLWRP, shaderSRPHDRP };
				List<string> shadersToLookFor = new List<string> { shaderTess, shaderStandard, shaderSimple };
				//	List<string> SRPNamingConvention = new Lsit<string>() {  }
				validShaderVariants = new List<string>();
				foreach (var shaderName in shadersToLookFor)
				{
					{
						foreach (var subSRP in SRPs)
						{
							validShaderVariants.Add(shaderBaseFolder + subSRP + shaderName);
					//		validShaderVariants.Add(subFol + shaderName + " Debug");
					//		validShaderVariants.Add(subFol + shaderName + " LW");
					//		validShaderVariants.Add(subFol + shaderName + " HD");
						}
						
					}
				}
				/*
				foreach (var shader in validShaderVariants)
				{
					validShaderVariants.Add(shader + " Debug");
				}*/
			}

			void OnEnable()
			{
				undoManager = new HOEditorUndoManager(this, "MaterialWindow");
			}

			//Rename this to something that makes some sense
			void OnValidate()
			{
				if (GO != null)
				{
					if (mesh == null) { mesh = ((GameObject)GO).GetComponent<MeshRenderer>(); }
					if (mat == null) { mat = ((MeshRenderer)mesh).sharedMaterial; }
				}
			}
			void OnGUI()
			{
				undoManager.CheckUndo(this);
				GUIInstance = uTireGlobalGUI.Instance;

				GUIInstance.drawLogo(logo, logoText);

				EditorGUI.BeginChangeCheck();
				GUILayout.Space(5);

				drawInputFields();

				if (EditorGUI.EndChangeCheck())
				{
					OnValidate();
					if (checkIfEverythingIsSetupProperly())
					{
						revertToDefaultValues();
						checkIfShaderUsingTessellation();
						getMaterialValues();
						getMeshSize();
					}
				}

				EditorGUI.BeginChangeCheck();

				if (!checkIfEverythingIsSetupProperly())
				{
					drawWarnings();
					return;
				}

				drawMaterialButtons();

				EditorGUILayout.Space();
				EditorGUI.BeginChangeCheck();
				drawTabs();

				GUIInstance.drawCloseBtn(this);

				if(this != null)
				{
					undoManager.CheckDirty(this);
				}
			}

			void drawInputFields()
			{
				EditorGUILayout.BeginHorizontal();
				GO = EditorGUILayout.ObjectField(GO, typeof(GameObject), true);
				mat = EditorGUILayout.ObjectField(mat, typeof(Material), true);
				mesh = EditorGUILayout.ObjectField(mesh, typeof(MeshRenderer), true);
				if (GO != null || mat != null || mesh != null)
				{
					if (GUILayout.Button("Clear")) { GO = null; mat = null; mesh = null; return; }
				}
				EditorGUILayout.EndHorizontal();
			}

			void drawMaterialButtons()
			{
				//buttons to switch between shader types
				drawBackBox(showShaderSelector ? 7f : 4.75f, verticalOffset: -EditorGUIUtility.singleLineHeight * .5f);
				GUILayout.Label("Material Type");

				if (GUILayout.Button(showShaderSelector ? "Hide Shader Variants" : "Show Shader Variants", GUIInstance.ToggleStyle(showShaderSelector), GUILayout.Height(GUIInstance.biggerBtn.fixedHeight))) { showShaderSelector = !showShaderSelector; }

				if (showShaderSelector)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);

					EditorGUILayout.BeginVertical();

					sShaderBase = (DirectionType)GUILayout.SelectionGrid((int)sShaderBase, shaderBaseButtonsGUI, shaderBaseButtonsGUI.Length, GUIInstance.biggerBtn);
					//#if UNITY_2018_1_OR_NEWER
					//	sSelectedSRP = (SRPType)GUILayout.SelectionGrid((int)sSelectedSRP, shaderSRPButtonsGUI, shaderSRPButtonsGUI.Length, GUIInstance.biggerBtn);
					bool isLWRPImported = true;
					bool isHDRPImported = true;
#if !TSD_LWRP
					isLWRPImported = false;
#endif
#if !TSD_HDRP
					isHDRPImported = false;
#endif
					EditorGUILayout.BeginHorizontal();
					for (int i = 0; i < shaderSRPButtonsGUI.Length; i++)
					{
						EditorGUI.BeginDisabledGroup((i == 1 && !isLWRPImported) || (i == 2 && !isHDRPImported));
						if (GUILayout.Button(shaderSRPButtonsGUI[i], GUIInstance.ToggleStyle(sSelectedSRP == (SRPType)i, uTireGlobalGUI.buttonSize.big)))
						{
							sSelectedSRP = (SRPType)i;
						}
						EditorGUI.EndDisabledGroup();
					}
					EditorGUILayout.EndHorizontal();
						
					//#endif
					///////
					//	sShaderType = (ShaderType)GUILayout.SelectionGrid((int)sShaderType, shaderButtonsGUI, shaderButtonsGUI.Length, GUIInstance.biggerBtn);
					EditorGUILayout.BeginHorizontal();
					for (int i = 0; i < shaderButtonsGUI.Length; i++)
					{
						//yep, I woke up this morning and decided to do THIS, this very thing
						//if LWRP is selected disable the Tessellation btn, duh
						bool isTessellationSupportedOnSRP = true;
						if (sSelectedSRP == SRPType.LWRP || sSelectedSRP == SRPType.HDRP)
						{
							if (i == 0) { isTessellationSupportedOnSRP = false; }
						}
						EditorGUI.BeginDisabledGroup(!isTessellationSupportedOnSRP);
						if (GUILayout.Button(shaderButtonsGUI[i], GUIInstance.ToggleStyle(sShaderType == (ShaderType)i, uTireGlobalGUI.buttonSize.big)))
						{
							sShaderType = (ShaderType)i;
						}
						EditorGUI.EndDisabledGroup();
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.BeginDisabledGroup(isMaterialUsingTessellation);
					if (GUILayout.Button("Instancing", GUIInstance.ToggleStyle(isMaterialUsingInstancing), GUILayout.Height(GUIInstance.biggerBtn.fixedHeight)))
					{
						isMaterialUsingInstancing = !isMaterialUsingInstancing;
						setMaterialValues();
					}
					EditorGUI.EndDisabledGroup();

					if (GUILayout.Button(new GUIContent("Save settings as default", "The next material you make will use these settings")))
					{
						//save current settings as default 
						saveDefaultMaterialSettings();
					}
						
					EditorGUILayout.EndVertical();
					GUILayout.Space(16);
					GUILayout.EndHorizontal();
					
				//	GUILayout.EndArea();
				}

				if (sShaderType != lastShaderSelectionID || sShaderBase != lastShaderBaseSelectionID || sSelectedSRP != lastSelectedSRP) { changeShader(); }
				lastShaderSelectionID = sShaderType;
				lastShaderBaseSelectionID = sShaderBase;
				lastSelectedSRP = sSelectedSRP;
			}

			//currently only used for tessellation(as only Legacy supports it as of 1.2)
			void checkIfSRPSupportsSelectedShaderFeatures()
			{
				if(sSelectedSRP == SRPType.LWRP || sSelectedSRP == SRPType.HDRP)
				{
					if(sShaderType == ShaderType.tessellation)
					{
						sShaderType = ShaderType.standard;
					}
				}
			}

			/// <summary>
			/// Changes the shader type to sShaderType(variable)
			/// </summary>
			void changeShader()
			{
				string sShaderFolder = shaderBaseFolder + SelectedSRP;
				switch (sShaderBase)
				{
					case DirectionType.Base:
						((Material)mat).DisableKeyword("_3DCOLLISION_ON");
						break;
					case DirectionType.Collision3D:
						((Material)mat).EnableKeyword("_3DCOLLISION_ON");
						break;
					default:
						break;
				}
				
				
				switch (sShaderType)
				{
					case ShaderType.tessellation:
						((Material)mat).shader = Shader.Find(sShaderFolder + shaderTess);
						break;
					case ShaderType.standard:
						((Material)mat).shader = Shader.Find(sShaderFolder + shaderStandard);
						break;
					case ShaderType.simple:
						((Material)mat).shader = Shader.Find(sShaderFolder+ shaderSimple);
						break;
					default:
						break;
				}

				checkIfSRPSupportsSelectedShaderFeatures();

				checkIfShaderUsingTessellation();
				setMaterialValues();
			}

			#region Draw

			void drawTabs()
			{
				materialTabID = GUILayout.SelectionGrid(materialTabID, materialTabs, materialTabs.Length, GUIInstance.biggerBtn);
				if (checkIfEverythingIsSetupProperly())
				{
					switch (materialTabID)
					{
						case 0:
							drawBaseTab();
							break;
						case 1:
							drawMaskTab();
							//drawHints(); It was replaced by localized hints but later on it might be good for something
							break;
						case 2:
							drawDeformationDebug();
							drawDeformation();
							break;
						case 3:
							drawMisc();
							break;
						default:
							break;
					}
					if (EditorGUI.EndChangeCheck())
					{
						setBaseMaterialValues();
						setMaterialValues();
						
						enableDebugShader();
						disableDebugShader();
						setDebugValues();
					}
				}
				else
				{
					foreach (var item in warningMessages)
					{
						EditorGUILayout.HelpBox(item, MessageType.Warning);
					}
				}
			}
			
			void drawBaseTab()
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(1);

				EditorGUILayout.BeginVertical();
				if(!isShaderSupportsPBR)
				{
					EditorGUILayout.HelpBox("Selected shader doesn't support PBR", MessageType.Info);
				}

				albedo = EditorGUILayout.ObjectField("Albedo: ", albedo, typeof(Texture), false);

				EditorGUI.BeginDisabledGroup(!isShaderSupportsPBR);
				normal = EditorGUILayout.ObjectField("Normal: ", normal, typeof(Texture), false);
				EditorGUILayout.LabelField("Metallic(R), Smoothness(G), AO(R): ");
				metallicSmoothness = EditorGUILayout.ObjectField("", metallicSmoothness, typeof(Texture), false);
				EditorGUI.EndDisabledGroup();

				EditorGUILayout.LabelField("Tiling: ");
				if(material_tiling.x == 0 || material_tiling.y == 0)
				{
					EditorGUILayout.HelpBox("Tiling set to 0", MessageType.Warning);
				}
				material_tiling = EditorGUILayout.Vector2Field("", material_tiling);
				EditorGUILayout.LabelField("Offset: ");
				material_offset = EditorGUILayout.Vector2Field("", material_offset);

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();

				EditorGUI.BeginDisabledGroup(!isShaderSupportsPBR);
				metallicMultiplier = EditorGUILayout.Slider("Metallic Multiplier: ", metallicMultiplier, 0f, 1f);
				smoothnessMultiplier = EditorGUILayout.Slider("Smoothness Multiplier: ", smoothnessMultiplier, 0f, 1f);
				AOMultiplier = EditorGUILayout.Slider("Ambient Occlusion Multiplier: ", AOMultiplier, 0f, 1f);
				EditorGUI.EndDisabledGroup();
			}

			void drawMaskTab()
			{
				if (GUILayout.Button("Debug", GUIInstance.ToggleStyle(mat_debugToggle))) //mat_debugToggle ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
				{
					mat_debugToggle = !mat_debugToggle;
				}
				if (mat_debugToggle)
				{
					if (sShaderBase == DirectionType.Collision3D)
					{
						selectedDebugID = 1; //2 is the index of the distance check
					}
					else
					{
						selectedDebugID = GUILayout.SelectionGrid(selectedDebugID, debugButtons, 4);
					}
				}

				/*
				/////////////////////////////////////////////////////
				drawBackBox(8.5f, mat_debugToggle && selectedDebugID == 0);
				EditorGUILayout.Separator();
				EditorGUILayout.HelpBox(hints[0], MessageType.Info);
				EditorGUILayout.LabelField("Height Mask");
				/////////////////////////////////////////////////////
				//as of 03.12 the height is normalized, but in case later I'll need it 
				//heightMinMax.x = EditorGUILayout.Slider("Height: ", heightMinMax.x, 0f, getMeshMaxHeight());
				//[REMOVE] from final version if everything works as expected
				heightMinMax.x = EditorGUILayout.Slider("Height: ", heightMinMax.x, 0f, 1f);
				heightMinMax.y = EditorGUILayout.Slider("Smoothness: ", heightMinMax.y, 0f, 3f);
				*/
				/////////////////////////////////////////////////////
				drawBackBox(9.5f, mat_debugToggle && selectedDebugID == 1);
				EditorGUILayout.Separator();
				EditorGUILayout.HelpBox(hints[1], MessageType.Info);
				EditorGUILayout.LabelField("Deformation Area");
				//	GUILayout.Label("Lossy " + meshLossyScale.ToString());
				//	GUILayout.Label("Extents " + meshExtents.ToString());

				deformationMinMax.x = EditorGUILayout.Slider("Radius: ", deformationMinMax.x, 0f, meshNormalizedLossyScale.y * meshExtents.y);
				deformationMinMax.y = EditorGUILayout.Slider("Smoothness: ", deformationMinMax.y, 0.01f, 1f / meshExtents.y);
				deformationTopMultiplier = EditorGUILayout.Slider(new GUIContent("Top Modifier", "The top of the tire should use a 'smoother' radius"), deformationTopMultiplier, 0f, 10f);

				if (sShaderBase == DirectionType.Collision3D)
				{
					return;
				}
				/////////////////////////////////////////////////////
				drawBackBox(8.5f, mat_debugToggle && selectedDebugID == 2);
				EditorGUILayout.Separator();
				EditorGUILayout.HelpBox(hints[2], MessageType.Info);
				EditorGUILayout.LabelField("Side Mask");
				/////////////////////////////////////////////////////
				sideMinMax.x = EditorGUILayout.Slider("Radius: ", sideMinMax.x, 0f, meshNormalizedLossyScale.x * meshExtents.x);
				sideMinMax.y = EditorGUILayout.Slider("Smoothness: ", sideMinMax.y, 0.01f, meshNormalizedLossyScale.x * meshExtents.x);

				/////////////////////////////////////////////////////
				drawBackBox(9.5f, mat_debugToggle && selectedDebugID == 3);
				EditorGUILayout.Separator();
				EditorGUILayout.HelpBox(hints[3], MessageType.Info);
				EditorGUILayout.LabelField("Wall Mask");
				/////////////////////////////////////////////////////
				wallMinMax.x = EditorGUILayout.Slider("Radius: ", wallMinMax.x, 0f, meshNormalizedLossyScale.y * meshExtents.y);
				wallMinMax.y = EditorGUILayout.Slider("Smoothness: ", wallMinMax.y, 0.005f, 1);
				wallContrast = EditorGUILayout.Slider("Contrast: ", wallContrast, 1f, 10f);

				EditorGUILayout.Separator();
				/////////////////////////////////////////////////////

				/////////////////////////////////////////////////////
			}

			void drawDeformation()
			{

				/////////////////////////////////////////////////////
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
				drawBackBox(6.0f, verticalOffset: -7f);
				GUILayout.Label("Bottom Flatten");
				EditorGUILayout.BeginVertical(); //meshLossyScale
			//	GUILayout.Label(string.Format("Mesh Size : {0} , meshExtents : {1}", meshSize.y, meshExtents.y));
				//bottomFlatten.x = EditorGUILayout.Slider(GUIHelp("Bottom Min", "The highest point(in world space) which will be flatten"), bottomFlatten.x, meshSize.y, meshSize.y * 2f);
				bottomFlatten.z = EditorGUILayout.Slider(GUIHelp("Bottom Y Position", "The offset on the Y axis for everything below 'Bottom Min'"), bottomFlatten.z, -meshNormalizedLossyScale.y *.5f, meshNormalizedLossyScale.y * .5f);
				bottomFlatten.y = EditorGUILayout.Slider(GUIHelp("Transition adjustment", "Mask's smoothness between the flattened and normal area(look at the bottom of the tire)"), bottomFlatten.y, 1f, 1f +  ( meshNormalizedLossyScale.y  - Mathf.Abs(bottomFlatten.z)) * 2f);
				EditorGUILayout.EndVertical();
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();

				/////////////////////////////////////////////////////
				EditorGUILayout.Separator();

				drawBackBox(7.5f, verticalOffset: -14f);
				GUILayout.Label(new GUIContent("Flatness Base(x,y,z)", "We will interpolate between this and the 'Result' value"));
				EditorGUILayout.BeginHorizontal();
				tireA.x = EditorGUILayout.Slider(tireA.x, 0f, meshSize.x);
				tireA.y = EditorGUILayout.Slider(tireA.y, 0f, meshSize.y);
				tireA.z = EditorGUILayout.Slider(tireA.z, 0f, meshSize.z);
				EditorGUILayout.EndHorizontal();

				GUILayout.Label(new GUIContent("Flatness Result(x,y,z)", "We will interpolate between this and the 'Base' value"));
				EditorGUILayout.BeginHorizontal();
				tireB.x = EditorGUILayout.Slider(tireB.x, 0f, meshNormalizedLossyScale.x * meshExtents.x);
				tireB.y = EditorGUILayout.Slider(tireB.y, 0f, meshNormalizedLossyScale.y * meshExtents.y);
				tireB.z = EditorGUILayout.Slider(tireB.z, 0f, meshNormalizedLossyScale.z * meshExtents.z);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();


				/////////////////////////////////////////////////////
				EditorGUILayout.Separator();
				drawBackBox(7f, verticalOffset: -14f);
				GUILayout.Label("Controls");
				tireT = EditorGUILayout.Slider("Flatness ", tireT, 0f, 1f);
				EditorGUILayout.Separator();
				turnT = EditorGUILayout.Slider("Turn ", turnT, -1f, 1f);
			}

			void drawDeformationDebug()
			{
			//	drawBackBox(7.5f, verticalOffset: -14f);
				if (GUILayout.Button("Toggle Displacement", GUIInstance.ToggleStyle(mat_debugDisplacementToggle)))// mat_debugDisplacementToggle ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
				{
					mat_debugDisplacementToggle = !mat_debugDisplacementToggle;
				}
				if (!mat_debugDisplacementToggle)
				{
					if (GUILayout.Button("Toggle Bottom Displacement", GUIInstance.ToggleStyle(mat_debugDisplacementBottomToggle)))// mat_debugDisplacementBottomToggle ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
					{
						mat_debugDisplacementBottomToggle = !mat_debugDisplacementBottomToggle;
					}
				}
				if (mat_debugDisplacementToggle) { EditorGUILayout.HelpBox("Displacement turned off", MessageType.Error); }
				if (mat_debugDisplacementBottomToggle) { EditorGUILayout.HelpBox("Bottom Displacement turned off", MessageType.Error); }
			}

			void drawMaterialCreator()
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(GUIHelp("Create and assign a new Material", "The new material will be placed in the same folder as the original.")))
				{
					string path = AssetDatabase.GetAssetPath(mat).Substring(0, AssetDatabase.GetAssetPath(mat).LastIndexOf(".")) + "_TireDeformation.mat";
					AssetDatabase.CreateAsset(new Material(Shader.Find(validShaderVariants[0])), path);
					AssetDatabase.Refresh();
					mat = AssetDatabase.LoadAssetAtPath(path, typeof(Material));

					getDefaultMaterialSettings();
					changeShader();
					if (mesh != null)
					{
						MeshRenderer m_mesh = (MeshRenderer)mesh;

						//look for other wheels(they have to be registered first)
						Rigidbody rb = m_mesh.GetComponentInParent<Rigidbody>();
						if(rb!= null)
						{
							TSD.uTireRuntime.Vehicle m_vehicle = uTireRuntime.uTireManager.GetVehicle(rb);
							if(m_vehicle != null)
							{
								foreach (var wmc in m_vehicle.wheels)
								{
									wmc.meshRenderer.sharedMaterial = (Material)mat;
								}
							}
						}
						//in case the vehicle wasn't registered we will change only this tire's material
						m_mesh.sharedMaterial = (Material)mat;
					}
					checkIfShaderUsingTessellation();
				}
				if (GUILayout.Button("Change Material"))
				{
					Material m_mat = (Material)mat;
					m_mat.shader = Shader.Find(validShaderVariants[0]);
					checkIfShaderUsingTessellation();

					OnValidate();
					if (checkIfEverythingIsSetupProperly())
					{
						revertToDefaultValues();
						checkIfShaderUsingTessellation();
						getMaterialValues();
						getMeshSize();
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			
			/// <summary>
			/// Position offset and Tessellation
			/// </summary>
			void drawMisc()
			{
				/////////////////////////////////////////////////////
				//Position offset
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
				drawBackBox(7.5f, verticalOffset: -14f);
				GUILayout.Label("Position Offset");
				EditorGUILayout.BeginVertical();
				positionOffset.x = EditorGUILayout.Slider("X", positionOffset.x, -meshSize.x, meshSize.x);
				positionOffset.y = EditorGUILayout.Slider("Y", positionOffset.y, -meshSize.y, meshSize.y);
				positionOffset.z = EditorGUILayout.Slider("Z", positionOffset.z, -meshSize.z, meshSize.z);
				EditorGUILayout.EndVertical();
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();

				/////////////////////////////////////////////////////
				//if using tessellation
				if (isMaterialUsingTessellation)
				{
					EditorGUILayout.Separator();
					EditorGUILayout.Separator();
					drawBackBox(6.5f, verticalOffset: -14f);
					GUILayout.Label("Tessellation");
					EditorGUILayout.BeginVertical();
					tessellationValues.x = Mathf.Round(EditorGUILayout.Slider("Max Tessellation", tessellationValues.x, 1, 32));
					tessellationValues.y = EditorGUILayout.Slider("Phong Tessellation", tessellationValues.y, 0f, 1f);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Separator();
					EditorGUILayout.Separator();
				}

			}

			//It was replaced by localized hints but later on it might be good for something later
			void drawHints()
			{
				if (GUILayout.Button("Hints", GUIInstance.ToggleStyle(showHints)))
				{
					showHints = !showHints;
				}

				if (!showHints) { return; }

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("<", GUILayout.Height(120f))) { hintID = (hintID > 0) ? hintID - 1 : hints.Length - 1; }
				EditorGUILayout.HelpBox(hints[hintID] + "\n\n" + (hintID + 1).ToString() + "/" + hints.Length, MessageType.Info);
				if (GUILayout.Button(">", GUILayout.Height(120f))) { hintID = (hintID < hints.Length - 1) ? hintID + 1 : 0; }
				EditorGUILayout.EndHorizontal();

			}

			void drawWarnings()
			{
				foreach (var item in warningMessages)
				{
					EditorGUILayout.HelpBox(item, MessageType.Warning);
				}
				if (mat != null && errorWrongMaterial)
				{
					drawMaterialCreator();
				}
			}

			void drawBackBox(float multiplier, bool toggle = false, float verticalOffset = 0f)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				Rect outlineRect = new Rect(0f, lastRect.y + lastRect.height + EditorGUIUtility.singleLineHeight * .5f + verticalOffset, lastRect.width + 6f, EditorGUIUtility.singleLineHeight * multiplier - EditorGUIUtility.singleLineHeight * 2);

				if (toggle) { GUI.Box(outlineRect, "", GUIInstance.ToggleButtonStyleToggled); }// ToggleButtonStyleToggled); }
				else { GUI.Box(outlineRect, ""); }
			}

			#endregion

			void saveDefaultMaterialSettings()
			{
				uTireDefaultMaterialSettings.Instance.SetSettings(sSelectedSRP, sShaderBase, sShaderType, isMaterialUsingInstancing);
			}

			void getDefaultMaterialSettings()
			{
				uTireDefaultMaterialSettings settings = uTireDefaultMaterialSettings.Instance;
				sSelectedSRP = settings.sRPType;
				sShaderBase = settings.directionType;
				sShaderType = settings.shaderFeatures;
				isMaterialUsingInstancing = settings.instancing;
			}

			bool checkIfEverythingIsSetupProperly()
			{
				bool state = true;
				warningMessages.Clear();
				if (mat == null && mesh == null) { state = false; warningMessages.Add(wMessages[2]); }
				if (mat == null) { state = false; warningMessages.Add(wMessages[1]); }
				if (mesh == null) { state = false; warningMessages.Add(wMessages[0]); }

				if (mat != null)
				{
					state = false;
					foreach (var item in validShaderVariants)
					{
						if (item == ((Material)mat).shader.name)
						{
							state = true;
						}
					}
					if (!state) { warningMessages.Add(wMessages[3]); errorWrongMaterial = true; }
				}
				return state;
			}

			void setBaseMaterialValues()
			{
				Material m_material = (mat as Material);
				m_material.SetTexture("_Albedo", albedo as Texture);
				m_material.SetTexture("_Normal", normal as Texture);
				m_material.SetTexture("_Combinedrmetallicgsmoothnessbao", metallicSmoothness as Texture);

				m_material.SetFloat("_Smoothnessmultiplier", smoothnessMultiplier);
				m_material.SetFloat("_Metallicmultiplier", metallicMultiplier);
				m_material.SetFloat("_AmbientOcclusionMultiplier", AOMultiplier);

				setTextureScaleAndOffset(m_material, "_Albedo", material_tiling, material_offset);
				setTextureScaleAndOffset(m_material, "_Normal", material_tiling, material_offset);
				setTextureScaleAndOffset(m_material, "_Combinedrmetallicgsmoothnessbao", material_tiling, material_offset);
				/*
				m_material.SetTextureScale("_Albedo", material_tiling);
				m_material.SetTextureScale("_Normal", material_tiling);
				m_material.SetTextureScale("_Combinedrmetallicgsmoothnessbao", material_tiling);
				*/
			}

			void setTextureScaleAndOffset( Material _mat, string _textureName, Vector2 _scale, Vector2 _offset)
			{
				_mat.SetTextureScale(_textureName, _scale);
				_mat.SetTextureOffset(_textureName, _offset);
			}

			//if everything went well call home, we are donnneee
			void setMaterialValues()
			{
				Material m_material = (Material)mat;
				m_material.SetFloat("_MaskHeight", heightMinMax.x);
				m_material.SetFloat("_MaskSoftness", heightMinMax.y);
				m_material.SetVector("_DeformationAreaRadiusxSmoothnessy", new Vector2(deformationMinMax.x, deformationMinMax.x + deformationMinMax.y));
				m_material.SetFloat("_radiusTopMultiplier", deformationTopMultiplier);

				m_material.SetVector("_SideMaskMinMax", new Vector2(sideMinMax.x, sideMinMax.x + sideMinMax.y));

				m_material.SetFloat("_maskSideCenter", wallMinMax.x);
				m_material.SetFloat("_maskSideSoftness", wallMinMax.y);
				m_material.SetFloat("_maskSideContrast", wallContrast);

				m_material.SetVector("_TireFlatnessA", tireA);
				m_material.SetVector("_TireFlatnessB", tireB);
				m_material.SetFloat("_TireFlatnessT", tireT);

				m_material.SetFloat("_turn", turnT);
				m_material.SetFloat("_TurnWSXAmount", meshExtents.x * .25f * Mathf.InverseLerp(0f, meshNormalizedLossyScale.x * meshExtents.x, tireB.x ));

				if(sShaderBase == DirectionType.Base)
				{
					//(y)softness, (z)height
					m_material.SetFloat("_BottomMaskSoftness", bottomFlatten.y);
					m_material.SetFloat("_BottomYPosition", bottomFlatten.z);
				}

				//Misc
				m_material.SetVector("_positionoffset", positionOffset);

				if (isMaterialUsingTessellation)
				{
					m_material.SetFloat("_TessValue", tessellationValues.x);
					m_material.SetFloat("_TessPhongStrength", tessellationValues.y);
					m_material.enableInstancing = false; //just to make sure
				}
				else
				{
					m_material.enableInstancing = isMaterialUsingInstancing;
				}
			}

			void setDebugValues()
			{
				if (!debugShaderSet) { return; }
				Material m_material = (Material)mat;
				m_material.SetFloat("_ToggleMaskDebug", 0f);
				m_material.SetFloat("_ToggleMaskRadiusFromCenterDebug", 0f);
				m_material.SetFloat("_ToggleMaskBothSideDebug", 0f);
				m_material.SetFloat("_ToggleSwitchmiddleDebug", 0f);

				m_material.SetFloat("_ToggleMaskDebug", mat_debugToggle ? 1f : 0f);
				if (mat_debugToggle)
				{
					switch (selectedDebugID)
					{
						case 0:
							break;
						case 1:
							m_material.SetFloat("_ToggleMaskRadiusFromCenterDebug", 1f);
							break;
						case 2:
							m_material.SetFloat("_ToggleMaskBothSideDebug", 1f);
							break;
						case 3:
							m_material.SetFloat("_ToggleSwitchmiddleDebug", 1f);
							break;
						default:
							break;
					}
				}

				m_material.SetFloat("_ToggleSwitchdisplacement", convert(mat_debugDisplacementToggle));
				m_material.SetFloat("_ToggleBottomFlattenDisplacement", convert(mat_debugDisplacementBottomToggle));
			}

			void enableDebugShader()
			{
				if (debugShaderSet) { return; }
				if (mat_debugToggle || mat_debugDisplacementBottomToggle || mat_debugDisplacementToggle) //make sure we are actually in debug mode
				{
					/*
					string m_debugShader = ((Material)mat).shader.name + " Debug";
					if (Shader.Find(m_debugShader) == null)
					{
						Debug.LogError("Unable to find shader " + m_debugShader);
						return;
					}

					((Material)mat).shader = Shader.Find(m_debugShader);
					*/
					((Material)mat).EnableKeyword("_SHOWDEBUGMASKS_ON");
					debugShaderSet = true;
				}
			}

			void disableDebugShader()
			{
				if (debugShaderSet && !mat_debugToggle && !mat_debugDisplacementBottomToggle && !mat_debugDisplacementToggle)
				{
					((Material)mat).DisableKeyword("_SHOWDEBUGMASKS_ON");
				//	changeShader();
					debugShaderSet = false;
				}
			}

			/// <summary>
			/// Convert bool to float(true = 1, false = 0)
			/// </summary>
			/// <param name="state"></param>
			/// <returns></returns>
			float convert(bool state)
			{
				return state ? 1f : 0f;
			}
			/// <summary>
			/// Convert float to bool(1 = true, 0 = false)
			/// </summary>
			/// <param name="state"></param>
			/// <returns></returns>
			bool convert(float state)
			{
				return state == 1f ? true : false;
			}

			void getMaterialValues()
			{
				Material m_material = (Material)mat;
				SetSelectedShaderBase(m_material);


				albedo = m_material.GetTexture("_Albedo");
				metallicSmoothness = m_material.GetTexture("_Combinedrmetallicgsmoothnessbao");
				normal = m_material.GetTexture("_Normal");

				//Base
				metallicMultiplier = m_material.GetFloat("_Metallicmultiplier");
				AOMultiplier = m_material.GetFloat("_AmbientOcclusionMultiplier");
				smoothnessMultiplier = m_material.GetFloat("_Metallicmultiplier");

				material_tiling = m_material.GetTextureScale("_Albedo");
				material_offset = m_material.GetTextureOffset("_Albedo");

				positionOffset = m_material.GetVector("_positionoffset");

				//Masking\Deformation
				heightMinMax = new Vector2(m_material.GetFloat("_MaskHeight"), m_material.GetFloat("_MaskSoftness"));
				deformationMinMax = m_material.GetVector("_DeformationAreaRadiusxSmoothnessy");
				deformationMinMax.y -= deformationMinMax.x;
				deformationTopMultiplier = m_material.GetFloat("_radiusTopMultiplier");
				sideMinMax = m_material.GetVector("_SideMaskMinMax");
				sideMinMax.y -= sideMinMax.x;
				wallMinMax = new Vector2(m_material.GetFloat("_maskSideCenter"), m_material.GetFloat("_maskSideSoftness"));
				wallContrast = m_material.GetFloat("_maskSideContrast");

				//bottomFlatten = new Vector3(m_material.GetFloat("_BottomMaskMin"), m_material.GetFloat("_BottomMaskSoftness") - m_material.GetFloat("_BottomMaskMin"), m_material.GetFloat("_BottomYPosition"));
				if(sShaderBase == DirectionType.Base) //the 'custom angles' variant don't this softness setting
				{
					bottomFlatten = new Vector3(0f, m_material.GetFloat("_BottomMaskSoftness"), m_material.GetFloat("_BottomYPosition"));
				}
				else if(sShaderBase == DirectionType.Collision3D)
				{
					bottomFlatten = new Vector3(0f, 0f, m_material.GetFloat("_BottomYPosition"));
				}
				

				tireA = m_material.GetVector("_TireFlatnessA");
				tireB = m_material.GetVector("_TireFlatnessB");
				tireT = m_material.GetFloat("_TireFlatnessT");

				turnT = m_material.GetFloat("_turn");

				checkIfShaderUsingTessellation();
				if (isMaterialUsingTessellation)
				{
					tessellationValues = new Vector2(m_material.GetFloat("_TessValue"), m_material.GetFloat("_TessPhongStrength"));
				}

				isMaterialUsingInstancing = m_material.enableInstancing;
				//if it has the _ToggleMaskDebug variable it's already a Debug variant
				if (m_material.HasProperty("_ToggleMaskDebug"))
				{
					debugShaderSet = true;

					if(m_material.IsKeywordEnabled("_SHOWDEBUGMASKS_ON"))
					{
						mat_debugToggle = convert(m_material.GetFloat("_ToggleMaskDebug"));
					}
					else { mat_debugToggle = false; }
					

					if (convert(m_material.GetFloat("_ToggleMaskRadiusFromCenterDebug")))
						selectedDebugID = 1;
					else if (convert(m_material.GetFloat("_ToggleMaskBothSideDebug")))
						selectedDebugID = 2;
					else if (convert(m_material.GetFloat("_ToggleSwitchmiddleDebug")))
						selectedDebugID = 3;


					mat_debugDisplacementBottomToggle = convert(m_material.GetFloat("_ToggleBottomFlattenDisplacement"));
					mat_debugDisplacementToggle = convert(m_material.GetFloat("_ToggleSwitchdisplacement"));
				}

				if (isMaterialCertainVariant(m_material, shaderTess))
				{
					sShaderType = ShaderType.tessellation;
				}
				else if(isMaterialCertainVariant(m_material, shaderStandard))
				{
					sShaderType = ShaderType.standard;
				}
				else if(isMaterialCertainVariant(m_material, shaderSimple))
				{
					sShaderType = ShaderType.simple;
				}
			}

			bool isMaterialCertainVariant(Material m_material, string shaderName)
			{
				if(m_material.shader.name == shaderBaseFolder + SelectedSRP + shaderName) // || m_material.shader.name == shaderName + shaderDebug)
				{
					return true;
				}
				return false;
			}

			void revertToDefaultValues()
			{
				albedo = null;
				metallicSmoothness = null;
				normal = null;

				//Base
				metallicMultiplier = 1f;
				AOMultiplier = 1f;
				smoothnessMultiplier = 1f;

				material_tiling = new Vector2(1f, 1f);
				material_offset = new Vector2(0f, 0f);

				positionOffset = new Vector3(0, 0, 0);

				//Masking\Deformation
				heightMinMax = new Vector2(1, 0);
				deformationMinMax = new Vector2(0, 1);
				deformationTopMultiplier = 1f;
				sideMinMax = new Vector2(0, 0);
				wallMinMax = new Vector2(0, 0);
				wallContrast = 1f;

				bottomFlatten = new Vector3(0, 1, 0);

				tireA = new Vector3(0, 0, 0);
				tireB = new Vector3(1, 0, 0);
				tireT = 1f;

				turnT = 0f;

				if(debugShaderSet)
				{
					mat_debugDisplacementBottomToggle = true;
					mat_debugDisplacementToggle = true;
				}

				tessellationValues = new Vector2(3f, 0.5f);
			}

			public void loadValuesFromManager(MeshRenderer m_tireMesh)
			{
				//TODO 
				//Safety checks
				mesh = m_tireMesh;
				mat = m_tireMesh.sharedMaterial;

				revertToDefaultValues();
				if (checkIfEverythingIsSetupProperly())
				{
					getMaterialValues();
				}
				getMeshSize();
			}

			GUIContent GUIHelp(string label, string tooltip)
			{
				return new GUIContent(label, tooltip);
			}
			void getMeshSize()
			{
				MeshRenderer mr = (MeshRenderer)mesh;
				mr = Instantiate(mr, null);
				//The easiest way to get the local scale in world space(duh..) 
				//is to unparent it, measure it then parent it again
				//Also lossyScale will be accurate here as well 
			//	Transform oldParent = mr.transform.parent;
			//	mr.transform.parent = null;

			//	meshWSScale = mr.transform.localScale;
				meshLossyScale = mr.transform.lossyScale;

				meshSize = mr.bounds.extents * 2f;
				meshExtents = mr.bounds.extents;

				meshNormalizedLossyScale = new Vector3(1f / meshLossyScale.x, 1f / meshLossyScale.y, 1f / meshLossyScale.z);

				DestroyImmediate(mr.gameObject);
			//	mr.transform.parent = oldParent;
			}
			Vector3 meshNormalizedLossyScale;
			Vector3 meshLossyScale;
		//	Vector3 meshWSScale;
			float getMeshMaxHeight()
			{
				return ((MeshRenderer)mesh).bounds.extents.magnitude;
			}

			void checkIfShaderUsingTessellation()
			{
				isMaterialUsingTessellation = isMaterialCertainVariant((Material)mat, shaderTess);// || isMaterialCertainVariant((Material)mat, shaderCustomAngleFolder + shaderTess);
			//	isMaterialUsingTessellation = ((Material)mat).shader.name == shaderTess || ((Material)mat).shader.name == shaderTess + " Debug" ? true : false;
			}

			string SelectedSRP
			{
				get
				{
					switch (sSelectedSRP)
					{
						case SRPType.Legacy:
							return shaderSRPLegacy;
						case SRPType.LWRP:
							return shaderSRPLWRP;
						case SRPType.HDRP:
							return shaderSRPHDRP;
						default:
							return shaderSRPLegacy;
					}
				}
			}

			string SelectedShaderBase
			{
				get
				{
					if(sShaderBase == DirectionType.Base) { return shaderBaseFolder; }
					else if(sShaderBase == DirectionType.Collision3D) { return shaderCustomAngleFolder; }
					else { return null; }
				}
			}

			void SetSelectedShaderBase(Material m_material)
			{
				/*int index = m_material.shader.name.LastIndexOf("/");
				string input = "";
				if (index > 0)
				{
					input = m_material.shader.name.Substring(0, index + 1);
				}*/
				if (m_material.IsKeywordEnabled("_3DCOLLISION_ON")) { sShaderBase = DirectionType.Collision3D; }
				else { sShaderBase = DirectionType.Base; }

				//Obsolete as of 1.2
				/*
				if (input == shaderBaseFolder) { sShaderBase = selectedShaderBase.defaultBase; }
				else if (input == shaderCustomAngleFolder) { sShaderBase = selectedShaderBase.customAngle; }
				*/
			}

			bool isShaderSupportsPBR
			{
				get
				{
					return sShaderType == ShaderType.simple ? false : true;
				}
			}


		}
	}
}