using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CommonGames.Utilities.CGTK.CGPooling
{
	/// <summary>This class allows you to pool normal C# classes, for example:
	/// var foo = ClassPool<Foo>.Spawn() ?? new Foo();
	/// ClassPool<Foo>.Despawn(foo);</summary>
	public static class ClassPool<T> where T : class
	{
		// Store cache of all despawned classes here, in a list so we can search it
		private static readonly List<T> Cache = new List<T>();

		#region Spawning
		
		// This will either return a pooled class instance, or null
		[PublicAPI]
		public static T Spawn()
		{
			int __count = Cache.Count;

			if (__count <= 0) return null;
			
			int __index    = __count - 1;
			T __instance = Cache[__index];

			Cache.RemoveAt(__index);

			return __instance;

		}

		/// <summary>This will either return a pooled class instance, or null. If an instance it found, onSpawn will be called with it. NOTE: onSpawn is expected to not be null.</summary>
		[PublicAPI]
		public static T Spawn(Action<T> onSpawn)
		{
			T __instance = default(T);

			TrySpawn(onSpawn, ref __instance);

			return __instance;
		}

		[PublicAPI]
		public static bool TrySpawn(Action<T> onSpawn, ref T instance)
		{
			int __count = Cache.Count;

			if (__count <= 0) return false;
			
			int __index = __count - 1;

			instance = Cache[__index];

			Cache.RemoveAt(__index);

			onSpawn(instance);

			return true;

		}

		/// <summary>This will either return a pooled class instance, or null.
		/// All pooled classes will be checked with match to see if they qualify.
		/// NOTE: match is expected to not be null.</summary>
		[PublicAPI]
		public static T Spawn(Predicate<T> match)
		{
			T __instance = default(T);

			TrySpawn(match, ref __instance);

			return __instance;
		}

		[PublicAPI]
		public static bool TrySpawn(Predicate<T> match, ref T instance)
		{
			int __index = Cache.FindIndex(match);

			if (__index < 0) return false;
			
			instance = Cache[__index];

			Cache.RemoveAt(__index);

			return true;

		}

		/// <summary>This will either return a pooled class instance, or null.
		/// All pooled classes will be checked with match to see if they qualify.
		/// If an instance it found, onSpawn will be called with it.
		/// NOTE: match is expected to not be null.
		/// NOTE: onSpawn is expected to not be null.</summary>
		[PublicAPI]
		public static T Spawn(Predicate<T> match, Action<T> onSpawn)
		{
			T __instance = default(T);

			TrySpawn(match, onSpawn, ref __instance);

			return __instance;
		}

		[PublicAPI]
		public static bool TrySpawn(Predicate<T> match, Action<T> onSpawn, ref T instance)
		{
			int __index = Cache.FindIndex(match);

			if (__index < 0) return false;
			
			instance = Cache[__index];

			Cache.RemoveAt(__index);

			onSpawn(instance);

			return true;
		}
		
		#endregion

		#region Despawning

		/// <summary>This will pool the passed class instance.</summary>
		[PublicAPI]
		public static void Despawn(T instance)
		{
			if (instance != null)
			{
				Cache.Add(instance);
			}
		}

		/// <summary>This will pool the passed class instance.
		/// If you need to perform de-spawning code then you can do that via onDespawn.</summary>
		[PublicAPI]
		public static void Despawn(T instance, Action<T> onDespawn)
		{
			if (instance == null) return;
			
			onDespawn(instance);

			Cache.Add(instance);
		}
		
		#endregion
	}
}