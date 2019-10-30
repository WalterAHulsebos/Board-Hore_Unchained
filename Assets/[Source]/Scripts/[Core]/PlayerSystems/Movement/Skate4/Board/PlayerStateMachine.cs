using FSMHelper;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : FSMStateMachine
{
	public GameObject gameObject;

	public PlayerStateMachine(GameObject playerObj)
	{
		gameObject = playerObj;
	}

	public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
	{
		children.Add(typeof(PlayerState_Riding));
		children.Add(typeof(PlayerState_OnBoard));
		children.Add(typeof(PlayerState_Impact));
		children.Add(typeof(PlayerState_Manualling));
		children.Add(typeof(PlayerState_Setup));
		children.Add(typeof(PlayerState_BeginPop));
		children.Add(typeof(PlayerState_Pop));
		children.Add(typeof(PlayerState_Released));
		children.Add(typeof(PlayerState_InAir));
		children.Add(typeof(PlayerState_Grinding));
		children.Add(typeof(PlayerState_OffBoard));
		children.Add(typeof(PlayerState_Bailed));
		children.Add(typeof(PlayerState_Pushing));
		children.Add(typeof(PlayerState_Braking));
	}
}