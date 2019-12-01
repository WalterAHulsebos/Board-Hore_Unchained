namespace TSD
{
	namespace uTireEditor
	{
		using UnityEngine;
		using UnityEditor;
		using System.Linq;
		using uTireRuntime;
		using TSD.uTireIntegration;

		/// <summary>
		/// The default vehicle setup window
		/// </summary>
		public class uTireSetupDefault : uTireSetupWindowBase
		{

			//[MenuItem("Tools/TSD/uTire Setup - Default")]
			public static void Init()
			{
				// Get existing open window or if none, make a new one:
				uTireSetupDefault window = (uTireSetupDefault)EditorWindow.GetWindow(typeof(uTireSetupDefault));
				window.myLabel = "Default";
				window.logo = (Texture)Resources.Load("uTire_Logo_text_bare");
				window.logoText = (Texture)Resources.Load("uTire_Logo_VEHICLE");
				window.Show();
			}

			public override void customize()
			{
				if(isVehicleRegistered)
				{
					uTireDefault.SetActiveVehicle(uTireManager.GetVehicle(m_vehicleRigidbody));
				}

				uTireDefault.vehicleRigidbody = m_vehicleRigidbody;
				uTireDefault.flatTireRadiusMultiplier = m_flatTireRadius;
				uTireDefault.meshRenderers = wheelMeshes.Cast<MeshRenderer>().ToArray();
			//	uTireDefault.wheelColliders = wheelColliders.Cast<WheelCollider>().ToArray();
				uTireDefault.wheelControllers = wheelColliders;
				uTireDefault.FindConnections();
				if (!isVehicleRegistered)
				{
				//	uTireDefault.FindConnections();
					uTireDefault.RegisterVehicle();
				}
				else
				{
					uTireDefault.activeVehicle.flatTireRadiusMultiplier = m_flatTireRadius;
				}

				if(isVehicleRegistered)
				{
					
				}

				base.customize();
			}

			public override void customizeGUI()
			{
			}

			public override void OnRigidbodyAdded()
			{
				uTireDefault.vehicleRigidbody = m_vehicleRigidbody;
				uTireDefault.SetActiveVehicle(uTireManager.GetVehicle(m_vehicleRigidbody));
				checkIfVehicleIsRegistered();
				uTireDefault.flatTireRadiusMultiplier = m_flatTireRadius;

				if (isVehicleRegistered)
				{
					loadValuesFromManager(uTireDefault.activeVehicle);// TSD.uTireIntegration.uTireDefault.vehicle);
				}
				else
				{
					resetArrays();
				}
			}
		}
	}
}