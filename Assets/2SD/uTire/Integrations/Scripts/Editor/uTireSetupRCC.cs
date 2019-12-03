#if TSD_INTEGRATION_RCC3
namespace TSD
{
	namespace uTireEditor
	{
		using UnityEngine;
		using UnityEditor;
		using uTireRuntime;

		/// <summary>
		/// Vehicle setup window for the Realistic Car Controller(v3) asset 
		/// </summary>
		public class uTireSetupRCC : uTireSetupWindowBase
		{
			//[MenuItem("Tools/TSD/Tire Deformation Vehicle Setup - Realistic Car Controller V3", priority = 2)]
			public static void Init()
			{
				// Get existing open window or if none, make a new one:
				uTireSetupRCC window = (uTireSetupRCC)EditorWindow.GetWindow(typeof(uTireSetupRCC));
				window.myLabel = "Realistic Car Controller V3";
				window.logo = (Texture)Resources.Load("RCC_Logo_286");
				window.Show();
			}

			public override void customize()
			{
				TSD.uTireIntegration.uTireRealisticCarController.vehicleRigidbody = m_vehicleRigidbody;
				TSD.uTireIntegration.uTireRealisticCarController.flatTireRadiusMultiplier = m_flatTireRadius;
				TSD.uTireIntegration.uTireRealisticCarController.LookForRequiredComponents();
				TSD.uTireIntegration.uTireRealisticCarController.RegisterVehicle();

				base.customize();
			}

			public override void OnRigidbodyAdded()
			{
				base.OnRigidbodyAdded();

				TSD.uTireIntegration.uTireRealisticCarController.vehicleRigidbody = m_vehicleRigidbody;
				checkIfVehicleIsRegistered();
				TSD.uTireIntegration.uTireRealisticCarController.flatTireRadiusMultiplier = m_flatTireRadius;
				TSD.uTireIntegration.uTireRealisticCarController.LookForRequiredComponents();
				TSD.uTireIntegration.uTireRealisticCarController.FindConnections();

				loadValuesFromManager(TSD.uTireIntegration.uTireRealisticCarController.activeVehicle);
			}
		}
	}
}
#endif