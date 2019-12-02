namespace TSD
{
	namespace uTireExamples
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;
		using Demonstration;
		using uTireRuntime;
		using System;
		
		/// <summary>
		/// In this example we'll instantiate a prefab at runtime, 
		/// register its tires then make sure it gets removed when
		/// we don't need it anymore 
		/// We'll use the Car Controller from the Standard Assets
		/// </summary>
		public class uTireRuntimeExample : MonoBehaviour
		{

			enum CreateMethod
			{
				fromScratch,		//We will set the arguments manually
				fromProvidedFile,	//We will use a template saved to disk previously to set the arguments
				
				fromProvidedPrefab	//We will use a prefab to look for the settings in the database 
									//so you don't have to store a reference by hand to the uTireSetting object
									//but otherwise it's exactly the same as fromProvideFile
			}

			public uTireSettings.uTirePrefabSettings loadSettingsFromFile;

			public GameObject vehiclePrefab; //The prefab we are going to instantiate
			public Transform spawnPoint;

			GameObject clone;
			CarController m_carController;	//In this example we are using Unity's CarController script
											//(so we can easily access the wheel colliders and meshes)

			[SerializeField]
			CreateMethod createMethod = CreateMethod.fromScratch;

			void Update()
			{
				if(Input.GetKeyDown(KeyCode.F2))
				{
					if (clone == null) 
					{ 
						spawn();
						GetComponent<MouseOrbitImproved>().target = clone.transform;
					}
					else 
					{ 
						remove();
						GetComponent<MouseOrbitImproved>().target = null;
					}
				}

				if (Input.GetKeyDown(KeyCode.PageUp)) 
				{
					if ((int)createMethod < Enum.GetNames(typeof(CreateMethod)).Length) { createMethod++; } else { createMethod = 0; } 
				}
				if (Input.GetKeyDown(KeyCode.PageDown))
				{
					if ((int)createMethod > 0) { createMethod--; } else { createMethod = (CreateMethod)Enum.GetNames(typeof(CreateMethod)).Length - 1; }
				}
			}

			void OnGUI()
			{
				Rect myRect = new Rect(5,5,200,30);
				GUI.Box(myRect, "");
				GUI.Label(myRect, (clone == null) ? "Press F2 to spawn a vehicle" : "Press F2 to remove the vehicle");
				myRect = new Rect(5, 40, 200, 40);
				GUI.Box(myRect, "");
				GUI.Label(myRect, "Creation method: \n" +  createMethod.ToString());

				myRect = new Rect(5, 85, 200, 35);
				GUI.Box(myRect, "");
				GUI.Label(myRect, "Switch between modes with PageUp/PageDown");
			}

			void spawn()
			{
				//Instantiate the vehicle at world zero
				clone = Instantiate(vehiclePrefab, spawnPoint.position, Quaternion.identity);
				m_carController = clone.GetComponent<CarController>();

				//The rigidbody is used to calculate the speed of the vehicle, this is important because over a certain speed the tire 
				//mesh will stretch a bit
				Rigidbody vehicleRigidbody = clone.GetComponent<Rigidbody>();
				
				//The WheelCollider and the MeshRenderer groups have to be set in the same order
				//The wheelColliders is every WheelCollider and the meshRenderers is every corresponding MeshRenderer
				List<WheelCollider> wheelColliders = new List<WheelCollider>();
				List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
				for (int i = 0; i < m_carController.m_WheelColliders.Length; i++)
				{
					wheelColliders.Add(m_carController.m_WheelColliders[i]);
					//in this case we have multiple meshes(the rim and the tire) so to make sure it won't accidentally register the rim
					//we have to look for the tire mesh by name
					meshRenderers.Add(m_carController.m_WheelMeshes[i].transform.Find("Tire").GetComponent<MeshRenderer>());
				}

				//Here you can see the 3 methods you can use to set up a new vehicle
				switch (createMethod)
				{
					case CreateMethod.fromScratch:
						//flatTireRadius will tell the system to how much can it shrink the tire compared to its original state(it's a multiplier)
						float flatTireRadiusMultiplier = 0.85f;
						//maxSteeringAngle have to be the same as the WheelCollider's maximum angle
						float maxSteeringAngle = 40f;

						//Send the data to the uTireManager, it'll make sure the vehicle is only registered once - and we are done here!
						uTireManager.RegisterVehicle(vehicleRigidbody, wheelColliders, meshRenderers, flatTireRadiusMultiplier, maxSteeringAngle);
						
						//////////////////////
						//note
						//////////////////////

						//You can also pass in more data(like per wheel pressure, if for whatever reason you use a different metric for this specific vehicle 
						//you can overwrite the default one(which is set in the GlobalSettings(Tools\TSD\Global Settings), and the sideways slide min\max values)
						//but for the sake of simplicity we will stick to the bare minimum here
						break;
					case CreateMethod.fromProvidedFile:
						//Alternatively you can also save the settings to file and then you can reuse it multiple times
						uTireManager.RegisterVehicle(vehicleRigidbody, wheelColliders, meshRenderers, loadSettingsFromFile);
						break;
					case CreateMethod.fromProvidedPrefab:
						//Or if you don't want to store a reference to the file itself you can look for the data associated with a given prefab
						//keep in mind it doesn't matter what the prefab actually is, it's only used as a way to find the data
						uTireManager.RegisterVehicle(vehicleRigidbody, wheelColliders, meshRenderers, uTireSettings.uTireGlobalSettings.Instance.GetPrefabData(vehiclePrefab));
						break;
					default:
						break;
				}
			}

			void remove()
			{
				//first we'll remove the wheels from the manager
				uTireManager.RemoveVehicle(clone.GetComponent<Rigidbody>());
				//then we can safely destroy the vehicle
				Destroy(clone);
			}
		}

	}
}
