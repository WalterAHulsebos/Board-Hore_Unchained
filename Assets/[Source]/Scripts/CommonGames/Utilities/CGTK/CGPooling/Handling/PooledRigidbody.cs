using UnityEngine;

namespace CommonGames.Utilities.CGTK.CGPooling
{
	///<summary> This component automatically resets a Rigidbody's velocity on Despawn. </summary>
	[RequireComponent(typeof(Rigidbody))]
	[AddComponentMenu(CGPool.COMPONENT_PATH_PREFIX + "Pooled Rigidbody")]
	public sealed class PooledRigidbody : MonoBehaviour, IPoolable
	{
		public void OnSpawn()
		{
		}

		public void OnDespawn()
		{
			Rigidbody __rigidbody = GetComponent<Rigidbody>();

			__rigidbody.velocity = Vector3.zero;
			__rigidbody.angularVelocity = Vector3.zero;
		}
	}
}