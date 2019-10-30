using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class ForeGrounder : MonoBehaviour
{
	private const uint LOCK = 1;

	private const uint UNLOCK = 2;

	private IntPtr window;

	public ForeGrounder()
	{
	}

	[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern bool BringWindowToTop(IntPtr hWnd);

	private IEnumerator Checker()
	{
		ForeGrounder foreGrounder = null;
		while (true)
		{
			yield return new WaitForSeconds(1f);
			IntPtr activeWindow = GetActiveWindow();
			if (foreGrounder.window != activeWindow)
			{
				bool zero = foreGrounder.window == IntPtr.Zero;
				UnityEngine.Debug.Log(string.Concat("Set to foreground. ptr zero:", zero.ToString()));
				SetForegroundWindow(foreGrounder.window);
				BringWindowToTop(foreGrounder.window);
			}
		}
	}

	[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern bool LockSetForegroundWindow(uint uLockCode);

	[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern bool SetForegroundWindow(IntPtr hWnd);

	private void Start()
	{
		window = GetActiveWindow();
		StartCoroutine(Checker());
	}

	[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
}