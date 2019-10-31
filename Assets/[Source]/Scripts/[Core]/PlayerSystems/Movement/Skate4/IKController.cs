using FSMHelper;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using System;
using UnityEngine;

public class IKController : MonoBehaviour
{
	[SerializeField]
	private FullBodyBipedIK _finalIk;

	[SerializeField]
	private Animator _anim;

	[SerializeField]
	private AnimationCurve _animCurve;

	[SerializeField]
	private Transform skaterLeftFoot;

	[SerializeField]
	private Transform skaterRightFoot;

	[SerializeField]
	private Transform skaterLeftFootTargetParent;

	[SerializeField]
	private Transform skaterRightFootTargetParent;

	[SerializeField]
	private Transform skaterLeftFootTarget;

	[SerializeField]
	private Transform skaterRightFootTarget;

	[SerializeField]
	private Transform steezeLeftFootTarget;

	[SerializeField]
	private Transform steezeRightFootTarget;

	[SerializeField]
	private Transform ikAnimLeftFootTarget;

	[SerializeField]
	private Transform ikAnimRightFootTarget;

	[SerializeField]
	private Transform ikLeftFootPosition;

	[SerializeField]
	private Transform ikRightFootPosition;

	[SerializeField]
	private Transform ikLeftFootPositionOffset;

	[SerializeField]
	private Transform ikRightFootPositionOffset;

	[SerializeField]
	private Transform ikAnimBoard;

	[SerializeField]
	private Rigidbody physicsBoard;

	[SerializeField]
	private Transform physicsBoardBackwards;

	[SerializeField]
	private Rigidbody _ikAnim;

	[SerializeField]
	private float _lerpSpeed = 10f;

	[SerializeField]
	private float _steezeLerpSpeed = 2.5f;

	[Range(0f, 1f)]
	[SerializeField]
	private float _ikLeftPosLerp;

	[Range(0f, 1f)]
	[SerializeField]
	private float _ikLeftRotLerp;

	[Range(0f, 1f)]
	[SerializeField]
	private float _ikLeftLerpPosTarget;

	[Range(0f, 1f)]
	[SerializeField]
	private float _ikLeftLerpRotTarget;

	[Range(0f, 1f)]
	[SerializeField]
	private float _ikRightPosLerp;

	[Range(0f, 1f)]
	[SerializeField]
	private float _ikRightRotLerp;

	[Range(0f, 1f)]
	[SerializeField]
	private float _ikRightLerpPosTarget;

	[Range(0f, 1f)]
	[SerializeField]
	private float _ikRightLerpRotTarget;

	private float _rightPositionWeight = 1f;

	private float _leftPositionWeight = 1f;

	private float _rightRotationWeight = 1f;

	private float _leftRotationWeight = 1f;

	[SerializeField]
	private float _leftSteezeMax;

	[SerializeField]
	private float _rightSteezeMax;

	private float _leftSteezeTarget;

	[SerializeField]
	private float _leftSteezeWeight;

	private float _rightSteezeTarget;

	[SerializeField]
	private float _rightSteezeWeight;

	private Vector3 _skaterLeftFootPos = Vector3.zero;

	private Quaternion _skaterLeftFootRot = Quaternion.identity;

	private Vector3 _skaterRightFootPos = Vector3.zero;

	private Quaternion _skaterRightFootRot = Quaternion.identity;

	public float offsetScaler = 0.05f;

	public float popOffsetScaler = 0.5f;

	private Vector3 _boardPrevPos = Vector3.zero;

	private Vector3 _boardLastPos = Vector3.zero;

	private bool _impactSet;

	public IKController()
	{
	}

	private void FixedUpdate()
	{
		MoveAndRotateIKGameObject();
		LerpToTarget();
		SetSteezeWeight();
		SetIK();
	}

	public void ForceLeftLerpValue(float p_value)
	{
		_ikLeftPosLerp = p_value;
		_ikLeftLerpPosTarget = p_value;
	}

	public void ForceRightLerpValue(float p_value)
	{
		_ikRightPosLerp = p_value;
		_ikRightLerpPosTarget = p_value;
	}

	public float GetKneeBendWeight()
	{
		return _finalIk.solver.leftLegChain.bendConstraint.weight;
	}

	public void LeftIKWeight(float p_value)
	{
		_leftPositionWeight = p_value;
		_leftRotationWeight = p_value;
		_finalIk.solver.leftFootEffector.positionWeight = p_value;
		_finalIk.solver.leftFootEffector.rotationWeight = p_value;
	}

	private void LerpToTarget()
	{
		_ikLeftPosLerp = Mathf.MoveTowards(_ikLeftPosLerp, _ikLeftLerpPosTarget, Time.deltaTime * _lerpSpeed);
		_ikRightPosLerp = Mathf.MoveTowards(_ikRightPosLerp, _ikRightLerpPosTarget, Time.deltaTime * _lerpSpeed);
		_ikLeftRotLerp = Mathf.MoveTowards(_ikLeftRotLerp, _ikLeftLerpRotTarget, Time.deltaTime * _lerpSpeed);
		_ikRightRotLerp = Mathf.MoveTowards(_ikRightRotLerp, _ikRightLerpRotTarget, Time.deltaTime * _lerpSpeed);
	}

	private void MoveAndRotateIKGameObject()
	{
		if (PlayerController.Instance.playerSM.IsInImpactStateSM())
		{
			if (!_impactSet)
			{
				PlayerController.Instance.respawn.behaviourPuppet.BoostImmunity(1000f);
				_impactSet = true;
			}
		}
		else if (_impactSet)
		{
			_impactSet = false;
		}
		_ikAnim.velocity = physicsBoard.velocity;
		_ikAnim.position = physicsBoard.position;
		Quaternion instance = PlayerController.Instance.boardController.boardMesh.rotation;
		if (PlayerController.Instance.GetBoardBackwards())
		{
			instance = Quaternion.AngleAxis(180f, PlayerController.Instance.boardController.boardMesh.up) * instance;
		}
		Vector3 vector3 = (!PlayerController.Instance.GetBoardBackwards() ? PlayerController.Instance.boardController.boardMesh.forward : -PlayerController.Instance.boardController.boardMesh.forward);
		Vector3 vector31 = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterTransform.up, vector3);
		Vector3 vector32 = vector31.normalized;
		if (!PlayerController.Instance.IsGrounded() && !PlayerController.Instance.boardController.triggerManager.IsColliding)
		{
			instance = Quaternion.LookRotation(vector3, vector32);
		}
		Quaternion quaternion = Quaternion.Inverse(ikAnimBoard.rotation) * instance;
		Rigidbody rigidbody = _ikAnim;
		rigidbody.rotation = rigidbody.rotation * quaternion;
		_ikAnim.angularVelocity = physicsBoard.angularVelocity;
		_boardLastPos = physicsBoard.position;
	}

	private void MoveAndRotateSkaterIKTargets()
	{
		skaterLeftFootTargetParent.position = skaterLeftFoot.position;
		skaterLeftFootTargetParent.rotation = skaterLeftFoot.rotation;
		skaterRightFootTargetParent.position = skaterRightFoot.position;
		skaterRightFootTargetParent.rotation = skaterRightFoot.rotation;
	}

	private void OnAnimatorIK(int layer)
	{
		MoveAndRotateSkaterIKTargets();
	}

	public void OnOffIK(float p_value)
	{
		_leftPositionWeight = p_value;
		_rightPositionWeight = p_value;
		_rightRotationWeight = p_value;
		_leftRotationWeight = p_value;
		_finalIk.solver.leftFootEffector.positionWeight = p_value;
		_finalIk.solver.rightFootEffector.positionWeight = p_value;
		_finalIk.solver.rightFootEffector.rotationWeight = p_value;
		_finalIk.solver.leftFootEffector.rotationWeight = p_value;
	}

	public void ResetIKOffsets()
	{
		Vector3 vector3 = ikRightFootPositionOffset.localPosition;
		vector3.x = 0f;
		vector3.z = 0f;
		ikRightFootPositionOffset.localPosition = vector3;
		Vector3 vector31 = ikLeftFootPositionOffset.localPosition;
		vector31.x = 0f;
		vector31.z = 0f;
		ikLeftFootPositionOffset.localPosition = vector31;
	}

	public void RightIKWeight(float p_value)
	{
		_rightPositionWeight = p_value;
		_rightRotationWeight = p_value;
		_finalIk.solver.rightFootEffector.positionWeight = p_value;
		_finalIk.solver.rightFootEffector.rotationWeight = p_value;
	}

	private void SetIK()
	{
		ikLeftFootPosition.position = ikAnimLeftFootTarget.position;
		ikRightFootPosition.position = ikAnimRightFootTarget.position;
		_finalIk.solver.leftFootEffector.position = Vector3.Lerp(ikLeftFootPositionOffset.position, _skaterLeftFootPos, _ikLeftPosLerp);
		_finalIk.solver.rightFootEffector.position = Vector3.Lerp(ikRightFootPositionOffset.position, _skaterRightFootPos, _ikRightPosLerp);
		_finalIk.solver.leftFootEffector.rotation = Quaternion.Slerp(ikAnimLeftFootTarget.rotation, _skaterLeftFootRot, _ikLeftRotLerp);
		_finalIk.solver.rightFootEffector.rotation = Quaternion.Slerp(ikAnimRightFootTarget.rotation, _skaterRightFootRot, _ikRightRotLerp);
		_finalIk.solver.leftFootEffector.positionWeight = Mathf.MoveTowards(_finalIk.solver.leftFootEffector.positionWeight, _leftPositionWeight, Time.deltaTime * 5f);
		_finalIk.solver.rightFootEffector.positionWeight = Mathf.MoveTowards(_finalIk.solver.rightFootEffector.positionWeight, _rightPositionWeight, Time.deltaTime * 5f);
		_finalIk.solver.rightFootEffector.rotationWeight = Mathf.MoveTowards(_finalIk.solver.rightFootEffector.rotationWeight, _rightRotationWeight, Time.deltaTime * 5f);
		_finalIk.solver.leftFootEffector.rotationWeight = Mathf.MoveTowards(_finalIk.solver.leftFootEffector.rotationWeight, _leftRotationWeight, Time.deltaTime * 5f);
	}

	public void SetIKRigidbodyKinematic(bool p_value)
	{
		_ikAnim.isKinematic = p_value;
	}

	public void SetKneeBendWeight(float p_value)
	{
		_finalIk.solver.leftLegChain.bendConstraint.weight = p_value;
		_finalIk.solver.rightLegChain.bendConstraint.weight = p_value;
	}

	public void SetLeftIKOffset(float p_toeAxis, float p_forwardDir, float p_popDir, bool p_isPopStick, bool p_lockHorizontal, bool p_popping)
	{
		Vector3 pForwardDir = ikLeftFootPositionOffset.localPosition;
		if (p_lockHorizontal)
		{
			pForwardDir.y = 0f;
			pForwardDir.x = 0f;
			pForwardDir.z = p_forwardDir * offsetScaler;
		}
		else
		{
			pForwardDir.x = p_toeAxis * (p_popping ? popOffsetScaler : offsetScaler);
			if (!p_isPopStick)
			{
				pForwardDir.z = p_forwardDir * offsetScaler;
			}
			pForwardDir.y = 0f;
		}
		if (SettingsManager.Instance.stance == SettingsManager.Stance.Goofy)
		{
			pForwardDir.x = -pForwardDir.x;
		}
		pForwardDir.y = -0.01f;
		ikLeftFootPositionOffset.localPosition = Vector3.Lerp(ikLeftFootPositionOffset.localPosition, pForwardDir, Time.deltaTime * 10f);
	}

	public void SetLeftIKRotationWeight(float p_value)
	{
	}

	public void SetLeftLerpTarget(float pos, float rot)
	{
		_ikLeftLerpPosTarget = pos;
		_ikLeftLerpRotTarget = rot;
	}

	public void SetLeftSteezeWeight(float p_value)
	{
		_leftSteezeTarget = p_value;
	}

	public void SetMaxSteeze(float p_value)
	{
		_leftSteezeMax = p_value;
		_rightSteezeMax = p_value;
	}

	public void SetMaxSteezeLeft(float p_value)
	{
		_leftSteezeMax = p_value;
	}

	public void SetMaxSteezeRight(float p_value)
	{
		_rightSteezeMax = p_value;
	}

	public void SetRightIKOffset(float p_toeAxis, float p_forwardDir, float p_popDir, bool p_isPopStick, bool p_lockHorizontal, bool p_popping)
	{
		Vector3 pForwardDir = ikRightFootPositionOffset.localPosition;
		if (p_lockHorizontal)
		{
			pForwardDir.y = 0f;
			pForwardDir.x = 0f;
			pForwardDir.z = p_forwardDir * offsetScaler;
		}
		else
		{
			pForwardDir.x = p_toeAxis * (p_popping ? popOffsetScaler : offsetScaler);
			if (!p_isPopStick)
			{
				pForwardDir.z = p_forwardDir * offsetScaler;
			}
			pForwardDir.y = 0f;
		}
		if (SettingsManager.Instance.stance == SettingsManager.Stance.Goofy)
		{
			pForwardDir.x = -pForwardDir.x;
		}
		pForwardDir.y = 0.005f;
		ikRightFootPositionOffset.localPosition = Vector3.Lerp(ikRightFootPositionOffset.localPosition, pForwardDir, Time.deltaTime * 10f);
	}

	public void SetRightIKRotationWeight(float p_value)
	{
	}

	public void SetRightLerpTarget(float pos, float rot)
	{
		_ikRightLerpPosTarget = pos;
		_ikRightLerpRotTarget = rot;
	}

	public void SetRightSteezeWeight(float p_value)
	{
		_rightSteezeTarget = p_value;
	}

	private void SetSteezeWeight()
	{
		float single = Mathf.Clamp(_leftSteezeTarget, 0f, _leftSteezeMax);
		float single1 = Mathf.Clamp(_rightSteezeTarget, 0f, _rightSteezeMax);
		if (PlayerController.Instance.playerSM.LeftFootOffSM())
		{
			single = 1f;
		}
		if (PlayerController.Instance.playerSM.RightFootOffSM())
		{
			single1 = 1f;
		}
		_leftSteezeWeight = Mathf.MoveTowards(_leftSteezeWeight, single, Time.deltaTime * _steezeLerpSpeed);
		_rightSteezeWeight = Mathf.MoveTowards(_rightSteezeWeight, single1, Time.deltaTime * _steezeLerpSpeed);
		_skaterLeftFootPos = Vector3.Lerp(skaterLeftFootTarget.position, steezeLeftFootTarget.position, _leftSteezeWeight);
		_skaterLeftFootRot = Quaternion.Slerp(skaterLeftFootTarget.rotation, steezeLeftFootTarget.rotation, _leftSteezeWeight);
		_skaterRightFootPos = Vector3.Lerp(skaterRightFootTarget.position, steezeRightFootTarget.position, _rightSteezeWeight);
		_skaterRightFootRot = Quaternion.Slerp(skaterRightFootTarget.rotation, steezeRightFootTarget.rotation, _rightSteezeWeight);
	}

	private void Start()
	{
	}
}