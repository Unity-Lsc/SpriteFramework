using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 本地数据持久化 管理类
    /// </summary>
    public class PlayerPrefsManager
    {

        private Dictionary<string, int> m_IntDict;
        private Dictionary<string, float> m_FloatDict;
        private Dictionary<string, string> m_StringDict;

        public PlayerPrefsManager() {
            m_IntDict = GetObject<Dictionary<string, int>>("m_IntDict");
            m_FloatDict = GetObject<Dictionary<string, float>>("m_FloatDict");
            m_StringDict = GetObject<Dictionary<string, string>>("m_StringDict");
        }

        public int GetInt(string key,int defaultValue = 0) {
            if (m_IntDict.TryGetValue(key, out int retValue))
                return retValue;
            return defaultValue;
        }
        public void SetInt(string key, int value) {
            m_IntDict[key] = value;
        }
        public void SetIntAdd(string key, int value) {
            SetInt(key, GetInt(key) + value);
        }
        public void SetIntHas(string key, int value) {
            if (PlayerPrefs.HasKey(key.ToString())) return;
            SetInt(key, value);
        }

        public bool GetBool(string key, bool defaultValue) {
            return GetInt(key, defaultValue ? 1 : 0) == 1;
        }
        public bool GetBool(string key) {
            return GetInt(key) == 1;
        }
        public void SetBool(string key, bool value, object param = null) {
            SetInt(key, value ? 1 : 0);
        }
        public void SetBoolHas(string key, bool value) {
            if (PlayerPrefs.HasKey(key.ToString())) return;
            SetBool(key, value);
        }

        public float GetFloat(string key, float defaultValue = 0) {
            if (m_FloatDict.TryGetValue(key, out float retValue))
                return retValue;
            else
                return defaultValue;
        }
        public void SetFloat(string key, float value, object param = null) {
            m_FloatDict[key] = value;
        }
        public void SetFloatAdd(string key, float value) {
            SetFloat(key, GetFloat(key) + value);
        }
        public void SetFloatHas(string key, float value) {
            if (PlayerPrefs.HasKey(key.ToString())) return;
            SetFloat(key, value);
        }

        public string GetString(string key, string defaultValue = null) {
            if (m_StringDict.TryGetValue(key, out string retValue))
                return retValue;
            else
                return defaultValue;
        }
        public void SetString(string key, string value, object param = null) {
            m_StringDict[key] = value;
        }
        public void SetStringHas(string key, string value) {
            if (m_StringDict.ContainsKey(key)) return;
            SetString(key, value);
        }

        /// <summary>
        /// 保存所有数据
        /// </summary>
        public void SaveAllData() {
            SetObject("m_IntDict", m_IntDict);
            SetObject("m_FloatDict", m_FloatDict);
            SetObject("m_StringDict", m_StringDict);
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void DeleteAllData() {
            PlayerPrefs.DeleteAll();
        }

        private T GetObject<T>(string key) where T : new() {
            string value = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(value)) {
                return value.ToObject<T>();
            } else {
                return new T();
            }
        }

        private void SetObject<T>(string key, T data) {
            PlayerPrefs.SetString(key, data.ToJson());
        }

        public void Dispose() {
            m_IntDict.Clear();
            m_FloatDict.Clear();
            m_StringDict.Clear();
        }

    }
}
