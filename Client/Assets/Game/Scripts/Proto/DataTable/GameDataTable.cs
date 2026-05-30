using SpriteFramework;

public static class GameDataTable
{
    public static void CreateAllDataTable(DataTableManager dataTableManager)
    {
        dataTableManager.CreateDataTable<Sys_UIFormDBModel>();
        dataTableManager.CreateDataTable<Sys_AudioDBModel>();
        dataTableManager.CreateDataTable<Sys_BGMDBModel>();
        dataTableManager.CreateDataTable<Sys_SceneDBModel>();
        dataTableManager.CreateDataTable<Sys_DialogDBModel>();
        dataTableManager.CreateDataTable<Sys_TipDBModel>();
    }
}
