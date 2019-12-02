using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TSD.uTireSettings;

namespace TSD.uTireRuntime
{
	[ExecuteInEditMode]
	public class uTireWorldSpaceBehaviour : MonoBehaviour
	{
		public float speed = 35f;
		public float speedNoCollision = 75f;
		public int rayCount = -1;
		public int rayRowCount = -1;
		public float rayLength = 0.08f;
		public float radius = 0f;

		public float minRadius = 0f; //clamp the end pos to this so the wheel cant look like a baloon
		[Range(0f, 360f)]
		public float angle = 360f;
		[Range(0f, 360f)]
		public float offsetAngle = 0f;

		//material instanced properties
		public float matStrength = 1f;// { get; set; }
		public float matDistance = 1f;// { get; set; }  //distanceCheckMultiplier

		public Vector3 originOffset;

	//	public Vector3 startPositionOffset = new Vector3(0f, 1f, 0f);
		public Vector3 castDirectionMax = new Vector3(0f, 1f, 0f);

		public Vector3 offsetPos = new Vector3(0, 0, 0);

		public EditorData editorData { get; set; }

		/// <summary>
		/// When changed it'll automatically calculate the multiplier on the material to get a similar result despite not having the same amount of rays
		/// Only modify this value from uTireWorldSpaceCollisionEditor
		/// </summary>
		public float rayCountEditor
		{
			get
			{
				return rayCount;
			}
			set
			{
				if(value == rayCount) { return; }
			//	float multiplier = value / rayCount * rayRowCount;
				rayCount = Mathf.Clamp((int)value, 1, 64);
			//	rayCount = Mathf.Round(value);
			//	renderer.sharedMaterial.SetFloat("_distance", renderer.sharedMaterial.GetFloat("_distance") * multiplier);
				vectorPositions = new Vector4[shaderArraySize]; //reset the array otherwise the old values will still be there if we downsize it
			}
		}

		public float rayRowCountEditor
		{
			get
			{
				return rayRowCount;
			}
			set
			{
				if (value != rayRowCount) {
					rayRowCount = (int)value;
					vectorPositions = new Vector4[shaderArraySize]; //reset the array otherwise the old values will still be there if we downsize it
					return;
				}
			}
		}

		MaterialPropertyBlock materialProperty;
		Vector4[] vectorPositions;
		new Renderer renderer;

		static Transform _dummyTransform;
		static Transform dummyTransform { get {
				if (_dummyTransform == null)
				{
					var go = GameObject.Find("uTire3DCollisionDummyTransform");
					if (go == null)
					{
						_dummyTransform = new GameObject("uTire3DCollisionDummyTransform").transform;
					}
					else { _dummyTransform = go.transform; }
				}
				return _dummyTransform;
			} }

		//public LayerMask layerMask;

		public float uniformScale { get; private set; }

		[HideInInspector]
		public static int shaderArraySize = 320;

		public static bool drawDebug = true;
		uTireWorldSpaceDebugData[] debugData;

		private void Awake()
		{
			if(rayCount == -1 || rayRowCount == -1)
			{
				setDefaultSettings();
			}
		}

		[ContextMenu("start")]
		void Start()
		{
			debugData = new uTireWorldSpaceDebugData[shaderArraySize];
			renderer = GetComponent<Renderer>();
			vectorPositions = new Vector4[shaderArraySize];
			materialProperty = new MaterialPropertyBlock();

			var bound = new Bounds(transform.position, Vector3.zero);
			bound.Encapsulate(renderer.bounds);
			originOffset = transform.InverseTransformPoint(bound.center);
		}

#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			if (UnityEditor.Selection.activeGameObject != gameObject) { return; }
			editorSafetyChecks();
			Color originalHandleColor = UnityEditor.Handles.color;
			if (drawDebug)
			{
				foreach (var data in debugData)
				{
					//	Debug.DrawLine(data.startPos, data.endPos, data.lineColor);
					if (data.endPos.w > 0)
					{
						UnityEditor.Handles.Label(data.endPos, data.endPos.w.ToString());
					}
					UnityEditor.Handles.color = data.lineColor;
					UnityEditor.Handles.DrawDottedLine(data.startPos, data.endPos, 4f);
				}
				float scaledRadius = radius * uniformScale ;
				float scaledRay = rayLength ;
				float scaledMinRadius = minRadius ;

				Vector3 pos = transform.position + transform.TransformVector(originOffset);

				UnityEditor.Handles.Label(pos + transform.up * scaledRadius, "radius");
				UnityEditor.Handles.color = Color.yellow;
				UnityEditor.Handles.DrawWireDisc(pos, transform.right, scaledRadius);
				
				UnityEditor.Handles.color = Color.green;
				UnityEditor.Handles.DrawWireDisc(pos, transform.right, scaledRadius + scaledRay/ 2);

				UnityEditor.Handles.Label(pos + transform.up * (scaledRadius + scaledMinRadius), "Min Radius");
				UnityEditor.Handles.color = Color.cyan;
				UnityEditor.Handles.DrawWireDisc(pos, transform.right, scaledRadius + scaledMinRadius);

				UnityEditor.Handles.Label(pos + transform.up * (scaledRadius + scaledRay), "Ray Length");
				UnityEditor.Handles.color = Color.green;
				UnityEditor.Handles.DrawWireDisc(pos, transform.right, scaledRadius + scaledRay);

				
			}
			UnityEditor.Handles.color = originalHandleColor;
		}
#endif
		
		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
			if (drawDebug) { clearDebugData(); }
#endif
			uniformScale = transform.lossyScale.x;
			float scaledRayLength = rayLength * uniformScale;
		
			dummyTransform.position = transform.position + transform.TransformVector(originOffset);
			dummyTransform.right = transform.right;
		//	dummyTransform.position += dummyTransform.TransformPoint(originOffset);

		//	var invertedStartPositionOffset = new Vector3(-startPositionOffset.x, startPositionOffset.y, startPositionOffset.z) * uniformScale;
			var invertedCastDirectionMax = new Vector3(-castDirectionMax.x, castDirectionMax.y, castDirectionMax.z) * uniformScale;
			for (float x = 0f; x < rayRowCount; x++)
			{
				float t = x / (rayRowCount -1);
				if( rayRowCount == 1)
				{
					t = .5f;
				}
				var transformedPosOffset = dummyTransform.TransformDirection( offsetPos * uniformScale);
				var lerpedPosition =  Vector3.Lerp(transformedPosOffset, -transformedPosOffset, t);
			//	var lerpedDirection = Vector3.Lerp(startPositionOffset, invertedStartPositionOffset, t);
			//	lerpedDirection = Vector3.up;
				var lerpedCastDirection = Vector3.Lerp(invertedCastDirectionMax, castDirectionMax, t);
				for (float i = 0f; i < rayCount; i++)
				{
					var rot = Quaternion.AngleAxis(angle * ((i + 1) / rayCount) + offsetAngle, -Vector3.right);
					//	var rot2 = Quaternion.AngleAxis(360f * ((i + 1) / rayCount), outdirCastDirection);
					// that's a local direction vector that points in forward direction but also 45 upwards.
					var lDirection = rot * lerpedCastDirection;
					//local space, we want world space
					var outDir = dummyTransform.TransformDirection(rot * Vector3.up).normalized; 
					Vector3 dir = dummyTransform.TransformDirection(lDirection).normalized * scaledRayLength;

				//	outDir = (rot * lerpedDirection).normalized;
				//	dir = lDirection.normalized * rayLength;

					var startPos = dummyTransform.position + lerpedPosition + outDir * radius * uniformScale;

					Ray ray = new Ray(startPos, dir);
					RaycastHit hit = new RaycastHit();

					int index = Mathf.Clamp((int)x * rayCount + (int)i, 0, shaderArraySize);
					Vector4 temp;
					if (Physics.Raycast(ray, out hit, scaledRayLength, uTireSettings.uTireGlobalSettings.Instance.phyicsLayermask))
					{
						var dist = Mathf.Max( hit.distance, minRadius * uniformScale);
						temp = hit.point + hit.normal * (-(scaledRayLength * 2f) * (dist / scaledRayLength));
					//	temp = hit.point + outDir * ((rayLength * 2f) * (dist / rayLength));
						temp = new Vector4(temp.x, temp.y, temp.z, (1f - dist / scaledRayLength));// rayLength / hit.distance / rayLength);
						//	temp = hit.point + outDir * ( (rayLength * 2f) * (hit.distance / rayLength)); //use ray direction instead of hit normal?
						//	Vector3 localPosition = transform.InverseTransformPoint(temp);
					//	temp = new Vector4(temp.x, temp.y, temp.z, ( 1f - hit.distance / rayLength));// rayLength / hit.distance / rayLength);
					//	hitPoints.Add(temp);
					//	vectorPositions[index] = Vector4.Lerp(vectorPositions[index] , temp, Time.deltaTime * speed);
					//	Debug.DrawLine(startPos, temp, Color.red);
					}
					else
					{
						temp = startPos + dir;
						temp.w = 0f;
					//	Vector3 localPosition = transform.InverseTransformPoint(temp);
						
					//	vectorPositions[index] = new Vector4(temp.x, temp.y, temp.z, 0f);
					//	Debug.DrawLine(startPos, temp, Color.white);
					}

					vectorPositions[index] = Vector4.Lerp(vectorPositions[index], temp, temp.w == 0f ? Time.deltaTime * uTireGlobalSettings.Instance.animationSpeedOnCollision : Time.deltaTime * uTireGlobalSettings.Instance.animationSpeedOnCollision);
#if UNITY_EDITOR
					editorSafetyChecks();
					if (drawDebug)
					{
						debugData[index] = new uTireWorldSpaceDebugData(startPos, vectorPositions[index], temp.w == 0f ? Color.white : Color.red);
						//	Debug.DrawLine(startPos, temp, temp.w == 0f ? Color.white : Color.red);
					}
#endif
				}
			}
			
			materialProperty.SetVectorArray("positionsArray", vectorPositions);
			//todo
			//only send these when they changed
			materialProperty.SetFloat("_distanceCheckStrength", matStrength);
			materialProperty.SetFloat("_distanceCheckMultiplier", matDistance);

			renderer.SetPropertyBlock(materialProperty);
		}
		void editorSafetyChecks()
		{
			if(materialProperty == null || vectorPositions == null || renderer == null || debugData == null) { Start(); }
		}

		void clearDebugData()
		{
			debugData = new uTireWorldSpaceDebugData[vectorPositions.Length];
		}

		public void pasteComponentData(uTireWorldSpaceBehaviour source)
		{
			speed = source.speed;
			speedNoCollision = source.speedNoCollision;
			rayCount = source.rayCount;
			rayRowCount = source.rayRowCount;
			rayLength = source.rayLength;
			radius = source.radius;

			minRadius = source.minRadius; 
			angle = source.angle;
		
			offsetAngle = source.offsetAngle;
			matStrength = source.matStrength;
			matDistance = source.matDistance;
			originOffset = source.originOffset;

		//	startPositionOffset = source.startPositionOffset;
			castDirectionMax = source.castDirectionMax;
			offsetPos = source.offsetPos;

			if(source.editorData == null) { return; }
			if(editorData == null)
			{
				editorData = new EditorData();
			}
			Debug.Log(source.editorData);
			editorData.selectedMeshSide = source.editorData.selectedMeshSide;
		}

		void setDefaultSettings()
		{
			rayCount	= uTireGlobalSettings.Instance.rayCount;
			rayRowCount = uTireGlobalSettings.Instance.rayRingCount;
			angle		= uTireGlobalSettings.Instance.rayAngle;
			offsetAngle = uTireGlobalSettings.Instance.rayAngleOffset;
		}
	}
	[System.Serializable]
	public class EditorData
	{
		public int selectedMeshSide { get; set; }

		public void Init(Bounds meshBounds)
		{
			var arr = new float[] { meshBounds.extents.x, meshBounds.extents.y, meshBounds.extents.z };
			selectedMeshSide = arr.Select((axis, index) => new { axis, index }).First(element => element.axis == Mathf.Max(arr)).index;
		}
	}

	internal struct uTireWorldSpaceDebugData
	{
		public Vector4 startPos;
		public Vector4 endPos;
		public Color lineColor;

		public uTireWorldSpaceDebugData(Vector4 _startPos, Vector4 _endPos, Color _lineColor)
		{
			startPos = _startPos;
			endPos = _endPos;
			lineColor = _lineColor;
		}
	}
}