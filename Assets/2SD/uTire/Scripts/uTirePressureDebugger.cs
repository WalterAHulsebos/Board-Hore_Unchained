namespace TSD
{
	namespace uTireRuntime
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;

		public class uTirePressureDebugger : MonoBehaviour
		{
			public Camera cam;

			public KeyCode toggleDebugger = KeyCode.F5;

			public KeyCode pauseTimeKey = KeyCode.Q;
			public KeyCode slowTime = KeyCode.R;
			
			GUIStyle centeredStyle;
			bool state = true;

			// Use this for initialization
			void Start()
			{
				if (cam == null) { cam = Camera.main; }
			}

			// Update is called once per frame
			void Update()
			{
				if (Input.GetKeyDown(toggleDebugger))
				{
					state = !state;
				}
				if(Input.GetKeyDown(pauseTimeKey))
				{
					Time.timeScale = Time.timeScale > 0f ? 0f : 1f;
				}

				if (Input.GetKeyDown(slowTime))
				{
					Time.timeScale = Time.timeScale == .3f ? 1f : .3f;
				}
			}

			void OnGUI()
			{
				if (!state) { return; }
				centeredStyle = GUI.skin.GetStyle("Label");
				centeredStyle.alignment = TextAnchor.MiddleCenter;

				foreach (var vehicle in uTireManager.Instance.vehicles)
				{
					Rect myRect;
					foreach (var wmc in vehicle.wheels)
					{
						GUI.color = Color.red;
						if (wmc.flatness < .6f)
						{
							GUI.color = Color.yellow;
						}
						if (wmc.flatness < .3f)
						{
							GUI.color = Color.green;
						}

						Vector3 screenPos = cam.WorldToScreenPoint(wmc.meshRenderer.transform.position);
						if (wmc.meshRenderer.isVisible)
						{
							myRect = new Rect(screenPos.x - 30, Screen.height - screenPos.y, 60, 20);
							GUI.Box(myRect, "");
							GUI.Label(myRect, string.Format("{0:0.00}", wmc.flatness), centeredStyle);
						}
						
					}
					
					myRect = new Rect(5, Screen.height - 65, 180, 60);
					GUI.Box(myRect, "");
					GUI.color = Color.white;
					GUI.Label(myRect, string.Format("Press {0} to pause/start time \nPress {1} to slow down time", pauseTimeKey.ToString(), slowTime.ToString()));
				}
			}

			public void toggleState()
			{
				state = !state;
			}
		}
	}
}