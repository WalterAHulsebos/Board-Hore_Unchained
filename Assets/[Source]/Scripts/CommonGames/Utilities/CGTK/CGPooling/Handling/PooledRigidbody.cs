using UnityEngine;

namespace CommonGames.Utilities.CGTK.CGPooling
{
	///<summary> This component automatically resets a Rigidbody's velocity on Despawn. </summary>
	[RequireComponent(typeof(Rigidbody))]
	[AddComponentMenu(CGPool.COMPONENT_PATH_PREFIX + "Pooled Rigidbody")]
	public sealed class PooledRigidbody : MonoBehaviour, IPoolable
	{
		private Rigidbody _rigidbody = null;

		private void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();	
		}

		public void OnSpawn()
		{
			if(_rigidbody != null) return;
			
			_rigidbody = GetComponent<Rigidbody>();
		}

		public void OnDespawn()
		{
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;
		}
	}
}