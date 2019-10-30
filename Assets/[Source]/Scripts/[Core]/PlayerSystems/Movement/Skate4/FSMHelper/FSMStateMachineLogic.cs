using Dreamteck.Splines;
using System;
using System.Collections.Generic;

namespace FSMHelper
{
	public class FSMStateMachineLogic
	{
		private BaseFSMState m_State;

		private FSMStateMachine m_OwnerSM;

		private FSMStateMachineLogic m_Parent;

		private List<FSMStateMachineLogic> m_ChildSMs = new List<FSMStateMachineLogic>();

		private Type m_StateClass;

		private FSMStateType m_StateType;

		private List<Type> m_ChildrenTypes;

		public FSMStateMachineLogic(FSMStateType stateType, List<Type> childrenTypes, FSMStateMachine ownerSM, FSMStateMachineLogic parent)
		{
			m_StateClass = null;
			m_StateType = stateType;
			m_ChildrenTypes = childrenTypes;
			m_Parent = parent;
			m_OwnerSM = ownerSM;
		}

		public FSMStateMachineLogic(Type stateClass, FSMStateMachine ownerSM, FSMStateMachineLogic parent)
		{
			m_ChildrenTypes = new List<Type>();
			m_StateClass = stateClass;
			m_OwnerSM = ownerSM;
			m_Parent = parent;
		}

		public void BothTriggersReleased(InputController.TurningMode turningMode)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].BothTriggersReleased(turningMode);
			}
			if (m_State != null)
			{
				m_State.BothTriggersReleased(turningMode);
			}
		}

		public void BroadcastMessage(object[] args)
		{
			m_OwnerSM.BroadcastMessage(args);
		}

		public bool CanGrind()
		{
			bool flag = false;
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				flag = m_ChildSMs[i].CanGrind();
			}
			if (m_State != null)
			{
				flag = m_State.CanGrind();
			}
			return flag;
		}

		public bool CapsuleEnabled()
		{
			bool flag = false;
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				flag = m_ChildSMs[i].CapsuleEnabled();
			}
			if (m_State != null)
			{
				flag = m_State.CapsuleEnabled();
			}
			return flag;
		}

		public bool DoTransition(Type nextState, object[] args)
		{
			if (m_Parent == null)
			{
				return false;
			}
			return m_Parent.RequestChildTransition(this, nextState, args);
		}

		public void Enter(object[] args)
		{
			if (m_StateClass != null)
			{
				m_State = (BaseFSMState)Activator.CreateInstance(m_StateClass, args);
				m_State._InternalSetOwnerLogic(this);
				m_State.SetupDefinition(ref m_StateType, ref m_ChildrenTypes);
				m_State.Enter();
			}
			for (int i = 0; i < m_ChildrenTypes.Count; i++)
			{
				FSMStateMachineLogic fSMStateMachineLogic = new FSMStateMachineLogic(m_ChildrenTypes[i], m_OwnerSM, this);
				m_ChildSMs.Add(fSMStateMachineLogic);
				fSMStateMachineLogic.Enter(null);
				if (m_StateType == FSMStateType.Type_OR)
				{
					break;
				}
			}
		}

		public void Exit()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].Exit();
			}
			if (m_State != null)
			{
				m_State.Exit();
			}
			m_OwnerSM = null;
			m_Parent = null;
			m_State = null;
			m_ChildSMs.Clear();
		}

		public void FixedUpdate()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].FixedUpdate();
			}
			if (m_State != null)
			{
				m_State.FixedUpdate();
			}
		}

		public string GetActiveStateTreeText(int level)
		{
			string str = "";
			if (m_State != null)
			{
				for (int i = 0; i < level * 4; i++)
				{
					str = string.Concat(str, " ");
				}
				str = string.Concat(str, m_State.ToString());
				str = string.Concat(str, "\n");
			}
			for (int j = 0; j < m_ChildSMs.Count; j++)
			{
				str = string.Concat(str, m_ChildSMs[j].GetActiveStateTreeText(level + 1));
			}
			return str;
		}

		public float GetAugmentedAngle(StickInput p_stick)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				float augmentedAngle = m_ChildSMs[i].GetAugmentedAngle(p_stick);
				if (augmentedAngle != 0f)
				{
					return augmentedAngle;
				}
			}
			if (m_State == null)
			{
				return 0f;
			}
			return m_State.GetAugmentedAngle(p_stick);
		}

		public BaseFSMState GetParentState()
		{
			if (m_Parent == null)
			{
				return null;
			}
			return m_Parent.m_State;
		}

		public StickInput GetPopStick()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				StickInput popStick = m_ChildSMs[i].GetPopStick();
				if (popStick)
				{
					return popStick;
				}
			}
			if (m_State == null)
			{
				return null;
			}
			return m_State.GetPopStick();
		}

		public FSMStateMachine GetStateMachine()
		{
			return m_OwnerSM;
		}

		public bool IsCurrentSpline(SplineComputer p_spline)
		{
			int num = 0;
			if (num < m_ChildSMs.Count)
			{
				return m_ChildSMs[num].IsCurrentSpline(p_spline);
			}
			if (m_State == null)
			{
				return false;
			}
			return m_State.IsCurrentSpline(p_spline);
		}

		public bool IsGrinding()
		{
			bool flag = false;
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				flag = m_ChildSMs[i].IsGrinding();
			}
			if (m_State != null)
			{
				flag = m_State.IsGrinding();
			}
			return flag;
		}

		public bool IsInImpactState()
		{
			bool flag = false;
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				flag = m_ChildSMs[i].IsInImpactState();
			}
			if (m_State != null)
			{
				flag = m_State.IsInImpactState();
			}
			return flag;
		}

		public bool IsInState(Type state)
		{
			if (m_StateClass == state)
			{
				return true;
			}
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				if (m_ChildSMs[i].IsInState(state))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsOnGroundState()
		{
			bool flag = false;
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				flag = m_ChildSMs[i].IsOnGroundState();
			}
			if (m_State != null)
			{
				flag = m_State.IsOnGroundState();
			}
			return flag;
		}

		public bool IsPushing()
		{
			bool flag = false;
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				flag = m_ChildSMs[i].IsPushing();
			}
			if (m_State != null)
			{
				flag = m_State.IsPushing();
			}
			return flag;
		}

		public bool LeftFootOff()
		{
			bool flag = false;
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				flag = m_ChildSMs[i].LeftFootOff();
			}
			if (m_State != null)
			{
				flag = m_State.LeftFootOff();
			}
			return flag;
		}

		public void LeftTriggerHeld(float value, InputController.TurningMode turningMode)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].LeftTriggerHeld(value, turningMode);
			}
			if (m_State != null)
			{
				m_State.LeftTriggerHeld(value, turningMode);
			}
		}

		public void LeftTriggerPressed()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].LeftTriggerPressed();
			}
			if (m_State != null)
			{
				m_State.LeftTriggerPressed();
			}
		}

		public void LeftTriggerReleased()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].LeftTriggerReleased();
			}
			if (m_State != null)
			{
				m_State.LeftTriggerReleased();
			}
		}

		public void OnAllWheelsDown()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnAllWheelsDown();
			}
			if (m_State != null)
			{
				m_State.OnAllWheelsDown();
			}
		}

		public void OnAnimatorUpdate()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnAnimatorUpdate();
			}
			if (m_State != null)
			{
				m_State.OnAnimatorUpdate();
			}
		}

		public void OnBailed()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnBailed();
			}
			if (m_State != null)
			{
				m_State.OnBailed();
			}
		}

		public void OnBoardSeparatedFromTarget()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnBoardSeparatedFromTarget();
			}
			if (m_State != null)
			{
				m_State.OnBoardSeparatedFromTarget();
			}
		}

		public void OnBrakeHeld()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnBrakeHeld();
			}
			if (m_State != null)
			{
				m_State.OnBrakeHeld();
			}
		}

		public void OnBrakePressed()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnBrakePressed();
			}
			if (m_State != null)
			{
				m_State.OnBrakePressed();
			}
		}

		public void OnBrakeReleased()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnBrakeReleased();
			}
			if (m_State != null)
			{
				m_State.OnBrakeReleased();
			}
		}

		public void OnCanManual()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnCanManual();
			}
			if (m_State != null)
			{
				m_State.OnCanManual();
			}
		}

		public void OnCollisionEnterEvent()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnCollisionEnterEvent();
			}
			if (m_State != null)
			{
				m_State.OnCollisionEnterEvent();
			}
		}

		public void OnCollisionExitEvent()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnCollisionExitEvent();
			}
			if (m_State != null)
			{
				m_State.OnCollisionExitEvent();
			}
		}

		public void OnCollisionStayEvent()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnCollisionStayEvent();
			}
			if (m_State != null)
			{
				m_State.OnCollisionStayEvent();
			}
		}

		public void OnEndImpact()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnEndImpact();
			}
			if (m_State != null)
			{
				m_State.OnEndImpact();
			}
		}

		public void OnFirstWheelDown()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnFirstWheelDown();
			}
			if (m_State != null)
			{
				m_State.OnFirstWheelDown();
			}
		}

		public void OnFirstWheelUp()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnFirstWheelUp();
			}
			if (m_State != null)
			{
				m_State.OnFirstWheelUp();
			}
		}

		public void OnFlipStickCentered()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnFlipStickCentered();
			}
			if (m_State != null)
			{
				m_State.OnFlipStickCentered();
			}
		}

		public void OnFlipStickUpdate()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnFlipStickUpdate();
			}
			if (m_State != null)
			{
				m_State.OnFlipStickUpdate();
			}
		}

		public void OnForcePop()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnForcePop();
			}
			if (m_State != null)
			{
				m_State.OnForcePop();
			}
		}

		public void OnGrindDetected()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnGrindDetected();
			}
			if (m_State != null)
			{
				m_State.OnGrindDetected();
			}
		}

		public void OnGrindEnded()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnGrindEnded();
			}
			if (m_State != null)
			{
				m_State.OnGrindEnded();
			}
		}

		public void OnGrindStay()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnGrindStay();
			}
			if (m_State != null)
			{
				m_State.OnGrindStay();
			}
		}

		public void OnImpactUpdate()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnImpactUpdate();
			}
			if (m_State != null)
			{
				m_State.OnImpactUpdate();
			}
		}

		public void OnLeftStickCenteredUpdate()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnLeftStickCenteredUpdate();
			}
			if (m_State != null)
			{
				m_State.OnLeftStickCenteredUpdate();
			}
		}

		public void OnManualEnter(StickInput popStick, StickInput flipStick)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnManualEnter(popStick, flipStick);
			}
			if (m_State != null)
			{
				m_State.OnManualEnter(popStick, flipStick);
			}
		}

		public void OnManualExit()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnManualExit();
			}
			if (m_State != null)
			{
				m_State.OnManualExit();
			}
		}

		public void OnManualUpdate(StickInput popStick, StickInput flipStick)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnManualUpdate(popStick, flipStick);
			}
			if (m_State != null)
			{
				m_State.OnManualUpdate(popStick, flipStick);
			}
		}

		public void OnNextState()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnNextState();
			}
			if (m_State != null)
			{
				m_State.OnNextState();
			}
		}

		public void OnNoseManualEnter(StickInput popStick, StickInput flipStick)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnNoseManualEnter(popStick, flipStick);
			}
			if (m_State != null)
			{
				m_State.OnNoseManualEnter(popStick, flipStick);
			}
		}

		public void OnNoseManualExit()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnNoseManualExit();
			}
			if (m_State != null)
			{
				m_State.OnNoseManualExit();
			}
		}

		public void OnNoseManualUpdate(StickInput popStick, StickInput flipStick)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnNoseManualUpdate(popStick, flipStick);
			}
			if (m_State != null)
			{
				m_State.OnNoseManualUpdate(popStick, flipStick);
			}
		}

		public void OnPopStickCentered()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPopStickCentered();
			}
			if (m_State != null)
			{
				m_State.OnPopStickCentered();
			}
		}

		public void OnPopStickUpdate()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPopStickUpdate();
			}
			if (m_State != null)
			{
				m_State.OnPopStickUpdate();
			}
		}

		public void OnPredictedCollisionEvent()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPredictedCollisionEvent();
			}
			if (m_State != null)
			{
				m_State.OnPredictedCollisionEvent();
			}
		}

		public void OnPreLandingEvent()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPreLandingEvent();
			}
			if (m_State != null)
			{
				m_State.OnPreLandingEvent();
			}
		}

		public void OnPush()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPush();
			}
			if (m_State != null)
			{
				m_State.OnPush();
			}
		}

		public void OnPushButtonHeld(bool mongo)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPushButtonHeld(mongo);
			}
			if (m_State != null)
			{
				m_State.OnPushButtonHeld(mongo);
			}
		}

		public void OnPushButtonPressed(bool mongo)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPushButtonPressed(mongo);
			}
			if (m_State != null)
			{
				m_State.OnPushButtonPressed(mongo);
			}
		}

		public void OnPushButtonReleased()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPushButtonReleased();
			}
			if (m_State != null)
			{
				m_State.OnPushButtonReleased();
			}
		}

		public void OnPushEnd()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPushEnd();
			}
			if (m_State != null)
			{
				m_State.OnPushEnd();
			}
		}

		public void OnPushLastCheck()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnPushLastCheck();
			}
			if (m_State != null)
			{
				m_State.OnPushLastCheck();
			}
		}

		public void OnRespawn()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnRespawn();
			}
			if (m_State != null)
			{
				m_State.OnRespawn();
			}
		}

		public void OnRightStickCenteredUpdate()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnRightStickCenteredUpdate();
			}
			if (m_State != null)
			{
				m_State.OnRightStickCenteredUpdate();
			}
		}

		public void OnStickFixedUpdate(StickInput stick1, StickInput stick2)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnStickFixedUpdate(stick1, stick2);
			}
			if (m_State != null)
			{
				m_State.OnStickFixedUpdate(stick1, stick2);
			}
		}

		public void OnStickPressed(bool right)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnStickPressed(right);
			}
			if (m_State != null)
			{
				m_State.OnStickPressed(right);
			}
		}

		public void OnStickUpdate(StickInput stick1, StickInput stick2)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnStickUpdate(stick1, stick2);
			}
			if (m_State != null)
			{
				m_State.OnStickUpdate(stick1, stick2);
			}
		}

		public void OnWheelsLeftGround()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].OnWheelsLeftGround();
			}
			if (m_State != null)
			{
				m_State.OnWheelsLeftGround();
			}
		}

		public bool Popped()
		{
			bool flag = false;
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				flag = m_ChildSMs[i].Popped();
			}
			if (m_State != null)
			{
				flag = m_State.Popped();
			}
			return flag;
		}

		public void ReceiveMessage(object[] args)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].ReceiveMessage(args);
			}
			if (m_State != null)
			{
				m_State.ReceiveMessage(args);
			}
		}

		public bool RequestChildTransition(FSMStateMachineLogic child, Type nextState, object[] args)
		{
			if (m_StateType == FSMStateType.Type_AND)
			{
				return false;
			}
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				if (m_ChildSMs[i] == child)
				{
					for (int j = 0; j < m_ChildrenTypes.Count; j++)
					{
						if (m_ChildrenTypes[j] == nextState)
						{
							m_ChildSMs[i].Exit();
							m_ChildSMs[i] = new FSMStateMachineLogic(nextState, m_OwnerSM, this);
							m_ChildSMs[i].Enter(args);
							return true;
						}
					}
					return false;
				}
			}
			return false;
		}

		public bool RightFootOff()
		{
			bool flag = false;
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				flag = m_ChildSMs[i].RightFootOff();
			}
			if (m_State != null)
			{
				flag = m_State.RightFootOff();
			}
			return flag;
		}

		public void RightTriggerHeld(float value, InputController.TurningMode turningMode)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].RightTriggerHeld(value, turningMode);
			}
			if (m_State != null)
			{
				m_State.RightTriggerHeld(value, turningMode);
			}
		}

		public void RightTriggerPressed()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].RightTriggerPressed();
			}
			if (m_State != null)
			{
				m_State.RightTriggerPressed();
			}
		}

		public void RightTriggerReleased()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].RightTriggerReleased();
			}
			if (m_State != null)
			{
				m_State.RightTriggerReleased();
			}
		}

		public void SendEventBeginPop()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].SendEventBeginPop();
			}
			if (m_State != null)
			{
				m_State.SendEventBeginPop();
			}
		}

		public void SendEventEndFlipPeriod()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].SendEventEndFlipPeriod();
			}
			if (m_State != null)
			{
				m_State.SendEventEndFlipPeriod();
			}
		}

		public void SendEventExtend(float value)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].SendEventExtend(value);
			}
			if (m_State != null)
			{
				m_State.SendEventExtend(value);
			}
		}

		public void SendEventPop(float value)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].SendEventPop(value);
			}
			if (m_State != null)
			{
				m_State.SendEventPop(value);
			}
		}

		public void SendEventReleased()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].SendEventReleased();
			}
			if (m_State != null)
			{
				m_State.SendEventReleased();
			}
		}

		public void SetSpline(SplineComputer p_spline)
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].SetSpline(p_spline);
			}
			if (m_State != null)
			{
				m_State.SetSpline(p_spline);
			}
		}

		public void Update()
		{
			for (int i = 0; i < m_ChildSMs.Count; i++)
			{
				m_ChildSMs[i].Update();
			}
			if (m_State != null)
			{
				m_State.Update();
			}
		}
	}
}