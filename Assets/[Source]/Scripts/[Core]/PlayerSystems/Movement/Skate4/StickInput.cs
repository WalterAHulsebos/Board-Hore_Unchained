using FSMHelper;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StickInput : MonoBehaviour
{
	public StickInput.InputData rawInput;

	public StickInput.InputData augmentedInput;

	private bool _holdingManual;

	private int _manualFrameCount;

	private bool _holdingNoseManual;

	private int _noseManualFrameCount;

	private float _toeAxisLerp;

	private float _forwardDirLerp;

	private bool _popStickCentered = true;

	private bool _flipStickCentered = true;

	public float AugmentedFlipDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.x : augmentedInput.pos.x);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.x : augmentedInput.pos.x);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedForwardDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedLastSetupDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.y : -augmentedInput.prevPos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.prevPos.y : augmentedInput.prevPos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.prevPos.y : augmentedInput.prevPos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.y : -augmentedInput.prevPos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.y : -augmentedInput.prevPos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.prevPos.y : augmentedInput.prevPos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.y : augmentedInput.prevPos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.prevPos.y : -augmentedInput.prevPos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedLastToeAxis
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.x : -augmentedInput.prevPos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.x : -augmentedInput.prevPos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.prevPos.x : augmentedInput.prevPos.x);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.prevPos.x : augmentedInput.prevPos.x);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.x : -augmentedInput.prevPos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.x : -augmentedInput.prevPos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.x : -augmentedInput.prevPos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.prevPos.x : -augmentedInput.prevPos.x);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedManualAxis
	{
		get
		{
			float single = 0f;
			if (!IsRightStick)
			{
				switch (SettingsManager.Instance.controlType)
				{
					case SettingsManager.ControlType.Same:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -augmentedInput.pos.y);
							break;
						}
					}
					case SettingsManager.ControlType.Swap:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -augmentedInput.pos.y);
							break;
						}
					}
					case SettingsManager.ControlType.Simple:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
					}
				}
			}
			else
			{
				switch (SettingsManager.Instance.controlType)
				{
					case SettingsManager.ControlType.Same:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : 0f);
							break;
						}
					}
					case SettingsManager.ControlType.Swap:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : 0f);
							break;
						}
					}
					case SettingsManager.ControlType.Simple:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
							break;
						}
					}
				}
			}
			return Mathf.Clamp(single, 0f, 1f);
		}
	}

	public float AugmentedNollieSetupDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : 0f);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : augmentedInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : augmentedInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : 0f);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedNoseManualAxis
	{
		get
		{
			float single = 0f;
			if (!IsRightStick)
			{
				switch (SettingsManager.Instance.controlType)
				{
					case SettingsManager.ControlType.Same:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : 0f);
							break;
						}
					}
					case SettingsManager.ControlType.Swap:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : 0f);
							break;
						}
					}
					case SettingsManager.ControlType.Simple:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
					}
				}
			}
			else
			{
				switch (SettingsManager.Instance.controlType)
				{
					case SettingsManager.ControlType.Same:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : augmentedInput.pos.y);
							break;
						}
					}
					case SettingsManager.ControlType.Swap:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : augmentedInput.pos.y);
							break;
						}
					}
					case SettingsManager.ControlType.Simple:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
					}
				}
			}
			return Mathf.Clamp(single, 0f, 1f);
		}
	}

	public float AugmentedOllieSetupDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : 0f);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : 0f);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : 0f);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -augmentedInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedPopDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (IsPopStick)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : -augmentedInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : augmentedInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (IsPopStick)
						{
							if (!IsRightStick)
							{
								single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : -augmentedInput.pos.y);
								break;
							}
							else
							{
								single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : augmentedInput.pos.y);
								break;
							}
						}
						else if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : -augmentedInput.pos.y);
							break;
						}
					}
					else if (IsPopStick)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : -augmentedInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : augmentedInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsPopStick)
					{
						break;
					}
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedPopSpeed => AugmentedPopToeVel.y;

	public float AugmentedPopToeSpeed => AugmentedPopToeVel.magnitude;

	public Vector2 AugmentedPopToeVector => Vector2.ClampMagnitude(new Vector2(AugmentedToeAxis, AugmentedSetupDir), 1f);

	public Vector2 AugmentedPopToeVel => new Vector2(AugmentedToeAxisVel, AugmentedSetupDirVel);

	public float AugmentedSetupDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : augmentedInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : augmentedInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : -augmentedInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : augmentedInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.y : augmentedInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.y : -augmentedInput.pos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedSetupDirVel
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.y : -augmentedInput.maxVelLastUpdate.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.maxVelLastUpdate.y : augmentedInput.maxVelLastUpdate.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.maxVelLastUpdate.y : augmentedInput.maxVelLastUpdate.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.y : -augmentedInput.maxVelLastUpdate.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.y : -augmentedInput.maxVelLastUpdate.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.maxVelLastUpdate.y : augmentedInput.maxVelLastUpdate.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.y : augmentedInput.maxVelLastUpdate.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.maxVelLastUpdate.y : -augmentedInput.maxVelLastUpdate.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedToeAxis
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.x : augmentedInput.pos.x);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.pos.x : augmentedInput.pos.x);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.pos.x : -augmentedInput.pos.x);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedToeAxisVel
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.x : -augmentedInput.maxVelLastUpdate.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.x : -augmentedInput.maxVelLastUpdate.x);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.maxVelLastUpdate.x : augmentedInput.maxVelLastUpdate.x);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -augmentedInput.maxVelLastUpdate.x : augmentedInput.maxVelLastUpdate.x);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.x : -augmentedInput.maxVelLastUpdate.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.x : -augmentedInput.maxVelLastUpdate.x);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.x : -augmentedInput.maxVelLastUpdate.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? augmentedInput.maxVelLastUpdate.x : -augmentedInput.maxVelLastUpdate.x);
						break;
					}
				}
			}
			return single;
		}
	}

	public float AugmentedToeSpeed => AugmentedPopToeVel.x;

	public float FlipDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.x : -rawInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.x : -rawInput.pos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.x : rawInput.pos.x);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.x : rawInput.pos.x);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.x : -rawInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.x : -rawInput.pos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.x : -rawInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.x : -rawInput.pos.x);
						break;
					}
				}
			}
			return single;
		}
	}

	public float ForwardDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public bool HoldingManual
	{
		get => _holdingManual;
		set => _holdingManual = value;
	}

	public bool HoldingNoseManual
	{
		get => _holdingNoseManual;
		set => _holdingNoseManual = value;
	}

	public bool IsFrontFoot
	{
		get;
		set;
	}

	public bool IsPopStick
	{
		get
		{
			if (PlayerController.Instance.playerSM.GetPopStickSM() != this)
			{
				return false;
			}
			return true;
		}
	}

	public bool IsRightStick
	{
		get;
		set;
	}

	public float LastSetupDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.y : -rawInput.prevPos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.prevPos.y : rawInput.prevPos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.prevPos.y : rawInput.prevPos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.y : -rawInput.prevPos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.y : -rawInput.prevPos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.prevPos.y : rawInput.prevPos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.y : rawInput.prevPos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.prevPos.y : -rawInput.prevPos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float LastToeAxis
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.x : -rawInput.prevPos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.x : -rawInput.prevPos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.prevPos.x : rawInput.prevPos.x);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.prevPos.x : rawInput.prevPos.x);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.x : -rawInput.prevPos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.x : -rawInput.prevPos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.x : -rawInput.prevPos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.prevPos.x : -rawInput.prevPos.x);
						break;
					}
				}
			}
			return single;
		}
	}

	public float ManualAxis
	{
		get
		{
			float single = 0f;
			if (!IsRightStick)
			{
				switch (SettingsManager.Instance.controlType)
				{
					case SettingsManager.ControlType.Same:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -rawInput.pos.y);
							break;
						}
					}
					case SettingsManager.ControlType.Swap:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -rawInput.pos.y);
							break;
						}
					}
					case SettingsManager.ControlType.Simple:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
					}
				}
			}
			else
			{
				switch (SettingsManager.Instance.controlType)
				{
					case SettingsManager.ControlType.Same:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : 0f);
							break;
						}
					}
					case SettingsManager.ControlType.Swap:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : 0f);
							break;
						}
					}
					case SettingsManager.ControlType.Simple:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
							break;
						}
					}
				}
			}
			return Mathf.Clamp(single, 0f, 1f);
		}
	}

	public int ManualFrameCount
	{
		get => _manualFrameCount;
		set => _manualFrameCount = value;
	}

	public float NollieSetupDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : 0f);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : rawInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : rawInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : 0f);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
						break;
					}
				}
			}
			return single;
		}
	}

	public float NoseManualAxis
	{
		get
		{
			float single = 0f;
			if (!IsRightStick)
			{
				switch (SettingsManager.Instance.controlType)
				{
					case SettingsManager.ControlType.Same:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : 0f);
							break;
						}
					}
					case SettingsManager.ControlType.Swap:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : 0f);
							break;
						}
					}
					case SettingsManager.ControlType.Simple:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
							break;
						}
					}
				}
			}
			else
			{
				switch (SettingsManager.Instance.controlType)
				{
					case SettingsManager.ControlType.Same:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : rawInput.pos.y);
							break;
						}
					}
					case SettingsManager.ControlType.Swap:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : rawInput.pos.y);
							break;
						}
					}
					case SettingsManager.ControlType.Simple:
					{
						if (PlayerController.Instance.IsSwitch)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
					}
				}
			}
			return Mathf.Clamp(single, 0f, 1f);
		}
	}

	public int NoseManualFrameCount
	{
		get => _noseManualFrameCount;
		set => _noseManualFrameCount = value;
	}

	public float OllieSetupDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : 0f);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : 0f);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : 0f);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : -rawInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? 0f : 0f);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float PopDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (IsPopStick)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : -rawInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : -rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : rawInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (IsPopStick)
						{
							if (!IsRightStick)
							{
								single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : -rawInput.pos.y);
								break;
							}
							else
							{
								single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : rawInput.pos.y);
								break;
							}
						}
						else if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : -rawInput.pos.y);
							break;
						}
					}
					else if (IsPopStick)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : -rawInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : -rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : rawInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsPopStick)
					{
						break;
					}
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float PopSpeed => PopToeVel.y;

	public float PopToeSpeed => PopToeVel.magnitude;

	public Vector2 PopToeVector => Vector2.ClampMagnitude(new Vector2(ToeAxis, SetupDir), 1f);

	public Vector2 PopToeVel => new Vector2(ToeAxisVel, SetupDirVel);

	public float SetupDir
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : -rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : rawInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : rawInput.pos.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : -rawInput.pos.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : -rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : rawInput.pos.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.y : rawInput.pos.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.y : -rawInput.pos.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float SetupDirVel
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.y : -rawInput.maxVelLastUpdate.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.maxVelLastUpdate.y : rawInput.maxVelLastUpdate.y);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.maxVelLastUpdate.y : rawInput.maxVelLastUpdate.y);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.y : -rawInput.maxVelLastUpdate.y);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.y : -rawInput.maxVelLastUpdate.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.maxVelLastUpdate.y : rawInput.maxVelLastUpdate.y);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.y : rawInput.maxVelLastUpdate.y);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.maxVelLastUpdate.y : -rawInput.maxVelLastUpdate.y);
						break;
					}
				}
			}
			return single;
		}
	}

	public float ToeAxis
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.x : -rawInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.x : -rawInput.pos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.x : rawInput.pos.x);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.pos.x : rawInput.pos.x);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.x : -rawInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.x : -rawInput.pos.x);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.x : -rawInput.pos.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.pos.x : -rawInput.pos.x);
						break;
					}
				}
			}
			return single;
		}
	}

	public float ToeAxisVel
	{
		get
		{
			float single = 0f;
			switch (SettingsManager.Instance.controlType)
			{
				case SettingsManager.ControlType.Same:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.x : -rawInput.maxVelLastUpdate.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.x : -rawInput.maxVelLastUpdate.x);
						break;
					}
				}
				case SettingsManager.ControlType.Swap:
				{
					if (PlayerController.Instance.IsSwitch)
					{
						if (!IsRightStick)
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.maxVelLastUpdate.x : rawInput.maxVelLastUpdate.x);
							break;
						}
						else
						{
							single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? -rawInput.maxVelLastUpdate.x : rawInput.maxVelLastUpdate.x);
							break;
						}
					}
					else if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.x : -rawInput.maxVelLastUpdate.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.x : -rawInput.maxVelLastUpdate.x);
						break;
					}
				}
				case SettingsManager.ControlType.Simple:
				{
					if (!IsRightStick)
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.x : -rawInput.maxVelLastUpdate.x);
						break;
					}
					else
					{
						single = (SettingsManager.Instance.stance == SettingsManager.Stance.Regular ? rawInput.maxVelLastUpdate.x : -rawInput.maxVelLastUpdate.x);
						break;
					}
				}
			}
			return single;
		}
	}

	public float ToeSpeed => PopToeVel.x;

	public StickInput()
	{
	}

	private void Awake()
	{
		InitializeRawData();
	}

	private void InitializeRawData()
	{
		rawInput.pos = Vector2.zero;
		rawInput.prevPos = Vector2.zero;
		rawInput.lastPos = Vector2.zero;
		rawInput.avgSpeedLastUpdate = 0f;
		augmentedInput.pos = Vector2.zero;
		augmentedInput.prevPos = Vector2.zero;
		augmentedInput.lastPos = Vector2.zero;
		augmentedInput.avgSpeedLastUpdate = 0f;
	}

	private void LerpFootDir()
	{
		_toeAxisLerp = Mathf.Lerp(_toeAxisLerp, ToeAxis, Time.deltaTime * 10f);
		_forwardDirLerp = Mathf.Lerp(_forwardDirLerp, ForwardDir, Time.deltaTime * 10f);
	}

	private void OnFlipStickCentered()
	{
		if (!_flipStickCentered)
		{
			if (rawInput.lastPos.magnitude < 0.05f && rawInput.prevPos.magnitude < 0.05f && rawInput.pos.magnitude < 0.05f && rawInput.avgSpeedLastUpdate < 5f)
			{
				_flipStickCentered = true;
				PlayerController.Instance.playerSM.OnFlipStickCenteredSM();
				return;
			}
		}
		else if (rawInput.pos.magnitude > 0.05f || rawInput.avgSpeedLastUpdate < 5f)
		{
			_flipStickCentered = false;
		}
	}

	private void OnPopStickCentered()
	{
		if (!_popStickCentered)
		{
			if (rawInput.lastPos.magnitude < 0.05f && rawInput.prevPos.magnitude < 0.05f && rawInput.pos.magnitude < 0.05f && rawInput.avgSpeedLastUpdate < 1f)
			{
				_popStickCentered = true;
				PlayerController.Instance.playerSM.OnPopStickCenteredSM();
				return;
			}
		}
		else if (rawInput.pos.magnitude > 0.05f || rawInput.avgSpeedLastUpdate < 5f)
		{
			_popStickCentered = false;
		}
	}

	private void OnStickCenteredUpdate(bool p_right)
	{
		if (p_right)
		{
			PlayerController.Instance.playerSM.OnRightStickCenteredUpdateSM();
			return;
		}
		PlayerController.Instance.playerSM.OnLeftStickCenteredUpdateSM();
	}

	public void OnStickPressed(bool p_right)
	{
		PlayerController.Instance.playerSM.OnStickPressedSM(p_right);
	}

	private void SteezeIKWeights()
	{
		if (SettingsManager.Instance.controlType != SettingsManager.ControlType.Simple)
		{
			if (IsRightStick)
			{
				PlayerController.Instance.SetRightSteezeWeight(rawInput.pos.magnitude);
				return;
			}
			PlayerController.Instance.SetLeftSteezeWeight(rawInput.pos.magnitude);
			return;
		}
		if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
		{
			if (!PlayerController.Instance.IsSwitch)
			{
				if (IsRightStick)
				{
					PlayerController.Instance.SetRightSteezeWeight(rawInput.pos.magnitude);
					return;
				}
				PlayerController.Instance.SetLeftSteezeWeight(rawInput.pos.magnitude);
				return;
			}
			if (IsRightStick)
			{
				PlayerController.Instance.SetLeftSteezeWeight(rawInput.pos.magnitude);
				return;
			}
			PlayerController.Instance.SetRightSteezeWeight(rawInput.pos.magnitude);
			return;
		}
		if (!PlayerController.Instance.IsSwitch)
		{
			if (IsRightStick)
			{
				PlayerController.Instance.SetLeftSteezeWeight(rawInput.pos.magnitude);
				return;
			}
			PlayerController.Instance.SetRightSteezeWeight(rawInput.pos.magnitude);
			return;
		}
		if (IsRightStick)
		{
			PlayerController.Instance.SetRightSteezeWeight(rawInput.pos.magnitude);
			return;
		}
		PlayerController.Instance.SetLeftSteezeWeight(rawInput.pos.magnitude);
	}

	public void StickUpdate(bool p_right, InputThread _inputThread)
	{
		UpdateRawInput(p_right, _inputThread);
		UpdateInterpretedInput(p_right);
		if (rawInput.pos.magnitude < 0.1f && rawInput.avgVelLastUpdate.magnitude < 5f)
		{
			OnStickCenteredUpdate(p_right);
		}
		if (!IsPopStick)
		{
			PlayerController.Instance.playerSM.OnFlipStickUpdateSM();
			OnFlipStickCentered();
			PlayerController.Instance.DebugPopStick(false, p_right);
		}
		else
		{
			PlayerController.Instance.playerSM.OnPopStickUpdateSM();
			OnPopStickCentered();
			PlayerController.Instance.DebugPopStick(true, p_right);
		}
		PlayerController.Instance.OnManualUpdate(this);
		PlayerController.Instance.OnNoseManualUpdate(this);
		LerpFootDir();
		PlayerController.Instance.SetInAirFootPlacement(_toeAxisLerp, _forwardDirLerp, IsFrontFoot);
		SteezeIKWeights();
	}

	private void UpdateInterpretedInput(bool p_right)
	{
		IsRightStick = p_right;
	}

	private void UpdateRawInput(bool _right, InputThread _inputThread)
	{
		Vector2 vector2;
		Vector2 vector21;
		Vector2 vector22;
		Vector2 vector23;
		rawInput.stick = (_right ? StickInput.InputData.Stick.Right : StickInput.InputData.Stick.Left);
		rawInput.lastPos = rawInput.prevPos;
		rawInput.prevPos = rawInput.pos;
		vector2 = (_right ? _inputThread.lastPosRight : _inputThread.lastPosLeft);
		rawInput.pos = vector2;
		vector21 = (_right ? _inputThread.maxVelLastUpdateRight : _inputThread.maxVelLastUpdateLeft);
		rawInput.maxVelLastUpdate = vector21;
		rawInput.radialVel = (rawInput.pos.magnitude - rawInput.prevPos.magnitude) / Time.deltaTime;
		augmentedInput.stick = (_right ? StickInput.InputData.Stick.Right : StickInput.InputData.Stick.Left);
		augmentedInput.lastPos = augmentedInput.prevPos;
		augmentedInput.prevPos = augmentedInput.pos;
		vector22 = (_right ? Mathd.RotateVector2(_inputThread.lastPosRight, PlayerController.Instance.playerSM.GetAugmentedAngleSM(this)) : Mathd.RotateVector2(_inputThread.lastPosLeft, PlayerController.Instance.playerSM.GetAugmentedAngleSM(this)));
		augmentedInput.pos = vector22;
		vector23 = (_right ? Mathd.RotateVector2(_inputThread.maxVelLastUpdateRight, PlayerController.Instance.playerSM.GetAugmentedAngleSM(this)) : Mathd.RotateVector2(_inputThread.maxVelLastUpdateLeft, PlayerController.Instance.playerSM.GetAugmentedAngleSM(this)));
		augmentedInput.maxVelLastUpdate = vector23;
		if (SettingsManager.Instance.controlType != SettingsManager.ControlType.Same && SettingsManager.Instance.controlType != SettingsManager.ControlType.Swap)
		{
			if (IsRightStick)
			{
				IsFrontFoot = false;
				return;
			}
			IsFrontFoot = true;
			return;
		}
		if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
		{
			if (IsRightStick)
			{
				IsFrontFoot = false;
				return;
			}
			IsFrontFoot = true;
			return;
		}
		if (IsRightStick)
		{
			IsFrontFoot = true;
			return;
		}
		IsFrontFoot = false;
	}

	public struct InputData
	{
		public StickInput.InputData.Stick stick;

		public Vector2 pos;

		public Vector2 prevPos;

		public Vector2 lastPos;

		public float avgSpeedLastUpdate;

		public Vector2 maxVelLastUpdate;

		public Vector2 avgVelLastUpdate;

		public float radialVel;

		public enum Stick
		{
			Left,
			Right
		}
	}
}