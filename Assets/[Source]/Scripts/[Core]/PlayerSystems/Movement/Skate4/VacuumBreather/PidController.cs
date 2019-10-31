using System;
using UnityEngine;

namespace VacuumBreather
{
	public class PidController
	{
		private const float MaxOutput = 1000000f;

		private float _integralMax;

		private float _integral;

		private float _kp;

		private float _ki;

		private float _kd;

		public float Kd
		{
			get => _kd;
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Kd must be a non-negative number.");
				}
				_kd = value;
			}
		}

		public float Ki
		{
			get => _ki;
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Ki must be a non-negative number.");
				}
				_ki = value;
				_integralMax = 1000000f / Ki;
				_integral = Mathf.Clamp(_integral, -_integralMax, _integralMax);
			}
		}

		public float Kp
		{
			get => _kp;
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Kp must be a non-negative number.");
				}
				_kp = value;
			}
		}

		public PidController(float kp, float ki, float kd)
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
			Kp = kp;
			Ki = ki;
			Kd = kd;
			_integralMax = 1000000f / Ki;
		}

		public float ComputeOutput(float error, float delta, float deltaTime)
		{
			_integral = _integral + error * deltaTime;
			_integral = Mathf.Clamp(_integral, -_integralMax, _integralMax);
			float single = delta / deltaTime;
			return Mathf.Clamp(Kp * error + Ki * _integral + Kd * single, -1000000f, 1000000f);
		}
	}
}