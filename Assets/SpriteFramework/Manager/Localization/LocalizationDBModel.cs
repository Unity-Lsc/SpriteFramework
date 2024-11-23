using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// 本地化(多语言)数据管理
    /// </summary>
    public class LocalizationDBModel : DataTableDBModelBase<LocalizationDBModel, DataTableEntityBase> {

        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName {
            get {
                return "Localization/" + GameEntry.CurLanguage.ToString();
            }
        }

        /// <summary>
        /// 当前语言字典
        /// </summary>
        public Dictionary<string, string> LocalizationDict = new();

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms) {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++) {
                LocalizationDict[ms.ReadUTF8String()] = ms.ReadUTF8String();
            }
        }

    }
}
