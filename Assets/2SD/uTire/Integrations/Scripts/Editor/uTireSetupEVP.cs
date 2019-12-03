
namespace TSD
{
	namespace uTireEditor
	{
		using UnityEngine;
		using UnityEditor;
		using System.Linq;
		using uTireRuntime;

		/// <summary>
		/// Vehicle setup window for the Edy's Vehicle Physics asset
		/// </summary>
		public class uTireSetupEVP : uTireSetupWindowBase
		{
#if TSD_INTEGRATION_EVP
			//[MenuItem("Tools/TSD/Tire Deformation Setup - Edy's Vehicle Physics", priority = 2)]
			public static void Init()
			{
				// Get existing open window or if none, make a new one:
				//	TireDeformationSetupWindowEVP window = (TireDeformationSetupWindowEVP)EditorWindow.GetWindow(typeof(TireDeformationSetupWindowEVP));
				//	window.myLabel = "Edy's Vehicle Physics 5";
				//	window.logo = (Texture)Resources.Load("EVP_logo_286");
				//	window.Show();
				uTireSetupEVP window = ((uTireSetupEVP)EditorWindow.GetWindow(typeof(uTireSetupEVP)));
			//	window.checkIfVehicleIsRegistered();
				window.Show();

			}
			public override void OnEnable()
			{
				base.OnEnable();
				uTireSetupEVP window = (uTireSetupEVP)EditorWindow.GetWindow(typeof(uTireSetupEVP));
				window.myLabel = "Edy's Vehicle Physics 5";
				window.logo = (Texture)Resources.Load("EVP_logo_286");
			}

			public override void customize()
			{
				if (isVehicleRegistered)
				{
					TSD.uTireIntegration.uTireEVP.SetActiveVehicle(uTireManager.GetVehicle(m_vehicleRigidbody));
				}

				TSD.uTireIntegration.uTireEVP.vehicleRigidbody = m_vehicleRigidbody;
				TSD.uTireIntegration.uTireEVP.flatTireRadiusMultiplier = m_flatTireRadius;
				TSD.uTireIntegration.uTireEVP.meshRenderers = wheelMeshes.Cast<MeshRenderer>().ToArray();
				TSD.uTireIntegration.uTireEVP.wheelControllers = wheelColliders.Cast<WheelCollider>().ToArray();

				if (!isVehicleRegistered)
				{
					TSD.uTireIntegration.uTireEVP.FindConnections();
					TSD.uTireIntegration.uTireEVP.RegisterVehicle();
				}
				else
				{
					TSD.uTireIntegration.uTireEVP.activeVehicle.flatTireRadiusMultiplier = m_flatTireRadius;
				}

				base.customize();
			}

			public override void OnRigidbodyAdded()
			{
				TSD.uTireIntegration.uTireEVP.vehicleRigidbody = m_vehicleRigidbody;
				checkIfVehicleIsRegistered();
				TSD.uTireIntegration.uTireEVP.flatTireRadiusMultiplier = m_flatTireRadius;
				TSD.uTireIntegration.uTireEVP.LookForRequiredComponents();
			//	TSD.uTireIntegration.uTireEVP.FindConnections();

				loadValuesFromManager(TSD.uTireIntegration.uTireEVP.activeVehicle);
			}

			public override void OnFlatRadiusSliderChanged()
			{
				base.OnFlatRadiusSliderChanged();
			//	if (!isVehicleRegistered) { return; }
			//	TSD.uTireIntegration.uTireIntegrationBase.vehicle.flatTireRadiusMultiplier = m_flatTireRadius;
			}
#endif
		}	
	}
			
}