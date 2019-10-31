using System;
using UnityEngine;

public class GrindDetection : MonoBehaviour
{
	public GrindDetection.GrindSide grindSide;

	public GrindDetection.GrindType grindType;

	private Vector2 _grindAnim = Vector2.zero;

	public GrindDetection()
	{
	}

	public void DetectGrind(bool p_boardTrigger, bool p_tailTrigger, bool p_noseTrigger, bool p_backTruckTrigger, bool p_frontTruckTrigger, Vector3 p_grindUp, Vector3 p_grindDirection, Vector3 p_grindRight, ref bool p_canOllie, ref bool p_canNollie, bool p_backTruckCollision, bool p_frontTruckCollision, bool p_boardCollision)
	{
		Vector3 vector3;
		vector3 = (!PlayerController.Instance.GetBoardBackwards() ? PlayerController.Instance.boardController.boardTransform.forward : PlayerController.Instance.boardController.boardTransform.forward);
		float single = Vector3.SignedAngle(Vector3.ProjectOnPlane(vector3, p_grindUp), p_grindDirection, p_grindUp);
		float single1 = Vector3.Angle(Vector3.ProjectOnPlane(vector3, p_grindUp), p_grindDirection);
		float single2 = Vector3.SignedAngle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.up, p_grindRight), p_grindUp, p_grindRight);
		if (p_boardCollision && !p_backTruckCollision && !p_frontTruckCollision && !p_tailTrigger && !p_noseTrigger)
		{
			p_canOllie = true;
			p_canNollie = true;
		}
		if (p_backTruckCollision & p_frontTruckCollision)
		{
			p_canOllie = true;
			p_canNollie = true;
		}
		else if (!PlayerController.Instance.GetBoardBackwards())
		{
			if (p_tailTrigger | p_backTruckCollision && !p_frontTruckCollision)
			{
				p_canOllie = true;
				p_canNollie = false;
			}
			else if (!p_backTruckCollision && p_frontTruckCollision | p_noseTrigger)
			{
				p_canNollie = true;
				p_canOllie = false;
			}
		}
		else if (p_tailTrigger | p_backTruckCollision && !p_frontTruckCollision)
		{
			p_canOllie = false;
			p_canNollie = true;
		}
		else if (!p_backTruckCollision && p_frontTruckCollision | p_noseTrigger)
		{
			p_canNollie = false;
			p_canOllie = true;
		}
		if (single1 >= 60f && single1 <= 120f)
		{
			if (!p_boardTrigger || p_tailTrigger || p_noseTrigger)
			{
				if (p_canOllie)
				{
					if (single > 0f)
					{
						if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
						{
							if (grindSide != GrindDetection.GrindSide.Frontside)
							{
								grindType = GrindDetection.GrindType.BsBluntSlide;
							}
							else
							{
								grindType = GrindDetection.GrindType.FsTailSlide;
							}
						}
						else if (grindSide != GrindDetection.GrindSide.Backside)
						{
							grindType = GrindDetection.GrindType.FsBluntSlide;
						}
						else
						{
							grindType = GrindDetection.GrindType.BsTailSlide;
						}
					}
					else if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
					{
						if (grindSide != GrindDetection.GrindSide.Backside)
						{
							grindType = GrindDetection.GrindType.FsBluntSlide;
						}
						else
						{
							grindType = GrindDetection.GrindType.BsTailSlide;
						}
					}
					else if (grindSide != GrindDetection.GrindSide.Frontside)
					{
						grindType = GrindDetection.GrindType.BsBluntSlide;
					}
					else
					{
						grindType = GrindDetection.GrindType.FsTailSlide;
					}
				}
				if (p_canNollie)
				{
					if (single > 0f)
					{
						if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
						{
							if (grindSide != GrindDetection.GrindSide.Backside)
							{
								grindType = GrindDetection.GrindType.FsNoseBluntSlide;
							}
							else
							{
								grindType = GrindDetection.GrindType.BsNoseSlide;
							}
						}
						else if (grindSide != GrindDetection.GrindSide.Frontside)
						{
							grindType = GrindDetection.GrindType.BsNoseBluntSlide;
						}
						else
						{
							grindType = GrindDetection.GrindType.FsNoseSlide;
						}
					}
					else if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
					{
						if (grindSide != GrindDetection.GrindSide.Frontside)
						{
							grindType = GrindDetection.GrindType.BsNoseBluntSlide;
						}
						else
						{
							grindType = GrindDetection.GrindType.FsNoseSlide;
						}
					}
					else if (grindSide != GrindDetection.GrindSide.Backside)
					{
						grindType = GrindDetection.GrindType.FsNoseBluntSlide;
					}
					else
					{
						grindType = GrindDetection.GrindType.BsNoseSlide;
					}
				}
			}
			else if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
			{
				if (grindSide == GrindDetection.GrindSide.Frontside)
				{
					if (single <= 0f)
					{
						grindType = GrindDetection.GrindType.FsBoardSlide;
					}
					else
					{
						grindType = GrindDetection.GrindType.FsLipSlide;
					}
				}
				else if (single <= 0f)
				{
					grindType = GrindDetection.GrindType.BsLipSlide;
				}
				else
				{
					grindType = GrindDetection.GrindType.BsBoardSlide;
				}
			}
			else if (grindSide == GrindDetection.GrindSide.Frontside)
			{
				if (single <= 0f)
				{
					grindType = GrindDetection.GrindType.FsLipSlide;
				}
				else
				{
					grindType = GrindDetection.GrindType.FsBoardSlide;
				}
			}
			else if (single <= 0f)
			{
				grindType = GrindDetection.GrindType.BsBoardSlide;
			}
			else
			{
				grindType = GrindDetection.GrindType.BsLipSlide;
			}
		}
		else if (!p_canOllie)
		{
			if (p_canNollie)
			{
				if (single1 <= 25f || single1 >= 75f)
				{
					grindType = GrindDetection.GrindType.NoseGrind;
				}
				else if (single > 0f)
				{
					if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
					{
						if (grindSide != GrindDetection.GrindSide.Backside)
						{
							grindType = GrindDetection.GrindType.FsOverCrook;
						}
						else
						{
							grindType = GrindDetection.GrindType.BsCrook;
						}
					}
					else if (grindSide != GrindDetection.GrindSide.Backside)
					{
						grindType = GrindDetection.GrindType.FsCrook;
					}
					else
					{
						grindType = GrindDetection.GrindType.BsOverCrook;
					}
				}
				else if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
				{
					if (grindSide != GrindDetection.GrindSide.Backside)
					{
						grindType = GrindDetection.GrindType.FsCrook;
					}
					else
					{
						grindType = GrindDetection.GrindType.BsOverCrook;
					}
				}
				else if (grindSide != GrindDetection.GrindSide.Backside)
				{
					grindType = GrindDetection.GrindType.FsOverCrook;
				}
				else
				{
					grindType = GrindDetection.GrindType.BsCrook;
				}
			}
		}
		else if (single1 <= 25f || single1 >= 75f)
		{
			if (!(p_backTruckCollision & p_frontTruckCollision))
			{
				grindType = GrindDetection.GrindType.FiveO;
			}
			else
			{
				grindType = GrindDetection.GrindType.FiftyFifty;
			}
		}
		else if (single2 >= 5f)
		{
			if (single > 0f)
			{
				if (SettingsManager.Instance.stance != SettingsManager.Stance.Regular)
				{
					grindType = GrindDetection.GrindType.FiveO;
				}
				else
				{
					grindType = GrindDetection.GrindType.FiveO;
				}
			}
			else if (SettingsManager.Instance.stance != SettingsManager.Stance.Regular)
			{
				grindType = GrindDetection.GrindType.FiveO;
			}
			else
			{
				grindType = GrindDetection.GrindType.FiveO;
			}
		}
		else if (single > 0f)
		{
			if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
			{
				if (grindSide != GrindDetection.GrindSide.Backside)
				{
					grindType = GrindDetection.GrindType.FsSmith;
				}
				else
				{
					grindType = GrindDetection.GrindType.BsFeeble;
				}
			}
			else if (grindSide != GrindDetection.GrindSide.Backside)
			{
				grindType = GrindDetection.GrindType.FsFeeble;
			}
			else
			{
				grindType = GrindDetection.GrindType.BsSmith;
			}
		}
		else if (SettingsManager.Instance.stance == SettingsManager.Stance.Regular)
		{
			if (grindSide != GrindDetection.GrindSide.Backside)
			{
				grindType = GrindDetection.GrindType.FsFeeble;
			}
			else
			{
				grindType = GrindDetection.GrindType.BsSmith;
			}
		}
		else if (grindSide != GrindDetection.GrindSide.Backside)
		{
			grindType = GrindDetection.GrindType.FsSmith;
		}
		else
		{
			grindType = GrindDetection.GrindType.BsFeeble;
		}
		_grindAnim = GrindAnimation();
		PlayerController.Instance.AnimSetGrindBlend(_grindAnim.x, _grindAnim.y);
	}

	private Vector2 GrindAnimation()
	{
		return Vector2.Lerp(_grindAnim, GrindBlendValues(), Time.deltaTime * 10f);
	}

	private Vector2 GrindBlendValues()
	{
		Vector3 vector2 = Vector2.zero;
		switch (grindType)
		{
			case GrindDetection.GrindType.FiftyFifty:
			{
				vector2 = new Vector2(0f, 0f);
				PlayerController.Instance.boardController.isSliding = false;
				break;
			}
			case GrindDetection.GrindType.NoseGrind:
			{
				vector2 = new Vector2(0f, 1f);
				PlayerController.Instance.boardController.isSliding = false;
				break;
			}
			case GrindDetection.GrindType.BsCrook:
			{
				vector2 = new Vector2(-1f, 1f);
				PlayerController.Instance.boardController.isSliding = false;
				break;
			}
			case GrindDetection.GrindType.FsOverCrook:
			{
				vector2 = new Vector2(-1f, 1f);
				PlayerController.Instance.boardController.isSliding = false;
				break;
			}
			case GrindDetection.GrindType.BsNoseSlide:
			{
				vector2 = new Vector2(-2f, 1f);
				PlayerController.Instance.boardController.isSliding = true;
				break;
			}
			case GrindDetection.GrindType.FsNoseBluntSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(-2f, 1f);
				break;
			}
			case GrindDetection.GrindType.FsCrook:
			{
				PlayerController.Instance.boardController.isSliding = false;
				vector2 = new Vector2(1f, 1f);
				break;
			}
			case GrindDetection.GrindType.BsOverCrook:
			{
				PlayerController.Instance.boardController.isSliding = false;
				vector2 = new Vector2(1f, 1f);
				break;
			}
			case GrindDetection.GrindType.FsNoseSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(2f, 1f);
				break;
			}
			case GrindDetection.GrindType.BsNoseBluntSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(2f, 1f);
				break;
			}
			case GrindDetection.GrindType.BsBoardSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(-2f, 0f);
				break;
			}
			case GrindDetection.GrindType.FsBoardSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(2f, 0f);
				break;
			}
			case GrindDetection.GrindType.FiveO:
			{
				PlayerController.Instance.boardController.isSliding = false;
				vector2 = new Vector2(0f, -1f);
				break;
			}
			case GrindDetection.GrindType.BsFeeble:
			{
				PlayerController.Instance.boardController.isSliding = false;
				vector2 = new Vector2(-1f, -1f);
				break;
			}
			case GrindDetection.GrindType.FsSmith:
			{
				PlayerController.Instance.boardController.isSliding = false;
				vector2 = new Vector2(-1f, -1f);
				break;
			}
			case GrindDetection.GrindType.FsTailSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(-2f, -1f);
				break;
			}
			case GrindDetection.GrindType.BsBluntSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(-2f, -1f);
				break;
			}
			case GrindDetection.GrindType.FsFeeble:
			{
				PlayerController.Instance.boardController.isSliding = false;
				vector2 = new Vector2(1f, -1f);
				break;
			}
			case GrindDetection.GrindType.BsSmith:
			{
				PlayerController.Instance.boardController.isSliding = false;
				vector2 = new Vector2(1f, -1f);
				break;
			}
			case GrindDetection.GrindType.BsTailSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(2f, -1f);
				break;
			}
			case GrindDetection.GrindType.FsBluntSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(2f, -1f);
				break;
			}
			case GrindDetection.GrindType.FsLipSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(-2f, 0f);
				break;
			}
			case GrindDetection.GrindType.BsLipSlide:
			{
				PlayerController.Instance.boardController.isSliding = true;
				vector2 = new Vector2(2f, 0f);
				break;
			}
		}
		return vector2;
	}

	public enum GrindSide
	{
		Frontside,
		Backside
	}

	public enum GrindType
	{
		FiftyFifty,
		NoseGrind,
		BsCrook,
		FsOverCrook,
		BsNoseSlide,
		FsNoseBluntSlide,
		FsCrook,
		BsOverCrook,
		FsNoseSlide,
		BsNoseBluntSlide,
		BsBoardSlide,
		FsBoardSlide,
		FiveO,
		BsFeeble,
		FsSmith,
		FsTailSlide,
		BsBluntSlide,
		FsFeeble,
		BsSmith,
		BsTailSlide,
		FsBluntSlide,
		FsLipSlide,
		BsLipSlide
	}
}