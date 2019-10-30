using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TutorialControllerUI : MonoBehaviour
{
	public Color neutralColor;

	public Color hightlightColor;

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
	private Image LeftTriggerFadeSprite;

	[SerializeField]
	private Text LeftTriggerFadeText;

	[SerializeField]
	private RectTransform RightTriggerFill;

	[SerializeField]
	private RectTransform RightTriggerZero;

	[SerializeField]
	private RectTransform RightTriggerOne;

	[SerializeField]
	private Image RightTriggerFadeSprite;

	[SerializeField]
	private Text RightTriggerFadeText;

	[SerializeField]
	private RectTransform[] LeftAnalogPos = new RectTransform[4];

	[SerializeField]
	private RectTransform[] RightAnalogPos = new RectTransform[4];

	[SerializeField]
	private RectTransform leftArrow;

	[SerializeField]
	private Image leftArrowImage;

	[SerializeField]
	private RectTransform rightArrow;

	[SerializeField]
	private Image rightArrowImage;

	[Header("Helper Data")]
	[SerializeField]
	private float _stickInputScaler = 300f;

	public float stickBoundsMult;

	public float _leftRotMult;

	private Vector2 _leftLastPos;

	private Vector2 _leftLerpedVel;

	private Vector2 _rightLastPos;

	private Vector2 _rightLerpedVel;

	public TutorialControllerUI()
	{
	}

	private Image GetButton(TutorialControllerUI.Buttons p_button)
	{
		Image a = A;
		switch (p_button)
		{
			case TutorialControllerUI.Buttons.A:
			{
				a = A;
				break;
			}
			case TutorialControllerUI.Buttons.B:
			{
				a = B;
				break;
			}
			case TutorialControllerUI.Buttons.X:
			{
				a = X;
				break;
			}
			case TutorialControllerUI.Buttons.Y:
			{
				a = Y;
				break;
			}
			case TutorialControllerUI.Buttons.Start:
			{
				a = Start;
				break;
			}
			case TutorialControllerUI.Buttons.Select:
			{
				a = Select;
				break;
			}
			case TutorialControllerUI.Buttons.DPadUp:
			{
				a = DPadUp;
				break;
			}
			case TutorialControllerUI.Buttons.DPadDown:
			{
				a = DPadDown;
				break;
			}
			case TutorialControllerUI.Buttons.DPadLeft:
			{
				a = DPadLeft;
				break;
			}
			case TutorialControllerUI.Buttons.DPadRight:
			{
				a = DPadRight;
				break;
			}
			case TutorialControllerUI.Buttons.RT:
			{
				a = RT;
				break;
			}
			case TutorialControllerUI.Buttons.LT:
			{
				a = LT;
				break;
			}
			case TutorialControllerUI.Buttons.LS:
			{
				a = LeftStickClick;
				break;
			}
			case TutorialControllerUI.Buttons.RS:
			{
				a = RightStickClick;
				break;
			}
		}
		return a;
	}

	public void LerpLeftTrigger(float p_value)
	{
		LeftTriggerFadeSprite.color = new Color(1f, 1f, 1f, p_value);
		LeftTriggerFadeText.color = new Color(0.007843138f, 0.4509804f, 1f, p_value);
		LeftTriggerFill.localPosition = Vector3.Lerp(LeftTriggerZero.localPosition, LeftTriggerOne.localPosition, p_value);
	}

	public void LerpRightTrigger(float p_value)
	{
		RightTriggerFadeSprite.color = new Color(1f, 1f, 1f, p_value);
		RightTriggerFadeText.color = new Color(0.007843138f, 0.4509804f, 1f, p_value);
		RightTriggerFill.localPosition = Vector3.Lerp(RightTriggerZero.localPosition, RightTriggerOne.localPosition, p_value);
	}

	public void ResetColor(TutorialControllerUI.Buttons p_button)
	{
		if (GetButton(p_button).color != neutralColor)
		{
			GetButton(p_button).color = neutralColor;
		}
	}

	public void ResetColor(TutorialControllerUI.Buttons p_button, float p_alpha)
	{
		if (GetButton(p_button).color != new Color(neutralColor.r, neutralColor.g, neutralColor.b, p_alpha))
		{
			GetButton(p_button).color = new Color(neutralColor.r, neutralColor.g, neutralColor.b, p_alpha);
		}
	}

	public IEnumerator ResetColor(Image p_image)
	{
		TutorialControllerUI tutorialControllerUI = null;
		yield return null;
		p_image.color = tutorialControllerUI.neutralColor;
	}

	public void SetColor(TutorialControllerUI.Buttons p_button)
	{
		if (GetButton(p_button).color != hightlightColor)
		{
			GetButton(p_button).color = hightlightColor;
		}
	}

	public void SetColorOnce(TutorialControllerUI.Buttons p_button)
	{
		UnityEngine.Debug.Log("Asdfasdf");
		GetButton(p_button).color = hightlightColor;
		StartCoroutine(ResetColor(GetButton(p_button)));
	}

	public void ShowClickLeft(bool val)
	{
		LeftStickClick.enabled = val;
	}

	public void ShowClickRight(bool val)
	{
		RightStickClick.enabled = val;
	}

	public void UpdateLeftStickInput(Vector2 p_pos)
	{
		if (p_pos.magnitude != 0f)
		{
			Vector2 pPos = p_pos - _leftLastPos;
			leftArrow.localPosition = p_pos.normalized * stickBoundsMult;
			leftArrow.localRotation = Quaternion.FromToRotation(Vector3.up, p_pos);
			_leftLastPos = p_pos;
		}
		if (leftArrowImage.color.a != p_pos.magnitude)
		{
			leftArrowImage.color = new Color(0.01176471f, 0.8705882f, 0.7450981f, p_pos.magnitude);
		}
		UpdateStickInput(ref LeftAnalogPos, p_pos);
	}

	public void UpdateRightStickInput(Vector2 p_pos)
	{
		if (p_pos.magnitude != 0f)
		{
			Vector2 pPos = p_pos - _rightLastPos;
			rightArrow.localPosition = p_pos.normalized * stickBoundsMult;
			rightArrow.localRotation = Quaternion.FromToRotation(Vector3.up, p_pos);
			_rightLastPos = p_pos;
		}
		rightArrowImage.color = new Color(1f, 0.003921569f, 0.4588235f, p_pos.magnitude * 2f);
		UpdateStickInput(ref RightAnalogPos, p_pos);
	}

	public void UpdateStickInput(ref RectTransform[] p_rectTransform, Vector2 p_pos)
	{
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