//using System;

using System.Collections.Generic;

using UnityEngine;

using JetBrains.Annotations;

//using Object = UnityEngine.Object;

namespace CommonGames.Utilities.CGTK.CGPooling
{
	/// <summary>This class handles the association between a spawned prefab, and the GameObjectPool component that manages it.</summary>
	public static class CGPool
	{
		public const string COMPONENT_PATH_PREFIX = "CGTK/CGPooling/";

		/// <summary>This stores all references between a spawned GameObject and its pool.</summary>
		[PublicAPI]
		public static readonly Dictionary<GameObject, GameObjectPool> Links = new Dictionary<GameObject, GameObjectPool>();

		#region Spawning
		
		/// <summary>This allows you to spawn a prefab via Component.</summary>
		public static T Spawn<T>(T prefab) where T : Component
			=> Spawn(prefab, Vector3.zero, Quaternion.identity);

		/// <summary>This allows you to spawn a prefab via Component.</summary>
		[PublicAPI]
		public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
			where T : Component
		{
			// Clone this prefab's GameObject
			GameObject __gameObject = prefab != null ? prefab.gameObject : null;
			GameObject __clone = Spawn(__gameObject, position, rotation, parent);

			// Return the same component from the clone
			return __clone != null ? __clone.GetComponent<T>() : null;
		}

		/// <summary>This allows you to spawn a prefab via GameObject.</summary>
		[PublicAPI]
		public static GameObject Spawn(GameObject prefab)
			=> Spawn(prefab, Vector3.zero, Quaternion.identity, null);

		/// <summary>This allows you to spawn a prefab via GameObject.</summary>
		[PublicAPI]
		public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
			=> Spawn(prefab, position, rotation, null);

		/// <summary>This allows you to spawn a prefab via GameObject.</summary>
		[PublicAPI]
		public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
		{
			if (prefab != null)
			{
				// Find the pool that handles this prefab

				// Create a new pool for this prefab?
				if (GameObjectPool.TryFindPoolByPrefab(prefab, out GameObjectPool __pool) == false)
				{
					__pool = new GameObject("CGPool(" + prefab.name + ")").AddComponent<GameObjectPool>();

					__pool.prefab = prefab;
				}

				// Try and spawn a clone from this pool
				GameObject __clone = default(GameObject);

				if (__pool.TrySpawn(position, rotation, parent, ref __clone) != true) return null;
				
				// Clone already registered?
				if (Links.Remove(__clone))
				{
					// If this pool recycles clones, then this can be expected
					if (__pool.Recycle)
					{
							
					}
					// This shouldn't happen
					else
					{
						CGDebug.LogWarning("You're attempting to spawn a clone that hasn't been despawned. Make sure all your Spawn and Despawn calls match, you shouldn't be manually destroying them!", __clone);
					}
				}

				// Associate this clone with this pool
				Links.Add(__clone, __pool);

				return __clone;
			}

			CGDebug.LogError("Attempting to spawn a null prefab");
			return null;
		}
		
		#endregion

		#region DespawningAll
		
		/// <summary>This will despawn all pool clones.</summary>
		[PublicAPI]
		public static void DespawnAll()
		{
			for (int __i = GameObjectPool.Instances.Count - 1; __i >= 0; __i--)
			{
				GameObjectPool.Instances[__i].DespawnAll();
			}

			Links.Clear();
		}
		
		/// <summary>This will despawn all pool clones.</summary>
		[PublicAPI]
		public static void DespawnAll(GameObject prefab)
		{
			if (GameObjectPool.TryFindPoolByPrefab(prefab, out GameObjectPool __pool) == false)
			{
				__pool = new GameObject("CGPool(" + prefab.name + ")").AddComponent<GameObjectPool>();

				__pool.prefab = prefab;
			}
			
			__pool.DespawnAll();

			Links.Clear();
		}
		
		#endregion

		#region Despawning
		
		/// <summary>This allows you to despawn a clone via Component, with optional delay.</summary>
		[PublicAPI]
		public static void Despawn(Component clone, float delay = 0.0f)
		{
			if (clone != null)Despawn(clone.gameObject, delay);
		}

		/// <summary>This allows you to despawn a clone via GameObject, with optional delay.</summary>
		[PublicAPI]
		public static void Despawn(GameObject clone, float delay = 0.0f)
		{
			if (clone != null)
			{
				// Try and find the pool associated with this clone
				if (Links.TryGetValue(clone, out GameObjectPool __pool))
				{
					// Remove the association
					Links.Remove(clone);

					__pool.Despawn(clone, delay);
				}
				else
				{
					if (GameObjectPool.TryFindPoolByClone(clone, ref __pool))
					{
						__pool.Despawn(clone, delay);
					}
					else
					{
						CGDebug.LogWarning("You're attempting to despawn a gameObject that wasn't spawned from this pool", clone);

						// Fall back to normal destroying
						#if UNITY_EDITOR
						//Object.DestroyImmediate(clone);
						#else
						//Object.Destroy(clone);
						#endif
					}
				}
			}
			else
			{
				CGDebug.LogWarning("You're attempting to despawn a null gameObject", clone);
			}
		}
		
		#endregion
	}
}