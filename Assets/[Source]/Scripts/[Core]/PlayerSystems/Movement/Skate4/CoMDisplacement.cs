using System;
using UnityEngine;

public class CoMDisplacement : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve _displacementCurve;

	[SerializeField]
	private float _targetCoMHeight = 1.13956f;

	[SerializeField]
	private float _lowestCoMHeight = 0.7370192f;

	[SerializeField]
	private float _curveScalar = 1f;

	private float _lastValue;

	private float _currentValue;

	public float sum;

	public CoMDisplacement()
	{
	}

	public float GetDisplacement(float p_timeStep)
	{
		_lastValue = _currentValue;
		_currentValue = _displacementCurve.Evaluate(p_timeStep);
		sum = sum + (_currentValue - _lastValue) * _curveScalar;
		return (_currentValue - _lastValue) * _curveScalar;
	}

	public void ScaleDisplacementCurve(float p_skaterHeight)
	{
		p_skaterHeight = Mathf.Clamp(p_skaterHeight, _lowestCoMHeight, _targetCoMHeight);
		_curveScalar = 1f - (p_skaterHeight - _lowestCoMHeight) / (_targetCoMHeight - _lowestCoMHeight);
		_lastValue = 0f;
		_currentValue = 0f;
		_curveScalar = Mathf.Clamp(_curveScalar, 0f, 1f);
		sum = 0f;
	}
}