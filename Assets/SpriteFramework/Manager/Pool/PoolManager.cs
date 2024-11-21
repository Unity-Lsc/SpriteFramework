namespace SpriteFramework
{
    /// <summary>
    /// 对象池管理器
    /// </summary>
    public class PoolManager
    {

        /// <summary>
        /// 类对象池
        /// </summary>
        public ClassObjectPool ClassObjectPool { get; private set; }

        /// <summary>
        /// 游戏物体对象池
        /// </summary>
        public GameObjectPool GameObjectPool { get; private set; }

        /// <summary>
        /// 变量对象池
        /// </summary>
        public VarObjectPool VarObjectPool { get; private set; }

        internal PoolManager() {
            ClassObjectPool = new ClassObjectPool();
            GameObjectPool = new GameObjectPool();
            VarObjectPool = new VarObjectPool();
        }

        internal void OnUpdate() {
            ClassObjectPool.OnUpdate();
        }

        internal void Dispose() {
            ClassObjectPool.Dispose();
            GameObjectPool.Dispose();
        }

    }
}
