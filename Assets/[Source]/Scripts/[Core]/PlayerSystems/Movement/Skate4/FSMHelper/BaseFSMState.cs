using Dreamteck.Splines;
using System;
using System.Collections.Generic;

namespace FSMHelper
{
	public class BaseFSMState : FSMStateInterface
	{
		private FSMStateMachineLogic m_OwnerLogic;

		public BaseFSMState()
		{
		}

		public void _InternalSetOwnerLogic(FSMStateMachineLogic ownerLogic)
		{
			m_OwnerLogic = ownerLogic;
		}

		public virtual void BothTriggersReleased(InputController.TurningMode turningMode)
		{
		}

		protected void BroadcastMessage(object[] args)
		{
			m_OwnerLogic.BroadcastMessage(args);
		}

		public virtual bool CanGrind()
		{
			return false;
		}

		public virtual bool CapsuleEnabled()
		{
			return false;
		}

		public bool DoTransition(Type nextState, object[] args = null)
		{
			return m_OwnerLogic.DoTransition(nextState, args);
		}

		public virtual void Enter()
		{
		}

		public virtual void Exit()
		{
		}

		public virtual void FixedUpdate()
		{
		}

		public virtual float GetAugmentedAngle(StickInput p_stick)
		{
			return 0f;
		}

		public BaseFSMState GetParentState()
		{
			return m_OwnerLogic.GetParentState();
		}

		public virtual StickInput GetPopStick()
		{
			return null;
		}

		public FSMStateMachine GetStateMachine()
		{
			return m_OwnerLogic.GetStateMachine();
		}

		public virtual bool IsCurrentSpline(SplineComputer p_spline)
		{
			return false;
		}

		public virtual bool IsGrinding()
		{
			return false;
		}

		public virtual bool IsInImpactState()
		{
			return false;
		}

		public virtual bool IsOnGroundState()
		{
			return false;
		}

		public virtual bool IsPushing()
		{
			return false;
		}

		public virtual bool LeftFootOff()
		{
			return false;
		}

		public virtual void LeftTriggerHeld(float value, InputController.TurningMode turningMode)
		{
		}

		public virtual void LeftTriggerPressed()
		{
		}

		public virtual void LeftTriggerReleased()
		{
		}

		public virtual void OnAllWheelsDown()
		{
		}

		public virtual void OnAnimatorUpdate()
		{
		}

		public virtual void OnBailed()
		{
		}

		public virtual void OnBoardSeparatedFromTarget()
		{
		}

		public virtual void OnBrakeHeld()
		{
		}

		public virtual void OnBrakePressed()
		{
		}

		public virtual void OnBrakeReleased()
		{
		}

		public virtual void OnCanManual()
		{
		}

		public virtual void OnCollisionEnterEvent()
		{
		}

		public virtual void OnCollisionExitEvent()
		{
		}

		public virtual void OnCollisionStayEvent()
		{
		}

		public virtual void OnEndImpact()
		{
		}

		public virtual void OnFirstWheelDown()
		{
		}

		public virtual void OnFirstWheelUp()
		{
		}

		public virtual void OnFlipStickCentered()
		{
		}

		public virtual void OnFlipStickUpdate()
		{
		}

		public virtual void OnForcePop()
		{
		}

		public virtual void OnGrindDetected()
		{
		}

		public virtual void OnGrindEnded()
		{
		}

		public virtual void OnGrindStay()
		{
		}

		public virtual void OnImpactUpdate()
		{
		}

		public virtual void OnLeftStickCenteredUpdate()
		{
		}

		public virtual void OnManualEnter(StickInput popStick, StickInput flipStick)
		{
		}

		public virtual void OnManualExit()
		{
		}

		public virtual void OnManualUpdate(StickInput popStick, StickInput flipStick)
		{
		}

		public virtual void OnNextState()
		{
		}

		public virtual void OnNoseManualEnter(StickInput popStick, StickInput flipStick)
		{
		}

		public virtual void OnNoseManualExit()
		{
		}

		public virtual void OnNoseManualUpdate(StickInput popStick, StickInput flipStick)
		{
		}

		public virtual void OnPopStickCentered()
		{
		}

		public virtual void OnPopStickUpdate()
		{
		}

		public virtual void OnPredictedCollisionEvent()
		{
		}

		public virtual void OnPreLandingEvent()
		{
		}

		public virtual void OnPush()
		{
		}

		public virtual void OnPushButtonHeld(bool mongo)
		{
		}

		public virtual void OnPushButtonPressed(bool mongo)
		{
		}

		public virtual void OnPushButtonReleased()
		{
		}

		public virtual void OnPushEnd()
		{
		}

		public virtual void OnPushLastCheck()
		{
		}

		public virtual void OnRespawn()
		{
		}

		public virtual void OnRightStickCenteredUpdate()
		{
		}

		public virtual void OnStickFixedUpdate(StickInput stick1, StickInput stick2)
		{
		}

		public virtual void OnStickPressed(bool right)
		{
		}

		public virtual void OnStickUpdate(StickInput stick1, StickInput stick2)
		{
		}

		public virtual void OnWheelsLeftGround()
		{
		}

		public virtual bool Popped()
		{
			return false;
		}

		public virtual void ReceiveMessage(object[] args)
		{
		}

		public virtual bool RightFootOff()
		{
			return false;
		}

		public virtual void RightTriggerHeld(float value, InputController.TurningMode turningMode)
		{
		}

		public virtual void RightTriggerPressed()
		{
		}

		public virtual void RightTriggerReleased()
		{
		}

		public virtual void SendEventBeginPop()
		{
		}

		public virtual void SendEventEndFlipPeriod()
		{
		}

		public virtual void SendEventExtend(float value)
		{
		}

		public virtual void SendEventPop(float value)
		{
		}

		public virtual void SendEventReleased()
		{
		}

		public virtual void SetSpline(SplineComputer p_spline)
		{
		}

		public virtual void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
		{
		}

		public virtual void Update()
		{
		}
	}
}