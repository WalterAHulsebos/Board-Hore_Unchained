namespace TSD
{
	namespace uTireSettings
	{
		using System.Collections;
		using System.Collections.Generic;
		using System.Linq;
		using UnityEngine;

		/// <summary>
		/// Used for storing\retrieving the default material settings(instancing, tessellation etc)
		/// </summary>
		[CreateAssetMenu(fileName = "uTire Default Shader Settings", menuName = "TSD/Tire Default Shader Settings", order = 1)]
		public class uTireDefaultMaterialSettings : ScriptableObject
		{
			public SRPType sRPType = SRPType.Legacy;
			public DirectionType directionType = DirectionType.Base;
			public ShaderType shaderFeatures = ShaderType.standard;
			public bool instancing = false;

			public void SetSettings(SRPType _srpType, DirectionType _directionType, ShaderType _shaderFeatures, bool _instancing)
			{
				sRPType = _srpType;
				directionType = _directionType;
				shaderFeatures = _shaderFeatures;
				instancing = _instancing;
			}

			static uTireDefaultMaterialSettings _instance;
			public static uTireDefaultMaterialSettings Instance
			{
				get
				{
					if (_instance == null)
					{
						Resources.LoadAll("", typeof(uTireDefaultMaterialSettings));
						_instance = Resources.FindObjectsOfTypeAll<uTireDefaultMaterialSettings>().FirstOrDefault();
#if UNITY_EDITOR
						if (_instance == null)
						{
							uTireDefaultMaterialSettings asset = CreateInstance<uTireDefaultMaterialSettings>();

							if( !UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
							{
								UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
							}
							UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/Resources/uTireDefaultMaterialSettings.asset");
							UnityEditor.AssetDatabase.SaveAssets();
							UnityEditor.AssetDatabase.Refresh();
						}
#endif
						Resources.LoadAll("", typeof(uTireDefaultMaterialSettings));
						_instance = Resources.FindObjectsOfTypeAll<uTireDefaultMaterialSettings>().FirstOrDefault();
					}

					return _instance;
				}
			}
		}

		public enum DirectionType
		{
			Base,
			Collision3D
		}

		public enum ShaderType
		{
			tessellation,
			standard,
			simple
		}

		public enum SRPType
		{
			Legacy,
			LWRP,
			HDRP
		}
	}
}