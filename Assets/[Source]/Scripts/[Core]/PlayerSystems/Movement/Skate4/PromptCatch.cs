using Rewired;
using System;
using UnityEngine;

public class PromptCatch : MonoBehaviour
{
	public PromptCatch()
	{
	}

	private void Update()
	{
		if (InputController.Instance.controlsActive && PlayerController.Instance.inputController.player.GetButtonDown("Left Stick Button") || PlayerController.Instance.inputController.player.GetButtonDown("Right Stick Button"))
		{
			PromptController.Instance.DoUnPause();
			gameObject.SetActive(false);
		}
	}
}