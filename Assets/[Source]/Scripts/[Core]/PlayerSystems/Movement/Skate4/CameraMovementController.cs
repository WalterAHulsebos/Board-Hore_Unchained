using Rewired;
using System;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
	public Camera inGameCam;

	public Camera photoCam;

	public float movementSpeed = 1.5f;

	public float rotateSpeed = 3f;

	private float lift;

	private float drop;

	private float horizontal;

	private float vertical;

	private float stickX;

	private float stickY;

	private float delayTimer;

	private bool _cameraMode;

	private bool slowmo;

	private bool movementDelay;

	private bool freezeCamera;

	public CameraMovementController()
	{
	}

	private void ActivateCameraMode()
	{
		if (PlayerController.Instance.inputController.player.GetButtonDown("Y") && !_cameraMode)
		{
			if (slowmo)
			{
				Time.timeScale = 1f;
				slowmo = false;
			}
			else
			{
				Time.timeScale = 0.05f;
				slowmo = true;
			}
		}
		if (PlayerController.Instance.inputController.player.GetButtonDown("X"))
		{
			if (_cameraMode)
			{
				freezeCamera = false;
				photoCam.enabled = false;
				_cameraMode = false;
				slowmo = false;
				Time.timeScale = 1f;
			}
			else
			{
				movementDelay = true;
				photoCam.transform.position = inGameCam.transform.position;
				photoCam.transform.rotation = inGameCam.transform.rotation;
				photoCam.enabled = true;
				_cameraMode = true;
				slowmo = false;
				Time.timeScale = 1E-06f;
			}
		}
		if (_cameraMode && PlayerController.Instance.inputController.player.GetButtonDown("RB"))
		{
			if (!freezeCamera)
			{
				slowmo = false;
				Time.timeScale = 1f;
				freezeCamera = true;
				return;
			}
			movementDelay = true;
			_cameraMode = true;
			slowmo = false;
			freezeCamera = false;
			Time.timeScale = 1E-06f;
		}
	}

	private void EndMovementDelay()
	{
		movementDelay = false;
		delayTimer = 0f;
	}

	private void MoveCamera()
	{
		drop = PlayerController.Instance.inputController.player.GetAxis("LT") * Time.deltaTime * (1f / Time.timeScale);
		lift = PlayerController.Instance.inputController.player.GetAxis("RT") * Time.deltaTime * (1f / Time.timeScale);
		horizontal = PlayerController.Instance.inputController.player.GetAxis("LeftStickX") * Time.deltaTime * (1f / Time.timeScale);
		vertical = PlayerController.Instance.inputController.player.GetAxis("LeftStickY") * Time.deltaTime * (1f / Time.timeScale);
		drop *= movementSpeed;
		lift *= movementSpeed;
		horizontal *= movementSpeed;
		vertical *= movementSpeed;
		Vector3 vector3 = ((photoCam.transform.right * horizontal) + (photoCam.transform.forward * vertical)) + (Vector3.up * (lift - drop));
		Transform transforms = photoCam.transform;
		transforms.position = transforms.position + vector3;
		stickX = PlayerController.Instance.inputController.player.GetAxis("RightStickX") * Time.deltaTime * (1f / Time.timeScale);
		stickY = PlayerController.Instance.inputController.player.GetAxis("RightStickY") * Time.deltaTime * (1f / Time.timeScale);
		photoCam.transform.rotation = Quaternion.AngleAxis(stickY * rotateSpeed, Vector3.ProjectOnPlane(-photoCam.transform.right, Vector3.up)) * photoCam.transform.rotation;
		photoCam.transform.rotation = Quaternion.AngleAxis(stickX * rotateSpeed, Vector3.up) * photoCam.transform.rotation;
	}

	private void MovePhotoCamToInGameCam()
	{
		photoCam.transform.position = inGameCam.transform.position;
		photoCam.transform.rotation = inGameCam.transform.rotation;
	}

	private void Update()
	{
		ActivateCameraMode();
		if (!_cameraMode)
		{
			MovePhotoCamToInGameCam();
			return;
		}
		if (movementDelay)
		{
			delayTimer = delayTimer + Time.deltaTime * (1f / Time.timeScale);
			if (delayTimer > 0.3f)
			{
				EndMovementDelay();
			}
		}
		else if (!freezeCamera)
		{
			MoveCamera();
			return;
		}
	}
}