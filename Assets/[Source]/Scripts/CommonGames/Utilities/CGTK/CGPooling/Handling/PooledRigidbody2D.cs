using UnityEngine;

namespace CommonGames.Utilities.CGTK.CGPooling
{
	///<summary> This component automatically resets a Rigidbody2D's velocity on Despawn. </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu(CGPool.COMPONENT_PATH_PREFIX + "Pooled Rigidbody2D")]
	public sealed class PooledRigidbody2D : MonoBehaviour, IPoolable
	{
		public void OnSpawn()
		{
		}

		public void OnDespawn()
		{
			Rigidbody2D __rigidbody2D = GetComponent<Rigidbody2D>();

			__rigidbody2D.velocity = Vector2.zero;
			__rigidbody2D.angularVelocity = 0.0f;
		}
	}
}