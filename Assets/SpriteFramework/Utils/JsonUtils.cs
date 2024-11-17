using System.Collections.Generic;
using LitJson;
using System.Data;

/// <summary>
/// Json工具类
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// 对象序列化成JSON字符串。
    /// </summary>
    public static string ToJson(this object obj) {
        return JsonMapper.ToJson(obj);
    }

    /// <summary>
    /// JSON字符串序列化成对象
    /// </summary>
    public static T ToObject<T>(this string json) {
        return json == null ? default : JsonMapper.ToObject<T>(json);
    }

    /// <summary>
    /// JSON字符串序列化成集合。
    /// </summary>
    public static List<T> ToList<T>(this string json) {
        return ToObject<List<T>>(json);
    }

    /// <summary>
    /// JSON字符串序列化成DataTable
    /// </summary>
    public static DataTable ToTable(this string json) {
        return json == null ? null : JsonMapper.ToObject<DataTable>(json);
    }

}
