using Dreamteck.Splines;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSMHelper
{
	public class FSMStateMachine
	{
		private FSMStateMachineLogic m_Logic;

		public FSMStateMachine()
		{
		}

		public string ActiveStateTreeString()
		{
			string str = ToString();
			str = string.Concat(str, m_Logic.GetActiveStateTreeText(0));
			str = str.Remove(0, 34);
			str = str.Substring(0, str.Length - 1);
			return str;
		}

		public virtual void BothTriggersReleasedSM(InputController.TurningMode turningMode)
		{
			if (m_Logic != null)
			{
				m_Logic.BothTriggersReleased(turningMode);
			}
		}

		public void BroadcastMessage(object[] args)
		{
			m_Logic.ReceiveMessage(args);
		}

		public virtual bool CanGrindSM()
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.CanGrind();
		}

		public virtual bool CapsuleEnabledSM()
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.CapsuleEnabled();
		}

		public virtual void FixedUpdateSM()
		{
			if (m_Logic != null)
			{
				m_Logic.FixedUpdate();
			}
		}

		public virtual float GetAugmentedAngleSM(StickInput p_stick)
		{
			if (m_Logic == null)
			{
				return 0f;
			}
			return m_Logic.GetAugmentedAngle(p_stick);
		}

		public virtual StickInput GetPopStickSM()
		{
			if (m_Logic == null)
			{
				return null;
			}
			return m_Logic.GetPopStick();
		}

		public virtual bool IsCurrentSplineSM(SplineComputer p_spline)
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.IsCurrentSpline(p_spline);
		}

		public virtual bool IsGrindingSM()
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.IsGrinding();
		}

		public virtual bool IsInImpactStateSM()
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.IsInImpactState();
		}

		public bool IsInState(Type state)
		{
			return m_Logic.IsInState(state);
		}

		public virtual bool IsOnGroundStateSM()
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.IsOnGroundState();
		}

		public virtual bool IsPushingSM()
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.IsPushing();
		}

		public virtual bool LeftFootOffSM()
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.LeftFootOff();
		}

		public virtual void LeftTriggerHeldSM(float value, InputController.TurningMode turningMode)
		{
			if (m_Logic != null)
			{
				m_Logic.LeftTriggerHeld(value, turningMode);
			}
		}

		public virtual void LeftTriggerPressedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.LeftTriggerPressed();
			}
		}

		public virtual void LeftTriggerReleasedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.LeftTriggerReleased();
			}
		}

		public virtual void OnAllWheelsDownSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnAllWheelsDown();
			}
		}

		public virtual void OnAnimatorUpdateSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnAnimatorUpdate();
			}
		}

		public virtual void OnBailedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnBailed();
			}
		}

		public virtual void OnBoardSeparatedFromTargetSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnBoardSeparatedFromTarget();
			}
		}

		public virtual void OnBrakeHeldSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnBrakeHeld();
			}
		}

		public virtual void OnBrakePressedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnBrakePressed();
			}
		}

		public virtual void OnBrakeReleasedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnBrakeReleased();
			}
		}

		public virtual void OnCanManualSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnCanManual();
			}
		}

		public virtual void OnCollisionEnterEventSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnCollisionEnterEvent();
			}
		}

		public virtual void OnCollisionExitEventSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnCollisionExitEvent();
			}
		}

		public virtual void OnCollisionStayEventSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnCollisionStayEvent();
			}
		}

		public virtual void OnEndImpactSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnEndImpact();
			}
		}

		public virtual void OnFirstWheelDownSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnFirstWheelDown();
			}
		}

		public virtual void OnFirstWheelUpSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnFirstWheelUp();
			}
		}

		public virtual void OnFlipStickCenteredSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnFlipStickCentered();
			}
		}

		public virtual void OnFlipStickUpdateSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnFlipStickUpdate();
			}
		}

		public virtual void OnForcePopSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnForcePop();
			}
		}

		public virtual void OnGrindDetectedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnGrindDetected();
			}
		}

		public virtual void OnGrindEndedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnGrindEnded();
			}
		}

		public virtual void OnGrindStaySM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnGrindStay();
			}
		}

		public virtual void OnImpactUpdateSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnImpactUpdate();
			}
		}

		public virtual void OnLeftStickCenteredUpdateSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnLeftStickCenteredUpdate();
			}
		}

		public virtual void OnManualEnterSM(StickInput popStick, StickInput flipStick)
		{
			if (m_Logic != null)
			{
				m_Logic.OnManualEnter(popStick, flipStick);
			}
		}

		public virtual void OnManualExitSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnManualExit();
			}
		}

		public virtual void OnManualUpdateSM(StickInput popStick, StickInput flipStick)
		{
			if (m_Logic != null)
			{
				m_Logic.OnManualUpdate(popStick, flipStick);
			}
		}

		public virtual void OnNextStateSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnNextState();
			}
		}

		public virtual void OnNoseManualEnterSM(StickInput popStick, StickInput flipStick)
		{
			if (m_Logic != null)
			{
				m_Logic.OnNoseManualEnter(popStick, flipStick);
			}
		}

		public virtual void OnNoseManualExitSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnNoseManualExit();
			}
		}

		public virtual void OnNoseManualUpdateSM(StickInput popStick, StickInput flipStick)
		{
			if (m_Logic != null)
			{
				m_Logic.OnNoseManualUpdate(popStick, flipStick);
			}
		}

		public virtual void OnPopStickCenteredSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnPopStickCentered();
			}
		}

		public virtual void OnPopStickUpdateSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnPopStickUpdate();
			}
		}

		public virtual void OnPredictedCollisionEventSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnPredictedCollisionEvent();
			}
		}

		public virtual void OnPreLandingEventSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnPreLandingEvent();
			}
		}

		public virtual void OnPushButtonHeldSM(bool mongo)
		{
			if (m_Logic != null)
			{
				m_Logic.OnPushButtonHeld(mongo);
			}
		}

		public virtual void OnPushButtonPressedSM(bool mongo)
		{
			if (m_Logic != null)
			{
				m_Logic.OnPushButtonPressed(mongo);
			}
		}

		public virtual void OnPushButtonReleasedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnPushButtonReleased();
			}
		}

		public virtual void OnPushEndSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnPushEnd();
			}
		}

		public virtual void OnPushLastCheckSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnPushLastCheck();
			}
		}

		public virtual void OnPushSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnPush();
			}
		}

		public virtual void OnRespawnSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnRespawn();
			}
		}

		public virtual void OnRightStickCenteredUpdateSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnRightStickCenteredUpdate();
			}
		}

		public virtual void OnStickFixedUpdateSM(StickInput stick1, StickInput stick2)
		{
			if (m_Logic != null)
			{
				m_Logic.OnStickFixedUpdate(stick1, stick2);
			}
		}

		public virtual void OnStickPressedSM(bool right)
		{
			if (m_Logic != null)
			{
				m_Logic.OnStickPressed(right);
			}
		}

		public virtual void OnStickUpdateSM(StickInput stick1, StickInput stick2)
		{
			if (m_Logic != null)
			{
				m_Logic.OnStickUpdate(stick1, stick2);
			}
		}

		public virtual void OnWheelsLeftGroundSM()
		{
			if (m_Logic != null)
			{
				m_Logic.OnWheelsLeftGround();
			}
		}

		public virtual bool PoppedSM()
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.Popped();
		}

		public void PrintActiveStateTree()
		{
			Debug.Log(string.Concat(string.Concat(ToString(), "\n"), m_Logic.GetActiveStateTreeText(0)));
		}

		public virtual bool RightFootOffSM()
		{
			if (m_Logic == null)
			{
				return false;
			}
			return m_Logic.RightFootOff();
		}

		public virtual void RightTriggerHeldSM(float value, InputController.TurningMode turningMode)
		{
			if (m_Logic != null)
			{
				m_Logic.RightTriggerHeld(value, turningMode);
			}
		}

		public virtual void RightTriggerPressedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.RightTriggerPressed();
			}
		}

		public virtual void RightTriggerReleasedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.RightTriggerReleased();
			}
		}

		public virtual void SendEventBeginPopSM()
		{
			if (m_Logic != null)
			{
				m_Logic.SendEventBeginPop();
			}
		}

		public virtual void SendEventEndFlipPeriodSM()
		{
			if (m_Logic != null)
			{
				m_Logic.SendEventEndFlipPeriod();
			}
		}

		public virtual void SendEventExtendSM(float value)
		{
			if (m_Logic != null)
			{
				m_Logic.SendEventExtend(value);
			}
		}

		public virtual void SendEventPopSM(float value)
		{
			if (m_Logic != null)
			{
				m_Logic.SendEventPop(value);
			}
		}

		public virtual void SendEventReleasedSM()
		{
			if (m_Logic != null)
			{
				m_Logic.SendEventReleased();
			}
		}

		public virtual void SetSplineSM(SplineComputer p_spline)
		{
			if (m_Logic != null)
			{
				m_Logic.SetSpline(p_spline);
			}
		}

		public virtual void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
		{
		}

		public virtual void StartSM()
		{
			FSMStateType fSMStateType = FSMStateType.Type_OR;
			List<Type> types = new List<Type>();
			SetupDefinition(ref fSMStateType, ref types);
			m_Logic = new FSMStateMachineLogic(fSMStateType, types, this, null);
			m_Logic.Enter(null);
		}

		public virtual void StopSM()
		{
			if (m_Logic != null)
			{
				m_Logic.Exit();
				m_Logic = null;
			}
		}

		public virtual void UpdateSM()
		{
			if (m_Logic != null)
			{
				m_Logic.Update();
			}
		}
	}
}