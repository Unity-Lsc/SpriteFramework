using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpriteFramework.Editor
{
    /// <summary>
    /// 设置面板
    /// </summary>
    public class SettingsWindow : EditorWindow
    {
        private readonly List<MacorItem> _macorList = new();
        private readonly Dictionary<string, bool> _selectDict = new();

        private string _macor = string.Empty;

        public SettingsWindow() {

            _macorList.Clear();
            _macorList.Add(new MacorItem() { Name = "TEST_MODE", DisplayName = "测试模式", IsDebug = true, IsRelease = false });
            _macorList.Add(new MacorItem() { Name = "DEBUG_MODE", DisplayName = "调试模式", IsDebug = true, IsRelease = false });
            _macorList.Add(new MacorItem() { Name = "RELEASE_MODE", DisplayName = "发布模式", IsDebug = false, IsRelease = true });
            _macorList.Add(new MacorItem() { Name = "DEBUG_LOG_NORMAL", DisplayName = "打印普通日志", IsDebug = true, IsRelease = false });
            _macorList.Add(new MacorItem() { Name = "DEBUG_LOG_NECESSARY", DisplayName = "打印必要日志", IsDebug = true, IsRelease = false });
            _macorList.Add(new MacorItem() { Name = "DEBUG_LOG_PROTO", DisplayName = "打印通讯日志", IsDebug = true, IsRelease = false });
            _macorList.Add(new MacorItem() { Name = "DEBUG_LOG_WARNING", DisplayName = "打印警告日志", IsDebug = true, IsRelease = false });
            _macorList.Add(new MacorItem() { Name = "DEBUG_LOG_ERROR", DisplayName = "打印错误日志", IsDebug = true, IsRelease = false });
            //mList.Add(new MacorItem() { Name = "STAT_TD", DisplayName = "开启统计", IsDebug = false, IsRelease = true });
            //mList.Add(new MacorItem() { Name = "DEBUG_ROLESTATE", DisplayName = "调试角色状态", IsDebug = false, IsRelease = false });
            //mList.Add(new MacorItem() { Name = "DISABLE_ASSETBUNDLE", DisplayName = "禁用AssetBundle", IsDebug = false, IsRelease = false });
            _macorList.Add(new MacorItem() { Name = "HOTFIX_ENABLE", DisplayName = "开启热补丁", IsDebug = false, IsRelease = true });
            //mList.Add(new MacorItem() { Name = "ASSETBUNDLE_ENCRYPT", DisplayName = "AssetBundle加密", IsDebug = false, IsRelease = true });
            //mList.Add(new MacorItem() { Name = "SDKCHANNEL_APPLE_STORE", DisplayName = "渠道_苹果商店", IsDebug = false, IsRelease = false });

        }

        [System.Obsolete]
        private void OnEnable() {
            _macor = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            for (int i = 0; i < _macorList.Count; i++) {
                if (!string.IsNullOrEmpty(_macor) && _macor.IndexOf(_macorList[i].Name) != -1) {
                    _selectDict[_macorList[i].Name] = true;
                } else {
                    _selectDict[_macorList[i].Name] = false;
                }
            }
        }

        [System.Obsolete]
        private void OnGUI() {
            for (int i = 0; i < _macorList.Count; i++) {
                EditorGUILayout.BeginHorizontal("box");
                _selectDict[_macorList[i].Name] = GUILayout.Toggle(_selectDict[_macorList[i].Name], _macorList[i].DisplayName);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存", GUILayout.Width(100))) {
                SaveMacor();
            }

            if (GUILayout.Button("调试模式", GUILayout.Width(100))) {
                for (int i = 0; i < _macorList.Count; i++) {
                    _selectDict[_macorList[i].Name] = _macorList[i].IsDebug;
                }
                //SaveMacor();
            }

            if (GUILayout.Button("发布模式", GUILayout.Width(100))) {
                for (int i = 0; i < _macorList.Count; i++) {
                    _selectDict[_macorList[i].Name] = _macorList[i].IsRelease;
                }
                //SaveMacor();
            }
            EditorGUILayout.EndHorizontal();

        }

        [System.Obsolete]
        private void SaveMacor() {
            _macor = string.Empty;
            var enumerator = _selectDict.GetEnumerator();
            while (enumerator.MoveNext()) {
                if (enumerator.Current.Value) {
                    _macor += string.Format("{0};", enumerator.Current.Key);
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _macor);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, _macor);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, _macor);
        }

        /// <summary>
        /// 宏项目
        /// </summary>
        public class MacorItem
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 显示的名称
            /// </summary>
            public string DisplayName;
            /// <summary>
            /// 是否调试项
            /// </summary>
            public bool IsDebug;
            /// <summary>
            /// 是否发布项
            /// </summary>
            public bool IsRelease;
        }

    }

}