using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSD.uTireRuntime;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace TSD
{
	namespace uTireIntegration
	{
		public class uTireIntegrationBase : MonoBehaviour
		{
			public static Rigidbody vehicleRigidbody;
			//REMOVE THIS COMPLETELY
		//	public static WheelCollider[] wheelColliders;
			//REPLACE IT WITH THIS ASAP
			public static Object[] wheelControllers;

			public static MeshRenderer[] meshRenderers;
			public static float flatTireRadiusMultiplier;
			public static float maximumSteeringAngle;
			
			public static Vehicle activeVehicle {get; private set;}
			public static void SetActiveVehicle(Vehicle newVehicle) { activeVehicle = newVehicle; }
			/// <summary>
			/// Creates and populates the WheelMeshConnection list, and then creates a vehicle
			/// (unless a vehicle is already registered with the vehicleRigidbody, in that case 
			/// it returns the registered vehicle.)
			/// </summary>
			public static void FindConnections()
			{
				List<WheelMeshConnection> wmc = new List<WheelMeshConnection>();

				for (int i = 0; i < wheelControllers.Length; i++)
				{
					wmc.Add(new WheelMeshConnection(wheelControllers[i], meshRenderers[i]));
				}

				//check if everything is good if so register the vehicle
				if (wmc.Count == wheelControllers.Length)
				{
					foreach (var i_vehicle in uTireManager.Instance.vehicles)
					{
						//check if the rigidbody is already registered, set it as active vehicle and update its wheels
						if (i_vehicle.rigidBody == vehicleRigidbody)
						{
							activeVehicle = i_vehicle;
							activeVehicle.wheels = wmc.ToArray();
							return;
						}
					}

					activeVehicle = new Vehicle(vehicleRigidbody, wmc, flatTireRadiusMultiplier, maximumSteeringAngle);
				}
			}

			#region old
			/*
			 public static void FindConnections()
			{
				List<WheelMeshConnection> wmc = new List<WheelMeshConnection>();
				
				for (int i = 0; i < wheelColliders.Length; i++)
				{
					wmc.Add(new WheelMeshConnection(wheelColliders[i], meshRenderers[i]));
				}

				//check if everything is good if so register the vehicle
				if (wmc.Count == wheelColliders.Length)
				{
					foreach (var i_vehicle in uTireManager.Instance.vehicles)
					{
						//check if the rigidbody is already registered, set it as active vehicle and update its wheels
						if (i_vehicle.rigidBody == vehicleRigidbody)
						{
							activeVehicle = i_vehicle;
							activeVehicle.wheels = wmc.ToArray();
							return;
						}
					}

					activeVehicle = new Vehicle(vehicleRigidbody, wmc, flatTireRadiusMultiplier, maximumSteeringAngle);
				}
			}
			*/
			#endregion

			/// <summary>
			/// Registers the currently active vehicle(call this after FindConnection())
			/// </summary>
			public static void RegisterVehicle()
			{
				if (activeVehicle == null)
				{
					Debug.LogWarning("The vehicle you tried to register was null");
					return; 
				}
				uTireManager.RegisterVehicle(activeVehicle);
			}
		}

	}
}