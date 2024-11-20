using System.Collections.Generic;

namespace SpriteFramework
{
    public partial class DTSysUIFormDBModel
    {

        public Dictionary<string, DTSysUIFormEntity> NameByDic;

        protected override void OnLoadListComple() {
            base.OnLoadListComple();
            NameByDic = new Dictionary<string, DTSysUIFormEntity>();
            for (int i = 0; i < m_List.Count; i++) {
                DTSysUIFormEntity entity = m_List[i];

                switch (GameEntry.CurLanguage) {
                    case SpriteLanguage.Chinese:
                        entity.AssetFullPath = entity.AssetPath_Chinese;
                        break;
                    case SpriteLanguage.English:
                        entity.AssetFullPath = string.IsNullOrWhiteSpace(entity.AssetPath_English) ? entity.AssetPath_Chinese : entity.AssetPath_English;
                        break;
                }
                string[] strs = entity.AssetFullPath.Split('.')[0].Split('/');
                if (strs.Length >= 1) {
                    string str = strs[strs.Length - 1];
                    if (NameByDic.ContainsKey(str)) {
                        GameEntry.LogError("名称:{0} 有重复============", str);
                    } else {
                        entity.UIFromName = str;
                        NameByDic.Add(str, entity);
                    }
                }
            }
        }

        public DTSysUIFormEntity GetEntity(string name) {
            if (NameByDic.ContainsKey(name)) {
                return NameByDic[name];
            }
            GameEntry.LogError("没有找到资源,Name:{0}", name);
            return null;
        }

    }
}
