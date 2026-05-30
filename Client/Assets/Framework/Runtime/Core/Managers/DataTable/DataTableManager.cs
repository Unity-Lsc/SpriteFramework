using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// 配置表管理器
    /// </summary>
    public class DataTableManager
    {
        private readonly List<IDataTableDBModel> m_AllTableLst = new();
        private readonly Dictionary<Type, IDataTableDBModel> m_AllTableDict = new();

        public T CreateDataTable<T>() where T : class, IDataTableDBModel, new()
        {
            Type type = typeof(T);
            if (m_AllTableDict.ContainsKey(type))
            {
                Debug.LogError($"[DataTable] Already exist: {type.Name}");
                return m_AllTableDict[type] as T;
            }
            T table = new T();
            m_AllTableLst.Add(table);
            m_AllTableDict.Add(type, table);
            return table;
        }

        public T Get<T>() where T : class, IDataTableDBModel
        {
            if (m_AllTableDict.TryGetValue(typeof(T), out var table))
                return table as T;

            Debug.LogError($"DataTableManager.Get<T> failed, not found table of type {typeof(T)}");
            return null;
        }

        /// <summary>
        /// 加载表格
        /// </summary>
        internal async UniTask LoadDataAllTable()
        {
            foreach (var table in m_AllTableLst)
            {
                await table.LoadData();
            }
        }

        /// <summary>
        /// 获取表格的字节数组
        /// </summary>
        public async UniTask<byte[]> GetDataTableBuffer(string dataTableName)
        {
            var operation = GameEntry.Loader.DefaultPackage.LoadAssetAsync($"Assets/Game/Download/DataTable/{dataTableName}.bytes");
            await operation.Task;
            TextAsset asset = operation.AssetObject as TextAsset;
            return asset.bytes;
        }

        public void Dispose()
        {
            foreach (var table in m_AllTableLst)
            {
                table.Clear();
            }
            m_AllTableLst.Clear();
            m_AllTableDict.Clear();
        }

    }
}