using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VacuumBreather
{
	public static class QuaternionExtensions
	{
		public static Quaternion Multiply(this Quaternion quaternion, float scalar)
		{
			return new Quaternion((float)((double)quaternion.x * (double)scalar), (float)((double)quaternion.y * (double)scalar), (float)((double)quaternion.z * (double)scalar), (float)((double)quaternion.w * (double)scalar));
		}

		public static Quaternion RequiredRotation(Quaternion from, Quaternion to)
		{
			Quaternion quaternion = to * Quaternion.Inverse(from);
			if (quaternion.w < 0f)
			{
				quaternion.x *= -1f;
				quaternion.y *= -1f;
				quaternion.z *= -1f;
				quaternion.w *= -1f;
			}
			return quaternion;
		}

		public static Quaternion Subtract(this Quaternion lhs, Quaternion rhs)
		{
			return new Quaternion((float)((double)lhs.x - (double)rhs.x), (float)((double)lhs.y - (double)rhs.y), (float)((double)lhs.z - (double)rhs.z), (float)((double)lhs.w - (double)rhs.w));
		}
	}
}