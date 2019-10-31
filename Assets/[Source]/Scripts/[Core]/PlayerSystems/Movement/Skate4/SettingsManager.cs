using RootMotion.FinalIK;
using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
	[SerializeField]
	private Animator _skaterAnimator;

	[SerializeField]
	private Animator _steezeAnimator;

	[SerializeField]
	private Animator _ikAnimator;

	[SerializeField]
	private RuntimeAnimatorController _regularAnim;

	[SerializeField]
	private RuntimeAnimatorController _goofyAnim;

	[SerializeField]
	private RuntimeAnimatorController _regularSteezeAnim;

	[SerializeField]
	private RuntimeAnimatorController _goofySteezeAnim;

	[SerializeField]
	private RuntimeAnimatorController _regularIkAnim;

	[SerializeField]
	private RuntimeAnimatorController _goofyIkAnim;

	private static SettingsManager _instance;

	public SettingsManager.Stance stance;

	public SettingsManager.ControlType controlType;

	public static SettingsManager Instance => _instance;

	public SettingsManager()
	{
	}

	private void Awake()
	{
		if (!(_instance != null) || !(_instance != this))
		{
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		SetStance((PlayerPrefs.GetInt("stance", 0) == 0 ? SettingsManager.Stance.Regular : SettingsManager.Stance.Goofy));
	}

	public void SetStance(SettingsManager.Stance p_stance)
	{
		stance = p_stance;
		PlayerPrefs.SetInt("stance", (stance == SettingsManager.Stance.Regular ? 0 : 1));
		if (stance == SettingsManager.Stance.Goofy)
		{
			PlayerController.Instance.skaterController.finalIk.solver.leftLegChain.bendConstraint.bendGoal = PlayerController.Instance.skaterController.goofyLeftKneeGuide;
			PlayerController.Instance.skaterController.finalIk.solver.rightLegChain.bendConstraint.bendGoal = PlayerController.Instance.skaterController.goofyRightKneeGuide;
			_skaterAnimator.runtimeAnimatorController = _goofyAnim;
			_steezeAnimator.runtimeAnimatorController = _goofySteezeAnim;
			_ikAnimator.runtimeAnimatorController = _goofyIkAnim;
			return;
		}
		PlayerController.Instance.skaterController.finalIk.solver.leftLegChain.bendConstraint.bendGoal = PlayerController.Instance.skaterController.regsLeftKneeGuide;
		PlayerController.Instance.skaterController.finalIk.solver.rightLegChain.bendConstraint.bendGoal = PlayerController.Instance.skaterController.regsRightKneeGuide;
		_skaterAnimator.runtimeAnimatorController = _regularAnim;
		_steezeAnimator.runtimeAnimatorController = _regularSteezeAnim;
		_ikAnimator.runtimeAnimatorController = _regularIkAnim;
	}

	public enum ControlType
	{
		Same,
		Swap,
		Simple
	}

	public enum Stance
	{
		Regular,
		Goofy
	}
}