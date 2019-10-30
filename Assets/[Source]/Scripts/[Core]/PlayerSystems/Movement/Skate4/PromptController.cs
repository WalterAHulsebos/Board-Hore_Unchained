using System;
using UnityEngine;

public class PromptController : MonoBehaviour
{
	public GameObject catchPrompt;

	public int catchCount;

	public int StateChangeReleaseToBailCount;

	public int ReleaseToBailTriggerCount;

	private bool doPause;

	private bool doUnPause;

	//public Menu menuthing;

	private static PromptController _instance;

	public static PromptController Instance => _instance;

	public PromptController()
	{
	}

	public void ActionTakenCatch()
	{
		catchCount++;
	}

	private void Awake()
	{
		if (!(_instance != null) || !(_instance != this))
		{
			_instance = this;
			return;
		}
		Destroy(gameObject);
	}

	private void DoPopupDelayed()
	{
		catchPrompt.SetActive(true);
		Time.timeScale = 0f;
		doPause = false;
		InputController.Instance.controlsActive = true;
	}

	public void DoUnPause()
	{
		if (Time.timeScale == 0f)
		{
			doPause = false;
			doUnPause = true;
			Time.timeScale = 0.05f;
		}
	}

	public void FixedUpdate()
	{
		if (doPause)
		{
			if (Time.timeScale > 0.1f)
			{
				Time.timeScale = Time.timeScale - Time.deltaTime * 6f;
				if (Time.timeScale < 0.1f)
				{
					Time.timeScale = 0.1f;
				}
			}
			else
			{
				Invoke("DoPopupDelayed", 0.005f);
				Time.timeScale = 0.1f;
			}
		}
		if (doUnPause)
		{
			Time.timeScale = Time.timeScale + Time.deltaTime * 6f;
			if (Time.timeScale >= 1f)
			{
				doUnPause = false;
				Time.timeScale = 1f;
				//this.menuthing.enabled = true;
			}
		}
	}

	public void StateChangePopToRelease()
	{
		StateChangeReleaseToBailCount++;
		int stateChangeReleaseToBailCount = StateChangeReleaseToBailCount;
		int releaseToBailTriggerCount = ReleaseToBailTriggerCount;
	}

	public void StateChangeReleaseToBail()
	{
	}
}