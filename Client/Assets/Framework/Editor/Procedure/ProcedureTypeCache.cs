using SpriteFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

/// <summary>
/// 编辑器阶段的 Procedure 类型缓存。
/// 负责收集所有继承自 ProcedureBase 的非抽象类型。
/// </summary>
public static class ProcedureTypeCache
{
    /// <summary>
    /// 获取全部可用 Procedure 类型。
    /// 这里使用 UnityEditor.TypeCache，只发生在编辑器阶段。
    /// 不会进入运行时启动链，也不会影响 WebGL callMain
    /// </summary>
    public static List<Type> GetAllProcedureTypes()
    {
        List<Type> types = TypeCache.GetTypesDerivedFrom<ProcedureBase>()
            .Where(type => type != null && type.IsClass && !type.IsAbstract && !type.IsGenericType)
            .OrderBy(type => type.FullName)
            .ToList();
        return types;
    }

}
