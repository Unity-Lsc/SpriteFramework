using System.Collections.Generic;

namespace SpriteFramework
{
    public partial class DTSysUIFormDBModel
    {

        public Dictionary<string, DTSysUIFormEntity> NameByDict;

        protected override void OnLoadListComple() {
            base.OnLoadListComple();
            NameByDict = new Dictionary<string, DTSysUIFormEntity>();
            for (int i = 0; i < _entityList.Count; i++) {
                DTSysUIFormEntity entity = _entityList[i];

                switch (GameEntry.CurLanguage) {
                    case SpriteLanguage.Chinese:
                        entity.AssetFullPath = SFConstDefine.UIPrefabRoot + entity.AssetPath_Chinese;
                        break;
                    case SpriteLanguage.English:
                        entity.AssetFullPath = SFConstDefine.UIPrefabRoot + (string.IsNullOrWhiteSpace(entity.AssetPath_English) ? entity.AssetPath_Chinese : entity.AssetPath_English);
                        break;
                }
                string[] strs = entity.AssetFullPath.Split('.')[0].Split('/');
                if (strs.Length >= 1) {
                    string str = strs[strs.Length - 1];
                    if (NameByDict.ContainsKey(str)) {
                        GameEntry.LogError("名称:{0} 有重复============", str);
                    } else {
                        entity.UIFromName = str;
                        NameByDict.Add(str, entity);
                    }
                }
            }
        }

        public DTSysUIFormEntity GetEntity(string name) {
            if (NameByDict.ContainsKey(name)) {
                return NameByDict[name];
            }
            GameEntry.LogError("没有找到资源,Name:{0}", name);
            return null;
        }

    }
}
