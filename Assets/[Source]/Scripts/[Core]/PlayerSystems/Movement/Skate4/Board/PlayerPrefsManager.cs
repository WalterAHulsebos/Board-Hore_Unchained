using System;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
	private static PlayerPrefsManager _instance;

	public static PlayerPrefsManager Instance => _instance;

	public PlayerPrefsManager()
	{
	}

	private void Awake()
	{
		if (!(_instance != null) || !(_instance != this))
		{
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		IncrementPlaySessions();
	}

	private void IncrementPlaySessions()
	{
		PlayerPrefs.SetInt("PlaySessions", PlayerPrefs.GetInt("PlaySessions", 0) + 1);
		PlayerPrefs.SetInt("PixelSessions", PlayerPrefs.GetInt("PixelSessions", 0) + 1);
	}

	private void OnApplicationQuit()
	{
		if (PlayerPrefs.GetInt("PixelSessions") == 1)
		{
			Application.OpenURL("https://skaterxl.com/ownermailinglist");
		}
	}
}