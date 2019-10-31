using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private static SoundManager _instance;

	public DeckSounds deckSounds;

	public PhysicMaterial mat;

	public float wheelRadius = 0.0275f;

	public Transform wheel1;

	public Transform wheel2;

	public Transform wheel3;

	public Transform wheel4;

	private float _rps;

	public static SoundManager Instance => _instance;

	public SoundManager()
	{
	}

	private void Awake()
	{
		if (!(_instance != null) || !(_instance != this))
		{
			_instance = this;
			return;
		}
		Destroy(gameObject);
	}

	public void PlayBoardImpactGround(float p_vol)
	{
		deckSounds.DoBoardImpactGround(p_vol);
	}

	public void PlayCatchSound()
	{
		deckSounds.DoShoesImpactBoardBack(1f);
	}

	public void PlayGrindSound(int p_grindType, float p_velocity)
	{
		switch (p_grindType)
		{
			case 0:
			{
				deckSounds.OnSmoothConcreteGrind(p_velocity);
				return;
			}
			case 1:
			{
				deckSounds.OnWoodGrind(p_velocity);
				return;
			}
			case 2:
			{
				deckSounds.OnMetalGrind(p_velocity);
				return;
			}
			default:
			{
				return;
			}
		}
	}

	public void PlayLandingSound(float p_vol)
	{
		deckSounds.DoLandingSound(p_vol);
	}

	public void PlayPopSound(float p_scoopSpeed)
	{
		deckSounds.DoOllie(p_scoopSpeed);
	}

	public void PlayPushOff(float p_vel)
	{
		deckSounds.OnPushOff(p_vel);
		deckSounds.OnPushImpact();
	}

	public void PlayWheelImpactGround(float p_vol)
	{
		deckSounds.DoWheelImpactGround(p_vol);
	}

	private void RollWheels(float _rotationsPerSecond)
	{
		if (Vector3.Angle(PlayerController.Instance.boardController.boardTransform.forward, PlayerController.Instance.boardController.boardRigidbody.velocity.normalized) > 90f)
		{
			_rotationsPerSecond = -_rotationsPerSecond;
		}
		Quaternion quaternion = Quaternion.Euler(_rotationsPerSecond, 0f, 0f);
		wheel1.rotation = wheel1.rotation * quaternion;
		Quaternion quaternion1 = Quaternion.Euler(_rotationsPerSecond, 0f, 0f);
		wheel2.rotation = wheel2.rotation * quaternion1;
		Quaternion quaternion2 = Quaternion.Euler(_rotationsPerSecond, 0f, 0f);
		wheel3.rotation = wheel3.rotation * quaternion2;
		Quaternion quaternion3 = Quaternion.Euler(_rotationsPerSecond, 0f, 0f);
		wheel4.rotation = wheel4.rotation * quaternion3;
	}

	public void SetGrindVolume(float p_velocity)
	{
		deckSounds.SetGrindingVolFromBoardSpeed(p_velocity);
	}

	public void SetRollingVolumeFromRPS(PhysicMaterial p_mat, float p_vel)
	{
		_rps = p_vel / (6.28318548f * wheelRadius);
		deckSounds.SetRollingVolFromRPS(p_mat, p_vel / (6.28318548f * wheelRadius));
		RollWheels(_rps);
	}

	public void StartBearingSound(float p_vol)
	{
		deckSounds.StartBearingSound(p_vol / (6.28318548f * wheelRadius));
	}

	public void StopBearingSound()
	{
		deckSounds.StopBearingSounds();
	}

	public void StopGrindSound(float p_exitVelocity)
	{
		deckSounds.StopGrind(p_exitVelocity);
	}
}