#if TSD_INTEGRATION_NWHVehiclePhysics
namespace TSD.uTireIntegration
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using NWH.VehiclePhysics;
	using System.Linq;

	public class uTireNWHVehicleController : uTireIntegrationBase
	{
		static NWH.VehiclePhysics.VehicleController vc;

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
			vc = vehicleRigidbody.GetComponent<NWH.VehiclePhysics.VehicleController>();
			if (vc == null) { Debug.LogError("There is no WHVehicleController script attached to the provided rigidbody."); return; }

			List<Axle> NWHsAxles = vc.GetAxles();
			int wheelCount = NWHsAxles.Count;
			
		//	wheelControllers = new WheelCollider[wheelCount * 2];
		//	meshRenderers = new MeshRenderer[wheelCount * 2];

			List<Object> _wc = new List<Object>();
			List<MeshRenderer> _mr = new List<MeshRenderer>();

			foreach (var item in NWHsAxles)
			{
				_wc.Add(item.leftWheel.wheelController);
				_mr.Add(item.leftWheel.VisualTransform.GetComponentInChildren<MeshRenderer>());
				_wc.Add(item.rightWheel.wheelController);
				_mr.Add(item.rightWheel.VisualTransform.GetComponentInChildren<MeshRenderer>());
			}

			wheelControllers = _wc.ToArray();
			meshRenderers = _mr.ToArray();

			maximumSteeringAngle = vc.steering.highSpeedAngle; //maybe should change this to the lowSpeedAngle?? Not sure
			FindConnections();
		}
	}
}
#endif