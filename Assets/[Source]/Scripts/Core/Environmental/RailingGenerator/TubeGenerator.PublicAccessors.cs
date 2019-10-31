using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UltEvents;

#if UNITY_EDITOR
using UnityEditor;
#endif

//using Curve.Utility;

namespace Curve
{
    public partial class TubeGenerator : MonoBehaviour
    {
        /*
        #region Public methods and accessors

        public void UpdateTransform(Transform transform)
        {
            this.transform = transform;
        }

        public int NumPoints
        {
            get { return localPoints.Length; }
        }

        public Vector3 GetTangent(int index)
        {
            return MathUtility.TransformDirection(localTangents[index], transform, space);
        }

        public Vector3 GetNormal(int index)
        {
            return MathUtility.TransformDirection(localNormals[index], transform, space);
        }

        public Vector3 GetPoint(int index)
        {
            return MathUtility.TransformPoint(localPoints[index], transform, space);
        }

        /// Gets point on path based on distance travelled.
        public Vector3 GetPointAtDistance(
            float dst,
            EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            float t = dst / length;
            return GetPointAtTime(t, endOfPathInstruction);
        }

        /// Gets forward direction on path based on distance travelled.
        public Vector3 GetDirectionAtDistance(
            float dst,
            EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            float t = dst / length;
            return GetDirection(t, endOfPathInstruction);
        }

        /// Gets normal vector on path based on distance travelled.
        public Vector3 GetNormalAtDistance(
            float dst,
            EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            float t = dst / length;
            return GetNormal(t, endOfPathInstruction);
        }

        /// Gets a rotation that will orient an object in the direction of the path at this point, with local up point along the path's normal
        public Quaternion GetRotationAtDistance(
            float dst,
            EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            float t = dst / length;
            return GetRotation(t, endOfPathInstruction);
        }

        /// Gets point on path based on 'time' (where 0 is start, and 1 is end of path).
        public Vector3 GetPointAtTime(float t, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var data = CalculatePercentOnPathData(t, endOfPathInstruction);
            return Vector3.Lerp(GetPoint(data.previousIndex), GetPoint(data.nextIndex), data.percentBetweenIndices);
        }

        /// Gets forward direction on path based on 'time' (where 0 is start, and 1 is end of path).
        public Vector3 GetDirection(float t, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var data = CalculatePercentOnPathData(t, endOfPathInstruction);
            Vector3 dir = Vector3.Lerp(localTangents[data.previousIndex], localTangents[data.nextIndex],
                data.percentBetweenIndices);
            return MathUtility.TransformDirection(dir, transform, space);
        }

        /// Gets normal vector on path based on 'time' (where 0 is start, and 1 is end of path).
        public Vector3 GetNormal(float t, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var data = CalculatePercentOnPathData(t, endOfPathInstruction);
            Vector3 normal = Vector3.Lerp(localNormals[data.previousIndex], localNormals[data.nextIndex],
                data.percentBetweenIndices);
            return MathUtility.TransformDirection(normal, transform, space);
        }

        /// Gets a rotation that will orient an object in the direction of the path at this point, with local up point along the path's normal
        public Quaternion GetRotation(float t, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var data = CalculatePercentOnPathData(t, endOfPathInstruction);
            Vector3 direction = Vector3.Lerp(localTangents[data.previousIndex], localTangents[data.nextIndex],
                data.percentBetweenIndices);
            Vector3 normal = Vector3.Lerp(localNormals[data.previousIndex], localNormals[data.nextIndex],
                data.percentBetweenIndices);
            return Quaternion.LookRotation(MathUtility.TransformDirection(direction, transform, space),
                MathUtility.TransformDirection(normal, transform, space));
        }

        /// Finds the closest point on the path from any point in the world
        public Vector3 GetClosestPointOnPath(Vector3 worldPoint)
        {
            TimeOnPathData data = CalculateClosestPointOnPathData(worldPoint);
            return Vector3.Lerp(GetPoint(data.previousIndex), GetPoint(data.nextIndex), data.percentBetweenIndices);
        }

        /// Finds the 'time' (0=start of path, 1=end of path) along the path that is closest to the given point
        public float GetClosestTimeOnPath(Vector3 worldPoint)
        {
            TimeOnPathData data = CalculateClosestPointOnPathData(worldPoint);
            return Mathf.Lerp(times[data.previousIndex], times[data.nextIndex], data.percentBetweenIndices);
        }

        /// Finds the distance along the path that is closest to the given point
        public float GetClosestDistanceAlongPath(Vector3 worldPoint)
        {
            TimeOnPathData data = CalculateClosestPointOnPathData(worldPoint);
            return Mathf.Lerp(cumulativeLengthAtEachVertex[data.previousIndex],
                cumulativeLengthAtEachVertex[data.nextIndex], data.percentBetweenIndices);
        }

        #endregion
        */
    }
}
