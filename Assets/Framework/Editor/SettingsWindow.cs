using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SettingsWindow : EditorWindow
{
    private readonly List<MacorItem> mList = new();
    private readonly Dictionary<string, bool> mDict = new();

    private string mMacor = string.Empty;

    public SettingsWindow() {

        mList.Clear();
        mList.Add(new MacorItem() { Name = "TEST_MODE", DisplayName = "测试模式", IsDebug = true, IsRelease = false });
        mList.Add(new MacorItem() { Name = "DEBUG_MODE", DisplayName = "调试模式", IsDebug = true, IsRelease = false });
        mList.Add(new MacorItem() { Name = "RELEASE_MODE", DisplayName = "发布模式", IsDebug = false, IsRelease = true });
        mList.Add(new MacorItem() { Name = "DEBUG_LOG_NORMAL", DisplayName = "打印普通日志", IsDebug = true, IsRelease = false });
        mList.Add(new MacorItem() { Name = "DEBUG_LOG_NECESSARY", DisplayName = "打印必要日志", IsDebug = true, IsRelease = false });
        //mList.Add(new MacorItem() { Name = "DEBUG_LOG_RESOURCE", DisplayName = "打印资源日志", IsDebug = true, IsRelease = false });
        mList.Add(new MacorItem() { Name = "DEBUG_LOG_PROTO", DisplayName = "打印通讯日志", IsDebug = true, IsRelease = false });
        mList.Add(new MacorItem() { Name = "DEBUG_LOG_ERROR", DisplayName = "打印错误日志", IsDebug = true, IsRelease = false });
        //mList.Add(new MacorItem() { Name = "STAT_TD", DisplayName = "开启统计", IsDebug = false, IsRelease = true });
        //mList.Add(new MacorItem() { Name = "DEBUG_ROLESTATE", DisplayName = "调试角色状态", IsDebug = false, IsRelease = false });
        //mList.Add(new MacorItem() { Name = "DISABLE_ASSETBUNDLE", DisplayName = "禁用AssetBundle", IsDebug = false, IsRelease = false });
        //mList.Add(new MacorItem() { Name = "HOTFIX_ENABLE", DisplayName = "开启热补丁", IsDebug = false, IsRelease = true });
        //mList.Add(new MacorItem() { Name = "ASSETBUNDLE_ENCRYPT", DisplayName = "AssetBundle加密", IsDebug = false, IsRelease = true });
        //mList.Add(new MacorItem() { Name = "SDKCHANNEL_APPLE_STORE", DisplayName = "渠道_苹果商店", IsDebug = false, IsRelease = false });

    }

    [System.Obsolete]
    private void OnEnable() {
        mMacor = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        for (int i = 0; i < mList.Count; i++) {
            if (!string.IsNullOrEmpty(mMacor) && mMacor.IndexOf(mList[i].Name) != -1) {
                mDict[mList[i].Name] = true;
            } else {
                mDict[mList[i].Name] = false;
            }
        }
    }

    [System.Obsolete]
    private void OnGUI() {
        for (int i = 0; i < mList.Count; i++) {
            EditorGUILayout.BeginHorizontal("box");
            mDict[mList[i].Name] = GUILayout.Toggle(mDict[mList[i].Name], mList[i].DisplayName);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("保存", GUILayout.Width(100))) {
            SaveMacor();
        }

        if (GUILayout.Button("调试模式", GUILayout.Width(100))) {
            for (int i = 0; i < mList.Count; i++) {
                mDict[mList[i].Name] = mList[i].IsDebug;
            }
            //SaveMacor();
        }

        if (GUILayout.Button("发布模式", GUILayout.Width(100))) {
            for (int i = 0; i < mList.Count; i++) {
                mDict[mList[i].Name] = mList[i].IsRelease;
            }
            //SaveMacor();
        }
        EditorGUILayout.EndHorizontal();

    }

    [System.Obsolete]
    private void SaveMacor() {
        mMacor = string.Empty;
        var enumerator = mDict.GetEnumerator();
        while (enumerator.MoveNext()) {
            if (enumerator.Current.Value) {
                mMacor += string.Format("{0};", enumerator.Current.Key);
            }
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, mMacor);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, mMacor);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, mMacor);
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
