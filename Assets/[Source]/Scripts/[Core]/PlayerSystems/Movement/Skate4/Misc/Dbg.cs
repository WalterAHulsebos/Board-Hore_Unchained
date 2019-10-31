using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class Dbg : MonoBehaviour
{
	public string LogFile = "logyo.txt";

	public bool EchoToConsole = true;

	public bool AddTimeStamp = true;

	private StreamWriter OutputStream;

	private static Dbg Singleton;

	public static Dbg Instance => Singleton;

	static Dbg()
	{
	}

	public Dbg()
	{
	}

	private void Awake()
	{
		if (Singleton != null)
		{
			UnityEngine.Debug.LogError("Multiple Dbg Singletons exist!");
			return;
		}
		Singleton = this;
		OutputStream = new StreamWriter(LogFile, true);
	}

	private void OnApplicationQuit()
	{
	}

	private void OnDestroy()
	{
		if (OutputStream != null)
		{
			OutputStream.Close();
			OutputStream = null;
		}
		UnityEngine.Debug.Log("stopping");
	}

	[Conditional("DEBUG")]
	[Conditional("PROFILE")]
	public static void Trace(string Message)
	{
		if (Instance == null)
		{
			UnityEngine.Debug.Log(Message);
			return;
		}
		Instance.Write(Message);
	}

	private void Write(string message)
	{
		if (OutputStream != null)
		{
			OutputStream.WriteLine(message);
			OutputStream.Flush();
		}
		if (EchoToConsole)
		{
			UnityEngine.Debug.Log(message);
		}
	}
}