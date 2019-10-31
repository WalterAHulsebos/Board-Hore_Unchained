using System;
using UnityEngine;

public class CameraPathFollower : MonoBehaviour
{
	public Transform cam;

	public Transform pointA;

	public Transform pointB;

	public float speed = 2f;

	public float lerp;

	public bool began;

	public CameraPathFollower()
	{
	}

	private void Update()
	{
		if (began)
		{
			lerp = lerp + Time.deltaTime * speed;
			lerp = Mathf.Clamp(lerp, 0f, 1f);
			cam.position = Vector3.Lerp(pointA.position, pointB.position, lerp);
			cam.rotation = Quaternion.Slerp(pointA.rotation, pointB.rotation, lerp);
		}
		else if (Input.GetKeyDown(KeyCode.Space))
		{
			began = true;
			return;
		}
	}
}