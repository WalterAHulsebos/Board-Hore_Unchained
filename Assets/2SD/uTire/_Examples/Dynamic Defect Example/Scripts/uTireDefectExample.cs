namespace TSD
{
	namespace uTireExamples
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;
		using System;
		using System.Linq;
		using uTireRuntime;

		public class uTireDefectExample : MonoBehaviour
		{
			
			//drag in the vehicle you want to modify
			public Rigidbody vehiclesRigidbody;
			//the speed the tire will deflate when clicked on the button
			public float deflateSpeed = 1f;

			//SFX
			public bool useAudio;
			public AudioClip deflateSFX;
			Transform deflateTransform;
			AudioSource deflateSFXSource;

			Vehicle vehicle;
			float savedTirePressure;
			public static uTireDefectExample instance;

			bool state = true;

			void Awake()
			{
				instance = this;
			}
			void Start()
			{

				vehicle = uTireManager.GetVehicle(vehiclesRigidbody);
				initSFX();
			}

			public void SetVehicle(Vehicle newVehicle)
			{
				vehicle = newVehicle;
			}

			void initSFX()
			{
				GameObject go = new GameObject("Audio Source");
				deflateTransform = go.transform;
				deflateTransform.SetParent(transform);

				deflateSFXSource  = go.AddComponent<AudioSource>();
				deflateSFXSource.spatialBlend = 1f;
				deflateSFXSource.clip = deflateSFX;
			}
			
			void setActiveWheels(Rigidbody m_vehicleRigidbody)
			{
				vehicle = uTireManager.GetVehicle(m_vehicleRigidbody);
			}

			public IEnumerator changePressure(UnityEngine.Object m_wc)
			{
				if (m_wc == null) { yield break;}
				WheelMeshConnection wmc = uTireManager.GetWheelMeshConnection(m_wc);
				float t = 0f;
				float targetPressure = Mathf.Round(wmc.tirePressure) > 0 ? 0f : 1f;
				float startPressure = wmc.tirePressure;
				if (useAudio && !deflateSFXSource.isPlaying) 
				{ 
					deflateTransform.position = wmc.iWheel.wheelTransform.position;
					deflateSFXSource.Play();
				}


				while (t < 1f)
				{
					t += Time.deltaTime * deflateSpeed;
					wmc.SetPressure(Mathf.Lerp(startPressure, targetPressure, t));
					yield return new WaitForEndOfFrame();
				}
				if (useAudio) { deflateSFXSource.Stop(); }
			}
			
			public void ToggleState()
			{
				state = !state;
			}
			
			#region GUI

			void OnGUI()
			{
				if (vehicle == null || !state) { return; }
				GUI.Box(new Rect(10, 30, 160, 166), "");
				GUILayout.BeginArea(new Rect(10, 30, 160, 400));
				GUILayout.Label("Inflate/Deflate");

				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Front Left"))
				{
					StartCoroutine(changePressure(vehicle.wheels[0].iWheel.wheelObject));
				}
				if ( GUILayout.Button("Front Right"))
				{
					StartCoroutine(changePressure(vehicle.wheels[1].iWheel.wheelObject));
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Rear Left"))
				{
					StartCoroutine(changePressure(vehicle.wheels[2].iWheel.wheelObject));
				}
				if (GUILayout.Button( "Rear Right"))
				{
					StartCoroutine(changePressure(vehicle.wheels[3].iWheel.wheelObject));
				}
				GUILayout.EndHorizontal();

				vehicle.SetPressureMultiplier(GUILayout.HorizontalSlider((float)Math.Round((double)vehicle.tirePressureMultiplier, 1), 0f, 2f));


				if (GUILayout.Button("Toggle Tire Deformation"))
				{
					uTireManager.Instance.updateMaterial = !uTireManager.Instance.updateMaterial;
					if (!uTireManager.Instance.updateMaterial)
					{
						savedTirePressure = uTireManager.Instance.vehicles[0].tirePressureMultiplier;
						foreach (var item in uTireManager.Instance.vehicles)
						{
							item.tirePressureMultiplier = 2f;
							//force a single update
							uTireManager.Instance.updateWheels();
						}
					}
					{
						foreach (var item in uTireManager.Instance.vehicles)
						{
							item.tirePressureMultiplier = savedTirePressure;
						}
					}
				}

				GUILayout.Box("Vehicle Turn: " + vehicle.wheels[0].turnAngle);
				GUILayout.Box("Vehicle Speed Ratio: " + vehicle.speedRatio);
				GUILayout.EndArea();
			}

			#endregion
		}
	}
}