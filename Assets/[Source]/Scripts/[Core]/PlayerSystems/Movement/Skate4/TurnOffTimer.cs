using System;
using UnityEngine;

public class TurnOffTimer : MonoBehaviour
{
	private float startTime;

	private bool startTimer;

	private float timer;

	public float duration = 3f;

	public GameObject target;

	private bool didSet;

	public TurnOffTimer()
	{
	}

	public void StartTimer()
	{
		startTimer = true;
		startTime = Time.time;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (startTimer && !didSet && timer - startTime > duration && PlayerPrefs.GetInt("PlaySessions") < 2)
		{
			target.SetActive(true);
			didSet = true;
		}
	}
}