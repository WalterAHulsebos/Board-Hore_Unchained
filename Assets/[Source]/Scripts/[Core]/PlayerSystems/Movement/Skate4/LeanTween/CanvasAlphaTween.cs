using System;
using UnityEngine;

public class CanvasAlphaTween : MonoBehaviour
{
	public CanvasGroup canv;

	public float tweenSpeed;

	public CanvasAlphaTween()
	{
	}

	private void Start()
	{
		if (canv != null)
		{
			LeanTween.alphaCanvas(canv, 0.3f, tweenSpeed).setLoopPingPong();
		}
	}

	private void Update()
	{
	}
}