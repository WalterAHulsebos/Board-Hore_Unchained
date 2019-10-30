using FSMHelper;
using RootMotion.Dynamics;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Bailed : PlayerState_OffBoard
{
	private float _fallBlend;

	private float _fallTarget;

	private Rigidbody _hips;

	public PlayerState_Bailed()
	{
	}

	public override void Enter()
	{
		PlayerController.Instance.DoBailDelay();
		PlayerController.Instance.skaterController.skaterRigidbody.useGravity = true;
		_hips = PlayerController.Instance.respawn.puppetMaster.muscles[0].rigidbody;
	}

	public override void Exit()
	{
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
	{
		stateType = FSMStateType.Type_OR;
	}

	public override void Update()
	{
		base.Update();
		if (Vector3.ProjectOnPlane(_hips.velocity, Vector3.up).magnitude >= 0.5f)
		{
			if (_hips.velocity.y <= -1f)
			{
				_fallTarget = 0.35f;
			}
			else
			{
				_fallTarget = 1f;
			}
			_fallBlend = Mathf.Lerp(_fallBlend, _fallTarget, Time.fixedDeltaTime * 10f);
			PlayerController.Instance.animationController.SetValue("FallBlend", _fallBlend);
		}
		else if (PlayerController.Instance.respawn.puppetMaster.isAlive)
		{
			PlayerController.Instance.respawn.puppetMaster.Kill();
			return;
		}
	}
}