using System;

namespace SpriteFramework
{
    /// <summary>
    /// 变量基类
    /// </summary>
    public abstract class VariableBase
    {

        /// <summary>
        /// 获取变量类型
        /// </summary>
        public abstract Type Type {
            get;
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        public byte ReferenceCount {
            get;
            private set;
        }

        /// <summary>
        /// 引用计数+1（分配对象的时候）
        /// </summary>
        public void AddRefCount() {
            ReferenceCount++;
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Release() {
            ReferenceCount--;
            if (ReferenceCount < 1) {
                //回池操作
                GameEntry.Pool.VarObjectPool.EnqueueVarObject(this);
            }
        }

    }
}
