using System;

namespace FSMHelper
{
	public interface FSMStateInterface
	{
		void Enter();

		void Exit();

		void FixedUpdate();

		void Update();
	}
}