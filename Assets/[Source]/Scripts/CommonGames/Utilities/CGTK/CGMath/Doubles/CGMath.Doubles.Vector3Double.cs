﻿namespace CommonGames.Utilities.CGTK
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Utilities.Extensions;

    using Math = System.Math;

    public static partial class CGMath
    {
        public struct Vector3Double
        {
            public const float kEpsilon = 1E-05f;
            public double x;
            public double y;
            public double z;

            public double this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return x;
                        case 1:
                            return y;
                        case 2:
                            return z;
                        default:
                            throw new IndexOutOfRangeException("Invalid index!");
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            x = value;
                            break;
                        case 1:
                            y = value;
                            break;
                        case 2:
                            z = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException("Invalid Vector3d index!");
                    }
                }
            }

            public Vector3Double normalized => Normalize(this);

            public double magnitude => Math.Sqrt(x * x + y * y + z * z);

            public double sqrMagnitude => x * x + y * y + z * z;

            public static Vector3Double zero => new Vector3Double(0d, 0d, 0d);

            public static Vector3Double one => new Vector3Double(1d, 1d, 1d);

            public static Vector3Double forward => new Vector3Double(0d, 0d, 1d);

            public static Vector3Double back => new Vector3Double(0d, 0d, -1d);

            public static Vector3Double up => new Vector3Double(0d, 1d, 0d);

            public static Vector3Double down => new Vector3Double(0d, -1d, 0d);

            public static Vector3Double left => new Vector3Double(-1d, 0d, 0d);

            public static Vector3Double right => new Vector3Double(1d, 0d, 0d);

            [Obsolete("Use Vector3d.forward instead.")]
            public static Vector3Double fwd => new Vector3Double(0d, 0d, 1d);

            public Vector3Double(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public Vector3Double(float x, float y, float z)
            {
                this.x = (double) x;
                this.y = (double) y;
                this.z = (double) z;
            }

            public Vector3Double(Vector3 v3)
            {
                x = (double) v3.x;
                y = (double) v3.y;
                z = (double) v3.z;
            }

            public Vector3Double(double x, double y)
            {
                this.x = x;
                this.y = y;
                z = 0d;
            }

            public static Vector3Double operator +(Vector3Double a, Vector3Double b)
            {
                return new Vector3Double(a.x + b.x, a.y + b.y, a.z + b.z);
            }

            public static Vector3Double operator -(Vector3Double a, Vector3Double b)
            {
                return new Vector3Double(a.x - b.x, a.y - b.y, a.z - b.z);
            }

            public static Vector3Double operator -(Vector3Double a)
            {
                return new Vector3Double(-a.x, -a.y, -a.z);
            }

            public static Vector3Double operator *(Vector3Double a, double d)
            {
                return new Vector3Double(a.x * d, a.y * d, a.z * d);
            }

            public static Vector3Double operator *(double d, Vector3Double a)
            {
                return new Vector3Double(a.x * d, a.y * d, a.z * d);
            }

            public static Vector3Double operator /(Vector3Double a, double d)
            {
                return new Vector3Double(a.x / d, a.y / d, a.z / d);
            }

            public static bool operator ==(Vector3Double lhs, Vector3Double rhs)
            {
                return (double) SqrMagnitude(lhs - rhs) < 0.0 / 1.0;
            }

            public static bool operator !=(Vector3Double lhs, Vector3Double rhs)
            {
                return (double) SqrMagnitude(lhs - rhs) >= 0.0 / 1.0;
            }

            public static explicit operator Vector3(Vector3Double vector3Double)
            {
                return new Vector3((float) vector3Double.x, (float) vector3Double.y, (float) vector3Double.z);
            }

            public static Vector3Double Lerp(Vector3Double from, Vector3Double to, double t)
            {
                t = Clamp01(t);
                return new Vector3Double(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t,
                    from.z + (to.z - from.z) * t);
            }

            public static Vector3Double Slerp(Vector3Double from, Vector3Double to, double t)
            {
                Vector3 v3 = Vector3.Slerp((Vector3) from, (Vector3) to, (float) t);
                return new Vector3Double(v3);
            }

            public static void OrthoNormalize(ref Vector3Double normal, ref Vector3Double tangent)
            {
                Vector3 v3normal = new Vector3();
                Vector3 v3tangent = new Vector3();
                v3normal = (Vector3) normal;
                v3tangent = (Vector3) tangent;
                Vector3.OrthoNormalize(ref v3normal, ref v3tangent);
                normal = new Vector3Double(v3normal);
                tangent = new Vector3Double(v3tangent);
            }

            public static void OrthoNormalize(ref Vector3Double normal, ref Vector3Double tangent, ref Vector3Double binormal)
            {
                Vector3 v3normal = new Vector3();
                Vector3 v3tangent = new Vector3();
                Vector3 v3binormal = new Vector3();
                v3normal = (Vector3) normal;
                v3tangent = (Vector3) tangent;
                v3binormal = (Vector3) binormal;
                Vector3.OrthoNormalize(ref v3normal, ref v3tangent, ref v3binormal);
                normal = new Vector3Double(v3normal);
                tangent = new Vector3Double(v3tangent);
                binormal = new Vector3Double(v3binormal);
            }

            public static Vector3Double MoveTowards(Vector3Double current, Vector3Double target, double maxDistanceDelta)
            {
                Vector3Double vector3 = target - current;
                double magnitude = vector3.magnitude;
                if (magnitude <= maxDistanceDelta || magnitude == 0.0d)
                    return target;
                return current + vector3 / magnitude * maxDistanceDelta;
            }

            public static Vector3Double RotateTowards(Vector3Double current, Vector3Double target, double maxRadiansDelta,
                double maxMagnitudeDelta)
            {
                Vector3 v3 = Vector3.RotateTowards((Vector3) current, (Vector3) target, (float) maxRadiansDelta,
                    (float) maxMagnitudeDelta);
                return new Vector3Double(v3);
            }

            public static Vector3Double SmoothDamp(Vector3Double current, Vector3Double target, ref Vector3Double currentVelocity,
                double smoothTime, double maxSpeed)
            {
                double deltaTime = (double) Time.deltaTime;
                return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
            }

            public static Vector3Double SmoothDamp(Vector3Double current, Vector3Double target, ref Vector3Double currentVelocity,
                double smoothTime)
            {
                double deltaTime = (double) Time.deltaTime;
                const double maxSpeed = double.PositiveInfinity;
                return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
            }

            public static Vector3Double SmoothDamp(Vector3Double current, Vector3Double target, ref Vector3Double currentVelocity,
                double smoothTime, double maxSpeed, double deltaTime)
            {
                smoothTime = CGMath.Max(0.0001d, smoothTime);
                double num1 = 2d / smoothTime;
                double num2 = num1 * deltaTime;
                double num3 = (1.0d / (1.0d + num2 + 0.479999989271164d * num2 * num2 +
                                       0.234999999403954d * num2 * num2 * num2));
                Vector3Double vector = current - target;
                Vector3Double vector3_1 = target;
                double maxLength = maxSpeed * smoothTime;
                Vector3Double vector3_2 = ClampMagnitude(vector, maxLength);
                target = current - vector3_2;
                Vector3Double vector3_3 = (currentVelocity + num1 * vector3_2) * deltaTime;
                currentVelocity = (currentVelocity - num1 * vector3_3) * num3;
                Vector3Double vector3_4 = target + (vector3_2 + vector3_3) * num3;
                
                if (!(Dot(vector3_1 - current, vector3_4 - vector3_1) > 0.0)) return vector3_4;
                
                vector3_4 = vector3_1;
                currentVelocity = (vector3_4 - vector3_1) / deltaTime;

                return vector3_4;
            }

            public void Set(double new_x, double new_y, double new_z)
            {
                x = new_x;
                y = new_y;
                z = new_z;
            }

            public static Vector3Double Scale(Vector3Double a, Vector3Double b)
            {
                return new Vector3Double(a.x * b.x, a.y * b.y, a.z * b.z);
            }

            public void Scale(Vector3Double scale)
            {
                x *= scale.x;
                y *= scale.y;
                z *= scale.z;
            }

            public static Vector3Double Cross(Vector3Double lhs, Vector3Double rhs)
            {
                return new Vector3Double(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z,
                    lhs.x * rhs.y - lhs.y * rhs.x);
            }

            public override int GetHashCode()
            {
                return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
            }

            public override bool Equals(object other)
            {
                if (!(other is Vector3Double))
                    return false;
                Vector3Double vector3Double = (Vector3Double) other;
                if (x.Equals(vector3Double.x) && y.Equals(vector3Double.y))
                    return z.Equals(vector3Double.z);
                return false;
            }

            public static Vector3Double Reflect(Vector3Double inDirection, Vector3Double inNormal)
            {
                return -2d * Dot(inNormal, inDirection) * inNormal + inDirection;
            }

            public static Vector3Double Normalize(Vector3Double value)
            {
                double num = Magnitude(value);
                if (num > 9.99999974737875E-06)
                    return value / num;
                return zero;
            }

            public void Normalize()
            {
                double num = Magnitude(this);
                if (num > 9.99999974737875E-06)
                    this = this / num;
                else
                    this = zero;
            }

            // TODO : fix formatting
            public override string ToString()
            {
                return $"({x},{y},{z})";
            }

            public static double Dot(Vector3Double lhs, Vector3Double rhs)
            {
                return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
            }

            public static Vector3Double Project(Vector3Double vector, Vector3Double onNormal)
            {
                double num = Dot(onNormal, onNormal);
                if (num < 1.40129846432482E-45d)
                    return zero;
                return onNormal * Dot(vector, onNormal) / num;
            }

            public static Vector3Double Exclude(Vector3Double excludeThis, Vector3Double fromThat)
            {
                return fromThat - Project(fromThat, excludeThis);
            }

            public static double Angle(Vector3Double from, Vector3Double to)
            {
                return Acos(Clamp(Dot(from.normalized, to.normalized), -1d, 1d)) * 57.29578d;
            }

            public static double Distance(Vector3Double a, Vector3Double b)
            {
                Vector3Double vector3Double = new Vector3Double(a.x - b.x, a.y - b.y, a.z - b.z);
                return Math.Sqrt(vector3Double.x * vector3Double.x + vector3Double.y * vector3Double.y + vector3Double.z * vector3Double.z);
            }

            public static Vector3Double ClampMagnitude(Vector3Double vector, double maxLength)
            {
                if (vector.sqrMagnitude > maxLength * maxLength)
                    return vector.normalized * maxLength;
                return vector;
            }

            public static double Magnitude(Vector3Double a)
            {
                return Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
            }

            public static double SqrMagnitude(Vector3Double a)
            {
                return a.x * a.x + a.y * a.y + a.z * a.z;
            }

            public static Vector3Double Min(Vector3Double lhs, Vector3Double rhs)
            {
                return new Vector3Double(CGMath.Min(lhs.x, rhs.x), CGMath.Min(lhs.y, rhs.y), CGMath.Min(lhs.z, rhs.z));
            }

            public static Vector3Double Max(Vector3Double lhs, Vector3Double rhs)
            {
                return new Vector3Double(CGMath.Max(lhs.x, rhs.x), CGMath.Max(lhs.y, rhs.y), CGMath.Max(lhs.z, rhs.z));
            }
        }
    }
}
