namespace SpriteFramework
{
    public interface IGameObjectPoolLifecycle
    {
        void OnSpawnedFromPool();
        void OnDespawnedFromPool();
    }
}
