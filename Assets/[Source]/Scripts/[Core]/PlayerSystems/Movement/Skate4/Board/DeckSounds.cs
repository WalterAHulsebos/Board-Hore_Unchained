using System;
using UnityEngine;

public class DeckSounds : MonoBehaviour
{
	public float rollFilterRamp;

	public float grindVolMult;

	public float grindHitVolMult;

	public float grindVolMax;

	public AudioLowPassFilter mainListenerFilter;

	private static DeckSounds _instance;

	public Rigidbody board;

	public AudioClip musicClip;

	public AudioClip[] bumps;

	public AudioClip[] ollieScooped;

	public AudioClip[] ollieSlow;

	public AudioClip[] ollieFast;

	public AudioClip[] boardLand;

	public AudioClip[] boardImpacts;

	public AudioClip tutorialBoardImpact;

	public AudioClip[] bearingSounds;

	public AudioClip[] shoesBoardBackImpacts;

	public AudioClip[] shoesImpactGroundSole;

	public AudioClip[] shoesImpactGroundUpper;

	public AudioClip[] shoesMovementShort;

	public AudioClip[] shoesMovementLong;

	public AudioClip[] shoesPivotHeavy;

	public AudioClip[] shoesPivotLight;

	public AudioClip[] shoesPushImpact;

	public AudioClip[] shoesPushOff;

	public AudioClip[] concreteGrindGeneralStart;

	public AudioClip[] concreteGrindGeneralLoop;

	public AudioClip[] concreteGrindGeneralEnd;

	public AudioClip[] metalGrindGeneralStart;

	public AudioClip[] metalGrindGeneralLoop;

	public AudioClip[] metalGrindGeneralEnd;

	public AudioClip[] woodGrindGeneralStart;

	public AudioClip[] woodGrindGeneralLoop;

	public AudioClip[] woodGrindGeneralEnd;

	public AudioClip grassRollLoop;

	public AudioClip rollingSoundSlow;

	public AudioClip rollingSoundFast;

	public AudioSource musicSource;

	public AudioSource deckSource;

	public AudioSource shoesBoardHitSource;

	public AudioSource leftShoeHitSource;

	public AudioSource rightShoeHitSource;

	public AudioSource shoesScrapeSource;

	public AudioSource grindHitSource;

	public AudioSource grindLoopSource;

	public AudioSource bearingSource;

	public AudioSource wheelHitSource;

	public AudioSource wheelRollingLoopLowSource;

	public AudioSource wheelRollingLoopHighSource;

	public AudioLowPassFilter wheelRollingLoopLowFilter;

	public AudioLowPassFilter wheelRollingLoopHighFilter;

	private AudioSource[] _allSources;

	private bool _isMuted;

	public float landVolMult;

	private bool _boardImpactAllowed = true;

	private bool _wheelImpactAllowed = true;

	private bool _shoeBoardBackImpactAllowed = true;

	private bool _shoeGroundImpactAllowed = true;

	public float scoopMult;

	public DeckSounds.GrindState grindState;

	public float pushOffVolLow;

	public float pushOffVolHigh;

	public float _muteLerp;

	private float _rollingLowVolCache;

	private float _rollingHighVolCache;

	public float muteTimeStep = 0.02f;

	public static DeckSounds Instance => _instance;

	static DeckSounds()
	{
	}

	public DeckSounds()
	{
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		_allSources = GetComponentsInChildren<AudioSource>();
	}

	public void DoBoardImpactGround(float vol)
	{
		if (_boardImpactAllowed && vol > 0.01f)
		{
			if (vol < 0.1f)
			{
				vol = 0.1f;
			}
			if (vol > 0.6f)
			{
				vol = 0.6f;
			}
			PlayRandomOneShotFromArray(boardImpacts, deckSource, vol);
			_boardImpactAllowed = false;
			Invoke("ResetBoardImpactAllowed", 0.3f);
		}
	}

	public void DoLandingSound(float vol)
	{
		vol *= landVolMult;
		PlayRandomOneShotFromArray(boardLand, deckSource, vol);
	}

	public void DoOllie(float scoop)
	{
		scoop = scoop * scoopMult / 30f;
		scoop = Mathf.Abs(scoop);
		float single = scoop;
		float single1 = 1f - single;
		PlayRandomOneShotFromArray(ollieScooped, deckSource, single * 0.7f);
		PlayRandomOneShotFromArray(ollieSlow, deckSource, single1 * 0.7f);
	}

	public void DoShoeFlipSlide()
	{
		PlayRandomFromArray(shoesMovementShort, shoesScrapeSource);
		shoesScrapeSource.volume = 0f;
	}

	public void DoShoeImpactBoardBack(float vol, bool isLeftShoe)
	{
		if (_shoeBoardBackImpactAllowed && vol > 0.01f)
		{
			if (vol < 0.1f)
			{
				vol = 0.1f;
			}
			if (vol > 0.6f)
			{
				vol = 0.6f;
			}
			PlayRandomOneShotFromArray(shoesBoardBackImpacts, (isLeftShoe ? leftShoeHitSource : rightShoeHitSource), vol);
			_shoeBoardBackImpactAllowed = false;
			Invoke("ResetShoeBoardBackImpactAllowed", 0.1f);
		}
	}

	public void DoShoeImpactGround(float vol, bool isLeftShoe)
	{
		if (_shoeGroundImpactAllowed && vol > 0.01f)
		{
			if (vol < 0.05f)
			{
				vol = 0.05f;
			}
			if (vol > 0.45f)
			{
				vol = 0.45f;
			}
			PlayRandomOneShotFromArray(shoesImpactGroundSole, (isLeftShoe ? leftShoeHitSource : rightShoeHitSource), vol);
			_shoeGroundImpactAllowed = false;
			Invoke("ResetShoeGroundImpactAllowed", 0.1f);
		}
	}

	public void DoShoesImpactBoardBack(float vol)
	{
		shoesBoardHitSource.volume = 1f;
		PlayRandomOneShotFromArray(shoesBoardBackImpacts, shoesBoardHitSource, vol);
	}

	private void DoSmoothMuteRolling()
	{
		_muteLerp = _muteLerp - muteTimeStep * 2.85f;
		if (_muteLerp < 0f)
		{
			_muteLerp = 0f;
		}
		wheelRollingLoopLowSource.volume = _muteLerp * _rollingLowVolCache;
		wheelRollingLoopHighSource.volume = _muteLerp * _rollingLowVolCache;
		if (_muteLerp == 0f)
		{
			CancelInvoke();
			wheelRollingLoopLowSource.mute = true;
			wheelRollingLoopHighSource.mute = true;
		}
	}

	private void DoSmoothUnMuteRolling()
	{
		_muteLerp += muteTimeStep;
		if (_muteLerp > 1f)
		{
			_muteLerp = 1f;
		}
		wheelRollingLoopLowSource.volume = _muteLerp * _rollingLowVolCache;
		wheelRollingLoopHighSource.volume = _muteLerp * _rollingHighVolCache;
		if (_muteLerp == 1f)
		{
			CancelInvoke();
		}
	}

	public void DoTutorialBoardImpact(float vol)
	{
		deckSource.PlayOneShot(tutorialBoardImpact, vol);
	}

	public void DoWheelImpactGround(float vol)
	{
		if (_wheelImpactAllowed && vol > 0.01f)
		{
			if (vol < 0.1f)
			{
				vol = 0.1f;
			}
			if (vol > 0.6f)
			{
				vol = 0.6f;
			}
			PlayRandomOneShotFromArray(boardImpacts, deckSource, vol);
			_wheelImpactAllowed = false;
			Invoke("ResetWheelImpactAllowed", 0.1f);
		}
	}

	private void FixedUpdate()
	{
	}

	public void MuteAll()
	{
		AudioSource[] audioSourceArray = _allSources;
		for (int i = 0; i < (int)audioSourceArray.Length; i++)
		{
			audioSourceArray[i].mute = true;
		}
		_isMuted = true;
	}

	public void OnMetalGrind(float impactForce)
	{
		if (grindState == DeckSounds.GrindState.none)
		{
			Vector3 vector3 = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up);
			SetGrindingVolFromBoardSpeed(vector3.magnitude);
			PlayRandomOneShotFromArray(metalGrindGeneralStart, grindHitSource, impactForce / 10f);
			PlayRandomFromArray(metalGrindGeneralLoop, grindLoopSource);
			grindState = DeckSounds.GrindState.metal;
		}
	}

	public void OnPivot()
	{
		PlayRandomOneShotFromArray(shoesPivotLight, shoesBoardHitSource, 1f);
	}

	public void OnPushImpact()
	{
		PlayRandomOneShotFromArray(shoesPushImpact, grindHitSource, UnityEngine.Random.Range(0.1f, 0.6f));
	}

	public void OnPushOff(float pushSpeed)
	{
		if (shoesScrapeSource.isPlaying)
		{
			ShoePushOffSetMinVol(pushSpeed / 15f);
			return;
		}
		shoesScrapeSource.volume = 0f;
		PlayRandomFromArray(shoesMovementShort, shoesScrapeSource);
		ShoePushOffSetMinVol(UnityEngine.Random.Range(pushOffVolLow, pushOffVolHigh));
	}

	public void OnSmoothConcreteGrind(float impactForce)
	{
		if (grindState == DeckSounds.GrindState.none)
		{
			Vector3 vector3 = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up);
			SetGrindingVolFromBoardSpeed(vector3.magnitude);
			PlayRandomOneShotFromArray(concreteGrindGeneralStart, grindHitSource, impactForce / 10f);
			PlayRandomFromArray(concreteGrindGeneralLoop, grindLoopSource);
			grindState = DeckSounds.GrindState.concrete;
		}
	}

	public void OnWoodGrind(float impactForce)
	{
		if (grindState == DeckSounds.GrindState.none)
		{
			Vector3 vector3 = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up);
			SetGrindingVolFromBoardSpeed(vector3.magnitude);
			PlayRandomOneShotFromArray(woodGrindGeneralStart, grindHitSource, impactForce / 10f);
			PlayRandomFromArray(woodGrindGeneralLoop, grindLoopSource);
			grindState = DeckSounds.GrindState.wood;
		}
	}

	private void PlayRandomFromArray(AudioClip[] array, AudioSource source)
	{
		int num = UnityEngine.Random.Range(0, (int)array.Length);
		source.clip = array[num];
		source.Play();
	}

	private void PlayRandomOneShotFromArray(AudioClip[] array, AudioSource source, float vol)
	{
		if (array == null || array.Length == 0)
		{
			return;
		}
		int num = UnityEngine.Random.Range(0, (int)array.Length);
		source.PlayOneShot(array[num], vol);
	}

	private void ResetBoardImpactAllowed()
	{
		_boardImpactAllowed = true;
	}

	private void ResetShoeBoardBackImpactAllowed()
	{
		_shoeBoardBackImpactAllowed = true;
	}

	private void ResetShoeGroundImpactAllowed()
	{
		_shoeGroundImpactAllowed = true;
	}

	private void ResetWheelImpactAllowed()
	{
		_wheelImpactAllowed = true;
	}

	public void SetGrindingVolFromBoardSpeed(float p_velocity)
	{
		float pVelocity = p_velocity * grindVolMult;
		if (pVelocity > grindVolMax)
		{
			pVelocity = grindVolMax;
		}
		grindLoopSource.volume = pVelocity;
	}

	public void SetRollingVolFromRPS(PhysicMaterial mat, float rps)
	{
		if (mat != null)
		{
			if (wheelRollingLoopLowSource.clip != rollingSoundSlow)
			{
				wheelRollingLoopLowSource.clip = rollingSoundSlow;
				wheelRollingLoopLowSource.Play();
			}
			if (wheelRollingLoopHighSource.clip != rollingSoundFast)
			{
				wheelRollingLoopHighSource.clip = rollingSoundFast;
				wheelRollingLoopHighSource.Play();
			}
		}
		float single = 0f;
		float single1 = 5f;
		float single2 = rps / 30f;
		float single3 = single2 * 0.4f;
		float single4 = 0f;
		if (single2 < 1f)
		{
			single4 = 0f;
		}
		else if (single2 >= single && single2 < single + single1)
		{
			single4 = (single2 - single) / single1;
		}
		else if (single2 >= single)
		{
			single4 = 1f;
		}
		wheelRollingLoopLowSource.volume = single3 * (1f - single4);
		wheelRollingLoopHighSource.volume = single3 * single4;
		wheelRollingLoopLowFilter.cutoffFrequency = single3 * 22000f * rollFilterRamp;
		wheelRollingLoopHighFilter.cutoffFrequency = wheelRollingLoopLowFilter.cutoffFrequency;
	}

	public void ShoeFlipFinish()
	{
		shoesScrapeSource.Stop();
		shoesScrapeSource.volume = 1f;
	}

	public void ShoeFlipSlideSetMinVol(float newVol)
	{
		if (newVol > shoesScrapeSource.volume)
		{
			shoesScrapeSource.volume = Mathf.Clamp(newVol, 0f, 0.7f);
		}
	}

	public void ShoePushOffFinish()
	{
		shoesScrapeSource.volume = 0f;
	}

	private void ShoePushOffSetMinVol(float newVol)
	{
		if (newVol > shoesScrapeSource.volume)
		{
			shoesScrapeSource.volume = Mathf.Clamp(newVol, 0f, 0.2f);
		}
	}

	public void SmoothMuteRolling()
	{
		_rollingLowVolCache = wheelRollingLoopLowSource.volume;
		_rollingHighVolCache = wheelRollingLoopHighSource.volume;
		CancelInvoke();
		InvokeRepeating("DoSmoothMuteRolling", 0f, muteTimeStep);
	}

	public void SmoothUnMuteRolling()
	{
		CancelInvoke();
		wheelRollingLoopLowSource.mute = false;
		wheelRollingLoopHighSource.mute = false;
		InvokeRepeating("DoSmoothUnMuteRolling", 0f, muteTimeStep);
	}

	public void StartBearingSound(float rps)
	{
		float single = rps / 30f * 0.4f;
		PlayRandomOneShotFromArray(bearingSounds, bearingSource, Mathf.Clamp(single, 0.03f, 0.6f));
	}

	public void StopBearingSounds()
	{
		bearingSource.Stop();
	}

	public void StopGrind(float exitForce)
	{
		grindLoopSource.Stop();
		float single = 1f;
		if (single > grindVolMax)
		{
			single = grindVolMax;
		}
		switch (grindState)
		{
			case DeckSounds.GrindState.wood:
			{
				PlayRandomOneShotFromArray(woodGrindGeneralEnd, grindHitSource, single * 0.6f);
				break;
			}
			case DeckSounds.GrindState.metal:
			{
				PlayRandomOneShotFromArray(metalGrindGeneralEnd, grindHitSource, single * 0.6f);
				break;
			}
			case DeckSounds.GrindState.concrete:
			{
				PlayRandomOneShotFromArray(concreteGrindGeneralEnd, grindHitSource, single * 0.6f);
				break;
			}
		}
		grindState = DeckSounds.GrindState.none;
	}

	public void UnMuteAll()
	{
		AudioSource[] audioSourceArray = _allSources;
		for (int i = 0; i < (int)audioSourceArray.Length; i++)
		{
			audioSourceArray[i].mute = false;
		}
		_isMuted = false;
	}

	public void Update()
	{
	}

	public enum GrindState
	{
		none,
		wood,
		metal,
		concrete
	}
}