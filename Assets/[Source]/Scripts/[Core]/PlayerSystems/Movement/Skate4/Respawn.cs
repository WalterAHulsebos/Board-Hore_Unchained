using FSMHelper;
using Rewired;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using System;
using UnityEngine;

public class Respawn : MonoBehaviour
{
	public Transform pin;

	[SerializeField]
	private FullBodyBipedIK _finalIk;

	public PuppetMaster puppetMaster;

	public BehaviourPuppet behaviourPuppet;

	public Bail bail;

	public bool respawning;

	private bool _backwards;

	private string _idleAnimation = "Riding";

	public Transform[] getSpawn = new Transform[8];

	private Vector3[] _setPos = new Vector3[8];

	private Quaternion[] _setRot = new Quaternion[8];

	private bool _canPress;

	private bool _init;

	private Vector3 _playerOffset = Vector3.zero;

	private bool _retryRespawn;

	private bool _dPadYCentered;

	private float _dPadResetTimer;

	public Respawn()
	{
	}

	private void DelayPress()
	{
		_canPress = true;
	}

	public void DoRespawn()
	{
		if (_canPress && !respawning)
		{
			PlayerController.Instance.IsRespawning = true;
			respawning = true;
			_canPress = false;
			GetSpawnPos();
			PlayerController.Instance.CancelInvoke("DoBail");
			CancelInvoke("DelayPress");
			CancelInvoke("EndRespawning");
			Invoke("DelayPress", 0.4f);
			Invoke("EndRespawning", 0.25f);
		}
	}

	private void EndRespawning()
	{
		respawning = false;
		PlayerController.Instance.IsRespawning = false;
		_retryRespawn = false;
	}

	private void GetSpawnPos()
	{
		PlayerController.Instance.respawn.behaviourPuppet.BoostImmunity(1000f);
		CancelInvoke("DoRespawn");
		PlayerController.Instance.CancelRespawnInvoke();
		puppetMaster.FixTargetToSampledState(1f);
		puppetMaster.FixMusclePositions();
		behaviourPuppet.StopAllCoroutines();
		_finalIk.enabled = false;
		for (int i = 0; i < (int)getSpawn.Length; i++)
		{
			getSpawn[i].position = _setPos[i];
			getSpawn[i].rotation = _setRot[i];
		}
		bail.bailed = false;
		PlayerController.Instance.playerSM.OnRespawnSM();
		PlayerController.Instance.ResetIKOffsets();
		PlayerController.Instance.cameraController._leanForward = false;
		PlayerController.Instance.cameraController._pivot.rotation = PlayerController.Instance.cameraController._pivotCentered.rotation;
		PlayerController.Instance.comController.COMRigidbody.velocity = Vector3.zero;
		PlayerController.Instance.boardController.boardRigidbody.velocity = Vector3.zero;
		PlayerController.Instance.boardController.boardRigidbody.angularVelocity = Vector3.zero;
		PlayerController.Instance.boardController.frontTruckRigidbody.velocity = Vector3.zero;
		PlayerController.Instance.boardController.frontTruckRigidbody.angularVelocity = Vector3.zero;
		PlayerController.Instance.boardController.backTruckRigidbody.velocity = Vector3.zero;
		PlayerController.Instance.boardController.backTruckRigidbody.angularVelocity = Vector3.zero;
		PlayerController.Instance.skaterController.skaterRigidbody.velocity = Vector3.zero;
		PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
		PlayerController.Instance.skaterController.skaterRigidbody.useGravity = false;
		PlayerController.Instance.boardController.IsBoardBackwards = _backwards;
		PlayerController.Instance.SetBoardToMaster();
		PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
		PlayerController.Instance.ResetAllAnimations();
		PlayerController.Instance.animationController.ForceAnimation("Riding");
		PlayerController.Instance.boardController.firstVel = 0f;
		PlayerController.Instance.boardController.secondVel = 0f;
		PlayerController.Instance.boardController.thirdVel = 0f;
		PlayerController.Instance.skaterController.ResetRotationLerps();
		PlayerController.Instance.SetLeftIKLerpTarget(0f);
		PlayerController.Instance.SetRightIKLerpTarget(0f);
		PlayerController.Instance.SetMaxSteeze(0f);
		PlayerController.Instance.AnimSetPush(false);
		PlayerController.Instance.AnimSetMongo(false);
		PlayerController.Instance.CrossFadeAnimation("Riding", 0.05f);
		PlayerController.Instance.cameraController.ResetAllCamera();
		puppetMaster.targetRoot.position = _setPos[1] + _playerOffset;
		puppetMaster.targetRoot.rotation = _setRot[0];
		puppetMaster.angularLimits = false;
		puppetMaster.Resurrect();
		puppetMaster.state = PuppetMaster.State.Alive;
		puppetMaster.targetAnimator.Play(_idleAnimation, 0, 0f);
		behaviourPuppet.SetState(BehaviourPuppet.State.Puppet);
		puppetMaster.Teleport(_setPos[1] + _playerOffset, _setRot[0], true);
		PlayerController.Instance.SetIKOnOff(1f);
		PlayerController.Instance.skaterController.skaterRigidbody.useGravity = false;
		PlayerController.Instance.skaterController.skaterRigidbody.constraints = RigidbodyConstraints.None;
		_finalIk.enabled = true;
		_retryRespawn = false;
		puppetMaster.FixMusclePositions();
		PlayerController.Instance.respawn.behaviourPuppet.BoostImmunity(1000f);
	}

	private void SetSpawnPos()
	{
		pin.position = getSpawn[1].position + _playerOffset;
		Quaternion quaternion = Quaternion.LookRotation(getSpawn[0].rotation * Vector3.forward, Vector3.up);
		_backwards = PlayerController.Instance.GetBoardBackwards();
		for (int i = 0; i < (int)_setPos.Length; i++)
		{
			if ((float)i == 0f)
			{
				_setPos[i] = getSpawn[1].position + _playerOffset;
				_setRot[i] = quaternion;
			}
			else if (i == 5)
			{
				_setPos[i] = getSpawn[i].position;
				_setRot[i] = quaternion;
			}
			else if (i != 7)
			{
				_setPos[i] = getSpawn[i].position;
				_setRot[i] = getSpawn[i].rotation;
			}
			else
			{
				_setPos[i] = getSpawn[1].position + _playerOffset;
				_setRot[i] = quaternion;
			}
		}
	}

	private void Start()
	{
		_canPress = true;
		_playerOffset = getSpawn[0].position - getSpawn[1].position;
	}

	public void Update()
	{
		if (!_init && PlayerController.Instance.boardController.AllDown)
		{
			SetSpawnPos();
			_init = true;
		}
		if (_init && !_dPadYCentered && _canPress && !respawning && !puppetMaster.isBlending)
		{
			if (PlayerController.Instance.inputController.player.GetAxis("DPadY") < 0f && PlayerController.Instance.IsGrounded() && !bail.bailed && Time.timeScale != 0f)
			{
				_dPadYCentered = true;
				SetSpawnPos();
				_canPress = false;
				CancelInvoke("DelayPress");
				Invoke("DelayPress", 0.4f);
				pin.gameObject.SetActive(true);
			}
			if (PlayerController.Instance.inputController.player.GetAxis("DPadY") > 0f && Time.timeScale != 0f)
			{
				_dPadYCentered = true;
				DoRespawn();
			}
		}
		if (PlayerController.Instance.inputController.player.GetAxis("DPadY") == 0f && _dPadYCentered)
		{
			if (_dPadResetTimer >= 0.2f)
			{
				_dPadResetTimer = 0f;
				_dPadYCentered = false;
			}
			else
			{
				_dPadResetTimer += Time.deltaTime;
			}
		}
		if (respawning && !_retryRespawn && behaviourPuppet.state == BehaviourPuppet.State.Unpinned)
		{
			_canPress = true;
			respawning = false;
			_retryRespawn = true;
			DoRespawn();
		}
	}
}