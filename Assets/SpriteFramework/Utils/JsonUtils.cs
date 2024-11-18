using System.Collections.Generic;
using LitJson;
using System.Data;
using System;

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

    /// <summary>
	/// 按属性名截取Json
	/// </summary>
	/// <param name="json">原Json字符串</param>
	/// <param name="attrArray">截取标识属性名</param>
	/// <returns></returns>
	public static string JsonCutApart(this string json, params string[] attrArray) {
        JsonData jsonData = JsonMapper.ToObject(json);
        foreach (var item in attrArray) {
            if (string.IsNullOrEmpty(item)) continue;
            jsonData = jsonData[item];
        }
        return Convert.ToString(jsonData);
    }
    public static T JsonCutApart<T>(this string json, params string[] attrArray) {
        return JsonCutApart(json, attrArray).ToObject<T>();
    }

}
