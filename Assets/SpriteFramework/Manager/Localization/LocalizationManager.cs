using System;
using UnityEngine;

namespace SpriteFramework
{

    /// <summary>
    /// 多语言枚举
    /// </summary>
    public enum SpriteLanguage
    {
        /// <summary>
        /// 中文
        /// </summary>
        Chinese = 0,
        /// <summary>
        /// 英文
        /// </summary>
        English = 1
    }

    /// <summary>
    /// 本地化(多语言)管理器
    /// </summary>
    public class LocalizationManager
    {

        public event Action OnChangeLanguage;

        /// <summary>
        /// 获取本地化文本内容
        /// </summary>
        public string GetString(string key, params object[] args) {
            if (GameEntry.DataTable.LocalizationDBModel.LocalizationDict.TryGetValue(key, out string value)) {
                return string.Format(value, args);
            }
            return value;
        }

        /// <summary>
        /// 切换本地化(多语言)
        /// </summary>
        /// <param name="language">要切换的本地化语言类型</param>
        public void ChangeLanguage(SpriteLanguage language) {
            GameEntry.CurLanguage = language;
            GameEntry.DataTable.LocalizationDBModel.LoadData();
            OnChangeLanguage?.Invoke();
        }

        /// <summary>
        /// 根据当前系统语言, 切换游戏语言
        /// </summary>
        public void ChangeLanguageToSystem() {
#if !UNITY_EDITOR
            switch (Application.systemLanguage) {
                default:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                case SystemLanguage.Chinese:
                    ChangeLanguage(SpriteLanguage.Chinese);
                    break;
                case SystemLanguage.English:
                    ChangeLanguage(SpriteLanguage.English);
                    break;
            }
#endif
        }

    }
}
