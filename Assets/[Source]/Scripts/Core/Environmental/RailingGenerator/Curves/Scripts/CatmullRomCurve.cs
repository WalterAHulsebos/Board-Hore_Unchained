using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curve 
{
	using CommonGames.Utilities.Extensions;

	public class CubicPoly3D 
	{
		private readonly Vector3 _c0, _c1, _c2, _c3;

		/*
         * Compute coefficients for a cubic polynomial
         *   p(s) = c0 + c1*s + c2*s^2 + c3*s^3
         * such that
         *   p(0) = x0, p(1) = x1
         *  and
         *   p'(0) = t0, p'(1) = t1.
         */
		public CubicPoly3D(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float tension = 0.5f) 
		{
			Vector3 __t0 = tension * (v2 - v0);
			Vector3 __t1 = tension * (v3 - v1);

			_c0 = v1;
            _c1 = __t0;
            _c2 = -3f * v1 + 3f * v2 - 2f * __t0 - __t1;
            _c3 = 2f * v1 - 2f * v2 + __t0 + __t1;
		}

		public Vector3 Calculate(float t) 
		{
			float __t2 = t * t;
			float __t3 = __t2 * t;
			return _c0 + _c1 * t + _c2 * __t2 + _c3 * __t3;
		}
	}

    public class CatmullRomCurve : Curve 
	{

        public CatmullRomCurve(List<Vector3> points, bool closed = false) : base(points, closed) 
		{
			
        }

        protected override Vector3 GetPoint(float t) 
		{
            List<Vector3> __points = this.Points;
            int __l = __points.Count;

            float __point = (__l - (this.Closed ? 0 : 1)) * t;
            int __intPoint = Mathf.FloorToInt(__point);
            float __weight = __point - __intPoint;

            if (this.Closed) 
			{
                __intPoint += __intPoint > 0 ? 0 : (Mathf.FloorToInt(Mathf.Abs(__intPoint) / (float)__points.Count) + 1) * __points.Count;
            } 
			else if (__weight.Approximately(0) && __intPoint == __l - 1) 
			{
                __intPoint = __l - 2;
                __weight = 1;
            }

            Vector3 __tmp, __p0, __p1, __p2, __p3; // 4 points
            if (this.Closed || __intPoint > 0) {
                __p0 = __points[(__intPoint - 1) % __l];
            } else {
                // extrapolate first point
                __tmp = (__points[0] - __points[1]) + __points[0];
                __p0 = __tmp;
            }

            __p1 = __points[__intPoint % __l];
            __p2 = __points[(__intPoint + 1) % __l];

            if (this.Closed || __intPoint + 2 < __l) {
                __p3 = __points[(__intPoint + 2) % __l];
            } else {
                // extrapolate last point
                __tmp = (__points[__l - 1] - __points[__l - 2]) + __points[__l - 1];
                __p3 = __tmp;
            }

			CubicPoly3D __poly = new CubicPoly3D(__p0, __p1, __p2, __p3);
			return __poly.Calculate(__weight);
        }


    }

}

