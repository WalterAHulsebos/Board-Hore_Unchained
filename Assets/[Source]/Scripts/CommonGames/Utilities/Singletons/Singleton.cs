namespace CommonGames.Utilities
{
	using UnityEngine;
	
	using JetBrains.Annotations;

	#if ODIN_INSPECTOR
	using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour; 
	#endif
	
	/// <typeparam name="T">Type of the Singleton</typeparam>
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		/// <summary> The static reference to the Instance </summary>
		[PublicAPI]
		public static T Instance { get; protected set; }

		/// <summary> Gets whether an Instance of this singleton exists </summary>
		[PublicAPI]
		public static bool InstanceExists => (Instance != null);

		/// <summary> OnEnable method to associate Singleton with Instance </summary>
		protected virtual void OnEnable()
		{
			if (InstanceExists)
			{
				Destroy(gameObject);
			}
			else
			{
				Instance = (T)this;
			}
		}

		/// <summary> OnDestroy method to clear Singleton association </summary>
		protected virtual void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}
	}
}