using UnityEngine;
using SpriteFramework;
using System.Threading.Tasks;

/// <summary>
/// Unity相关的工具类
/// </summary>
public class UnityUtils
{

    /// <summary>
    /// 球体中是否包含一个点
    /// </summary>
    /// <param name="center">球体的中心点</param>
    /// <param name="radius">球体的半径</param>
    /// <param name="point">要判断的点</param>
    public static bool SphereContain(Vector3 center, float radius, Vector3 point) {
        //计算球体中心点到目标点的距离
        float distance = Vector3.SqrMagnitude(point - center);
        if (distance <= radius) return true;
        return false;
    }

    /// <summary>
    /// 立方体中是否包含一个点
    /// </summary>
    public static bool BoundContain(Collider collider, Vector3 point) {
        Bounds bounds = collider.bounds;
        if (bounds != null && bounds.Contains(point)) return true;
        return false;
    }

    /// <summary>
    /// 删掉所有子物体
    /// </summary>
    public static void DestroyAllChildren(Transform parent) {
        int count = parent.childCount;
        for (int i = 0; i < count; i++) {
            Transform child = parent.GetChild(0);
            GameObject.DestroyImmediate(child.gameObject);
            count--;
        }
    }

    /// <summary>
    /// 将字符串转换成三维数组
    /// </summary>
    public static Vector3 StringToVector3(string vecStr, Vector3 defaultValue) {
        Vector3 value = defaultValue;
        string[] values = vecStr.Split(',');
        if (values.Length == 3) {
            try {
                value.x = float.Parse(values[0]);
                value.y = float.Parse(values[1]);
                value.z = float.Parse(values[2]);
            } catch (System.Exception e) {
                Debug.Log(e.ToString());
            }

        }

        return value;
    }

    /// <summary>
    /// 获取路径的最后名称
    /// </summary>
    public static string GetLastPathName(string path) {
        if (path.IndexOf('/') == -1) {
            return path;
        }
        return path.Substring(path.LastIndexOf('/') + 1);
    }

    /// <summary>
    /// 加载Prefab并克隆
    /// </summary>
    /// <param name="prefabFullPath">Prefab的资源路径</param>
    /// <param name="parent">父节点</param>
    public static GameObject LoadPrefabClone(string prefabFullPath, Transform parent = null) {
        var prefab = GameEntry.Resource.LoadAsset<GameObject>(prefabFullPath);
        if(prefab != null) {
            return Object.Instantiate(prefab, parent);
        }
        return null;
    }

    /// <summary>
    /// 异步加载Prefab并克隆
    /// </summary>
    /// <param name="prefabFullPath">Prefab的资源路径</param>
    /// <param name="parent">父节点</param>
    public static async Task<GameObject> LoadPrefabCloneAsync(string prefabFullPath, Transform parent = null) {
        var handler = GameEntry.Resource.LoadAssetAsync<GameObject>(prefabFullPath);
        await handler.Task;
        var prefab = handler.AssetObject as GameObject;
        if (prefab != null)
            return Object.Instantiate(prefab);
        return null;
    }

}
