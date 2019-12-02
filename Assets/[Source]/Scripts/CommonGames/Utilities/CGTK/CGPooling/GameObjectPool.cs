using System;
using System.Collections.Generic;

using UnityEngine;

using CommonGames.Utilities.Extensions;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using JetBrains.Annotations;

/*
#if UNITY_EDITOR
using UnityEditor;

namespace Utilities.CGTK.CGPooling
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GameObjectPool))]
	public class GameObjectPool_Inspector : Inspector<GameObjectPool>
	{
		protected override void DrawInspector()
		{
			BeginError(Any(t => t.prefab == null));
				Draw("Prefab", "The prefab this pool controls.");
			EndError();
			
			Draw("Notification", "If you need to peform a special action when a prefab is spawned or despawned, then this allows you to control how that action is performed. None = If you use this then you must rely on the OnEnable and OnDisable messages. SendMessage = The prefab clone is sent the OnSpawn and OnDespawn messages. BroadcastMessage = The prefab clone and all its children are sent the OnSpawn and OnDespawn messages. IPoolable = The prefab clone's components implementing IPoolable are called. Broadcast IPoolable = The prefab clone and all its child components implementing IPoolable are called.");
			Draw("Preload", "Should this pool preload some clones?");
			Draw("Capacity", "Should this pool have a maximum amount of spawnable clones?");
			Draw("Recycle", "If the pool reaches capacity, should new spawns force older ones to despawn?");
			Draw("Persist", "Should this pool be marked as DontDestroyOnLoad?");
			Draw("Stamp", "Should the spawned clones have their clone index appended to their name?");
			Draw("Warnings", "Should detected issues be output to the console?");

			EditorGUILayout.Separator();

			EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.IntField("Spawned", Target.Spawned);
				EditorGUILayout.IntField("Despawned", Target.Despawned);
				EditorGUILayout.IntField("Total", Target.Total);
			EditorGUI.EndDisabledGroup();
		}

		[MenuItem("GameObject//Pool", false, 1)]
		private static void CreateLocalization()
		{
			GameObject __gameObject = new GameObject(typeof(GameObjectPool).Name);

			Undo.RegisterCreatedObjectUndo(__gameObject, "Create GameObjectPool");

			__gameObject.AddComponent<GameObjectPool>();

			Selection.activeGameObject = __gameObject;
		}
	}
}
#endif
*/

namespace CommonGames.Utilities.CGTK.CGPooling
{
	/// <inheritdoc />
	/// <summary>This component allows you to pool GameObjects, giving you a very fast alternative to Instantiate and Destroy.
	/// Pools also have settings to preload, recycle, and set the spawn capacity, giving you lots of control over your spawning.</summary>
	//[AddComponentMenu(Pool.ComponentPathPrefix + "GameObject Pool")]
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	public class GameObjectPool : PersistentMultiton<GameObjectPool>
	{
		#region Variables
		
		[Serializable]
		public sealed class Delay
		{
			public GameObject clone;
			public float life;
		}

		public enum NotificationType
		{
			None,
			// ReSharper disable once InconsistentNaming
			IPoolable,
			BroadcastIPoolable
		}

		//public static readonly List<GameObjectPool> Instances = new List<GameObjectPool>();

		/// <summary>The prefab this pool controls.</summary>
		[Required]
		[AssetsOnly]
		public GameObject prefab;

		/// <summary>
		/// If you need to perform a special action when a prefab is spawned or despawned, then this allows you to control how that action is performed.
		/// 
		/// <tip>None</tip> If you use this then you must rely on the OnEnable and OnDisable messages.
		/// <tip>IPoolable</tip> The prefab clone's components implementing IPoolable are called.
		/// <tip>Broadcast IPoolable</tip> The prefab clone and all its child components implementing IPoolable are called.
		/// </summary>
		public NotificationType notification = NotificationType.IPoolable;

		/// <summary> Should this pool preload some clones? </summary>
		[PropertyTooltip(tooltip: "Should this pool preload some clones?")]
		[OdinSerialize] public int Preload { get; set; } = 5;

		/// <summary> Should this pool have a maximum amount of clones it can spawn? (0 is infinite) </summary>
		[PublicAPI]
		[PropertyTooltip(tooltip: "Should this pool have a maximum amount of clones it can spawn? (0 is infinite)")]
		[OdinSerialize] public int Capacity { get; set; } = 10;

		/// <summary> If the pool reaches capacity, should new spawns force older ones to despawn? </summary>
		[PublicAPI]
		[OdinSerialize] public bool Recycle { get; set; } = true;

		/// <summary> Should the spawned clones have their clone index appended to their name?</summary>
		[PublicAPI]
		[OdinSerialize] public bool Stamp { get; set; } = false;

		/// <summary> This stores all spawned clones in a list. This is used when Recycle is enabled, because knowing the spawn order must be known. This list is also used during serialization.</summary>
		[HideInInspector]
		[SerializeField] private List<GameObject> spawnedClones = new List<GameObject>();

		/// <summary> This stores all spawned clones in a hash set. This is used when Recycle is disabled, because their storage order isn't important. This allows us to quickly find the Clone associated with the specified GameObject.</summary>
		private readonly HashSet<GameObject> _spawnedClonesHashSet = new HashSet<GameObject>();

		/// <summary> All the currently despawned prefab instances.</summary>
		[HideInInspector]
		[SerializeField] private List<GameObject> despawnedClones = new List<GameObject>();

		/// <summary> All the delayed destruction objects.</summary>
		[HideInInspector]
		[SerializeField] private List<Delay> delays = new List<Delay>();

		private static readonly List<IPoolable> TempPoolables = new List<IPoolable>();
		
		#endregion

		#region Methods

		#region Public API
		
		/// <summary> Find a pool responsible for handling the specified prefab.</summary>
		[PublicAPI]
		public static bool TryFindPoolByPrefab(GameObject prefab, out GameObjectPool foundPool)
		{
			if (Instances == null)
			{
				foundPool = null;
				return false;
			}
			
			for (int __i = Instances.Count - 1; __i >= 0; __i--)
			{
				GameObjectPool __pool = Instances[index: __i];

				if (__pool.prefab != prefab) continue;
				
				foundPool = __pool;
				return true;
			}

			foundPool = null;
			return false;
		}

		/// <summary>Find the pool responsible for handling the specified prefab clone.
		/// (This can be expensive if you have large pools though)</summary>
		[PublicAPI]
		public static bool TryFindPoolByClone(GameObject clone, ref GameObjectPool foundPool)
		{
			if (Instances == null)
			{
				foundPool = null;
				return false;
			}
			
			for (int __i = Instances.Count - 1; __i >= 0; __i--)
			{
				foundPool = Instances[index: __i];

				// Search hash set
				if (foundPool._spawnedClonesHashSet.Contains(item: clone))
				{
					return true;
				}

				// Search list
				for (int __j = foundPool.spawnedClones.Count - 1; __j >= 0; __j--)
				{
					if (foundPool.spawnedClones[index: __j] == clone)
					{
						return true;
					}
				}
			}

			return false;
		}
		
		/// <summary> Assuredly get a pool for handling the specified prefab.</summary>
		[PublicAPI]
		public static void GetEnsuredPoolByPrefab(GameObject prefab, out GameObjectPool foundPool)
		{
			TryFindPoolByPrefab(prefab: prefab, foundPool: out foundPool);
			
			if(foundPool != null) return;
			
			foundPool = (new GameObject {name = $"{typeof(GameObjectPool)} {prefab.name}"}).AddComponent<GameObjectPool>();
		}

		/// <summary> The amount of spawned clones.</summary>
		[PublicAPI]
		public int Spawned => spawnedClones.Count + _spawnedClonesHashSet.Count;

		/// <summary> The amount of despawned clones.</summary>
		[PublicAPI]
		public int Despawned => despawnedClones.Count;

		/// <summary> The total amount of spawned and despawned clones.</summary>
		[PublicAPI]
		public int Total => Spawned + Despawned;

		/// <summary> This will either spawn a previously despawned/preloaded clone, recycle one, create a new one, or return null. </summary>
		[PublicAPI]
		public void Spawn()
		{
			Transform __transform = transform;
			Spawn(position: __transform.position, rotation: __transform.rotation);
		}

		[PublicAPI]
		public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
		{
			GameObject __clone = default(GameObject);

			TrySpawn(position: position, rotation: rotation, parent: parent, clone: ref __clone);

			return __clone;
		}

		/// <summary>This will either spawn a previously despawned/preloaded clone, recycle one, create a new one, or return null. </summary>
		[PublicAPI]
		public bool TrySpawn(Vector3 position, Quaternion rotation, Transform parent, ref GameObject clone)
		{
			if (prefab != null)
			{
				// Spawn a previously despawned/preloaded clone?
				for (int __i = despawnedClones.Count - 1; __i >= 0; __i--)
				{
					clone = despawnedClones[index: __i];

					despawnedClones.RemoveAt(index: __i);

					if (clone != null)
					{
						SpawnClone(clone: clone, position: position, rotation: rotation, parent: parent);

						return true;
					}

					CGDebug.LogWarning(message: "This pool contained a null despawned clone, did you accidentally destroy it?", context: this);
				}

				// Make a new clone?
				if (Capacity <= 0 || Total < Capacity)
				{
					clone = CreateClone(position: position, rotation: rotation, parent: parent);

					// Add clone to spawned list
					if (Recycle)
					{
						spawnedClones.Add(item: clone);
					}
					else
					{
						_spawnedClonesHashSet.Add(item: clone);
					}

					// Activate
					clone.SetActive(true);

					// Notifications
					InvokeOnSpawn(clone: clone);

					return true;
				}

				// Recycle?
				if (Recycle != true || 
					TryDespawnOldest(clone: ref clone, false) != true) return false;
				
				SpawnClone(clone: clone, position: position, rotation: rotation, parent: parent);

				return true;
			}

			 CGDebug.LogWarning(message: "You're attempting to spawn from a pool with a null prefab", context: this);

			return false;
		}

		/// <summary>This will despawn the oldest prefab clone that is still spawned.</summary>
		[ContextMenu(itemName: "Despawn Oldest")]
		public void DespawnOldest()
		{
			GameObject __clone = default(GameObject);

			TryDespawnOldest(clone: ref __clone, true);
		}

		private bool TryDespawnOldest(ref GameObject clone, bool registerDespawned)
		{
			MergeSpawnedClonesToList();

			// Loop through all spawnedClones from the front (oldest) until one is found
			while (spawnedClones.Count > 0)
			{
				clone = spawnedClones[0];

				spawnedClones.RemoveAt(0);

				if (clone != null)
				{
					DespawnNow(clone: clone, register: registerDespawned);

					return true;
				}

				CGDebug.LogWarning(message: "This pool contained a null spawned clone, did you accidentally destroy it?", context: this);
			}

			return false;
		}

		/// <summary>This method will despawn all currently spawned prefabs managed by this pool. </summary>
		[ContextMenu(itemName: "Despawn All")]
		public void DespawnAll()
		{
			// Merge
			MergeSpawnedClonesToList();

			// Despawn
			for (int __i = spawnedClones.Count - 1; __i >= 0; __i--)
			{
				GameObject __clone = spawnedClones[index: __i];

				if (__clone != null)
				{
					DespawnNow(clone: __clone);
				}
			}

			spawnedClones.Clear();

			// Clear all delays
			for (int __i = delays.Count - 1; __i >= 0; __i--)
			{
				ClassPool<Delay>.Despawn(instance: delays[index: __i]);
			}

			delays.Clear();
		}

		/// <summary>This will either instantly despawn the specified gameObject, or delay despawn it after t seconds. </summary>
		[PublicAPI]
		public void Despawn(GameObject clone, float t = 0.0f)
		{
			if (clone != null)
			{
				// Delay the despawn?
				if (t > 0.0f)
				{
					DespawnWithDelay(clone: clone, t: t);
				}
				// Despawn now?
				else
				{
					TryDespawn(clone: clone);

					// If this clone was marked for delayed despawn, remove it
					for (int __i = delays.Count - 1; __i >= 0; __i--)
					{
						Delay __delay = delays[index: __i];

						if (__delay.clone == clone)
						{
							delays.RemoveAt(index: __i);
						}
					}
				}
			}
			else
			{
				CGDebug.LogWarning(message: "You're attempting to despawn a null gameObject", context: this);
			}
		}

		/// <summary>This method will create an additional prefab clone and add it to the despawned list. </summary>
		[ContextMenu(itemName: "Preload One")]
		public void PreloadOne()
		{
			if (prefab != null)
			{
				// Create clone
				GameObject __clone = CreateClone(position: Vector3.zero, rotation: Quaternion.identity, null);

				// Add clone to despawned list
				despawnedClones.Add(item: __clone);

				// Deactivate it
				__clone.SetActive(false);

				// Move it under this GO
				__clone.transform.SetParent(parent: transform, false);

				if (Capacity > 0 && Total > Capacity)
				{
					CGDebug.LogWarning(message: "You've preloaded more than the pool capacity, please verify you're preloading the intended amount.", context: this);
				}
			}
			else
			{
				CGDebug.LogWarning(message: "Attempting to preload a null prefab.", context: this);
			}
		}

		/// <summary>This will preload the pool based on the Preload setting. </summary>
		[ContextMenu(itemName: "Preload All")]
		public void PreloadAll()
		{
			if (Preload <= 0) return;
			
			if (prefab != null)
			{
				for (int __i = Total; __i < Preload; __i++)
				{
					PreloadOne();
				}
			}
			else 
			{
				CGDebug.LogWarning(message: "Attempting to preload a null prefab", context: this);
			}
		}
		
		#endregion

		#region Private API
		
		protected virtual void Awake()
		{
			PreloadAll();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			
			Instances.Add(item: this);
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			
			Instances.Remove(item: this);
		}

		protected virtual void Update()
		{
			// Decay the life of all delayed destruction calls
			for (int __i = delays.Count - 1; __i >= 0; __i--)
			{
				Delay __delay = delays[index: __i];

				__delay.life -= Time.deltaTime;

				// Skip to next one?
				if (__delay.life > 0.0f)
				{
					continue;
				}

				// Remove and pool delay
				delays.RemoveAt(index: __i); ClassPool<Delay>.Despawn(instance: __delay);

				// Finally despawn it after delay
				if (__delay.clone != null)
				{
					Despawn(clone: __delay.clone);
				}
				else
				{
					CGDebug.LogWarning(message: "Attempting to update the delayed destruction of a prefab clone that no longer exists, did you accidentally delete it?", context: this);
				}
			}
		}

		private void DespawnWithDelay(GameObject clone, float t)
		{
			// If this object is already marked for delayed despawn, update the time and return
			for (int __i = delays.Count - 1; __i >= 0; __i--)
			{
				Delay __delay = delays[index: __i];

				if (__delay.clone != clone) continue;
				
				if (t < __delay.life)
				{
					__delay.life = t;
				}

				return;
			}

			// Create delay
			Delay __newDelay = ClassPool<Delay>.Spawn() ?? new Delay();

			__newDelay.clone = clone;
			__newDelay.life  = t;

			delays.Add(item: __newDelay);
		}

		private void TryDespawn(GameObject clone)
		{
			if (_spawnedClonesHashSet.Remove(item: clone) || spawnedClones.Remove(item: clone))
			{
				DespawnNow(clone: clone);
			}
			else
			{
				CGDebug.LogWarning(message: "You're attempting to despawn a GameObject that wasn't spawned from this pool, make sure your Spawn and Despawn calls match.", context: clone);
			}
		}

		private void DespawnNow(GameObject clone, bool register = true)
		{
			// Add clone to despawned list
			if (register)
			{
				despawnedClones.Add(item: clone);
			}

			// Messages?
			InvokeOnDespawn(clone: clone);

			// Deactivate it
			clone.SetActive(false);

			// Move it under this GO
			clone.transform.SetParent(parent: transform, false);
		}

		private GameObject CreateClone(Vector3 position, Quaternion rotation, Transform parent)
		{
			GameObject __clone = Instantiate(original: prefab, position: position, rotation: rotation);

			if (Stamp)
			{
				__clone.name = prefab.name + " " + Total;
			}
			else
			{
				__clone.name = prefab.name;
			}

			__clone.transform.SetParent(parent: parent, false);

			return __clone;
		}
		private void SpawnClone(GameObject clone, Vector3 position, Quaternion rotation, Transform parent)
		{
			// Register
			if (Recycle)
			{
				spawnedClones.Add(item: clone);
			}
			else
			{
				_spawnedClonesHashSet.Add(item: clone);
			}

			// Update transform
			Transform __cloneTransform = clone.transform;

			__cloneTransform.localPosition = position;
			__cloneTransform.localRotation = rotation;

			__cloneTransform.SetParent(parent: parent, false);

			// Activate
			clone.SetActive(true);

			// Notifications
			InvokeOnSpawn(clone: clone);
		}

		private void InvokeOnSpawn(GameObject clone)
		{
			switch (notification)
			{
				case NotificationType.IPoolable: 
					clone.GetComponents(results: TempPoolables);
					TempPoolables.For(action: component => component.OnSpawn());
					break;
				case NotificationType.BroadcastIPoolable: 
					clone.GetComponentsInChildren(results: TempPoolables); 
					TempPoolables.For(action: component => component.OnSpawn());
					break;
				case NotificationType.None:
					break;
				default:
					throw new System.ArgumentOutOfRangeException();
			}
		}
		private void InvokeOnDespawn(GameObject clone)
		{
			switch (notification)
			{
				case NotificationType.IPoolable: 
					clone.GetComponents(results: TempPoolables); 
					TempPoolables.For(action: component => component.OnDespawn());
					break;
				case NotificationType.BroadcastIPoolable: 
					clone.GetComponentsInChildren(results: TempPoolables); 
					TempPoolables.For(action: component => component.OnDespawn());
					break;
				case NotificationType.None:
					break;
				default:
					throw new System.ArgumentOutOfRangeException();
			}
		}

		private void MergeSpawnedClonesToList()
		{
			if (_spawnedClonesHashSet.Count <= 0) return;
			
			spawnedClones.AddRange(collection: _spawnedClonesHashSet);

			_spawnedClonesHashSet.Clear();
		}

		protected override void OnBeforeSerialize()
		{
			base.OnBeforeSerialize();
			
			MergeSpawnedClonesToList();
		}

		protected override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			
			if (Recycle) return;
			
			for (int __i = spawnedClones.Count - 1; __i >= 0; __i--)
			{
				GameObject __clone = spawnedClones[index: __i];

				_spawnedClonesHashSet.Add(item: __clone);
			}

			spawnedClones.Clear();
		}
		
		#endregion

		#endregion
	}
}