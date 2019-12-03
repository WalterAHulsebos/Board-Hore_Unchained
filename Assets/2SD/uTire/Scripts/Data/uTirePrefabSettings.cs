using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSD.uTireRuntime;

namespace TSD
{
	namespace uTireSettings
	{
		/// <summary>
		/// Class to store and access settings for stored uTire vehicles(tire pressure, slide min\max, etc)
		/// </summary>
		[CreateAssetMenu(fileName = "uTire Prefab",  menuName = "TSD/Tire Deformation/Prefab Settings")]
		public class uTirePrefabSettings : ScriptableObject
		{
			public uTirePrefabSettings(GameObject _prefab, float _maxSteeringAngle, float _tireFlatnessMultiplier, List<float> _tireFlantess, float _tireRadiusMultiplier, MinMax _slideMinMaxOverride)
			{
				Prefab					= _prefab;
				maxSteeringAngle		= _maxSteeringAngle;
				tirePressureMultiplier	= _tireFlatnessMultiplier;
				tirePressure			= new List<float>();
				tirePressure.AddRange(_tireFlantess);
				tireRadiusMultiplier	= _tireRadiusMultiplier;
				slideMinMaxOverride		= _slideMinMaxOverride;
			}

			/// <summary>
			/// This GO only serves as a identifier, you can use the same Prefab for multiple vehicles
			/// For example if you have 10 different car models with the exact same tires it makes sense 
			/// to only store the settings once.
			/// </summary>
			public GameObject Prefab;

			/// <summary>
			/// Maximum steering angle of the Wheel
			/// </summary>
			public float maxSteeringAngle;
			/// <summary>
			/// Global multiplier for tire pressure(it'll be applied after the individual settings)
			/// </summary>
			public float tirePressureMultiplier;
			/// <summary>
			/// Individual tire pressure for each tire
			/// </summary>
			public List<float> tirePressure;
			/// <summary>
			/// Radius multiplier when the tire is fully compressed
			/// </summary>
			public float tireRadiusMultiplier;
			/// <summary>
			/// If you want to override the global slide settings you can do so here
			/// </summary>
			public MinMax slideMinMaxOverride;
		}
	}
}