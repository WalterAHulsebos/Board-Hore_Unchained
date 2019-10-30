using System;
using UnityEngine;

public class MusicController : MonoBehaviour
{
	public AudioClip[] tracks;

	public AudioSource audioSource;

	private float startingVol;

	private static MusicController _instance;

	public static MusicController Instance => _instance;

	public MusicController()
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
		startingVol = audioSource.volume;
	}

	public void PlayGameMusic()
	{
		audioSource.clip = tracks[1];
		audioSource.Play();
		audioSource.volume = 0f;
		Debug.Log("play");
	}

	public void PlayTitleTrack()
	{
		audioSource.clip = tracks[0];
		audioSource.Play();
		audioSource.volume = startingVol;
	}
}