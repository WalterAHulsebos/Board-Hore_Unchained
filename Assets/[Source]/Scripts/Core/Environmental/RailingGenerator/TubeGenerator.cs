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
#endif

using JetBrains.Annotations;

using Debug = CommonGames.Utilities.CGTK.CGDebug;

#endif

namespace Curve
{
	using CommonGames.Utilities.Extensions;

	public enum CurveType
	{
		CatmullRom
	};

	public partial class TubeGenerator : MonoBehaviour 
	{
		#region Variables
		
		public UltEvent refresh;
		
		public List<Vector3> Points => points;

		public float Length { get; protected set; } = 0f;
		
		public float Units { get; protected set; } = 0f;

		[ReadOnly]
		public List<Vector3> generatedPoints;

		[SerializeField] protected CurveType type = CurveType.CatmullRom;
		
		[SerializeField] protected List<Vector3> points;
		
		[SerializeField] protected float radiusSize = 0.1f;
		
		[SerializeField] protected bool 
			pointDebug = true, 
			tangentDebug = true, 
			frameDebug = false,
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
			if(points.Count <= 0) 
			{
				points = new List<Vector3>() 
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
			=> generatedPoints.TwoClosestWithIndexes(point);

		[PublicAPI]
		public (Vector3 startPosition, Vector3 startDirection) GetInterpolatedStartPointAndDirection(in Vector3 position)
		{
			//Debug.Log("Check 01");
			
			(Vector3 __closestPoint, int __closestIndex, Vector3 __secondClosestPoint, int __secondClosestIndex) = GetTwoClosestPointsWithIndexes(position);
			
			//Debug.Log("Check 04");

			float __distanceToClosest = position.DistanceTo(__closestPoint);
			float __distanceToSecondClosest = position.DistanceTo(__secondClosestPoint);

			float __combinedDistance = __distanceToClosest + __distanceToSecondClosest;

			float __percent = (__distanceToClosest / __combinedDistance);

			float __lerpedPoint = Mathf.Lerp(__closestIndex, __secondClosestIndex, __percent);

			//Debug.Log($"__closestIndex = {__closestIndex}");
			//Debug.Log($"__secondClosestIndex = {__secondClosestIndex}");
			//Debug.Log($"__percent = {__percent}");
			//Debug.Log($"__lerpedPoint = {__lerpedPoint}");

			float __barPos = (__lerpedPoint / Length);

			Vector3 __calculatedStartPosition = Curve.GetPointAt(__barPos);
			
			//Debug.Log("Check 05");

			return (startPosition: __calculatedStartPosition, startDirection: Vector3.forward);
		}
		
		/// <summary> Gets the appropriate start point on the tube for your position. </summary>
		[PublicAPI]
		public float GetStartPoint(in Vector3 position)
		{
			(Vector3 __closestPoint, int __closestIndex, Vector3 __secondClosestPoint, int __secondClosestIndex) = GetTwoClosestPointsWithIndexes(position);

			float __distanceToClosest = position.DistanceTo(__closestPoint);
			float __distanceToSecondClosest = position.DistanceTo(__secondClosestPoint);

			float __combinedDistance = __distanceToClosest + __distanceToSecondClosest;

			float __percent = (__distanceToClosest / __combinedDistance);

			float __lerpedPoint = Mathf.Lerp(__closestIndex, __secondClosestIndex, __percent);

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
					__curve = new CatmullRomCurve(points);
					break;
				default:
					Debug.LogWarning("CurveType is not defined.");
					break;
			}
			
			Length = points.CombinedDistanceInOrder();
			
			Units = Length.FloorToInt();

			generatedPoints.Clear();
			
			for(int __index = 0; __index < Units; __index++)
			{
				if(__curve == null) continue;
				
				Vector3 __unitPos = __curve.GetPointAt(__index / (float) Units);
				
				generatedPoints.Add(__unitPos);
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
		public void AddPoint(Vector3 point) => points.Add(point);

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
			if(Points.Count < 2) return;
			if(generatedPoints.Count < 2) return;

			//const float __DELTA = 0.01f;
			float __deltaUnit = radiusSize * 2f;
			//int __count = Mathf.FloorToInt(1f / __DELTA);

			int __count = Length.FloorToInt() + 1;

			float __delta = Length;

			if (Frames == null) 
			{
				Frames = Curve.ComputeFrenetFrames(__count, false);
			}

			Gizmos.matrix = transform.localToWorldMatrix;
			
			for (int __index = 0; __index < __count; __index++)
			{
				float __t = __index / Length; // * __DELTA;

				if(__t > 1)
				{
					__t = 1;
				}

				Vector3 __point = Curve.GetPointAt(__t);

				if (pointDebug) 
				{
					Gizmos.color = Color.white;
					Gizmos.DrawSphere(__point, radiusSize);
				}

				if (tangentDebug) 
				{
					Vector3 __t1 = Curve.GetTangentAt(__t);
					Vector3 __n1 = (__t1 + Vector3.one) * 0.5f;
					
					Gizmos.color = new Color(__n1.x, __n1.y, __n1.z);
					Gizmos.DrawLine(__point, __point + __t1 * __deltaUnit);
				}

				if (indexDebug)
				{
					#if UNITY_EDITOR
					Handles.matrix = transform.localToWorldMatrix;
					Handles.color = Color.black;
					Handles.Label(__point, __index.ToString());
					#endif
				}

				if (!frameDebug) continue;
				
				FrenetFrame __frame = Frames[__index];

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(__point, __point + __frame.Tangent * __deltaUnit);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(__point, __point + __frame.Normal * __deltaUnit);

				Gizmos.color = Color.red;
				Gizmos.DrawLine(__point, __point + __frame.Binormal * __deltaUnit);
			}
		}
		
		#endregion
		
	}

	[CustomEditor (typeof(TubeGenerator))]
	public class TubeGeneratorEditor : OdinEditor 
	{
		private Quaternion _handle;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI ();

			if (!GUILayout.Button("Add")) return;
			
			TubeGenerator __tester = target as TubeGenerator;
			
			if (__tester == null) return;
			
			Vector3 __last = __tester.Points[__tester.Points.Count - 1];
			__tester.AddPoint(__last);
		}

		private void OnSceneGUI()
		{
			TubeGenerator __target = target as TubeGenerator;
			
			if (__target == null) return;
			
			List<Vector3> __points = __target.Points;

			Transform __testerTransform = __target.transform;
			
			_handle = Tools.pivotRotation == PivotRotation.Local ? __testerTransform.rotation : Quaternion.identity;
			
			Transform __transform = __testerTransform;

			for(int __index = 0, __n = __points.Count; __index < __n; __index++) 
			{
				Vector3 __point = __transform.TransformPoint(__points[__index]);
				
				EditorGUI.BeginChangeCheck();
				
				__point = Handles.DoPositionHandle(__point, _handle);

				if (!EditorGUI.EndChangeCheck()) continue;
				
				Undo.RecordObject(__target, "Point");
				
				__target.refresh?.Invoke();
				
				EditorUtility.SetDirty(__target);
				__points[__index] = __transform.InverseTransformPoint(__point);

				__target.Initialize();
			}
		}

	}
	
}