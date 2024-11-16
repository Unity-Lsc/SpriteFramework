using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 数据工具类
/// </summary>
public class DataUtils
{

    /// <summary>
    /// 拷贝数据
    /// </summary>
    /// <param name="copyFrom">源数据</param>
    /// <param name="offsetFrom">从哪个下标开始拷贝</param>
    /// <param name="copyTo">目标数据</param>
    /// <param name="offsetTo">从哪个下标结束拷贝</param>
    /// <param name="count">拷贝的数量</param>
    public static void CopyBytes(byte[] copyFrom, int offsetFrom, byte[] copyTo, int offsetTo, int count) {
        Array.Copy(copyFrom, offsetFrom, copyTo, offsetTo, count);
    }

    /// <summary>
    /// 将字符串转变成二进制数据
    /// </summary>
    public static byte[] StringToBytes(string str) {
        return Encoding.Default.GetBytes(str);
    }

    /// <summary>
    /// 将字符串转变成UTF8的二进制数据
    /// </summary>
    public static byte[] StringToUTFBytes(string str) {
        return Encoding.UTF8.GetBytes(str);
    }

    /// <summary>
    /// 将二进制转数据换成字符串
    /// </summary>
    public static string BytesToString(byte[] bytes) {
        return Encoding.Default.GetString(bytes).Trim();
    }


    public static Hashtable HttpGetInfo(string info) {
        if (string.IsNullOrEmpty(info)) {
            return null;
        }

        Hashtable table = new Hashtable();
        string[] paramList = info.Split('&');
        for (int i = 0; i < paramList.Length; i++) {
            string[] keyAndValue = paramList[i].Split('=');
            if (keyAndValue.Length >= 2) {
                if (!table.ContainsKey(keyAndValue[0])) {
                    table.Add(keyAndValue[0], keyAndValue[1]);
                }
            }
        }

        return table;
    }

    /// <summary>
    /// 获取区间中的一个随机数
    /// </summary>
    public static int RandInt(int min, int max) {
        byte[] b = new byte[4];
        new RNGCryptoServiceProvider().GetBytes(b);
        Random r = new Random(BitConverter.ToInt32(b, 0));

        return r.Next(min, max);
    }

    /// <summary>
    /// 获取一个随机字符串
    /// </summary>
    public static string RandString(int len) {
        byte[] b = new byte[4];
        new RNGCryptoServiceProvider().GetBytes(b);
        Random r = new Random(BitConverter.ToInt32(b, 0));

        string str = null;
        str += "0123456789";
        str += "abcdefghijklmnopqrstuvwxyz";
        str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        string s = null;

        for (int i = 0; i < len; i++) {
            s += str.Substring(r.Next(0, str.Length - 1), 1);
        }
        return s;
    }

    /// <summary>
    /// 将字符串转化成32位的MD5编码
    /// </summary>
    public static string Md5(string str) {
        string cl = str;
        StringBuilder md5_builder = new StringBuilder();
        MD5 md5 = MD5.Create();//实例化一个md5对像
                               // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
        // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
        for (int i = 0; i < s.Length; i++) {
            // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
            md5_builder.Append(s[i].ToString("X2"));
            //pwd = pwd + s[i].ToString("X");

        }
        return md5_builder.ToString();
    }

}
