namespace CommonGames.Utilities.CGTK.CGPooling
{
    /// <summary>If you implement this interface in a component on your pooled prefab, then the OnSpawn and OnDespawn methods will be automatically called.</summary>
    public interface IPoolable
    {
        /// <summary>Called when this poolable object is spawned.</summary>
        void OnSpawn();

        /// <summary>Called when this poolable object is despawned.</summary>
        void OnDespawn();
    }
}