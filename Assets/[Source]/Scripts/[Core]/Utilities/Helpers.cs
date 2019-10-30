using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if Odin_Inspector
using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;
#endif

namespace Core.Utilities
{
	public static class Helpers
	{
		#region Helpers

		[System.Serializable]
		public class BufferV2
		{
			public BufferV2()
			{
				target = Vector2.zero;
				buffer = Vector2.zero;
			}

			public BufferV2(Vector2 targetInit, Vector2 bufferInit)
			{
				target = targetInit;
				buffer = bufferInit;
			}

			/*private*/
			public Vector2 target, buffer;

			public Vector2 curDelta; //Delta: apply difference from lastBuffer state to current BufferState		//get difference between last and new buffer

			public Vector2 curAbs; //absolute

			///<summary> Update Buffer, by supplying new target </summary>
			public void UpdateByNewTarget(Vector2 newTarget, float dampLambda, float deltaTime)
			{
				target = newTarget;
				Update(dampLambda, deltaTime);
			}

			///<summary> Update Buffer, by supplying the rawDelta to the last target </summary>
			public void UpdateByDelta(Vector2 rawDelta, float dampLambda, float deltaTime)
			{
				target = (target + rawDelta); //update Target
				Update(dampLambda, deltaTime);
			}

			///<summary> Update Buffer </summary>
			public void Update(float dampLambda, float deltaTime, bool byPass = false)
			{
				Vector2 lastBufferState = buffer;

				buffer = byPass
					? target
					: DampToTargetLambda(buffer, target, dampLambda, deltaTime); //damp current to target

				curDelta = buffer - lastBufferState;

				curAbs = buffer;
			}

			public static Vector2 DampToTargetLambda(Vector2 current, Vector2 target, float lambda, float dt)
				=> Vector2.Lerp(current, target, 1f - Mathf.Exp(-lambda * dt));
		}

		#endregion Helpers
	}
}
