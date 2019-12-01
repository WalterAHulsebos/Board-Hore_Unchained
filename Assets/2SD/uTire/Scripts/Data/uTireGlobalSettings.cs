namespace TSD
{
	namespace uTireSettings
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;
		using System.Linq;
		using uTireRuntime;

		//[CreateAssetMenu(fileName = "Tire Deformation Settings", menuName = "TSD/Tire Deformation Settings", order = 1)]
		public class uTireGlobalSettings : ScriptableObject
		{
			//3D collision
			//[SerializeField]
			public LayerMask phyicsLayermask;// { get; set; }
			public int rayCount = 32;
			public int rayRingCount = 3;

			public float rayAngle = 360f;
			public float rayAngleOffset = 0f;

			public float animationSpeedOnCollision = 35f;
			public float animationSpeedOnNoCollision = 75f;


			[SerializeField]
			Measurement measurement = Measurement.KPH;

			[Tooltip("The tire will slide sideways when the vehicle's speed is between these limits(above the effect is clamped). KPH or MPH is based on 'measurement'")]
			[SerializeField]
			MinMax slideMinMax = new MinMax(20,40);
			
			List<uTirePrefabSettings> prefabDatabase; //coming soon
			
			public Measurement GetMeasurement()
			{
				return measurement;
			}
			public void SetMeasurement(Measurement m_measurement)
			{
				measurement = m_measurement;
			}

			/// <summary>
			/// The tire will slide sideways when the vehicle's speed is between these limits(above the effect is clamped). KPH or MPH is based on 'measurement'
			/// </summary>
			/// <returns></returns>
			public MinMax GetSlideMinMax()
			{
				return slideMinMax;
			}

			//[ContextMenu("Load Prefab Data")]
			void loadPrefabs()
			{
				prefabDatabase = Resources.LoadAll("", typeof(uTirePrefabSettings)).Cast<uTirePrefabSettings>().ToList();
			}

			/// <summary>
			/// Returns the settings stored with the provided GameObject
			/// Returns null if no data was found
			/// </summary>
			/// <param name="prefab">The prefab we saved with the data</param>
			/// <param name="generateMessageIfPrefabNotFound">If true Unity will log an error</param>
			/// <returns></returns>
			public uTirePrefabSettings GetPrefabData(GameObject prefab, bool generateMessageIfPrefabNotFound = false)
			{
				foreach (var item in prefabDatabase)
				{
					if (item.Prefab == prefab)
					{
						return item;
					}
				}

				if (generateMessageIfPrefabNotFound)
				{
					Debug.LogError("uTire was unable to find the saved data for the provided GameObject. Make sure the GameObject is in the database. For more information please read the manual.");
				}
				return null;
			}


			static uTireGlobalSettings _instance;

			public static uTireGlobalSettings Instance
			{
				get
				{
					if(_instance == null)
					{
						uTireGlobalSettings[] settingsHolderArray = Resources.LoadAll("", typeof(uTireGlobalSettings)).Cast<uTireGlobalSettings>().ToArray();
						if(settingsHolderArray.Length == 0)
						{
							Debug.LogWarning("No uTireGlobalSettings were found, creating one.");
#if UNITY_EDITOR
							settingsHolderArray = new uTireGlobalSettings[1];
							settingsHolderArray[0] = CreateInstance<uTireGlobalSettings>();
							UnityEditor.AssetDatabase.CreateAsset(settingsHolderArray[0], "Assets/2SD/uTire/Resources/uTire Settings.asset");
							UnityEditor.Selection.activeObject = settingsHolderArray[0];
#endif
						}
						if(settingsHolderArray.Length > 1)
						{
							Debug.LogWarning("You have multiple uTireGlobalSettings files, only the first one will be used, you should delete the rest.", settingsHolderArray[0]);
						}
						_instance = settingsHolderArray[0];
						_instance.loadPrefabs();
					}
					return _instance;
				}
			}

#if UNITY_EDITOR
			[UnityEditor.MenuItem("Tools/TSD/uTire Global Settings")]
			public static void SelectDatabase()
			{
				if(Instance == null)
				{
					uTireGlobalSettings settings = new uTireGlobalSettings();
					UnityEditor.AssetDatabase.CreateAsset(settings, "Assets/2SD/uTire/Resources/uTire Settings.asset");
				}

				UnityEditor.Selection.activeObject = Instance;
			}
#endif
		}
	}
}
