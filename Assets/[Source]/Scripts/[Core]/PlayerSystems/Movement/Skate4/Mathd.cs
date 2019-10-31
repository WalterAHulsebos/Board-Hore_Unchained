using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Mathd : MonoBehaviour
{
	public static int[] PowersOf2;

	static Mathd()
	{
		PowersOf2 = new int[] { 1, 2, 4, 8, 16, 32, 64, 128 };
	}

	public Mathd()
	{
	}

	private static float AbsAngleBetween(float fromAngle, float toAngle)
	{
		return Mathf.Abs(AngleBetween(fromAngle, toAngle));
	}

	public static float AngleBetween(float fromAngle, float toAngle)
	{
		fromAngle *= 0.0174532924f;
		toAngle *= 0.0174532924f;
		return Mathf.Atan2(Mathf.Sin(toAngle - fromAngle), Mathf.Cos(toAngle - fromAngle)) * 57.29578f;
	}

	public static float AngleBetweenInPlane(Vector3 fromAngle, Vector3 toAngle, Vector3 vector)
	{
		ProjectVectorOnPlane(fromAngle, vector);
		ProjectVectorOnPlane(toAngle, vector);
		return Vector3.Angle(fromAngle, toAngle);
	}

	public static float AverageFloatList(List<float> theList)
	{
		float item = 0f;
		float count = 1f / (float)theList.Count;
		for (int i = 0; i < theList.Count; i++)
		{
			item = item + theList[i] * (1f - count * (float)i);
		}
		return item / (float)theList.Count;
	}

	public static float CalculateElo(float Ra, float Rb, int Sa)
	{
		float single = 1f / (1f + Mathf.Pow(10f, (Rb - Ra) / 400f));
		return Ra + 32f * ((float)Sa - single);
	}

	public static float ClosestFloatFromList(float point, List<float> testPoints)
	{
		float item = testPoints[0];
		float single = Mathf.Abs(testPoints[0] - point);
		foreach (float testPoint in testPoints)
		{
			if (Mathf.Abs(testPoint - point) >= single)
			{
				continue;
			}
			item = testPoint;
			single = testPoint - point;
		}
		return item;
	}

	public static int ClosestIntAngleFromList(float angleIn, List<int> angleList)
	{
		int item = angleList[0];
		foreach (int num in angleList)
		{
			if (AbsAngleBetween(angleIn, (float)num) >= AbsAngleBetween(angleIn, (float)item))
			{
				continue;
			}
			item = num;
		}
		return item;
	}

	public static Vector3 ClosestVector3FromList(Vector3 point, List<Vector3> testPoints)
	{
		Vector3 item = testPoints[0];
		Vector3 vector3 = testPoints[0] - point;
		float single = vector3.magnitude;
		foreach (Vector3 testPoint in testPoints)
		{
			if ((testPoint - point).magnitude >= single)
			{
				continue;
			}
			item = testPoint;
			vector3 = testPoint - point;
			single = vector3.magnitude;
		}
		return item;
	}

	public static int ClosestVector3PlaceFromList(Vector3 point, List<Vector3> testPoints)
	{
		int num = 0;
		Vector3 item = testPoints[0];
		Vector3 vector3 = testPoints[0] - point;
		float single = vector3.magnitude;
		for (int i = 0; i < testPoints.Count; i++)
		{
			if ((testPoints[i] - point).magnitude < single)
			{
				num = i;
				Vector3 item1 = testPoints[i];
				vector3 = testPoints[i] - point;
				single = vector3.magnitude;
			}
		}
		return num;
	}

	public static void DampForceTowards(Rigidbody rigidbody, Vector3 forceDir, Vector3 velIn, float strength, float damp)
	{
		Vector3 vector3 = (forceDir * Time.deltaTime) * strength;
		Vector3 vector31 = (damp * velIn) * Time.deltaTime;
		rigidbody.AddForce(vector3 - vector31);
	}

	public static float DampSpring(float posIn, float velIn, float strength, float damp)
	{
		float single = damp * velIn * Time.deltaTime;
		return posIn * Time.deltaTime * strength - single;
	}

	public static void DampTorqueTowards(Rigidbody rigidbody, Quaternion fromRot, Quaternion toRot, float strength, float dampAmount)
	{
		Vector3 vector3;
		float single;
		Quaternion quaternion = toRot * Quaternion.Inverse(fromRot);
		quaternion.ToAngleAxis(out single, out vector3);
		if (IsInfinityOrNaN(vector3.x) || IsInfinityOrNaN(vector3.y) || IsInfinityOrNaN(vector3.z) || IsInfinityOrNaN(single))
		{
			return;
		}
		single = AngleBetween(0f, single);
		Vector3 vector31 = ((vector3 * single) * strength) / 120f;
		Vector3 vector32 = (dampAmount * rigidbody.angularVelocity) / 120f;
		rigidbody.AddTorque(vector31 - vector32);
	}

	public static void DampXTorqueTowards(Rigidbody rigidbody, Quaternion fromRot, Quaternion toRot, float strength, float dampAmount)
	{
		Vector3 vector3;
		float single;
		Quaternion quaternion = toRot * Quaternion.Inverse(fromRot);
		quaternion.ToAngleAxis(out single, out vector3);
		if (IsInfinityOrNaN(vector3.x) || IsInfinityOrNaN(vector3.y) || IsInfinityOrNaN(vector3.z) || IsInfinityOrNaN(single))
		{
			return;
		}
		single = AngleBetween(0f, single);
		Vector3 vector31 = rigidbody.transform.InverseTransformDirection(rigidbody.angularVelocity);
		vector31.x = dampAmount * vector31.x;
		Vector3 vector32 = ((vector3 * single) * Time.deltaTime) * strength;
		Vector3 vector33 = rigidbody.transform.TransformDirection(vector31) * Time.deltaTime;
		rigidbody.AddTorque(vector32 - vector33);
	}

	public static List<Quaternion> EnsureQuaternionListContinuity(List<Quaternion> Quaternions)
	{
		float single = 0f;
		float single1 = 0f;
		float single2 = 0f;
		float single3 = 0f;
		for (int i = 0; i < Quaternions.Count; i++)
		{
			float item = Quaternions[i].x;
			float item1 = Quaternions[i].y;
			float item2 = Quaternions[i].z;
			float item3 = Quaternions[i].w;
			if (single * item + single1 * item1 + single2 * item2 + single3 * item3 < 0f)
			{
				item = -item;
				item1 = -item1;
				item2 = -item2;
				item3 = -item3;
			}
			single = item;
			single1 = item1;
			single2 = item2;
			single3 = item3;
			Quaternions[i] = new Quaternion(item, item1, item2, item3);
		}
		return Quaternions;
	}

	public static Vector3 GlobalAngularVelocityFromLocal(Rigidbody rigidbodyIn, Vector3 velIn)
	{
		Vector3 vector3 = rigidbodyIn.transform.TransformDirection(rigidbodyIn.angularVelocity);
		return vector3.normalized * velIn.magnitude;
	}

	public static float GreatestAbsValueInFloatList(List<float> theList)
	{
		float single = 0f;
		foreach (float single1 in theList)
		{
			if (Mathf.Abs(single1) <= Mathf.Abs(single))
			{
				continue;
			}
			single = single1;
		}
		return single;
	}

	public static Vector2 GroundVect(Vector3 inVect)
	{
		return new Vector2(inVect.x, inVect.z);
	}

	public static string IntAsFourDigitString(int num)
	{
		string str = "";
		if (num >= 0 && num < 10)
		{
			str = string.Concat("000", num.ToString());
		}
		if (num >= 10 && num < 100)
		{
			str = string.Concat("00", num.ToString());
		}
		if (num >= 100 && num < 1000)
		{
			str = string.Concat("0", num.ToString());
		}
		if (num >= 1000 && num < 10000)
		{
			str = num.ToString();
		}
		if (num > 10000)
		{
			Debug.LogError("10k+, too many items! Might ruin object table order");
			str = num.ToString();
		}
		return str;
	}

	public static string IntAsThreeDigitString(int num)
	{
		string str = "";
		if (num >= 0 && num < 10)
		{
			str = string.Concat("00", num.ToString());
		}
		if (num >= 10 && num < 100)
		{
			str = string.Concat("0", num.ToString());
		}
		if (num >= 100 && num < 1000)
		{
			str = num.ToString();
		}
		if (num > 10000)
		{
			Debug.LogError("1k+, too many items! Might ruin object table order");
			str = num.ToString();
		}
		return str;
	}

	public static string IntAsTwoDigitString(int num)
	{
		string str = "";
		if (num >= 0 && num < 10)
		{
			str = string.Concat("0", num.ToString());
		}
		if (num >= 10 && num < 100)
		{
			str = num.ToString();
		}
		if (num > 100)
		{
			Debug.LogError("enum>=100, too many items! Might ruin encoding");
			str = num.ToString();
		}
		return str;
	}

	public static bool IsInfinityOrNaN(float value)
	{
		if (float.IsInfinity(value) || float.IsNegativeInfinity(value))
		{
			return true;
		}
		return float.IsNaN(value);
	}

	public static Vector3 LocalAngularVelocity(Rigidbody rigidbodyIn)
	{
		Vector3 vector3 = rigidbodyIn.transform.InverseTransformDirection(rigidbodyIn.angularVelocity);
		Vector3 vector31 = vector3.normalized;
		vector3 = rigidbodyIn.angularVelocity;
		return vector31 * vector3.magnitude;
	}

	public static int Log2Lookup(int i)
	{
		if (i == 1)
		{
			return 0;
		}
		if (i == 2)
		{
			return 1;
		}
		if (i == 4)
		{
			return 2;
		}
		if (i == 8)
		{
			return 3;
		}
		if (i == 16)
		{
			return 4;
		}
		if (i == 32)
		{
			return 5;
		}
		if (i == 64)
		{
			return 6;
		}
		if (i == 128)
		{
			return 7;
		}
		return -1;
	}

	public static float MoveTowardAngle(float fromAngle, float toAngle, float maxSpeed, float amountPerCycle)
	{
		float single = Mathf.Clamp(AngleBetween(fromAngle, toAngle) * amountPerCycle, -maxSpeed * Time.deltaTime, maxSpeed * Time.deltaTime);
		return fromAngle + single;
	}

	public static Rect NormalizedRect(Rect rectIn)
	{
		Rect rect = new Rect(0f, 0f, 0f, 0f)
		{
			x = rectIn.x * (float)Screen.width,
			y = rectIn.y * (float)Screen.height,
			width = rectIn.width * (float)Screen.width,
			height = rectIn.height * (float)Screen.height
		};
		return rect;
	}

	public static Vector3 ProjectVectorOnPlane(Vector3 vector, Vector3 planeNormal)
	{
		return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
	}

	public static Vector3 ProjectVectorOntoVector(Vector3 sourceVect, Vector3 targetVect)
	{
		return targetVect.normalized * Vector3.Dot(sourceVect, targetVect.normalized);
	}

	public static float Quantize(float input, float chunkSize)
	{
		return (float)Mathf.RoundToInt(input / chunkSize) * chunkSize;
	}

	public static Vector2 RotateVector2(Vector2 vec, float theta)
	{
		Vector2 vector2 = new Vector2();
		vector2.x = Mathf.Cos(0.0174532924f * theta) * vec.x - Mathf.Sin(0.0174532924f * theta) * vec.y;
		vector2.y = Mathf.Sin(0.0174532924f * theta) * vec.x + Mathf.Cos(0.0174532924f * theta) * vec.y;
		return vector2;
	}

	public static float SignedAngleBetweenInYPlane(Vector3 fromVect, Vector3 toVect)
	{
		Vector3 vector3 = ProjectVectorOnPlane(fromVect, Vector3.up);
		Vector3 vector31 = ProjectVectorOnPlane(toVect, Vector3.up);
		Vector2 vector2 = new Vector2(vector3.x, vector3.z);
		Vector2 vector21 = new Vector2(vector31.x, vector31.z);
		float single = Mathf.Atan2(vector21.x, vector21.y) * 57.29578f;
		return AngleBetween(Mathf.Atan2(vector2.x, vector2.y) * 57.29578f, single);
	}

	public static float SnapFloatToNearest(float input, float step)
	{
		return Mathf.Round(input / step) * step;
	}

	public static bool Vector3IsInfinityOrNan(Vector3 vect)
	{
		if (IsInfinityOrNaN(vect.x) || IsInfinityOrNaN(vect.y))
		{
			return true;
		}
		return IsInfinityOrNaN(vect.z);
	}

	public static class Tuple
	{
		public static Mathd.Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
		{
			return new Mathd.Tuple<T1, T2>(first, second);
		}
	}

	public class Tuple<T1, T2>
	{
		public T1 First
		{
			get;
			private set;
		}

		public T2 Second
		{
			get;
			private set;
		}

		internal Tuple(T1 first, T2 second)
		{
			First = first;
			Second = second;
		}
	}

	[Serializable]
	public struct Vector3Serializable
	{
		public float x;

		public float y;

		public float z;

		public Vector3 V3
		{
			get => new Vector3(x, y, z);
			set
			{
				x = value.x;
				y = value.y;
				z = value.z;
			}
		}
	}

	[Serializable]
	public struct Vector4Serializable
	{
		public float x;

		public float y;

		public float z;

		public float w;

		public Quaternion V4
		{
			get => new Quaternion(x, y, z, w);
			set
			{
				x = value.x;
				y = value.y;
				z = value.z;
				w = value.w;
			}
		}
	}
}