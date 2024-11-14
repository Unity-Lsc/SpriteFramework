using System;
using System.Text.RegularExpressions;

public static class StringUtils
{

    /// <summary>
    /// 判断对象是否为Null、DBNull、Empty或空白字符串
    /// </summary>
    public static bool IsNullOrEmpty(string value) {
        bool retVal = false;
        if (value == null || string.IsNullOrWhiteSpace(value.ToString())) {
            retVal = true;
        }
        return retVal;
    }

    /// <summary>
    /// 判断字符串是否为邮箱
    /// </summary>
    public static bool IsEmail(this string email) {
        Regex regex = new Regex("[a-zA-Z_0-9]+@[a-zA-Z_0-9]{2,6}(\\.[a-zA-Z_0-9]{2,3})+");
        return regex.IsMatch(email);
    }

    /// <summary>
    /// 判断字符串是否为手机号码
    /// </summary>
    public static bool IsPhoneNumber(this string strInput) {
        Regex reg = new Regex(@"(^\d{11}$)");
        return reg.IsMatch(strInput);
    }

    /// <summary>
    /// 检查后缀名
    /// </summary>
    public static bool IsSuffix(this string str, string suffix, StringComparison comparisonType = StringComparison.CurrentCulture) {
        //总长度减去后缀的索引等于后缀的长度
        int indexOf = str.LastIndexOf(suffix, StringComparison.CurrentCultureIgnoreCase);
        return indexOf != -1 && indexOf == str.Length - suffix.Length;
    }

    /// <summary>
    /// 把string类型转换成int
    /// </summary>
    public static int ToInt(this string str) {
        int.TryParse(str, out int temp);
        return temp;
    }

    /// <summary>
    /// 把string类型转换成long
    /// </summary>
    public static long ToLong(this string str) {
        long.TryParse(str, out long temp);
        return temp;
    }

    /// <summary>
    /// 把string类型转换成float
    /// </summary>
    public static float ToFloat(this string str) {
        float.TryParse(str, out float temp);
        return temp;
    }
}
