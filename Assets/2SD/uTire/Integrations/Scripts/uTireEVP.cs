#if TSD_INTEGRATION_EVP
namespace TSD
{
	namespace uTireIntegration
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;
		using EVP;

		public class uTireEVP : uTireIntegrationBase
		{
			
			static VehicleController vc;

			/// <summary>
			/// Register a RCC3 vehicle
			/// </summary>
			/// <param name="rb">Vehicle's rigidbody</param>
			/// <param name="_flatTireRadiusMultiplier">Wheel radius multiplier when fully deflated(~think of it as a normalized value, how smaller do you want it to be when fully compressed?)</param>
			public static void RegisterVehicle(Rigidbody rb, float _flatTireRadiusMultiplier = .65f)
			{
				vehicleRigidbody = rb;
				flatTireRadiusMultiplier = _flatTireRadiusMultiplier;
				LookForRequiredComponents();
				RegisterVehicle();
			}

			public static void LookForRequiredComponents()
			{
				//Safety check
				if (vehicleRigidbody == null) { Debug.LogError("Drag in the vehicle's rigidbody first!"); return; }
				vc = vehicleRigidbody.GetComponent<EVP.VehicleController>();
				if (vc == null) { Debug.LogError("There is no VehicleController script attached to the provided rigidbody."); return; }


				int wheelCount = vc.wheels.Length;

				wheelControllers = new WheelCollider[wheelCount];
				meshRenderers = new MeshRenderer[wheelCount];

				for (int i = 0; i < vc.wheels.Length; i++)
				{
					wheelControllers[i] = vc.wheels[i].wheelCollider;

					MeshRenderer[] childRenderers = vc.wheels[i].wheelTransform.GetComponentsInChildren<MeshRenderer>();
					
					//notify the user if we have no idea which child should we use as tire
					if( childRenderers.Length > 1)
					{
						Debug.LogWarning("There are multiple children MeshRenderers attached as 'WheelMesh', will use the first in hierarchy.", vc.wheels[i].wheelTransform);
					}
					meshRenderers[i] = childRenderers[0];
				}

				maximumSteeringAngle = vc.maxSteerAngle;
				FindConnections();
			}
		}

	}
}
#endif