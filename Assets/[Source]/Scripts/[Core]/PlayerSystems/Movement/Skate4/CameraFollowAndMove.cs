using Rewired;
using System;
using UnityEngine;

public class CameraFollowAndMove : MonoBehaviour
{
	public Transform skater;

	public Camera cam;

	public Animator khAnim;

	public AnimationClip reset;

	public AnimationClip line;

	private bool follow;

	public CameraFollowAndMove()
	{
	}

	private void FixedUpdate()
	{
		cam.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(skater.position - cam.transform.position, Vector3.up), Vector3.up);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!follow)
		{
			follow = true;
			khAnim.Play(line.name);
		}
	}

	private void Start()
	{
		cam.fieldOfView = 40f;
	}

	private void Update()
	{
		if (PlayerController.Instance.inputController.player.GetButtonDown("X"))
		{
			khAnim.Play(reset.name);
			follow = false;
		}
	}
}