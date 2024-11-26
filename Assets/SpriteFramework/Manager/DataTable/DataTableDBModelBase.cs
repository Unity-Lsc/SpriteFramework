using System.Collections.Generic;


namespace SpriteFramework
{
    /// <summary>
    /// 数据表管理基类
    /// </summary>
    /// <typeparam name="T">数据表管理子类的类型</typeparam>
    /// <typeparam name="P">数据表实体子类的类型</typeparam>
    public abstract class DataTableDBModelBase<T, P> where T : class, new() where P : DataTableEntityBase
    {
        /// <summary>
        /// Entity对象的集合
        /// </summary>
        protected List<P> _entityList;
        public int Count { get { return _entityList.Count; } }

        /// <summary>
        /// Key:Entity的ID
        /// Value:Entity对象
        /// </summary>
        protected Dictionary<int, P> _entityDict;

        public DataTableDBModelBase() {
            _entityList = new List<P>();
            _entityDict = new Dictionary<int, P>();
        }

        #region 需要子类实现的属性,方法
        /// <summary>
        /// 数据表名称
        /// </summary>
        public abstract string DataTableName { get; }
        /// <summary>
        /// 加载数据列表
        /// </summary>
        protected abstract void LoadList(SpriteMemoryStream ms);
        protected virtual void OnLoadListComple() { }
        #endregion

        #region LoadData 加载数据表数据
        /// <summary>
        /// 加载数据表数据
        /// </summary>
        internal void LoadData() {
            //1.拿到这个表格的buffer
            byte[] buffer = GameEntry.DataTable.GetDataTableBuffer(DataTableName);

            using (SpriteMemoryStream ms = new SpriteMemoryStream(buffer)) {
                LoadList(ms);
            }

            OnLoadListComple();
        }
        #endregion

        #region GetList 获取子类对应的数据实体List
        /// <summary>
        /// 获取子类对应的数据实体List
        /// </summary>
        /// <returns></returns>
        public List<P> GetDatas() {
            return _entityList;
        }
        #endregion

        #region GetDict 根据ID获取实体
        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        public P GetDataById(int id) {
            P p;
            if (_entityDict.TryGetValue(id, out p)) {
                return p;
            } else {
                GameEntry.LogError("该ID:{0} 对应的数据实体不存在", id);
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 清空数据
        /// </summary>
        internal void Clear() {
            _entityList.Clear();
            _entityDict.Clear();
        }

    }
}