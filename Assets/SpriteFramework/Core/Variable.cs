using System;

namespace SpriteFramework
{
    /// <summary>
    /// 变量泛型基类
    /// </summary>
    public class Variable<T> : VariableBase {

        /// <summary>
        /// 当前存储的真实值
        /// </summary>
        public T Value;

        /// <summary>
        /// 变量类型
        /// </summary>
        public override Type Type {
            get { return typeof(T); }
        }

    }
}