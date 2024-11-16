using System.IO;
using UnityEditor;
using UnityEngine;

public class Menu
{

    [MenuItem("工具/设置")]
    public static void Settings() {

        SettingsWindow window = EditorWindow.GetWindow<SettingsWindow>();
        window.titleContent = new GUIContent("全局设置");
        window.Show();

    }

    [MenuItem("工具/打开persisdentDataPath")]
    public static void AssetBundleOpenPersisdentDataPath() {
        string outPath = Application.persistentDataPath;
        if (!Directory.Exists(outPath)) {
            Directory.CreateDirectory(outPath);
        }
        outPath = outPath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", outPath);
    }

}
