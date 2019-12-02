namespace TSD
{
	namespace uTireExamples
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;

		/// <summary>
		/// Will change every material's shader to 
		/// </summary>
		public class uTireShaderChanger : MonoBehaviour
		{
			[Tooltip("If true every material using the deformation shader will switch to the runtime variant on start")]
			public bool changetoRuntime = true;
			[Tooltip("If ON tessellation shader variant will be used. Warning, Tessellation won't work with GPU Instancing!")]
			public bool tessellation = false;
			[Tooltip("If true materials will switch back on quit")]
			public bool resetOnQuit = true;

			public bool showDebugMessages = true;

			string runtimeShader = "TSD/Tire Vertex Deformation";
			string runtimeShaderTessellation = "TSD/Tire Vertex Deformation Tessellation";
			string debugShader = "TSD/Tire Vertex Deformation Debug";
			string debugShaderTessellation = "TSD/Tire Vertex Deformation Debug Tessellation";
			Shader rt, ds;

			// Use this for initialization
			void Start()
			{
				rt = Shader.Find(runtimeShader);
				ds = Shader.Find(debugShader);
				changeShaders(changetoRuntime);
			}

			void changeShaders(bool newState)
			{
				List<Renderer> renderers = new List<Renderer>();
				foreach (var vehicle in TSD.uTireRuntime.uTireManager.Instance.vehicles)
				{
					foreach (var wheelMeshConnection in vehicle.wheels)
					{
						if (wheelMeshConnection.meshRenderer == null)
						{
							if (showDebugMessages) { Debug.Log("wheelMesh is null, make sure to drag it in the inspector"); }
							continue;
						}
						renderers.Add(wheelMeshConnection.meshRenderer);
					}
				}
				Shader newShader = Shader.Find(newState ? runtimeShader : debugShader);
				List<Material> materialsToChange = new List<Material>();
				//Renderer[] renderers = (Renderer[])FindObjectsOfType(typeof(Renderer));
				foreach (var renderer in renderers)
				{
					foreach (var mat in renderer.sharedMaterials)
					{
						if (mat == null) { continue; }
						if (!materialsToChange.Contains(mat))
						{
							if (mat.shader == rt || mat.shader == ds)
							{
								materialsToChange.Add(mat);
							}
						}
					}
				}

				foreach (var mat in materialsToChange)
				{
					mat.shader = newShader;
				}
			}

			void OnApplicationQuit()
			{
				if (!resetOnQuit) { return; }
				changeShaders(!changetoRuntime);
			}
		}

	}
}