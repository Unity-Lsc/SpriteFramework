using SpriteFramework;
using System;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

/// <summary>
/// ProcedureComponent 的自定义 Inspector。
/// 功能：
/// 1. 自动显示所有继承自 ProcedureBase 的类型
/// 2. 自动同步可用 Procedure 列表到组件序列化字段
/// 3. 提供入口 Procedure 的显式选择
/// </summary>
[CustomEditor(typeof(ProcedureComponent))]
public class ProcedureComponentEditor : UnityEditor.Editor
{

    private SerializedProperty m_AvailableProcedureTypeNamesProp;
    private SerializedProperty m_EntranceProcedureTypeNameProp;

    private void OnEnable()
    {
        m_AvailableProcedureTypeNamesProp = serializedObject.FindProperty("m_AvailableProcedureTypeNames");
        m_EntranceProcedureTypeNameProp = serializedObject.FindProperty("m_EntranceProcedureTypeName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        List<Type> procedureTypes = ProcedureTypeCache.GetAllProcedureTypes();

        SyncAvailableProcedureTypes(procedureTypes);
        DrawProcedureList(procedureTypes);
        DrawEntranceProcedureField(procedureTypes);

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 把编辑器阶段发现到的 Procedure 类型列表同步到组件的隐藏序列化字段中。
    /// 运行时只读取这个结果，不需要做程序集扫描。
    /// </summary>
    private void SyncAvailableProcedureTypes(List<Type> procedureTypes)
    {
        List<string> newTypeNames = procedureTypes
            .Select(type => type.AssemblyQualifiedName)
            .ToList();

        bool changed = m_AvailableProcedureTypeNamesProp.arraySize != newTypeNames.Count;
        if (!changed)
        {
            for (int i = 0; i < newTypeNames.Count; i++)
            {
                if (m_AvailableProcedureTypeNamesProp.GetArrayElementAtIndex(i).stringValue != newTypeNames[i])
                {
                    changed = true;
                    break;
                }
            }
        }

        if (!changed)
        {
            return;
        }

        m_AvailableProcedureTypeNamesProp.arraySize = newTypeNames.Count;
        for (int i = 0; i < newTypeNames.Count; i++)
        {
            m_AvailableProcedureTypeNamesProp.GetArrayElementAtIndex(i).stringValue = newTypeNames[i];
        }
    }

    /// <summary>
    /// 只读显示当前全部可用 Procedure，便于确认自动发现结果。
    /// </summary>
    private void DrawProcedureList(List<Type> procedureTypes)
    {
        EditorGUILayout.LabelField("Available Procedures(可用的流程列表)", EditorStyles.boldLabel);

        if (procedureTypes.Count == 0)
        {
            EditorGUILayout.HelpBox("No ProcedureBase derived types found.", MessageType.Warning);
            return;
        }

        for (int i = 0; i < procedureTypes.Count; i++)
        {
            EditorGUILayout.LabelField($"[{i}] {procedureTypes[i].FullName}");
        }

        EditorGUILayout.Space();
    }

    /// <summary>
    /// 绘制入口 Procedure 选择框。
    /// 所有 Procedure 自动发现，但入口流程必须显式指定。
    /// </summary>
    private void DrawEntranceProcedureField(List<Type> procedureTypes)
    {
        EditorGUILayout.LabelField("Entrance Procedure(启动流程)", EditorStyles.boldLabel);

        if (procedureTypes.Count == 0)
        {
            m_EntranceProcedureTypeNameProp.stringValue = string.Empty;
            return;
        }

        string currentEntrance = m_EntranceProcedureTypeNameProp.stringValue;
        int currentIndex = 0;

        for (int i = 0; i < procedureTypes.Count; i++)
        {
            if (procedureTypes[i].AssemblyQualifiedName == currentEntrance)
            {
                currentIndex = i;
                break;
            }
        }

        string[] displayNames = procedureTypes
            .Select(type => type.FullName)
            .ToArray();

        int selectedIndex = EditorGUILayout.Popup("Entrance", currentIndex, displayNames);
        m_EntranceProcedureTypeNameProp.stringValue = procedureTypes[selectedIndex].AssemblyQualifiedName;
    }

}
