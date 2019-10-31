namespace CommonGames.Utilities
{
	/// <summary> EnsuredSingleton that persists across multiple scenes </summary>
	public class EnsuredPersistentSingleton<T> : EnsuredSingleton<T> where T : EnsuredSingleton<T>
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			DontDestroyOnLoad(gameObject);
		}
	}
}