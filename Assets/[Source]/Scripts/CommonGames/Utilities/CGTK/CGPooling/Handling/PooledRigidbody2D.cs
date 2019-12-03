using UnityEngine;

namespace CommonGames.Utilities.CGTK.CGPooling
{
	///<summary> This component automatically resets a Rigidbody2D's velocity on Despawn. </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu(CGPool.COMPONENT_PATH_PREFIX + "Pooled Rigidbody2D")]
	public sealed class PooledRigidbody2D : MonoBehaviour, IPoolable
	{
		private Rigidbody2D _rigidbody2D = null;

		private void Start()
		{
			_rigidbody2D = GetComponent<Rigidbody2D>();	
		}

		public void OnSpawn()
		{
			if(_rigidbody2D != null) return;
			
			_rigidbody2D = GetComponent<Rigidbody2D>();
		}

		public void OnDespawn()
		{
			_rigidbody2D.velocity = Vector2.zero;
			_rigidbody2D.angularVelocity = 0.0f;
		}
	}
}