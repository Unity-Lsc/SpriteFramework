using System.IO;
using System.Linq;
using UnityEngine;
using SpriteFramework;

/// <summary>
/// 文件工具类(调用操作系统的文件API,方便操作文件和目录等)
/// </summary>
public class FileUtils
{

    private static readonly string mAssetFolderName = "Assets";

    /// <summary>
    /// 把双反斜杠\\转换成单斜杠/
    /// </summary>
    public static string FormatToUnityPath(string path) {
        return path.Replace("\\", "/");
    }

    /// <summary>
    /// 把单斜杠/转换成双斜斜杠\\
    /// </summary>
    public static string FormatToSystemPath(string path) {
        return path.Replace("/", "\\");
    }

    /// <summary>
    /// 将全路径转化为Assets下的路径
    /// </summary>
    public static string FullPathToAssetPath(string fullPath) {
        fullPath = FormatToUnityPath(fullPath);
        if (!fullPath.StartsWith(Application.dataPath)) return null;
        string path = fullPath.Replace(Application.dataPath, "");
        return mAssetFolderName + path;
    }

    /// <summary>
    /// 获取文件的扩展名
    /// </summary>
    /// <param name="path">文件的路径</param>
    /// <returns>文件的扩展名</returns>
    public static string GetFileExtension(string path) {
        return Path.GetExtension(path).ToLower();
    }

    /// <summary>
    /// 通过限定条件检索文件列表
    /// </summary>
    /// <param name="extensions">通过扩展名进行检索</param>
    /// <param name="exclude">是否排除(当为false,是根据所给的扩展名寻找;当为true,是寻找给定的扩展名以外的文件)</param>
    /// <returns>返回符合限定条件的文件列表</returns>
    public static string[] GetFilesPathByCondition(string path, string[] extensions = null, bool exclude = false) {
        if (string.IsNullOrEmpty(path)) return null;
        if (extensions == null) {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        } else if (exclude) {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(f => !extensions.Contains(GetFileExtension(f))).ToArray();
        } else {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(f => extensions.Contains(GetFileExtension(f))).ToArray();
        }
    }

    public static string[] GetFilesPathByCondition(string path, string pattern) {
        if (string.IsNullOrEmpty(path)) return null;
        return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
    }

    /// <summary>
    /// 获取路径下的所有子文件路径
    /// </summary>
    public static string[] GetAllFilesInFolder(string path) {
        return GetFilesPathByCondition(path);
    }

    /// <summary>
    /// 获取路径下的所有文件夹路径
    /// </summary>
    public static string[] GetAllDirectorysInFolder(string path) {
        return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
    }

    /// <summary>
    /// 检查文件路径是否存在(如果不存在,就生成一个文件夹)
    /// </summary>
    public static void CheckFileAndCreateDir(string filePath) {
        if (string.IsNullOrEmpty(filePath)) return;
        FileInfo fileInfo = new FileInfo(filePath);
        DirectoryInfo dirInfo = fileInfo.Directory;
        if (!dirInfo.Exists) {
            Directory.CreateDirectory(dirInfo.FullName);
        }
    }

    /// <summary>
    /// 检查文件夹路径是否存在(如果不存在,就生成一个文件夹)
    /// </summary>
    public static void CheckDirectoryAndCreateDir(string folderPath) {
        if (string.IsNullOrEmpty(folderPath)) return;
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
    }

    /// <summary>
    /// 将二进制数据写进文件中
    /// </summary>
    /// <param name="outFile">要写入的文件</param>
    /// <param name="outBytes">要写入的二进制数据</param>
    /// <returns></returns>
    public static bool SafeWriteAllBytes(string outFile, byte[] outBytes) {
        try {
            if (string.IsNullOrEmpty(outFile)) return false;
            CheckFileAndCreateDir(outFile);
            if (File.Exists(outFile)) {
                //设置文件属性为正常
                File.SetAttributes(outFile, FileAttributes.Normal);
            }
            File.WriteAllBytes(outFile, outBytes);
            return true;
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeWriteAllBytes failed! path = {0} with error: {1}", outFile, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 将每一行字符串写进文件中
    /// </summary>
    public static bool SafeWriteAllLines(string outFile, string[] outLines) {
        try {
            if (string.IsNullOrEmpty(outFile)) return false;
            CheckFileAndCreateDir(outFile);
            if (File.Exists(outFile)) {
                //设置文件属性为正常
                File.SetAttributes(outFile, FileAttributes.Normal);
            }
            File.WriteAllLines(outFile, outLines);
            return true;
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeWriteAllLines failed! path = {0} with error: {1}", outFile, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 将文本字符串写进文件中
    /// </summary>
    public static bool SafeWriteAllText(string outFile, string text) {
        try {
            if (string.IsNullOrEmpty(outFile)) return false;
            CheckFileAndCreateDir(outFile);
            if (File.Exists(outFile)) {
                //设置文件属性为正常
                File.SetAttributes(outFile, FileAttributes.Normal);
            }
            File.WriteAllText(outFile, text);
            return true;
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeWriteAllText failed! path = {0} with error: {1}", outFile, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 从文件中读取二进制数据
    /// </summary>
    public static byte[] SafeReadAllBytes(string inFile) {
        try {
            if (string.IsNullOrEmpty(inFile)) return null;

            if (!File.Exists(inFile)) return null;

            File.SetAttributes(inFile, FileAttributes.Normal);
            return File.ReadAllBytes(inFile);
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeReadAllBytes failed! path = {0} with error = {1}", inFile, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 从文件中读取每一行的字符串
    /// </summary>
    public static string[] SafeReadAllLines(string inFile) {
        try {
            if (string.IsNullOrEmpty(inFile)) {
                return null;
            }

            if (!File.Exists(inFile)) {
                return null;
            }

            File.SetAttributes(inFile, FileAttributes.Normal);
            return File.ReadAllLines(inFile);
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeReadAllLines failed! path = {0} with error = {1}", inFile, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 从文件中读取文本数据
    /// </summary>
    public static string SafeReadAllText(string inFile) {
        try {
            if (string.IsNullOrEmpty(inFile)) {
                return null;
            }

            if (!File.Exists(inFile)) {
                return null;
            }

            File.SetAttributes(inFile, FileAttributes.Normal);
            return File.ReadAllText(inFile);
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeReadAllText failed! path = {0} with error = {1}", inFile, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 删除文件夹及其里面的所有文件
    /// </summary>
    public static void DeleteDirectory(string dirPath) {
        string[] files = Directory.GetFiles(dirPath);
        string[] dirs = Directory.GetDirectories(dirPath);

        foreach (string file in files) {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs) {
            DeleteDirectory(dir);
        }

        Directory.Delete(dirPath, false);
    }

    /// <summary>
    /// 清空文件夹(用生成的方式,保留自身空文件夹)
    /// </summary>
    public static bool SafeClearDir(string folderPath) {
        try {
            if (string.IsNullOrEmpty(folderPath)) {
                return true;
            }

            if (Directory.Exists(folderPath)) {
                DeleteDirectory(folderPath);
            }
            Directory.CreateDirectory(folderPath);
            return true;
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeClearDir failed! path = {0} with error = {1}", folderPath, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 删除文件夹
    /// </summary>
    public static bool SafeDeleteDir(string folderPath) {
        try {
            if (string.IsNullOrEmpty(folderPath)) {
                return true;
            }

            if (Directory.Exists(folderPath)) {
                DeleteDirectory(folderPath);
            }
            return true;
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeDeleteDir failed! path = {0} with error: {1}", folderPath, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    public static bool SafeDeleteFile(string filePath) {
        try {
            if (string.IsNullOrEmpty(filePath)) {
                return true;
            }

            if (!File.Exists(filePath)) {
                return true;
            }
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
            return true;
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeDeleteFile failed! path = {0} with error: {1}", filePath, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 文件重命名
    /// </summary>
    public static bool SafeRenameFile(string sourceFileName, string destFileName) {
        try {
            if (string.IsNullOrEmpty(sourceFileName)) {
                return false;
            }

            if (!File.Exists(sourceFileName)) {
                return true;
            }
            File.SetAttributes(sourceFileName, FileAttributes.Normal);
            File.Move(sourceFileName, destFileName);
            return true;
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeRenameFile failed! path = {0} with error: {1}", sourceFileName, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 拷贝文件
    /// </summary>
    public static bool SafeCopyFile(string fromFile, string toFile) {
        try {
            if (string.IsNullOrEmpty(fromFile)) {
                return false;
            }

            if (!File.Exists(fromFile)) {
                return false;
            }
            CheckFileAndCreateDir(toFile);
            if (File.Exists(toFile)) {
                File.SetAttributes(toFile, FileAttributes.Normal);
            }
            File.Copy(fromFile, toFile, true);
            return true;
        } catch (System.Exception ex) {
            GameEntry.LogError("SafeCopyFile failed! formFile = {0}, toFile = {1}, with error = {2}", fromFile, toFile, ex.Message);
            return false;
        }
    }

}
