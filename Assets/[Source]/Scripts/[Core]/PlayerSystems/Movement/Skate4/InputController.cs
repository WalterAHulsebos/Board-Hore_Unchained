using FSMHelper;
using Rewired;
using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
	public bool controlsActive;

	public InputThread inputThread;

	public DebugUI debugUI;

	public TutorialControllerUI tutControllerUI;

	public Player player;

	private float _timeSinceActivity;

	public float inactiveTime;

	[SerializeField]
	private StickInput _leftStick;

	[SerializeField]
	private StickInput _rightStick;

	public InputController.TurningMode turningMode = InputController.TurningMode.InAir;

	private static InputController _instance;

	private float _triggerMultiplier = 1f;

	private float _leftTrigger;

	private float _rightTrigger;

	private float _lastLeftTrigger;

	private float _lastRightTrigger;

	private float _triggerDeadZone = 0.05f;

	private bool _leftHeld;

	private bool _rightHeld;

	private float _turn;

	public static InputController Instance => _instance;

	public StickInput LeftStick => _leftStick;

	public StickInput RightStick => _rightStick;

	public float TriggerMultiplier
	{
		get => _triggerMultiplier;
		set => _triggerMultiplier = value;
	}

	public InputController()
	{
	}

	private void Awake()
	{
		player = ReInput.players.GetPlayer(0);
		_leftStick = gameObject.AddComponent<StickInput>();
		_rightStick = gameObject.AddComponent<StickInput>();
		if (!(_instance != null) || !(_instance != this))
		{
			_instance = this;
			return;
		}
		Destroy(gameObject);
	}

	private void BothTriggersReleased()
	{
		PlayerController.Instance.playerSM.BothTriggersReleasedSM(turningMode);
	}

	private void CheckForInactivity()
	{
		if (player.GetAnyButtonUp() || JoystickActive())
		{
			_timeSinceActivity = 0f;
		}
		else
		{
			_timeSinceActivity += Time.deltaTime;
		}
		if (_timeSinceActivity > 300f)
		{
			inactiveTime += Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		FixedUpdateTriggers();
		PlayerController.Instance.playerSM.OnStickFixedUpdateSM(_leftStick, _rightStick);
	}

	private void FixedUpdateTriggers()
	{
		if (PlayerController.Instance.playerSM.IsOnGroundStateSM())
		{
			if (!_leftStick.IsPopStick)
			{
				if (_leftStick.rawInput.pos.x < -0.3f)
				{
					LeftTriggerHeld(Mathf.Abs(_leftStick.rawInput.pos.x) * TriggerMultiplier);
				}
				else if (_leftStick.rawInput.pos.x > 0.3f)
				{
					RightTriggerHeld(Mathf.Abs(_leftStick.rawInput.pos.x) * TriggerMultiplier);
				}
			}
			if (!_rightStick.IsPopStick)
			{
				if (_rightStick.rawInput.pos.x < -0.3f)
				{
					LeftTriggerHeld(Mathf.Abs(_rightStick.rawInput.pos.x) * TriggerMultiplier);
				}
				else if (_rightStick.rawInput.pos.x > 0.3f)
				{
					RightTriggerHeld(Mathf.Abs(_rightStick.rawInput.pos.x) * TriggerMultiplier);
				}
			}
		}
		if (_leftHeld)
		{
			LeftTriggerHeld(_leftTrigger * TriggerMultiplier);
		}
		if (_rightHeld)
		{
			RightTriggerHeld(_rightTrigger * TriggerMultiplier);
		}
	}

	public float GetWindUp()
	{
		if (SettingsManager.Instance.stance != SettingsManager.Stance.Regular)
		{
			return _leftTrigger - _rightTrigger;
		}
		return -_leftTrigger + _rightTrigger;
	}

	private bool IsTurningWithSticks()
	{
		if (!PlayerController.Instance.playerSM.IsOnGroundStateSM())
		{
			return false;
		}
		if (Mathf.Abs(_leftStick.rawInput.pos.x) > 0.3f && Mathf.Abs(_leftStick.rawInput.pos.y) < 0.5f)
		{
			return true;
		}
		if (Mathf.Abs(_rightStick.rawInput.pos.x) <= 0.3f)
		{
			return false;
		}
		return Mathf.Abs(_rightStick.rawInput.pos.y) < 0.5f;
	}

	private bool JoystickActive()
	{
		if (Mathf.Abs(player.GetAxis("LeftStickX")) <= 0.1f && Mathf.Abs(player.GetAxis("RightStickX")) <= 0.1f && Mathf.Abs(player.GetAxis("LeftStickY")) <= 0.1f && Mathf.Abs(player.GetAxis("RightStickY")) <= 0.1f)
		{
			return false;
		}
		return true;
	}

	private void LeftTriggerHeld(float p_value)
	{
		PlayerController.Instance.playerSM.LeftTriggerHeldSM(p_value, turningMode);
	}

	private void LeftTriggerHeld(float p_value, bool p_skateControls)
	{
		PlayerController.Instance.playerSM.LeftTriggerHeldSM(p_value, turningMode);
	}

	private void LeftTriggerPressed()
	{
		PlayerController.Instance.playerSM.LeftTriggerPressedSM();
	}

	private void LeftTriggerReleased()
	{
		PlayerController.Instance.playerSM.LeftTriggerReleasedSM();
	}

	private void RightTriggerHeld(float p_value)
	{
		PlayerController.Instance.playerSM.RightTriggerHeldSM(p_value, turningMode);
	}

	private void RightTriggerPressed()
	{
		PlayerController.Instance.playerSM.RightTriggerPressedSM();
	}

	private void RightTriggerReleased()
	{
		PlayerController.Instance.playerSM.RightTriggerReleasedSM();
	}

	private void Update()
	{
		CheckForInactivity();
		if (controlsActive)
		{
			UpdateTriggers();
			UpdateSticks();
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
			if (player.GetButtonDown("Start"))
			{
				debugUI.SetColor(DebugUI.Buttons.Start);
			}
			if (player.GetButtonUp("Start"))
			{
				debugUI.ResetColor(DebugUI.Buttons.Start);
			}
			if (player.GetButtonDown("Select"))
			{
				if (PlayerController.Instance.playerSM.IsOnGroundStateSM())
				{
					SettingsManager.Instance.SetStance((SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? SettingsManager.Stance.Goofy : SettingsManager.Stance.Regular));
					PlayerController.Instance.respawn.DoRespawn();
				}
				debugUI.SetColor(DebugUI.Buttons.Select);
			}
			if (player.GetButtonUp("Select"))
			{
				debugUI.ResetColor(DebugUI.Buttons.Select);
			}
			if (player.GetButtonDown("Y"))
			{
				debugUI.SetColor(DebugUI.Buttons.Y);
				tutControllerUI.SetColor(TutorialControllerUI.Buttons.Y);
				if (Application.isEditor)
				{
					Time.timeScale = 0.05f;
				}
			}
			if (player.GetButtonUp("Y"))
			{
				debugUI.ResetColor(DebugUI.Buttons.Y);
				tutControllerUI.ResetColor(TutorialControllerUI.Buttons.Y);
			}
			if (player.GetButtonDown("B"))
			{
				PlayerController.Instance.playerSM.OnBrakePressedSM();
			}
			if (player.GetButton("B"))
			{
				debugUI.SetColor(DebugUI.Buttons.B);
				tutControllerUI.SetColor(TutorialControllerUI.Buttons.B);
				PlayerController.Instance.playerSM.OnBrakeHeldSM();
			}
			if (player.GetButtonUp("B"))
			{
				debugUI.ResetColor(DebugUI.Buttons.B);
				tutControllerUI.ResetColor(TutorialControllerUI.Buttons.B);
				PlayerController.Instance.playerSM.OnBrakeReleasedSM();
			}
			if (player.GetButtonDown("A"))
			{
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					if (PlayerController.Instance.IsSwitch)
					{
						PlayerController.Instance.playerSM.OnPushButtonPressedSM(true);
					}
					else
					{
						PlayerController.Instance.playerSM.OnPushButtonPressedSM(false);
					}
				}
				else if (PlayerController.Instance.IsSwitch)
				{
					PlayerController.Instance.playerSM.OnPushButtonPressedSM(false);
				}
				else
				{
					PlayerController.Instance.playerSM.OnPushButtonPressedSM(true);
				}
			}
			if (!player.GetButton("A"))
			{
				PlayerController.Instance.skaterController.pushBrakeForce = Vector3.zero;
			}
			else
			{
				debugUI.SetColor(DebugUI.Buttons.A);
				tutControllerUI.SetColor(TutorialControllerUI.Buttons.A);
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					if (PlayerController.Instance.IsSwitch)
					{
						PlayerController.Instance.playerSM.OnPushButtonHeldSM(true);
					}
					else
					{
						PlayerController.Instance.playerSM.OnPushButtonHeldSM(false);
					}
				}
				else if (PlayerController.Instance.IsSwitch)
				{
					PlayerController.Instance.playerSM.OnPushButtonHeldSM(false);
				}
				else
				{
					PlayerController.Instance.playerSM.OnPushButtonHeldSM(true);
				}
			}
			if (player.GetButtonUp("A"))
			{
				debugUI.ResetColor(DebugUI.Buttons.A);
				tutControllerUI.ResetColor(TutorialControllerUI.Buttons.A);
				PlayerController.Instance.playerSM.OnPushButtonReleasedSM();
			}
			if (player.GetButtonDown("X"))
			{
				if (Application.isEditor)
				{
					Time.timeScale = 1f;
				}
				debugUI.SetColor(DebugUI.Buttons.X);
				tutControllerUI.SetColor(TutorialControllerUI.Buttons.X);
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					if (PlayerController.Instance.IsSwitch)
					{
						PlayerController.Instance.playerSM.OnPushButtonPressedSM(false);
					}
					else
					{
						PlayerController.Instance.playerSM.OnPushButtonPressedSM(true);
					}
				}
				else if (PlayerController.Instance.IsSwitch)
				{
					PlayerController.Instance.playerSM.OnPushButtonPressedSM(true);
				}
				else
				{
					PlayerController.Instance.playerSM.OnPushButtonPressedSM(false);
				}
			}
			if (!player.GetButton("X"))
			{
				PlayerController.Instance.skaterController.pushBrakeForce = Vector3.zero;
			}
			else
			{
				debugUI.SetColor(DebugUI.Buttons.X);
				tutControllerUI.SetColor(TutorialControllerUI.Buttons.X);
				if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					if (PlayerController.Instance.IsSwitch)
					{
						PlayerController.Instance.playerSM.OnPushButtonHeldSM(false);
					}
					else
					{
						PlayerController.Instance.playerSM.OnPushButtonHeldSM(true);
					}
				}
				else if (PlayerController.Instance.IsSwitch)
				{
					PlayerController.Instance.playerSM.OnPushButtonHeldSM(true);
				}
				else
				{
					PlayerController.Instance.playerSM.OnPushButtonHeldSM(false);
				}
			}
			if (player.GetButtonUp("X"))
			{
				debugUI.ResetColor(DebugUI.Buttons.X);
				tutControllerUI.ResetColor(TutorialControllerUI.Buttons.X);
				PlayerController.Instance.playerSM.OnPushButtonReleasedSM();
			}
		}
		if (player.GetAxis("DPadX") > 0f)
		{
			debugUI.SetColor(DebugUI.Buttons.DPadRight);
			debugUI.ResetColor(DebugUI.Buttons.DPadLeft, 0.2f);
			tutControllerUI.SetColor(TutorialControllerUI.Buttons.DPadRight);
			tutControllerUI.ResetColor(TutorialControllerUI.Buttons.DPadLeft, 0.2f);
		}
		else if (player.GetAxis("DPadX") >= 0f)
		{
			debugUI.ResetColor(DebugUI.Buttons.DPadLeft, 0.2f);
			debugUI.ResetColor(DebugUI.Buttons.DPadRight, 0.2f);
			tutControllerUI.ResetColor(TutorialControllerUI.Buttons.DPadLeft, 0.2f);
			tutControllerUI.ResetColor(TutorialControllerUI.Buttons.DPadRight, 0.2f);
		}
		else
		{
			debugUI.SetColor(DebugUI.Buttons.DPadLeft);
			debugUI.ResetColor(DebugUI.Buttons.DPadRight, 0.2f);
			tutControllerUI.SetColor(TutorialControllerUI.Buttons.DPadLeft);
			tutControllerUI.ResetColor(TutorialControllerUI.Buttons.DPadRight, 0.2f);
		}
		if (player.GetAxis("DPadY") > 0f)
		{
			debugUI.SetColor(DebugUI.Buttons.DPadUp);
			debugUI.ResetColor(DebugUI.Buttons.DPadDown, 0.2f);
			tutControllerUI.SetColor(TutorialControllerUI.Buttons.DPadUp);
			tutControllerUI.ResetColor(TutorialControllerUI.Buttons.DPadDown, 0.2f);
		}
		else if (player.GetAxis("DPadY") >= 0f)
		{
			debugUI.ResetColor(DebugUI.Buttons.DPadDown, 0.2f);
			debugUI.ResetColor(DebugUI.Buttons.DPadUp, 0.2f);
			tutControllerUI.ResetColor(TutorialControllerUI.Buttons.DPadDown, 0.2f);
			tutControllerUI.ResetColor(TutorialControllerUI.Buttons.DPadUp, 0.2f);
		}
		else
		{
			debugUI.SetColor(DebugUI.Buttons.DPadDown);
			debugUI.ResetColor(DebugUI.Buttons.DPadUp, 0.2f);
			tutControllerUI.SetColor(TutorialControllerUI.Buttons.DPadDown);
			tutControllerUI.ResetColor(TutorialControllerUI.Buttons.DPadUp, 0.2f);
		}
		PlayerController.Instance.DebugRawAngles(_leftStick.rawInput.pos, false);
		PlayerController.Instance.DebugRawAngles(_rightStick.rawInput.pos, true);
	}

	private void UpdateSticks()
	{
		_leftStick.StickUpdate(false, inputThread);
		_rightStick.StickUpdate(true, inputThread);
		PlayerController.Instance.playerSM.OnStickUpdateSM(_leftStick, _rightStick);
		debugUI.UpdateLeftStickInput(_leftStick.rawInput.pos);
		debugUI.UpdateRightStickInput(_rightStick.rawInput.pos);
		tutControllerUI.UpdateLeftStickInput(_leftStick.rawInput.pos);
		tutControllerUI.UpdateRightStickInput(_rightStick.rawInput.pos);
		debugUI.UpdateLeftAugmentedStickInput(_leftStick.augmentedInput.pos);
		debugUI.UpdateRightAugmentedStickInput(_rightStick.augmentedInput.pos);
		if (player.GetButtonDown("Left Stick Button"))
		{
			_leftStick.OnStickPressed(false);
		}
		if (player.GetButtonDown("Right Stick Button"))
		{
			_rightStick.OnStickPressed(true);
		}
		if (player.GetButton("Left Stick Button"))
		{
			debugUI.SetColor(DebugUI.Buttons.LS);
			tutControllerUI.ShowClickLeft(true);
		}
		if (player.GetButtonUp("Left Stick Button"))
		{
			debugUI.ResetColor(DebugUI.Buttons.LS);
			tutControllerUI.ShowClickLeft(false);
		}
		if (player.GetButton("Right Stick Button"))
		{
			debugUI.SetColor(DebugUI.Buttons.RS);
			tutControllerUI.ShowClickRight(true);
		}
		if (player.GetButtonUp("Right Stick Button"))
		{
			debugUI.ResetColor(DebugUI.Buttons.RS);
			tutControllerUI.ShowClickRight(false);
		}
	}

	private void UpdateTriggers()
	{
		_leftTrigger = player.GetAxis("LT");
		_rightTrigger = player.GetAxis("RT");
		debugUI.LerpLeftTrigger(_leftTrigger);
		debugUI.LerpRightTrigger(_rightTrigger);
		tutControllerUI.LerpLeftTrigger(_leftTrigger);
		tutControllerUI.LerpRightTrigger(_rightTrigger);
		if (_lastLeftTrigger < _triggerDeadZone && _leftTrigger > _triggerDeadZone)
		{
			LeftTriggerPressed();
		}
		_leftHeld = (_leftTrigger > _triggerDeadZone ? true : false);
		if (_lastLeftTrigger > _triggerDeadZone && _leftTrigger < _triggerDeadZone && !IsTurningWithSticks())
		{
			LeftTriggerReleased();
		}
		if (_lastRightTrigger < _triggerDeadZone && _rightTrigger > _triggerDeadZone)
		{
			RightTriggerPressed();
		}
		_rightHeld = (_rightTrigger > _triggerDeadZone ? true : false);
		if (_lastRightTrigger > _triggerDeadZone && _rightTrigger < _triggerDeadZone && !IsTurningWithSticks())
		{
			RightTriggerReleased();
		}
		if (_leftTrigger < _triggerDeadZone && _rightTrigger < _triggerDeadZone && !IsTurningWithSticks())
		{
			BothTriggersReleased();
		}
		if (SettingsManager.Instance.stance != SettingsManager.Stance.Regular)
		{
			float single = _leftTrigger - _rightTrigger;
			PlayerController.Instance.AnimSetTurn(single);
			_turn = Mathf.MoveTowards(_turn, single, Time.deltaTime * 1.5f);
			PlayerController.Instance.AnimSetInAirTurn(_turn);
		}
		else
		{
			float single1 = -_leftTrigger + _rightTrigger;
			PlayerController.Instance.AnimSetTurn(single1);
			_turn = Mathf.MoveTowards(_turn, single1, Time.deltaTime * 1.5f);
			PlayerController.Instance.AnimSetInAirTurn(_turn);
		}
		_lastLeftTrigger = _leftTrigger;
		_lastRightTrigger = _rightTrigger;
	}

	public enum TurningMode
	{
		Grounded,
		PreWind,
		InAir,
		FastLeft,
		FastRight,
		Manual,
		None
	}
}