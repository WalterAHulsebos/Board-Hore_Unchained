using System;
using UnityEngine;

public class InputFilter
{
	public double cutoff = 10;

	public double dataCollection = 200;

	public double omega;

	public double a0;

	public double a1;

	public double a2;

	public double b1;

	public double b2;

	public double k1;

	public double k2;

	public double k3;

	public double twoPassCutoff;

	public double fsfc;

	public double[] raw = new double[3];

	public double[] filtered = new double[3];

	public InputFilter()
	{
		InitFilter();
	}

	public float Filter(double val)
	{
		Array.Copy(filtered, 1, filtered, 2, 1);
		Array.Copy(filtered, 0, filtered, 1, 1);
		Array.Copy(raw, 1, raw, 2, 1);
		Array.Copy(raw, 0, raw, 1, 1);
		raw[0] = val;
		filtered[0] = a0 * raw[0] + a1 * raw[1] + a2 * raw[2] + b1 * filtered[1] + b2 * filtered[2];
		return (float)filtered[0];
	}

	private void InitFilter()
	{
		omega = (double)Mathf.Tan((float)(3.14159274101257 * (cutoff / dataCollection))) / 0.802;
		k1 = (double)Mathf.Sqrt(2f) * omega;
		k2 = omega * omega;
		a0 = k2 / (1 + k1 + k2);
		a1 = 2 * a0;
		a2 = a0;
		k3 = a1 / k2;
		b1 = -2 * a0 + k3;
		b2 = 1 - 2 * a0 - k3;
		raw = new double[3];
		filtered = new double[3];
	}
}