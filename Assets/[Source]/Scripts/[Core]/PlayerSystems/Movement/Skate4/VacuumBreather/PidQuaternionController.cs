using System;
using UnityEngine;

namespace VacuumBreather
{
	public class PidQuaternionController
	{
		private readonly PidController[] _internalController;

		public float Kd
		{
			get => _internalController[0].Kd;
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Kd must be a non-negative number.");
				}
				_internalController[0].Kd = value;
				_internalController[1].Kd = value;
				_internalController[2].Kd = value;
				_internalController[3].Kd = value;
			}
		}

		public float Ki
		{
			get => _internalController[0].Ki;
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Ki must be a non-negative number.");
				}
				_internalController[0].Ki = value;
				_internalController[1].Ki = value;
				_internalController[2].Ki = value;
				_internalController[3].Ki = value;
			}
		}

		public float Kp
		{
			get => _internalController[0].Kp;
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Kp must be a non-negative number.");
				}
				_internalController[0].Kp = value;
				_internalController[1].Kp = value;
				_internalController[2].Kp = value;
				_internalController[3].Kp = value;
			}
		}

		public PidQuaternionController(float kp, float ki, float kd)
		{
			if (kp < 0f)
			{
				throw new ArgumentOutOfRangeException("kp", "kp must be a non-negative number.");
			}
			if (ki < 0f)
			{
				throw new ArgumentOutOfRangeException("ki", "ki must be a non-negative number.");
			}
			if (kd < 0f)
			{
				throw new ArgumentOutOfRangeException("kd", "kd must be a non-negative number.");
			}
			_internalController = new PidController[] { new PidController(kp, ki, kd), new PidController(kp, ki, kd), new PidController(kp, ki, kd), new PidController(kp, ki, kd) };
		}

		private Quaternion ComputeOutput(Quaternion error, Quaternion delta, float deltaTime)
		{
			Quaternion quaternion = new Quaternion()
			{
				x = _internalController[0].ComputeOutput(error.x, delta.x, deltaTime),
				y = _internalController[1].ComputeOutput(error.y, delta.y, deltaTime),
				z = _internalController[2].ComputeOutput(error.z, delta.z, deltaTime),
				w = _internalController[3].ComputeOutput(error.w, delta.w, deltaTime)
			};
			return quaternion;
		}

		public Vector3 ComputeRequiredAngularAcceleration(Quaternion currentOrientation, Quaternion desiredOrientation, Vector3 currentAngularVelocity, float deltaTime)
		{
			Quaternion quaternion = QuaternionExtensions.RequiredRotation(currentOrientation, desiredOrientation);
			Quaternion quaternion1 = Quaternion.identity.Subtract(quaternion);
			Quaternion eulerAngleQuaternion = ToEulerAngleQuaternion(currentAngularVelocity) * quaternion;
			Matrix4x4 matrix4x4 = new Matrix4x4()
			{
				m00 = -quaternion.x * -quaternion.x + -quaternion.y * -quaternion.y + -quaternion.z * -quaternion.z,
				m01 = -quaternion.x * quaternion.w + -quaternion.y * -quaternion.z + -quaternion.z * quaternion.y,
				m02 = -quaternion.x * quaternion.z + -quaternion.y * quaternion.w + -quaternion.z * -quaternion.x,
				m03 = -quaternion.x * -quaternion.y + -quaternion.y * quaternion.x + -quaternion.z * quaternion.w,
				m10 = quaternion.w * -quaternion.x + -quaternion.z * -quaternion.y + quaternion.y * -quaternion.z,
				m11 = quaternion.w * quaternion.w + -quaternion.z * -quaternion.z + quaternion.y * quaternion.y,
				m12 = quaternion.w * quaternion.z + -quaternion.z * quaternion.w + quaternion.y * -quaternion.x,
				m13 = quaternion.w * -quaternion.y + -quaternion.z * quaternion.x + quaternion.y * quaternion.w,
				m20 = quaternion.z * -quaternion.x + quaternion.w * -quaternion.y + -quaternion.x * -quaternion.z,
				m21 = quaternion.z * quaternion.w + quaternion.w * -quaternion.z + -quaternion.x * quaternion.y,
				m22 = quaternion.z * quaternion.z + quaternion.w * quaternion.w + -quaternion.x * -quaternion.x,
				m23 = quaternion.z * -quaternion.y + quaternion.w * quaternion.x + -quaternion.x * quaternion.w,
				m30 = -quaternion.y * -quaternion.x + quaternion.x * -quaternion.y + quaternion.w * -quaternion.z,
				m31 = -quaternion.y * quaternion.w + quaternion.x * -quaternion.z + quaternion.w * quaternion.y,
				m32 = -quaternion.y * quaternion.z + quaternion.x * quaternion.w + quaternion.w * -quaternion.x,
				m33 = -quaternion.y * -quaternion.y + quaternion.x * quaternion.x + quaternion.w * quaternion.w
			};
			Quaternion quaternion2 = ComputeOutput(quaternion1, eulerAngleQuaternion, deltaTime);
			quaternion2 = MultiplyAsVector(matrix4x4, quaternion2);
			Quaternion quaternion3 = quaternion2.Multiply(-2f) * Quaternion.Inverse(quaternion);
			return new Vector3(quaternion3.x, quaternion3.y, quaternion3.z);
		}

		public static Quaternion MultiplyAsVector(Matrix4x4 matrix, Quaternion quaternion)
		{
			Vector4 vector4 = new Vector4(quaternion.w, quaternion.x, quaternion.y, quaternion.z);
			Vector4 vector41 = matrix * vector4;
			return new Quaternion(vector41.y, vector41.z, vector41.w, vector41.x);
		}

		public static Quaternion ToEulerAngleQuaternion(Vector3 eulerAngles)
		{
			return new Quaternion(eulerAngles.x, eulerAngles.y, eulerAngles.z, 0f);
		}
	}
}