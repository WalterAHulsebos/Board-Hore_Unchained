#if TSD_INTEGRATION_NWHVehiclePhysics
namespace TSD.uTireEditor
{
	using UnityEngine;
	using UnityEditor;
	using uTireRuntime;
	/// <summary>
	/// Vehicle setup window for the Realistic Car Controller(v3) asset 
	/// </summary>
	public class uTireSetupNWHVehiclePhysics : uTireSetupWindowBase
	{
		//[MenuItem("Tools/TSD/Tire Deformation Vehicle Setup - Realistic Car Controller V3", priority = 2)]
		public static void Init()
		{
			// Get existing open window or if none, make a new one:
			uTireSetupNWHVehiclePhysics window = (uTireSetupNWHVehiclePhysics)EditorWindow.GetWindow(typeof(uTireSetupNWHVehiclePhysics));
			window.myLabel = "NWH Vehicle Controller";
			window.logo = (Texture)Resources.Load("NWH_logo_286");
			window.Show();
		}

		public override void customize()
		{
			TSD.uTireIntegration.uTireNWHVehicleController.vehicleRigidbody = m_vehicleRigidbody;
			TSD.uTireIntegration.uTireNWHVehicleController.flatTireRadiusMultiplier = m_flatTireRadius;
			TSD.uTireIntegration.uTireNWHVehicleController.LookForRequiredComponents();
			TSD.uTireIntegration.uTireNWHVehicleController.RegisterVehicle();

			base.customize();
		}

		public override void OnRigidbodyAdded()
		{
			base.OnRigidbodyAdded();

			TSD.uTireIntegration.uTireNWHVehicleController.vehicleRigidbody = m_vehicleRigidbody;
			checkIfVehicleIsRegistered();
			TSD.uTireIntegration.uTireNWHVehicleController.flatTireRadiusMultiplier = m_flatTireRadius;
			TSD.uTireIntegration.uTireNWHVehicleController.LookForRequiredComponents();
			TSD.uTireIntegration.uTireNWHVehicleController.FindConnections();

			loadValuesFromManager(TSD.uTireIntegration.uTireNWHVehicleController.activeVehicle);
		}
	}
}
#endif