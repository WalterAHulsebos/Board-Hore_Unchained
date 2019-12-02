#if TSD_INTEGRATION_RCC3
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSD
{
	namespace uTireIntegration
	{
		public class uTireRealisticCarController : uTireIntegrationBase
		{
			static RCC_CarControllerV3 vc;

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
				vc = vehicleRigidbody.GetComponent<RCC_CarControllerV3>();
				if (vc == null) { Debug.LogError("There is no RCC_CarControllerV3 script attached to the provided rigidbody."); return; }
				
				int wheelCount = 4 + vc.ExtraRearWheelsTransform.Length;

				wheelControllers = new WheelCollider[wheelCount];
				meshRenderers = new MeshRenderer[wheelCount];

				wheelControllers[0] = vc.FrontLeftWheelCollider.wheelCollider;
				meshRenderers[0] = vc.FrontLeftWheelTransform.GetComponent<MeshRenderer>();

				wheelControllers[1] = vc.FrontRightWheelCollider.wheelCollider;
				meshRenderers[1] = vc.FrontRightWheelTransform.GetComponent<MeshRenderer>();


				wheelControllers[2] = vc.RearRightWheelCollider.wheelCollider;
				meshRenderers[2] = vc.RearRightWheelTransform.GetComponent<MeshRenderer>();

				wheelControllers[3] = vc.RearLeftWheelCollider.wheelCollider;
				meshRenderers[3] = vc.RearLeftWheelTransform.GetComponent<MeshRenderer>();

				for (int i = 0; i < vc.ExtraRearWheelsTransform.Length; i++)
				{
					wheelControllers[4 + i] = vc.ExtraRearWheelsCollider[i].wheelCollider;
					meshRenderers[4 + i] = vc.ExtraRearWheelsTransform[i].GetComponent<MeshRenderer>();
				}


				maximumSteeringAngle = vc.steerAngle;
				FindConnections();
			}
		}
	}
}
#endif