using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Core.Environmental.Railings
{
    public abstract class Curve 
    {

        protected List<Vector3> Points;
        protected bool Closed;

        protected float[] CacheArcLengths;
        private bool _needsUpdate;

        public Curve(List<Vector3> points, bool closed = false) 
        {
            this.Points = points;
            this.Closed = closed;
        }

        protected abstract Vector3 GetPoint(float t);

        protected virtual Vector3 GetTangent(float t) 
        {
            const float __DELTA = 0.001f;
            float __t1 = t - __DELTA;
            float __t2 = t + __DELTA;

            // Capping in case of danger
            if (__t1 < 0f) __t1 = 0f;
            if (__t2 > 1f) __t2 = 1f;

            return (GetPoint(t: __t2) - GetPoint(t: __t1)).normalized;
        }

        public Vector3 GetPointAt(float u) 
        {
            float __t = GetUtoTmapping(u: u);
            return GetPoint(t: __t);
        }

        public Vector3 GetTangentAt(float u) 
        {
            float __t = GetUtoTmapping(u: u);
            return GetTangent(t: __t);
        }

        private float[] GetLengths(int divisions = -1) 
        {
            if (divisions <= 0) 
            {
                divisions = 100;
            }

            if (this.CacheArcLengths != null &&
                    (this.CacheArcLengths.Length == divisions + 1) &&
                    !this._needsUpdate) 
            {
                return this.CacheArcLengths;
            }

            this._needsUpdate = false;

            float[] __cache = new float[divisions + 1];
            Vector3 __last = this.GetPoint(0f);

            __cache[0] = 0f;

            float __sum = 0f;
            for (int __index = 1; __index <= divisions; __index++) 
            {
                Vector3 __current = this.GetPoint(t: 1f * __index / divisions);
                __sum += Vector3.Distance(a: __current, b: __last);
                __cache[__index] = __sum;
                __last = __current;
            }

            this.CacheArcLengths = __cache;
            return __cache;
        }

        // Given u ( 0 .. 1 ), get a t to find p. This gives you points which are equidistant
        protected float GetUtoTmapping(float u) 
        {
            float[] __arcLengths = this.GetLengths();

            int __i = 0, __il = __arcLengths.Length;

            // The targeted u distance value to get
            float __targetArcLength = u * __arcLengths[__il - 1];

            // binary search for the index with largest value smaller than target u distance
            int __low = 0, __high = __il - 1;
            float __comparison;

            while ( __low <= __high ) 
            {

                __i = Mathf.FloorToInt(f: __low + (__high - __low) / 2f);
                __comparison = __arcLengths[__i] - __targetArcLength;

                if (__comparison < 0f) 
                {
                    __low = __i + 1;
                } 
                else if (__comparison > 0f) 
                {
                    __high = __i - 1;
                } 
                else 
                {
                    __high = __i;
                    break;
                }

            }

            __i = __high;

            if (Mathf.Approximately(a: __arcLengths[__i], b: __targetArcLength)) {

                return 1f * __i / ( __il - 1 );

            }

            // we could get finer grain at lengths, or use simple interpolation between two points

            float __lengthBefore = __arcLengths[__i];
            float __lengthAfter = __arcLengths[__i + 1];

            float __segmentLength = __lengthAfter - __lengthBefore;

            // determine where we are between the 'before' and 'after' points

            float __segmentFraction = ( __targetArcLength - __lengthBefore ) / __segmentLength;

            // add that fractional amount to t
            float __t = 1f * (__i + __segmentFraction) / (__il - 1);

            return __t;
        }

        public List<FrenetFrame> ComputeFrenetFrames (int segments, bool closed) {
            Vector3 __normal = new Vector3();

            Vector3[] __tangents = new Vector3[segments + 1];
            Vector3[] __normals = new Vector3[segments + 1];
            Vector3[] __binormals = new Vector3[segments + 1];

            float __u, __theta;

            // compute the tangent vectors for each segment on the curve
            for (int __i = 0; __i <= segments; __i++) {
                __u = (1f * __i) / segments;
                __tangents[__i] = GetTangentAt(u: __u).normalized;
            }

            // select an initial normal vector perpendicular to the first tangent vector,
            // and in the direction of the minimum tangent xyz component

            __normals[0] = new Vector3();
            __binormals[0] = new Vector3();

            float __min = float.MaxValue;
            float __tx = Mathf.Abs(f: __tangents[0].x);
            float __ty = Mathf.Abs(f: __tangents[0].y);
            float __tz = Mathf.Abs(f: __tangents[0].z);
            if (__tx <= __min) 
            {
                __min = __tx;
                __normal.Set(1, 0, 0);
            }
            if (__ty <= __min) 
            {
                __min = __ty;
                __normal.Set(0, 1, 0);
            }
            if (__tz <= __min) 
            {
                __normal.Set(0, 0, 1);
            }

            Vector3 __vec = Vector3.Cross(lhs: __tangents[0], rhs: __normal).normalized;
            __normals[0] = Vector3.Cross(lhs: __tangents[0], rhs: __vec);
            __binormals[0] = Vector3.Cross(lhs: __tangents[0], rhs: __normals[0]);

            // compute the slowly-varying normal and binormal vectors for each segment on the curve

            for (int __i = 1; __i <= segments; __i++) 
            {
                // copy previous
                __normals[__i] = __normals[__i - 1];
                __binormals[__i] = __binormals[__i - 1];

                // Rotation axis
				Vector3 __axis = Vector3.Cross(lhs: __tangents[__i - 1], rhs: __tangents[__i]);
                if (__axis.magnitude > float.Epsilon) 
                {
                    __axis.Normalize();

                    float __dot = Vector3.Dot(lhs: __tangents[__i - 1], rhs: __tangents[__i]);

                    // clamp for floating pt errors
                    __theta = Mathf.Acos(f: Mathf.Clamp(value: __dot, -1f, 1f));

                    __normals[__i] = Quaternion.AngleAxis(angle: __theta * Mathf.Rad2Deg, axis: __axis) * __normals[__i];
                }

                __binormals[__i] = Vector3.Cross(lhs: __tangents[__i], rhs: __normals[__i]).normalized;
            }

            // if the curve is closed, postprocess the vectors so the first and last normal vectors are the same

            if (closed) 
            {
                __theta = Mathf.Acos(f: Mathf.Clamp(value: Vector3.Dot(lhs: __normals[0], rhs: __normals[segments]), -1f, 1f));
                __theta /= segments;

                if (Vector3.Dot(lhs: __tangents[0], rhs: Vector3.Cross(lhs: __normals[0], rhs: __normals[segments])) > 0f) {
                    __theta = - __theta;
                }

                for (int __i = 1; __i <= segments; __i++) 
                {
                    __normals[__i] = (Quaternion.AngleAxis(angle: Mathf.Deg2Rad * __theta * __i, axis: __tangents[__i]) * __normals[__i]);
                    __binormals[__i] = Vector3.Cross(lhs: __tangents[__i], rhs: __normals[__i]);
                }
            }

            List<FrenetFrame> __frames = new List<FrenetFrame>();
            int __n = __tangents.Length;
            for(int __i = 0; __i < __n; __i++) 
            {
                FrenetFrame __frame = new FrenetFrame(t: __tangents[__i], n: __normals[__i], bn: __binormals[__i]);
                __frames.Add(item: __frame);
            }
            return __frames;
        }
        
    }
}