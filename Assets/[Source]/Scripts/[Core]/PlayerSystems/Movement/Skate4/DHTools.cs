using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DHTools : MonoBehaviour
{
	private static DHTools _instance;

	public static DHTools Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (new GameObject("_DHToolsGameObject")).AddComponent<DHTools>();
			}
			return _instance;
		}
	}

	static DHTools()
	{
	}

	public DHTools()
	{
	}

	private IEnumerator _InvokeNextFrame(DHTools.Function function)
	{
		yield return null;
		function();
	}

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	private static string BytesToMB(float num)
	{
		int num1 = (int)(num / 1000000f);
		return string.Concat(num1.ToString(), "MB");
	}

	public static void DestroyGameObjectWithMeshes(GameObject theObject)
	{
		if (theObject != null)
		{
			MeshFilter[] componentsInChildren = theObject.gameObject.GetComponentsInChildren<MeshFilter>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				Destroy(componentsInChildren[i].mesh);
			}
			Destroy(theObject);
		}
	}

	public static Dictionary<string, int> DictConvert(Dictionary<string, object> dictIn)
	{
		Dictionary<string, int> strs = new Dictionary<string, int>();
		foreach (KeyValuePair<string, object> keyValuePair in dictIn)
		{
			strs.Add(keyValuePair.Key.ToString(), Convert.ToInt32(keyValuePair.Value));
		}
		return strs;
	}

	public static string FixUnicodeCommas(string theString)
	{
		return theString.Replace("\\u201d", "\"");
	}

	public void Invoke(DHTools.Function fuction, float delay)
	{
	}

	public void InvokeNextFrame(DHTools.Function function)
	{
		try
		{
			StartCoroutine(_InvokeNextFrame(function));
		}
		catch
		{
			UnityEngine.Debug.Log(string.Concat("Trying to invoke ", function.ToString(), " but it doesnt seem to exist"));
		}
	}

	public static bool IsEditor()
	{
		return Application.platform == RuntimePlatform.OSXEditor;
	}

	public static bool IsWithinRadiusLean(Vector3 vecIn, float thresh)
	{
		if (Mathf.Abs(vecIn.x) >= thresh || Mathf.Abs(vecIn.y) >= thresh)
		{
			return false;
		}
		return Mathf.Abs(vecIn.z) < thresh;
	}

	public static void Log(string message)
	{
	}

	public static void LogDrives()
	{
		DriveInfo[] drives = DriveInfo.GetDrives();
		for (int i = 0; i < (int)drives.Length; i++)
		{
			DriveInfo driveInfo = drives[i];
			if (driveInfo.IsReady)
			{
				UnityEngine.Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx Logging drives");
				UnityEngine.Debug.Log(string.Concat("name:", driveInfo.Name));
				UnityEngine.Debug.Log(string.Concat(new object[] { "avail free:", BytesToMB((float)driveInfo.AvailableFreeSpace), " ", driveInfo.AvailableFreeSpace }));
				UnityEngine.Debug.Log(string.Concat(new object[] { "tot free:", BytesToMB((float)driveInfo.TotalFreeSpace), " ", driveInfo.TotalFreeSpace }));
				UnityEngine.Debug.Log(string.Concat(new object[] { "tot size:", BytesToMB((float)driveInfo.TotalSize), " ", driveInfo.TotalSize }));
			}
		}
	}

	public static void LogError(string message)
	{
		Log(message);
	}

	public static Vector3 ToGroundVec(Vector3 vecIn)
	{
		return new Vector3(vecIn.x, 0f, vecIn.z);
	}

	public delegate void Function();
}