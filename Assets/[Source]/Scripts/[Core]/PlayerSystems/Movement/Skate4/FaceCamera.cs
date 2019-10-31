using System;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	public Transform camera;

	public FaceCamera()
	{
	}

	private void Update()
	{
		transform.rotation = Quaternion.LookRotation(-camera.forward);
	}
}