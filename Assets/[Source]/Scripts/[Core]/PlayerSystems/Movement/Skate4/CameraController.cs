using FSMHelper;
using Rewired;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private Transform _targetPos;

	[SerializeField]
	private Transform _camTransform;

	public Transform _pivot;

	public Transform _pivotCentered;

	public Transform _pivotForward;

	[SerializeField]
	private Rigidbody _camRigidbody;

	[SerializeField]
	private Rigidbody _bailTarget;

	public Transform _actualCam;

	[SerializeField]
	private Transform _leftTopPos;

	[SerializeField]
	private Transform _rightTopPos;

	private Vector3 _forwardTarget;

	private Vector3 _projectedVelocity;

	public LayerMask layerMask;

	private RaycastHit _hit;

	public PostProcessLayer postProcessLayer;

	private int _qualitySettings = 5;

	private float _pushLerp;

	private bool _right;

	private float _initialYPos;

	private float _targetY;

	private float _currentY;

	private float _skaterClampedY;

	private float _groundUnderCam;

	private float _lowestY;

	public bool _leanForward;

	private Vector3 _camVel;

	public CameraController()
	{
	}

	private void Awake()
	{
		_qualitySettings = QualitySettings.GetQualityLevel();
		int num = _qualitySettings;
		if (num <= 2)
		{
			postProcessLayer.enabled = false;
		}
		else if (num - 3 > 2)
		{
		}
		if (SettingsManager.Instance.stance != SettingsManager.Stance.Regular)
		{
			_actualCam.position = _leftTopPos.position;
			_actualCam.rotation = _leftTopPos.rotation;
			_right = false;
		}
		else
		{
			_actualCam.position = _rightTopPos.position;
			_actualCam.rotation = _rightTopPos.rotation;
			_right = true;
		}
		_initialYPos = _camTransform.position.y;
		_targetY = _initialYPos;
		_currentY = _targetY;
		_lowestY = _currentY;
	}

	public void LookAtPlayer()
	{
		_camVel = _camRigidbody.velocity;
		_camVel.x *= 0.95f;
		_camVel.y *= 0.5f;
		_camVel.z *= 0.95f;
		_camRigidbody.velocity = _camVel;
		Rigidbody rigidbody = _camRigidbody;
		rigidbody.angularVelocity = rigidbody.angularVelocity * 0.95f;
	}

	public void MoveCameraToPlayer()
	{
		if (PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude <= 1f)
		{
			_leanForward = false;
		}
		else if (Physics.Raycast((_actualCam.position + (Vector3.up * 3f)) + (Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterRigidbody.velocity.normalized, Vector3.up) * 10f), -Vector3.up, out _hit, 60f, layerMask))
		{
			if (_hit.point.y >= PlayerController.Instance.boardController.GroundY - 0.8f)
			{
				_leanForward = false;
			}
			else
			{
				_leanForward = true;
			}
		}
		if (!_leanForward)
		{
			_pivot.rotation = Quaternion.Slerp(_pivot.rotation, _pivotCentered.rotation, Time.deltaTime * 2f);
		}
		else
		{
			_pivot.rotation = Quaternion.Slerp(_pivot.rotation, _pivotForward.rotation, Time.deltaTime * 2f);
		}
		if (Physics.Raycast(_actualCam.position, -Vector3.up, out _hit, 10f, layerMask))
		{
			_groundUnderCam = _hit.point.y;
		}
		if (_groundUnderCam > PlayerController.Instance.boardController.GroundY)
		{
			_lowestY = PlayerController.Instance.boardController.GroundY + _initialYPos;
		}
		else
		{
			_lowestY = _groundUnderCam + _initialYPos;
		}
		if (!PlayerController.Instance.IsCurrentAnimationPlaying("Push Button"))
		{
			_pushLerp = Mathf.Lerp(_pushLerp, 0f, Time.deltaTime * 5f);
		}
		else
		{
			_pushLerp = Mathf.Lerp(_pushLerp, 1f, Time.deltaTime * 5f);
		}
		_camRigidbody.velocity = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterRigidbody.velocity, Vector3.up);
		Vector3 vector3 = Vector3.Lerp(PlayerController.Instance.skaterController.skaterTransform.position, _targetPos.position, _pushLerp);
		Vector3 vector31 = new Vector3(PlayerController.Instance.skaterController.skaterTransform.InverseTransformDirection(PlayerController.Instance.skaterController.skaterTransform.position).x, PlayerController.Instance.skaterController.skaterTransform.InverseTransformDirection(PlayerController.Instance.skaterController.skaterTransform.position).y, PlayerController.Instance.skaterController.skaterTransform.InverseTransformDirection(vector3).z);
		vector31 = PlayerController.Instance.skaterController.skaterTransform.TransformDirection(vector31);
		_targetY = Mathf.Lerp(PlayerController.Instance.boardController.GroundY + _initialYPos, PlayerController.Instance.skaterController.skaterTransform.position.y, 0.42f);
		if (!PlayerController.Instance.playerSM.IsGrindingSM() && PlayerController.Instance.skaterController.skaterRigidbody.velocity.y < 0f && !PlayerController.Instance.IsGrounded() && _targetY > PlayerController.Instance.skaterController.skaterTransform.position.y - 0.2f)
		{
			_targetY = PlayerController.Instance.skaterController.skaterTransform.position.y - 0.2f;
		}
		_targetY = Mathf.Clamp(_targetY, _lowestY, _targetY);
		_currentY = Mathf.Lerp(_currentY, _targetY, Time.fixedDeltaTime * 10f);
		vector31.y = _currentY;
		_camTransform.position = Vector3.MoveTowards(_camTransform.position, vector31, Time.fixedDeltaTime * 10f);
		_projectedVelocity = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterRigidbody.velocity, Vector3.up);
		if (_projectedVelocity.magnitude > 0.3f)
		{
			_forwardTarget = PlayerController.Instance.boardController.boardTransform.position + (_projectedVelocity * 10f);
			if (PlayerController.Instance.IsGrounded())
			{
				Quaternion rotation = Quaternion.FromToRotation(_camTransform.forward, _projectedVelocity);
				rotation *= _camTransform.rotation;
				_camTransform.rotation = Quaternion.Slerp(_camTransform.rotation, rotation, Time.fixedDeltaTime * 10f);
			}
			Quaternion quaternion = Quaternion.FromToRotation(_camTransform.up, Vector3.up);
			quaternion *= _camTransform.rotation;
			_camTransform.rotation = Quaternion.Slerp(_camTransform.rotation, quaternion, Time.fixedDeltaTime * 10f);
		}
		if (PlayerController.Instance.inputController.player.GetAxis("DPadX") < 0f)
		{
			_right = false;
		}
		else if (PlayerController.Instance.inputController.player.GetAxis("DPadX") > 0f)
		{
			_right = true;
		}
		if (_right)
		{
			_actualCam.position = Vector3.Lerp(_actualCam.position, _rightTopPos.position, Time.fixedDeltaTime * 4f);
			_actualCam.rotation = Quaternion.Slerp(_actualCam.rotation, _rightTopPos.rotation, Time.fixedDeltaTime * 4f);
			return;
		}
		_actualCam.position = Vector3.Lerp(_actualCam.position, _leftTopPos.position, Time.fixedDeltaTime * 4f);
		_actualCam.rotation = Quaternion.Slerp(_actualCam.rotation, _leftTopPos.rotation, Time.fixedDeltaTime * 4f);
	}

	public void ResetAllCamera()
	{
		_targetY = PlayerController.Instance.boardController.boardTransform.position.y + _initialYPos;
		_currentY = _targetY;
	}
}