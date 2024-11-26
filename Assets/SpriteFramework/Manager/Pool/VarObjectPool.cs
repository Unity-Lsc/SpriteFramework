namespace SpriteFramework
{
    /// <summary>
    /// 变量对象池,  也是在类对象池取的变量对象, 只不过包了一层
    /// </summary>
    public class VarObjectPool
    {

        /// <summary>
        /// 变量对象池锁
        /// </summary>
        private object _varObjectLock = new object();

        /// <summary>
        /// 取出一个变量对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T DequeueVarObject<T>() where T : VariableBase, new() {
            lock (_varObjectLock) {
                return GameEntry.Pool.ClassObjectPool.Dequeue<T>();
            }
        }

        /// <summary>
        /// 变量对象回池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void EnqueueVarObject<T>(T item) where T : VariableBase {
            lock (_varObjectLock) {
                GameEntry.Pool.ClassObjectPool.Enqueue(item);
            }
        }

    }
}
