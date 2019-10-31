using System;
using UnityEngine;
using VacuumBreather;

public class COMController : MonoBehaviour
{
	public Vector3 popForce;

	public Vector3 comOffset;

	public Rigidbody COMRigidbody;

	private readonly PidVector3Controller _pidVector3Controller = new PidVector3Controller(8f, 0f, 0.05f);

	public float Kp;

	public float Ki;

	public float Kd;

	public float KpImpact;

	public float KdImpact;

	public float KpImpactUp;

	public float KdImpactUp;

	public float KpSetup;

	public float KdSetup;

	public float KpGrind;

	public float KdGrind;

	private Vector3 _lastCOMTargetLocalPos;

	private Vector3 _lastCOMTargetLocalVel;

	public float maxLegForce;

	private Vector3 lastBoardVel;

	public float comHeightRiding;

	public float comHeightLoading;

	private Vector3 lastVel;

	private bool isCompressing => Vector3.Dot(-PlayerController.Instance.skaterController.skaterTransform.up, COMRigidbody.velocity) > 0f;

	public COMController()
	{
	}

	private void Start()
	{
		_lastCOMTargetLocalPos = transform.position;
	}

	public void UpdateCOM()
	{
		COMRigidbody.position = Vector3.Lerp(COMRigidbody.position, PlayerController.Instance.skaterController.skaterRigidbody.position, Time.deltaTime * 10f);
		COMRigidbody.velocity = PlayerController.Instance.skaterController.skaterRigidbody.velocity;
	}

	public void UpdateCOM(float targetHeight, int mode = 0)
	{
		if (mode == 1)
		{
			if (!isCompressing)
			{
				_pidVector3Controller.Kp = KpImpactUp;
				_pidVector3Controller.Kd = KdImpactUp;
			}
			else
			{
				_pidVector3Controller.Kp = KpImpact;
				_pidVector3Controller.Kd = KdImpact;
			}
		}
		else if (mode == 2)
		{
			_pidVector3Controller.Kp = KpSetup;
			_pidVector3Controller.Kd = KdSetup;
		}
		else if (mode != 3)
		{
			_pidVector3Controller.Kp = Kp;
			_pidVector3Controller.Ki = Ki;
			_pidVector3Controller.Kd = Kd;
		}
		else
		{
			_pidVector3Controller.Kp = KpGrind;
			_pidVector3Controller.Kd = KdGrind;
		}
		targetHeight = targetHeight + (-1f + PlayerController.Instance.skaterController.crouchAmount) / 4f;
		UpdateComObject(targetHeight);
		PlayerController.Instance.skaterController.UpdateSkaterPosFromComPos();
	}

	private void UpdateComObject(float targetHeight)
	{
		Vector3 vector3 = targetHeight * PlayerController.Instance.skaterController.skaterTransform.TransformDirection(Vector3.up);
		Vector3 instance = PlayerController.Instance.boardController.boardTransform.position + vector3;
		Vector3 cOMRigidbody = -(COMRigidbody.velocity - PlayerController.Instance.boardController.boardRigidbody.velocity) * Time.deltaTime;
		Vector3 vector31 = -(transform.position - instance);
		Vector3 vector32 = _pidVector3Controller.ComputeOutput(vector31, cOMRigidbody, Time.deltaTime);
		vector32 = new Vector3(Mathf.Clamp(vector32.x, -maxLegForce, maxLegForce), Mathf.Clamp(vector32.y, -maxLegForce, maxLegForce), Mathf.Clamp(vector32.z, -maxLegForce, maxLegForce));
		Quaternion rotation = Quaternion.FromToRotation(Vector3.ProjectOnPlane(lastBoardVel, Vector3.up), Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up));
		lastBoardVel = PlayerController.Instance.boardController.boardRigidbody.velocity;
		COMRigidbody.velocity = rotation * COMRigidbody.velocity;
		Vector3 vector33 = Mathf.Abs(PlayerController.Instance.boardController.xacceleration) * Mathf.Abs(Vector3.Dot(PlayerController.Instance.skaterController.skaterTransform.up, PlayerController.Instance.boardController.boardTransform.right)) * -PlayerController.Instance.skaterController.skaterTransform.up;
		COMRigidbody.AddForce(vector32, ForceMode.Force);
		float single = COMRigidbody.position.y - PlayerController.Instance.boardController.boardRigidbody.position.y;
		Vector3 vector34 = PlayerController.Instance.skaterController.skaterTransform.InverseTransformPoint(PlayerController.Instance.boardController.boardTransform.position);
		Vector3 vector35 = PlayerController.Instance.skaterController.skaterTransform.InverseTransformPoint(COMRigidbody.position);
		if (vector35.y - vector34.y < 0.53379f)
		{
			vector35 = new Vector3(vector35.x, vector34.y + 0.53379f, vector35.z);
			COMRigidbody.position = PlayerController.Instance.skaterController.skaterTransform.TransformPoint(vector35);
		}
		UpdateCrouch(single);
	}

	private void UpdateCrouch(float targetHeight)
	{
		float single = PlayerController.Instance.skaterController.skaterTransform.InverseTransformPoint(COMRigidbody.position).y - PlayerController.Instance.skaterController.skaterTransform.InverseTransformPoint(PlayerController.Instance.boardController.boardRigidbody.position).y;
		targetHeight = comHeightRiding;
		float single1 = Mathf.Clamp(targetHeight - single, 0f, Single.PositiveInfinity);
		single1 /= 0.53054f;
		PlayerController.Instance.animationController.SetValue("Impact", Mathf.Clamp(single1, 0f, 1f));
	}
}