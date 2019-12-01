using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TSD.uTireSettings;

namespace TSD
{
	namespace uTireRuntime
	{
		public class uTireManager : Singleton<uTireManager>
		{

			/// <summary>
			/// If true each vehicle's material will be updated
			/// </summary>
			public bool updateMaterial = true;


			/// <summary>
			/// If true each vehicle's WheelCollider radius will change dynamically
			/// </summary>
			public bool updateWheelColliderRadius = true;

			/// <summary>
			/// List of registered vehicles
			/// </summary>
			public List<Vehicle> vehicles = new List<Vehicle>();

			#region private properties

			//	float springCompression;
			//	WheelHit wheelHit;
			MaterialPropertyBlock props;

			#endregion


			public static WheelHit wHit;				///now this is more than questionable, but at this point I'll stick with a public static variable. WCGW
#if TSD_INTEGRATION_WC3D
			public static NWH.WheelController3D.WheelController.WheelHit WC3D_wHit;
#endif
			// Use this for initialization
			void Start()
			{
				props = new MaterialPropertyBlock();
				initVehicles();
			}

			void FixedUpdate()
			{
				if (updateMaterial)
				{
					UnityEngine.Profiling.Profiler.BeginSample("VEHICLES");
					updateWheels();
					UnityEngine.Profiling.Profiler.EndSample();
				}
			}

			public void updateWheels()
			{
				foreach (var vehicle in vehicles)
				{
					UnityEngine.Profiling.Profiler.BeginSample("vehicle");
					vehicle.speed = vehicle.rigidBody.velocity.magnitude * vehicle.speedMultiplier;
					//vehicle.speedRatio = Mathf.InverseLerp( vehicle.sidewaysSlideOverride.min uTireSettings.uTireGlobalSettings.Instance.GetSlideMinMax().min, uTireSettings.uTireGlobalSettings.Instance.GetSlideMinMax().max, vehicle.speed);
					vehicle.speedRatio = Mathf.InverseLerp( vehicle.sidewaysSlideOverride.min, vehicle.sidewaysSlideOverride.max, vehicle.speed);
					vehicle.CalculateHorizontalSpeed();
					foreach (var i_wheel in vehicle.wheels)
					{
						UnityEngine.Profiling.Profiler.BeginSample("Pressure");
						//	i_wheel.CalculatePressure(wheelHit);
						i_wheel.CalculatePressure(i_wheel.iWheel.GetGroundHitPoint());

						i_wheel.CalculateWheelAngle();
						UnityEngine.Profiling.Profiler.EndSample();

						UnityEngine.Profiling.Profiler.BeginSample("WCChange");
						if (updateWheelColliderRadius) { i_wheel.SetWheelColliderRadius(); }
						UnityEngine.Profiling.Profiler.EndSample();

						UnityEngine.Profiling.Profiler.BeginSample("SetShaderProperties");
						setTireDeformationMaterialInstancedProperties(vehicle, i_wheel);
						UnityEngine.Profiling.Profiler.EndSample();
					}
					UnityEngine.Profiling.Profiler.EndSample();
				}
			}

			void setTireDeformationMaterialInstancedProperties(Vehicle m_vehicle, WheelMeshConnection m_wmc)
			{
				props.SetFloat("_TireFlatnessT", m_wmc.flatness);
				props.SetFloat("_turn", m_wmc.turnAngle * m_vehicle.speedRatio);
				m_wmc.meshRenderer.SetPropertyBlock(props);
			}

			#region Public functions

			/// <summary>
			/// Returns the WheelMeshConnection object the provided WheelCollider is in(or null if it can't be found)
			/// Usecase: you can cache this value to efficiently change its properties over time 
			/// </summary>
			/// <param name="wc">WheelMeshConnection's WheelCollider/Wheel Controller 3D</param>
			/// <returns></returns>
			public static WheelMeshConnection GetWheelMeshConnection(Object wc)
			{
				foreach (var vehicle in Instance.vehicles)
				{
					foreach (var i_wheel in vehicle.wheels)
					{
						if (i_wheel.iWheel.wheelObject == wc)
						{
							return i_wheel;
						}
					}
				}
				return null;
			}

			/// <summary>
			/// Returns the Vehicle object the provided WheelCollider is in(or null if it can't be found)
			/// </summary>
			/// <param name="wc">Vehicle's WheelCollider/Wheel Controller 3D</param>
			/// <returns></returns>
			public static Vehicle GetVehicle(Object wc)
			{
				foreach (var vehicle in Instance.vehicles)
				{
					foreach (var i_wheel in vehicle.wheels)
					{
						if (i_wheel.iWheel.wheelObject == wc)
						{
							return vehicle;
						}
					}
				}
				return null;
			}

			/// <summary>
			/// Returns a registered Vehicle object or null if there isn't any with the provided rigidbody
			/// </summary>
			/// <param name="m_vehicleRigidbody">Vehicle's rigidbody</param>
			/// <returns></returns>
			public static Vehicle GetVehicle(Rigidbody m_vehicleRigidbody)
			{
				foreach (var vehicle in Instance.vehicles)
				{
					if (vehicle.rigidBody == m_vehicleRigidbody)
					{
						return vehicle;
					}
				}
				return null;
			}

			/// <summary>
			/// Sets the pressure of the provided tire
			/// </summary>
			/// <param name="wmc">The WheelMeshCollider we are about to modify</param>
			/// <param name="m_pressure">New pressure value(it's clamped between 0-2)</param>
			public static void SetTirePressure(WheelMeshConnection wmc, float m_pressure)
			{
				wmc.SetPressure(m_pressure);
			}

			/// <summary>
			/// Register vehicle as active vehicle
			/// </summary>
			/// <param name="m_vehicle">Vehicle to register</param>
			public static void RegisterVehicle(Vehicle m_vehicle)
			{
				m_vehicle.InitWheels();
				m_vehicle.SetMeasurement(uTireSettings.uTireGlobalSettings.Instance.GetMeasurement());// uTireManager.Instance.tireDeformationSettings.GetMeasurement());
				Instance.vehicles.Add(m_vehicle);
			}

			/// <summary>
			/// Creates and registers a vehicle as active vehicle
			/// </summary>
			/// <param name="m_vehicleRigidbody">Vehicle's rigidbody</param>
			/// <param name="m_wheelColliders">Vehicle's list of WheelColliders</param>
			/// <param name="m_wheelMeshes">Vehicle's list of wheel MeshRenderers</param>
			/// <param name="m_flatTireRadius">Vehicle's tire multiplier(when it's flat)</param>
			/// <param name="m_maxSteeringAngle">WheelCollider's maximum steering angle</param>
			/// <param name="m_sidewaysSlideMin">Speed when the tire will start to slide on the X axis</param>
			/// <param name="m_sidewaysSlideMax">Speed when the tire will be at maximum distance on the X axis</param>
			/// <param name="m_measurement">KPH\MPH(for side slide effect)</param>
			public static void RegisterVehicle(Rigidbody m_vehicleRigidbody, List<WheelCollider> m_wheelColliders, List<MeshRenderer> m_wheelMeshes, float m_flatTireRadius = 0.4f, float m_maxSteeringAngle = 40f, float m_sidewaysSlideMin = 0f, float m_sidewaysSlideMax = 0f, Measurement m_measurement = Measurement.KPH)
			{
				//make sure we have everything we need
				if (m_wheelColliders.Count != m_wheelMeshes.Count)
				{
					Debug.LogError("WheelCollider and WheelMesh count isn't the same, unable to register the vehicle", m_vehicleRigidbody);
					return;
				}

				//Store the connection between wheel colliders and their meshes
				List<WheelMeshConnection> wmc = new List<WheelMeshConnection>();
				for (int i = 0; i < m_wheelColliders.Count; i++)
				{
					wmc.Add(new WheelMeshConnection(m_wheelColliders[i], m_wheelMeshes[i]));
				}
				RegisterVehicle(new Vehicle(m_vehicleRigidbody, wmc, m_flatTireRadius, m_maxSteeringAngle, m_sidewaysSlideMin, m_sidewaysSlideMax, m_measurement));
			}

			/// <summary>
			/// Creates and registers a vehicle as active vehicle
			/// </summary>
			/// <param name="m_vehicleRigidbody">Vehicle's rigidbody</param>
			/// <param name="m_wheelColliders">Vehicle's list of WheelColliders</param>
			/// <param name="m_wheelMeshes">Vehicle's list of wheel MeshRenderers</param>
			/// <param name="settingsToLoad">Vehicle will load its settings from this object</param>
			public static void RegisterVehicle(Rigidbody m_vehicleRigidbody, List<WheelCollider> m_wheelColliders, List<MeshRenderer> m_wheelMeshes, uTirePrefabSettings settingsToLoad)
			{
				RegisterVehicle(m_vehicleRigidbody, m_wheelColliders, m_wheelMeshes, settingsToLoad.tireRadiusMultiplier, settingsToLoad.maxSteeringAngle, settingsToLoad.slideMinMaxOverride.min, settingsToLoad.slideMinMaxOverride.max);
			}

			/// <summary>
			/// Remove vehicle from active vehicles
			/// </summary>
			/// <param name="m_vehicleRigidbody">Vehicle's rigidbody</param>
			public static void RemoveVehicle(Rigidbody m_vehicleRigidbody)
			{
				Vehicle vehicle = null;
				foreach (var item in Instance.vehicles)
				{
					if (item.rigidBody == m_vehicleRigidbody)
					{
						vehicle = item;
						break;
					}
				}
				if (vehicle != null)
				{
					Instance.vehicles.Remove(vehicle);
				}
			}

			/// <summary>
			/// Some days you just want to say "yeah, fuck it", and then you fight when the null reference errors are coming out of the god damn walls
			/// In those days call this function and it will remove every missing element from the list vehicles. Slow, use with caution.
			/// </summary>
			public static void CleanupAfterVehicleRemoval()
			{
				Instance.vehicles.RemoveAll(item => item.rigidBody == null || item.rigidBody.Equals(null));
			}

			#endregion

			#region init vehicles

			void initVehicles()
			{
				foreach (var item in vehicles)
				{
					item.SetMeasurement(uTireSettings.uTireGlobalSettings.Instance.GetMeasurement());
					//	item.SetMeasurement(tireDeformationSettings.GetMeasurement());
					item.InitSettings();
					item.InitWheels();
				}
			}

			#endregion
		}

		[System.Serializable]
		public class VehicleData
		{
			public float tirePressureMultiplier = 1f;
			public MinMax steeringAngle;
		}

		/// <summary>
		/// Stores the vehicle's rigidbody, wheels, and pressure multiplier 
		/// </summary>
		[System.Serializable]
		public class Vehicle
		{
			public Rigidbody rigidBody;
			public float flatTireRadiusMultiplier = .85f;
			//	public float flatTireRadius = .4f;
		//	[Range(0f, 2f)]
			public float tirePressureMultiplier
			{
				get { return vehicleData.tirePressureMultiplier; }
				set { if (vehicleData == null) { vehicleData = new VehicleData(); } vehicleData.tirePressureMultiplier = value; }
			}
			[Range(0f, 90f)]
			public float maxSteeringAngle = 40f;

			public WheelMeshConnection[] wheels;
			public float speed;

			public VehicleData vehicleData;

			public float speedMultiplier = 3.6f;
			public float speedRatio = 0f;
			public float speedHorizontal = 0f;

			[HideInInspector]
			public MinMax steeringAngle
			{
				get { return vehicleData.steeringAngle; }
				set { if (vehicleData == null) { vehicleData = new VehicleData(); } vehicleData.steeringAngle = value; }
			}

			/// <summary>
			/// If other than 0-0 it'll be used in stead of GlobalSettings' sidewaysSlide
			/// </summary>
			public MinMax sidewaysSlideOverride;

			public void CalculateHorizontalSpeed()
			{
				Vector3 dir = rigidBody.transform.InverseTransformDirection(rigidBody.velocity);
				speedHorizontal = dir.x;
			}

			public Vehicle(Rigidbody m_rigidbody, List<WheelMeshConnection> m_wmc, float m_flatTireRadiusMultiplier = 0.4f, float m_maxSteeringAngle = 40f, float m_sidewaysSlideMin = 0f, float m_sidewaysSlideMax = 0f, Measurement m_measurement = Measurement.KPH)
			{

				flatTireRadiusMultiplier = m_flatTireRadiusMultiplier;

				maxSteeringAngle = m_maxSteeringAngle;
				rigidBody = m_rigidbody;
				wheels = m_wmc.ToArray();
				sidewaysSlideOverride = new MinMax(m_sidewaysSlideMin, m_sidewaysSlideMax);
				SetMeasurement(m_measurement);
			}

			public void SetPressureMultiplier(float m_pressureMultiplier)
			{
				tirePressureMultiplier = m_pressureMultiplier;
			}

			public void SetMeasurement(Measurement m_measurement)
			{
				switch (m_measurement)
				{
					case Measurement.KPH:
						speedMultiplier = 3.6f;
						break;
					case Measurement.MPH:
						speedMultiplier = 2.237f;
						break;
					default:
						speedMultiplier = 3.6f;
						break;
				}
			}


			public void SetVehicleSettings(uTirePrefabSettings uTireSavedSettings, bool logErrors = false)
			{
				SetVehicleSettings(uTireSavedSettings.maxSteeringAngle, uTireSavedSettings.tirePressureMultiplier, uTireSavedSettings.tireRadiusMultiplier, uTireSavedSettings.tirePressure, uTireSavedSettings.slideMinMaxOverride, logErrors);
			}

			public void SetVehicleSettings(float m_maxSteeringAngle, float m_tirePressureMultiplier, float m_tireRadiusMultiplier, List<float> m_tirePressure, MinMax m_sidewaysSlideOverride, bool logErrors = false)
			{
				maxSteeringAngle = m_maxSteeringAngle;
				tirePressureMultiplier = m_tirePressureMultiplier;
				flatTireRadiusMultiplier = m_tireRadiusMultiplier;
				sidewaysSlideOverride = m_sidewaysSlideOverride;

				//do safety check to make sure the wheel count is the same
				if (m_tirePressure.Count == wheels.Length)
				{
					for (int i = 0; i < m_tirePressure.Count; i++)
					{
						wheels[i].SetPressure(m_tirePressure[i]);
					}
				}
				else
				{
					if (logErrors) { Debug.LogWarning("Tried to load pressure settings from file, but the amount of wheels is different."); }	
				}
			}
			
			/// <summary>
			/// Sets the sideways slide values
			/// </summary>
			public void InitSettings()
			{
				//We don't want to overwrite the stored values so just 
				if (!Application.isPlaying) { return; }
				if (sidewaysSlideOverride.min == 0f)
				{
					sidewaysSlideOverride.min = uTireSettings.uTireGlobalSettings.Instance.GetSlideMinMax().min;
				}
				if(sidewaysSlideOverride.max == 0f)
				{
					sidewaysSlideOverride.max = uTireSettings.uTireGlobalSettings.Instance.GetSlideMinMax().max;
				}
			}

			public void InitWheels()
			{
				steeringAngle = new MinMax(maxSteeringAngle * .5f, -maxSteeringAngle * .5f);

				foreach (var wheel in wheels)
				{
					wheel.init(flatTireRadiusMultiplier, this);
				}
			}
		}

		[System.Serializable]
		public enum Measurement
		{
			KPH,
			MPH
		}


		/// <summary>
		/// Stores the wheel mesh and its corresponding WheelCollider
		/// </summary>
		[System.Serializable]
		public class WheelMeshConnection
		{
			[SerializeField]
			WheelBase wheel; //wheel exists so we can access its children easily
			[SerializeField]
			public IWheel iWheel; //iWheel interface contains WheelBase's children's functions
			[SerializeField]
			Object wheelObject; //the actual children we are using(as of today it can be WeelCollider or WheelController3D)
			//this is where we actually store the object, the wheel reference will get its value from wheelObject during init

			public Object GetWheelObject()
			{
				return wheelObject;
			}

			public MeshRenderer meshRenderer;

			[SerializeField]
			float springCompression;

			public float flatness;
			[Range(0f, 1f)]
			public float tirePressure = 1f;
			public float turnAngle;

			VehicleData vehicleData { get; set; }
			float tireFullRadius; //the radius set in the editor(received during initialization)
			float tireFlatRadius;
			float tireFlatMultiplier;

			//public WheelMeshConnection(WheelCollider wc, MeshRenderer mr)
			/// <summary>
			/// 
			/// </summary>
			/// <param name="wc">Should be either a built in WheelCollider or NWH's WheelController3D</param>
			/// <param name="mr">Mesh renderer</param>
			public WheelMeshConnection(Object wc, MeshRenderer mr)
			{
				wheelObject = wc;
				meshRenderer = mr;
			}

			public void init(float m_tireFlatMultiplier, Vehicle m_vehicle)
			{
				if (wheelObject.GetType() == typeof(WheelCollider)) { wheel = new WC(); }
#if TSD_INTEGRATION_WC3D
				else if (wheelObject.GetType() == typeof(NWH.WheelController3D.WheelController)) { wheel = new WC3D(); }
#endif
				wheel.wheelObject = wheelObject; //wheel can be serialized

				tireFlatMultiplier = m_tireFlatMultiplier;
				iWheel = (IWheel)wheel;
				tireFullRadius = iWheel.radius;
				if (tireFlatMultiplier > 0)
				{
					tireFlatRadius = tireFullRadius * tireFlatMultiplier;
				}
				else
				{
					tireFlatRadius = tireFullRadius;
					Debug.LogWarning("Invalid flat tire radius, it has to be bigger than 0", iWheel.wheelObject);
				}

				vehicleData = m_vehicle.vehicleData;
			}

			public void CalculatePressure(Vector3 wheelHitPoint)
			{

				if (iWheel.isGrounded)
				{
					//	float targetDistance = iWheel.suspensionDistance * (1f - iWheel.suspensionSpring.targetPosition);

					//	springCompression = (-iWheel.wheelTransform.InverseTransformPoint(wheelHit.point).y - iWheel.radius);

					/*
					springCompression = (-iWheel.wheelTransform.InverseTransformPoint(wheelHitPoint).y - iWheel.radius);
					springCompression = Mathf.Clamp(springCompression, 0, targetDistance);
					springCompression /= targetDistance + Mathf.Epsilon;
					*/
					springCompression = iWheel.springCompression;
					springCompression = 1 - springCompression + (1 - tirePressure * vehicleData.tirePressureMultiplier);
				}
				else
				{
					springCompression = 0f;
				}
				flatness = springCompression;
				flatness = Mathf.Clamp01(flatness);
			}

			public void CalculateWheelAngle()
			{
				turnAngle = (Mathf.InverseLerp(vehicleData.steeringAngle.min, vehicleData.steeringAngle.max, iWheel.steerAngle) - .5f) * 2f;
			}

			public void SetPressure(float m_pressure)
			{
				tirePressure = Mathf.Clamp01(m_pressure);
			}

			public void SetWheelColliderRadius()
			{
				iWheel.radius = Mathf.Lerp(tireFullRadius, tireFlatRadius, flatness);
			}

			public void SetFlatRadius(float m_flatRadius)
			{
				tireFlatRadius = m_flatRadius;
			}
		}

		[System.Serializable]
		public class MinMax
		{
			public float min;
			public float max;

			public MinMax(float m_min, float m_max)
			{
				min = m_min;
				max = m_max;
			}
		}
	}
}