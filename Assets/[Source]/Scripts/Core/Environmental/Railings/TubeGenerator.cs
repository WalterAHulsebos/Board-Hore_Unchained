using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UltEvents;

using CommonGames;
using CommonGames.Utilities;
using CommonGames.Utilities.Extensions;
using CommonGames.Utilities.CGTK;

#if UNITY_EDITOR
using UnityEditor;

#if ODIN_INSPECTOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Serialization.Editor;

using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;

#endif

using JetBrains.Annotations;

//using Debug = CommonGames.Utilities.CGTK.CGDebug;

#endif

namespace Core.Environmental.Railings
{
	using CommonGames.Utilities.Extensions;

	public enum CurveType
	{
		CatmullRom
	};

	public partial class TubeGenerator : MonoBehaviour 
	{
		#region Variables

		public bool rebakeAutomatically = true;
		
		public UltEvent refresh;

		[ShowInInspector]
		public List<Vector3> Points
		{
			get => points;
			set => points = value;
		}

		[SerializeField, HideInInspector]
		private List<Vector3> points;

		public float Length { get; protected set; } = 0f;
		
		public float Units { get; protected set; } = 0f;

		[ReadOnly]
		public List<Vector3> generatedPoints = null;

		[SerializeField] protected CurveType type = CurveType.CatmullRom;
		
		//[SerializeField] protected List<Vector3> points;
		
		[SerializeField] protected float radiusSize = 0.1f;
		
		[SerializeField] protected bool 
			pointDebug = false, 
			tangentDebug = true, 
			frameDebug = true,
			indexDebug = true;

		public Curve Curve
		{
			get;
			private set;
		}
		
		public List<FrenetFrame> Frames { get; private set; }
		
		#endregion

		#region Methods

		private void OnEnable() 
		{
			if(Points.Count <= 0) 
			{
				Points = new List<Vector3>() 
				{
					Vector3.zero,
					Vector3.up,
					Vector3.right
				};
			}
		}

		private void Start() => Initialize();

		#region Position Sampling on Bar

		private (Vector3 closestPosition, int closestIndex, Vector3 secondClosestPosition, int secondClosestIndex) GetTwoClosestPointsWithIndexes(in Vector3 point) 
			=> generatedPoints.TwoClosestWithIndexes(comparer: point);

		[PublicAPI]
		public (Vector3 startPosition, Vector3 startDirection) GetInterpolatedStartPointAndDirection(in Vector3 position)
		{
			//Debug.Log("Check 01");
			
			(Vector3 __closestPoint, int __closestIndex, Vector3 __secondClosestPoint, int __secondClosestIndex) = GetTwoClosestPointsWithIndexes(point: position);
			
			//Debug.Log("Check 04");

			float __distanceToClosest = position.DistanceTo(to: __closestPoint);
			float __distanceToSecondClosest = position.DistanceTo(to: __secondClosestPoint);

			float __combinedDistance = __distanceToClosest + __distanceToSecondClosest;

			float __percent = (__distanceToClosest / __combinedDistance);

			float __lerpedPoint = Mathf.Lerp(a: __closestIndex, b: __secondClosestIndex, t: __percent);

			//Debug.Log($"__closestIndex = {__closestIndex}");
			//Debug.Log($"__secondClosestIndex = {__secondClosestIndex}");
			//Debug.Log($"__percent = {__percent}");
			//Debug.Log($"__lerpedPoint = {__lerpedPoint}");

			float __barPos = (__lerpedPoint / Length);

			Vector3 __calculatedStartPosition = Curve.GetPointAt(u: __barPos);
			
			//Debug.Log("Check 05");

			return (startPosition: __calculatedStartPosition, startDirection: Vector3.forward);
		}
		
		/// <summary> Gets the appropriate start point on the tube for your position. </summary>
		[PublicAPI]
		public float GetStartPoint(in Vector3 position)
		{
			(Vector3 __closestPoint, int __closestIndex, Vector3 __secondClosestPoint, int __secondClosestIndex) = GetTwoClosestPointsWithIndexes(point: position);

			float __distanceToClosest = position.DistanceTo(to: __closestPoint);
			float __distanceToSecondClosest = position.DistanceTo(to: __secondClosestPoint);

			float __combinedDistance = __distanceToClosest + __distanceToSecondClosest;

			float __percent = (__distanceToClosest / __combinedDistance);

			float __lerpedPoint = Mathf.Lerp(a: __closestIndex, b: __secondClosestIndex, t: __percent);

			//Debug.Log($"__closestIndex = {__closestIndex}");
			//Debug.Log($"__secondClosestIndex = {__secondClosestIndex}");
			//Debug.Log($"__percent = {__percent}");
			//Debug.Log($"__lerpedPoint = {__lerpedPoint}");

			float __barPos = (__lerpedPoint / Length);

			return __barPos;
		}
		
		#endregion

		public void Initialize() => Curve = Build();

		public Curve Build() 
		{
			Curve __curve = default(Curve);
			switch(type) 
			{
				case CurveType.CatmullRom:
					__curve = new CatmullRomCurve(points: Points);
					break;
				default:
					Debug.LogWarning(message: "CurveType is not defined.");
					break;
			}

			Length = Points.CombinedDistanceInOrder();
			
			Units = Length.FloorToInt();

			generatedPoints?.Clear();
			
			for(int __index = 0; __index < Units; __index++)
			{
				if(__curve == null) continue;
				
				Vector3 __unitPos = __curve.GetPointAt(u: __index / (float) Units);
				
				generatedPoints.Add(item: __unitPos);
			}
			
			/*
			if (Frames == null) 
			{
				Frames = Curve.ComputeFrenetFrames(Length.FloorToInt()+1, false);
			}
			*/

			return __curve;
		}
		
		[PublicAPI]
		public void AddPoint(Vector3 point)
		{
			if(Points == null)
			{
				Points = new List<Vector3>(2);
			}
			
			Points.Add(item: point);
		}

		
		private void OnDrawGizmos() 
		{
			if(Curve == null)
			{
				Initialize();
			}

			DrawGizmos();
		}

		private void DrawGizmos() 
		{
			if(Points == null || Points.Count <= 1) return;
			
			Length = Points.CombinedDistanceInOrder();
			Units = Length.Floor();
			
			if(float.IsNaN(Units) || Units <= 0) return;
			if(float.IsNaN(Length) || Length <= 0) return;

			int __count = (int)Units;
			
			//Debug.Log(message: $"Length = {Length} Count = {__count}");

			if (Frames == null) 
			{
				Frames = Curve.ComputeFrenetFrames(segments: __count, false);
			}

			Gizmos.matrix = transform.localToWorldMatrix;
			
			// ReSharper disable once InvertIf
			for (int __index = 0; __index <= __count; __index++)
			{
				float __barPercentage = (__index / Length);
				__barPercentage = (__barPercentage <= 1)? __barPercentage : 1;

				Vector3 __point = Curve.GetPointAt(u: __barPercentage);

				if(pointDebug)
				{
					__DebugPoint(point: __point);
				}

				if(tangentDebug)
				{
					__DebugTangent(barPercentage: __barPercentage, point: __point);
				}

				if(indexDebug)
				{
					__DebugIndex(index: __index, point: __point);
				}

				if(frameDebug)
				{
					__DebugFrames(index: __index, point: __point);
				}
			}

			void __DebugPoint(in Vector3 point)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawSphere(center: point, radius: radiusSize);
			}
			
			void __DebugTangent(in float barPercentage, in Vector3 point)
			{
				Vector3 __tangent = Curve.GetTangentAt(u: barPercentage);
				Vector3 __n1 = (__tangent + Vector3.one) * 0.5f;
					
				Gizmos.color = new Color(r: __n1.x, g: __n1.y, b: __n1.z);
				Gizmos.DrawLine(@from: point, to: point + __tangent * (radiusSize * 2f));
			}
			
			void __DebugIndex(in int index, in Vector3 point)
			{
				#if UNITY_EDITOR
				Handles.matrix = transform.localToWorldMatrix;
				Handles.color = Color.black;
				Handles.Label(position: point, text: index.ToString());
				#endif
			}
			
			void __DebugFrames(in int index, in Vector3 point)
			{
				float __deltaUnit = (radiusSize * 2f);
				
				FrenetFrame __frame = Frames[index: index];

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(@from: point, to: point + __frame.Tangent * __deltaUnit);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(@from: point, to: point + __frame.Normal * __deltaUnit);

				Gizmos.color = Color.red;
				Gizmos.DrawLine(@from: point, to: point + __frame.Binormal * __deltaUnit);	
			}
		}
		
		#endregion
		
	}

	[CustomEditor (inspectedType: typeof(TubeGenerator))]
	public class TubeGeneratorEditor : OdinEditor 
	{
		private Quaternion _handle;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI ();

			if (!GUILayout.Button(text: "Add")) return;
			
			TubeGenerator __tester = target as TubeGenerator;
			
			if (__tester == null) return;

			Vector3? __lastPoint = null;
			
			if(__tester.Points != null)
			{
				int __lastIndex = __tester.Points.Count;

				if(__lastIndex > 0)
				{
					Vector3 __lastPointValue = __tester.Points[index: __lastIndex - 1];

					__lastPoint = new Vector3(x: __lastPointValue.x, y: __lastPointValue.y + 1, z: __lastPointValue.z);
				}
			}

			__tester.AddPoint(point: __lastPoint.GetValueOrDefault() );
		}

		private void OnSceneGUI()
		{
			TubeGenerator __target = target as TubeGenerator;
			
			if (__target == null) return;
			
			List<Vector3> __points = __target.Points;

			Transform __testerTransform = __target.transform;
			
			_handle = Tools.pivotRotation == PivotRotation.Local ? __testerTransform.rotation : Quaternion.identity;
			
			Transform __transform = __testerTransform;

			if(__points == null) return;
			
			for(int __index = 0, __n = __points.Count; __index < __n; __index++) 
			{
				Vector3 __point = __transform.TransformPoint(position: __points[index: __index]);
				
				EditorGUI.BeginChangeCheck();
				
				__point = Handles.DoPositionHandle(position: __point, rotation: _handle);

				if (!EditorGUI.EndChangeCheck()) continue;
				
				Undo.RecordObject(objectToUndo: __target, name: "Point");

				if(__target.rebakeAutomatically)
				{
					__target.refresh?.Invoke();
				}
				
				EditorUtility.SetDirty(target: __target);
				__points[index: __index] = __transform.InverseTransformPoint(position: __point);

				__target.Initialize();
			}
		}

	}
	
}