namespace TSD
{
	namespace uTireEditor
	{
		using UnityEngine;
		using UnityEditor;
		using TSD.uTireEditor;
		using TSD.uTireRuntime;

		public class uTireSetupWindowBase : EditorWindow
		{

			enum WheelTypes
			{
				WheelCollider,
				WheelController3D
			}

			HOEditorUndoManager undoManager;

			string labelPrefix = "Tire Deformation - ";
			public string myLabel = "";
			public Texture logo;
			public Texture logoText;
			public Rigidbody m_vehicleRigidbody;
			public float m_flatTireRadius = 0.85f;
			public float m_maximumSteeringAngle = 40f;
			Vector2 size = new Vector2(300, 500);

			string[] wheelTypes = new string[] { "WheelCollider", "Wheel Controller 3D" };
			WheelTypes selectedWheelType = WheelTypes.WheelCollider;

			public Object[] wheelColliders;
			public Object[] wheelMeshes;
			public GUIContent[] wheelContent; // the above combined into one
			public int wheelCount = 4;

		//	int selectedID = -1;
			IWheel selectedWC;
			MeshRenderer selectedWheelMesh;

			uTireGlobalGUI GUIInstance;

			public bool isVehicleRegistered = false; //true if we are updating an existing vehicle, false if it's a new one

			public virtual void OnEnable()
			{
				GUIInstance = uTireGlobalGUI.Instance;
				selectedWC = null;
				checkIfVehicleIsRegistered();

				undoManager = new HOEditorUndoManager(this, "uTireSetupWindow");
			}

			void OnGUI()
			{
				undoManager.CheckUndo();
				minSize = size;
				maxSize = size;

				GUILayout.Label(labelPrefix + myLabel, EditorStyles.boldLabel);

				GUIInstance.drawLogo(logoText == null ? logo : logo, logoText);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();

				EditorGUILayout.HelpBox("Drag in a rigidbody with a vehicle component attached to it", MessageType.Info);

				EditorGUI.BeginChangeCheck();

				m_vehicleRigidbody = (Rigidbody)EditorGUILayout.ObjectField(m_vehicleRigidbody, typeof(Rigidbody), true);
				if (EditorGUI.EndChangeCheck()) { OnRigidbodyAdded(); }

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();

				EditorGUI.BeginDisabledGroup((m_vehicleRigidbody == null) ? true : false);
				if (GUILayout.Button(isVehicleRegistered ? "Update Vehicle" : "Register Vehicle"))
				{
					customize();
				}
				EditorGUI.BeginChangeCheck();
				m_flatTireRadius = EditorGUILayout.Slider("Flat Radius Multiplier", m_flatTireRadius, 0.1f, 1f);
				if (EditorGUI.EndChangeCheck()) { OnFlatRadiusSliderChanged(); }

				EditorGUILayout.HelpBox("When the dynamic WheelCollider radius feature is enabled the WC will be scaled down to this value(based on compression)", MessageType.Info);

				drawWheelTypes();
			//	selectedWheelType = (WheelTypes)GUILayout.SelectionGrid((int)selectedWheelType, wheelTypes, 2, GUIInstance.biggerBtn);

				drawWheels();

				customizeGUI();

				EditorGUI.EndDisabledGroup();

				GUIInstance.drawCloseBtn(this);
				//	closeBtn();

				if (this != null) { undoManager.CheckDirty(); }
			}

			void drawWheelTypes()
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(wheelTypes[0], GUIInstance.ToggleStyle(selectedWheelType == WheelTypes.WheelCollider ? true : false, uTireGlobalGUI.buttonSize.big))) { selectedWheelType = WheelTypes.WheelCollider; }
#if !TSD_INTEGRATION_WC3D
				EditorGUI.BeginDisabledGroup(true);
#endif
				if (GUILayout.Button(wheelTypes[1], GUIInstance.ToggleStyle(selectedWheelType == WheelTypes.WheelController3D ? true : false, uTireGlobalGUI.buttonSize.big))) { selectedWheelType = WheelTypes.WheelController3D; }
#if !TSD_INTEGRATION_WC3D
				EditorGUI.EndDisabledGroup();
#endif
				EditorGUILayout.EndHorizontal();

			}

			void drawWheels()
			{

				EditorGUI.BeginChangeCheck();
				wheelCount = Mathf.Max(EditorGUILayout.IntField("Wheel Count", wheelCount), 1);
				if (EditorGUI.EndChangeCheck())
				{
					wheelMeshes = new Object[wheelCount];
					wheelColliders = new Object[wheelCount];
				}
			//	IWheel selectedIWheel = null;
				WheelMeshConnection selectedWMC = null;
				for (int i = 0; i < wheelMeshes.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					wheelColliders[i] = EditorGUILayout.ObjectField(wheelColliders[i], getSelectedWheelType(), true);
					wheelMeshes[i] = EditorGUILayout.ObjectField(wheelMeshes[i], typeof(MeshRenderer), true);

					EditorGUI.BeginDisabledGroup(wheelColliders[i] == null || wheelMeshes[i] == null || uTireManager.GetVehicle(m_vehicleRigidbody) == null);
					if (GUILayout.Button("Select"))
					{
						//to avoid null reference exception we'll cache the iWheel reference and focus 
						//outside of the for loop
						selectedWMC = uTireManager.GetVehicle(m_vehicleRigidbody).wheels[i];
					//	selectedIWheel = uTireManager.GetVehicle(m_vehicleRigidbody).wheels[i].iWheel;
		//				selectedWheelMesh = uTireManager.GetVehicle(m_vehicleRigidbody).wheels[i].meshRenderer;
					}
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
				}
				if (selectedWMC != null) 
				{ 
					changeSelection(selectedWMC.iWheel); 
					selectedWheelMesh = selectedWMC.meshRenderer; 
					selectedWMC = null; 
				}

			}

			System.Type getSelectedWheelType()
			{
				switch (selectedWheelType)
				{
					case WheelTypes.WheelCollider:
						return typeof(WheelCollider);
#if TSD_INTEGRATION_WC3D
					case WheelTypes.WheelController3D:
						return typeof(NWH.WheelController3D.WheelController);
#endif
					default:
						return typeof(WheelCollider);
				}
			}

			void changeSelection(IWheel objectToFocus)
			{
				selectedWC = objectToFocus;
				Selection.SetActiveObjectWithContext(selectedWC.wheelTransform, null);
				SceneView.FrameLastActiveSceneView();
			}

			public void loadValuesFromManager(Vehicle m_vehicle)
			{
				m_vehicleRigidbody = m_vehicle.rigidBody;
				wheelCount = m_vehicle.wheels.Length;

				wheelMeshes = new Object[wheelCount];
				wheelColliders = new Object[wheelCount];

				for (int i = 0; i < wheelCount; i++)
				{
					wheelMeshes[i] = m_vehicle.wheels[i].meshRenderer;
					if (m_vehicle.wheels[i].GetWheelObject().GetType() == typeof(WheelCollider)) 
					{ 
						wheelColliders[i] = (WheelCollider)m_vehicle.wheels[i].GetWheelObject();
						selectedWheelType = WheelTypes.WheelCollider;
					}
#if TSD_INTEGRATION_WC3D
					if (m_vehicle.wheels[i].GetWheelObject().GetType() == typeof(NWH.WheelController3D.WheelController)) 
					{ 
					wheelColliders[i] = (NWH.WheelController3D.WheelController)m_vehicle.wheels[i].GetWheelObject(); 
					selectedWheelType = WheelTypes.WheelController3D;
					}
#endif
					//	wheelColliders[i] = (WheelCollider)m_vehicle.wheels[i].iWheel.wheelObject;
				}
				m_vehicle.InitWheels();
				m_flatTireRadius = m_vehicle.flatTireRadiusMultiplier;

				//update the selected WheelCollider(so the gizmo in sceneView will be in the right place)
				if (wheelColliders.Length > 0) { selectedWC = m_vehicle.wheels[0].iWheel; }
				//if (wheelColliders.Length > 0) { selectedWC = wheelColliders[0]; }
			}

			void Awake()
			{
				initArrays();
				checkIfVehicleIsRegistered();
			}

			void initArrays()
			{
				wheelMeshes = new Object[wheelCount];
				wheelColliders = new Object[wheelCount];
			}

			/// <summary>
			/// Resets the arrays(WheelCollider, MeshRenderer)
			/// </summary>
			public void resetArrays()
			{
				wheelCount = 4;
				initArrays();
			}

			/// <summary>
			/// Called when "Register/Update vehicle" button is pressed.
			/// Call base.customize() AFTER your stuff so it'll be able to switch over to 
			/// "update vehicle" mode instead of "register vehicle" mode on the first run
			/// </summary>
			public virtual void customize()
			{
				checkIfVehicleIsRegistered();
				if (!isVehicleRegistered) { return; }
				loadValuesFromManager(uTireManager.GetVehicle(m_vehicleRigidbody));
			}

			public bool checkIfVehicleIsRegistered()
			{
				isVehicleRegistered = uTireManager.GetVehicle(m_vehicleRigidbody) == null ? false : true;
				return isVehicleRegistered;
			}

			#region On Scene GUI

			/// <summary>
			/// Called after the default GUI
			/// </summary>
			public virtual void customizeGUI()
			{
			}

			public virtual void OnRigidbodyAdded()
			{
			}

			public virtual void OnFlatRadiusSliderChanged()
			{
				if (!isVehicleRegistered) { return; }
				TSD.uTireIntegration.uTireIntegrationBase.activeVehicle.flatTireRadiusMultiplier = m_flatTireRadius;
			}

			///////////////////////////////////////////////
			//Show handle\onscreen button 

			// Window has been selected
			void OnFocus()
			{
				// Remove delegate listener if it has previously
				// been assigned.
				SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
				// Add (or re-add) the delegate.
				SceneView.onSceneGUIDelegate += this.OnSceneGUI;
			}

			void OnDestroy()
			{
				// When the window is destroyed, remove the delegate
				// so that it will no longer do any drawing.
				SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
			}

			void OnSceneGUI(SceneView sceneView)
			{
				if (selectedWC == null || m_vehicleRigidbody == null || selectedWheelMesh  == null) { return; }
				
				Vector3 wcPosition = selectedWC.wheelTransform.position + selectedWC.wheelTransform.TransformVector(selectedWC.center);
				wcPosition = selectedWheelMesh.transform.position;
				Vector3 wcPositionB = wcPosition;
			//	wcPositionB.y -= selectedWC.suspensionDistance;
				if(selectedWheelType == WheelTypes.WheelCollider)
				{
					wcPosition = Vector3.Lerp(wcPosition, wcPositionB, .5f);
					//this would be correct technically but WheelCollider is most likely set at .5 target distance
					//(and since WC3D don't have the equivalent of "targetPosition" we'll settle with this approximation)
					//wcPosition = Vector3.Lerp(wcPosition, wcPositionB, 1f - selectedWC.suspensionSpring.targetPosition);
				}

				Transform t = selectedWC.wheelTransform;
				Vector3 pos3D = wcPosition;

				Handles.SetCamera(Camera.current);
				// Do your drawing here using Handles.
				Handles.BeginGUI();

				/*Rect camRect = Camera.current.pixelRect;
				Rect bottomBtnRect = new Rect(camRect.width * .25f + (camRect.width * .25f) * .5f, camRect.height - camRect.height * .14f, camRect.width * .25f, camRect.height * .075f);
				if (GUI.Button(bottomBtnRect, showHandle ? "Hide Handle" : "Show Handle" , GUIInstance.ToggleStyle(showHandle))) { showHandle = !showHandle; }
				*/

			//	Vector3 pos = pos3D + Vector3.up * m_originalSize;
			//	Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);

				//	GUI.Box(new Rect(pos2D.x, pos2D.y-3, 132, 20), "", GUIInstance.ToggleButtonStyleToggled);

				// Do your drawing here using GUI.
				Handles.EndGUI();

		//		m_originalSize = ((WheelCollider)wheelColliders[0]).radius;
				m_originalSize = selectedWC.radius;
				Handles.color = Color.green;
				Handles.DrawWireArc(pos3D, t.right, t.up, 360, m_originalSize);

				Handles.color = Color.yellow;
				m_size = m_flatTireRadius * m_originalSize;
				Handles.DrawWireArc(pos3D, t.right, t.up, 360, m_size);

				GUI.color = Color.black;
				//	Handles.Label(pos, "Flat tire multiplier: " + m_flatTireRadius.ToString("#.##"));

				if (!showHandle) { return; }

				EditorGUI.BeginChangeCheck();
				undoManager.CheckUndo();
				m_size = (float)Handles.ScaleValueHandle(m_size, pos3D + -t.forward * m_size, Quaternion.Euler(t.up), .25f, Handles.SphereHandleCap, 0.001f);
				if (EditorGUI.EndChangeCheck())
				{
					m_size = Mathf.Clamp(m_size, 0.1f, m_originalSize);
					m_flatTireRadius = m_size / m_originalSize; //normalize again
				}
				if (this != null) { undoManager.CheckDirty(); }
			}
			//it's static so if we reopen the window it'll stay the same without manually saving values
			static bool showHandle = true;

			float m_size = 1f;
			float m_originalSize = .5f;

			#endregion
		}
	}
}