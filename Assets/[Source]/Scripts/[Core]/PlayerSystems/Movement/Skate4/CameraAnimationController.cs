using System;
using UnityEngine;

public class CameraAnimationController : MonoBehaviour
{
	public Animator cameraAnimator;

	public AnimationClip[] clips;

	public int index;

	public CameraAnimationController()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.LogError("Spacebar Pressed");
			if (index < (int)clips.Length)
			{
				Debug.LogError("Play");
				cameraAnimator.Play(clips[index].name);
				index++;
			}
		}
	}
}