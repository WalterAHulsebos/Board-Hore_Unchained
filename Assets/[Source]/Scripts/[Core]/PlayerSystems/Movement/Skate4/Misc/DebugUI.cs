using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
	[Header("Button Images")]
	[SerializeField]
	private Image A;

	[SerializeField]
	private Image B;

	[SerializeField]
	private Image X;

	[SerializeField]
	private Image Y;

	[SerializeField]
	private Image Start;

	[SerializeField]
	private Image Select;

	[SerializeField]
	private Image DPadUp;

	[SerializeField]
	private Image DPadDown;

	[SerializeField]
	private Image DPadLeft;

	[SerializeField]
	private Image DPadRight;

	[SerializeField]
	private Image RT;

	[SerializeField]
	private Image LT;

	[SerializeField]
	private Image LeftStickClick;

	[SerializeField]
	private Image RightStickClick;

	[Header("Transforms")]
	[SerializeField]
	private RectTransform LeftTriggerFill;

	[SerializeField]
	private RectTransform LeftTriggerZero;

	[SerializeField]
	private RectTransform LeftTriggerOne;

	[SerializeField]
	private RectTransform RightTriggerFill;

	[SerializeField]
	private RectTransform RightTriggerZero;

	[SerializeField]
	private RectTransform RightTriggerOne;

	[SerializeField]
	private RectTransform[] LeftAnalogPos = new RectTransform[4];

	[SerializeField]
	private RectTransform[] RightAnalogPos = new RectTransform[4];

	[SerializeField]
	private RectTransform[] LeftAugmentedAnalogPos = new RectTransform[4];

	[SerializeField]
	private RectTransform[] RightAugmentedAnalogPos = new RectTransform[4];

	[Header("Debug Text Info")]
	[SerializeField]
	private TextMeshProUGUI _inputThread;

	[SerializeField]
	private TextMeshProUGUI _skaterVelocity;

	[SerializeField]
	private TextMeshProUGUI _skaterAngularVelocity;

	[SerializeField]
	private TextMeshProUGUI _boardVelocity;

	[SerializeField]
	private TextMeshProUGUI _boardAngularVelocity;

	[Header("Helper Data")]
	[SerializeField]
	private float _stickInputScaler = 300f;

	public DebugUI()
	{
	}

	private void FixedUpdate()
	{
		TextMeshProUGUI str = _skaterVelocity;
		Vector3 instance = PlayerController.Instance.skaterController.skaterRigidbody.velocity;
		double num = Math.Round((double)instance.magnitude, 3);
		str.text = num.ToString("n3");
		TextMeshProUGUI textMeshProUGUI = _skaterAngularVelocity;
		instance = PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity;
		num = Math.Round((double)instance.magnitude, 3);
		textMeshProUGUI.text = num.ToString("n3");
		TextMeshProUGUI str1 = _boardVelocity;
		instance = PlayerController.Instance.boardController.boardRigidbody.velocity;
		num = Math.Round((double)instance.magnitude, 5);
		str1.text = num.ToString("n3");
		TextMeshProUGUI textMeshProUGUI1 = _boardAngularVelocity;
		instance = PlayerController.Instance.boardController.boardRigidbody.angularVelocity;
		num = Math.Round((double)instance.magnitude, 3);
		textMeshProUGUI1.text = num.ToString("n3");
	}

	private Image GetButton(DebugUI.Buttons p_button)
	{
		Image a = A;
		switch (p_button)
		{
			case DebugUI.Buttons.A:
			{
				a = A;
				break;
			}
			case DebugUI.Buttons.B:
			{
				a = B;
				break;
			}
			case DebugUI.Buttons.X:
			{
				a = X;
				break;
			}
			case DebugUI.Buttons.Y:
			{
				a = Y;
				break;
			}
			case DebugUI.Buttons.Start:
			{
				a = Start;
				break;
			}
			case DebugUI.Buttons.Select:
			{
				a = Select;
				break;
			}
			case DebugUI.Buttons.DPadUp:
			{
				a = DPadUp;
				break;
			}
			case DebugUI.Buttons.DPadDown:
			{
				a = DPadDown;
				break;
			}
			case DebugUI.Buttons.DPadLeft:
			{
				a = DPadLeft;
				break;
			}
			case DebugUI.Buttons.DPadRight:
			{
				a = DPadRight;
				break;
			}
			case DebugUI.Buttons.RT:
			{
				a = RT;
				break;
			}
			case DebugUI.Buttons.LT:
			{
				a = LT;
				break;
			}
			case DebugUI.Buttons.LS:
			{
				a = LeftStickClick;
				break;
			}
			case DebugUI.Buttons.RS:
			{
				a = RightStickClick;
				break;
			}
		}
		return a;
	}

	public void LerpLeftTrigger(float p_value)
	{
		LeftTriggerFill.localPosition = Vector3.Lerp(LeftTriggerZero.localPosition, LeftTriggerOne.localPosition, p_value);
	}

	public void LerpRightTrigger(float p_value)
	{
		RightTriggerFill.localPosition = Vector3.Lerp(RightTriggerZero.localPosition, RightTriggerOne.localPosition, p_value);
	}

	public void ResetColor(DebugUI.Buttons p_button)
	{
		if (GetButton(p_button).color != Color.white)
		{
			GetButton(p_button).color = Color.white;
		}
	}

	public void ResetColor(DebugUI.Buttons p_button, float p_alpha)
	{
		if (GetButton(p_button).color != new Color(Color.white.r, Color.white.g, Color.white.b, p_alpha))
		{
			GetButton(p_button).color = new Color(Color.white.r, Color.white.g, Color.white.b, p_alpha);
		}
	}

	public IEnumerator ResetColor(Image p_image)
	{
		yield return null;
		p_image.color = Color.white;
	}

	public void SetColor(DebugUI.Buttons p_button)
	{
		if (GetButton(p_button).color != Color.red)
		{
			GetButton(p_button).color = Color.red;
		}
	}

	public void SetColorOnce(DebugUI.Buttons p_button)
	{
		GetButton(p_button).color = Color.red;
		StartCoroutine(ResetColor(GetButton(p_button)));
	}

	public void SetThreadActive(bool p_active)
	{
	}

	public void UpdateLeftAugmentedStickInput(Vector2 p_pos)
	{
		UpdateStickInput(ref LeftAugmentedAnalogPos, p_pos);
	}

	public void UpdateLeftStickInput(Vector2 p_pos)
	{
		UpdateStickInput(ref LeftAnalogPos, p_pos);
	}

	public void UpdateRightAugmentedStickInput(Vector2 p_pos)
	{
		UpdateStickInput(ref RightAugmentedAnalogPos, p_pos);
	}

	public void UpdateRightStickInput(Vector2 p_pos)
	{
		UpdateStickInput(ref RightAnalogPos, p_pos);
	}

	public void UpdateStickInput(ref RectTransform[] p_rectTransform, Vector2 p_pos)
	{
		for (int i = 0; i < 3; i++)
		{
			p_rectTransform[i + 1].localPosition = p_rectTransform[i].localPosition;
		}
		p_rectTransform[0].localPosition = p_pos * _stickInputScaler;
	}

	public enum Buttons
	{
		A,
		B,
		X,
		Y,
		Start,
		Select,
		DPadUp,
		DPadDown,
		DPadLeft,
		DPadRight,
		RT,
		LT,
		LS,
		RS
	}
}