using SpriteFramework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unity的一些扩展方法
/// </summary>
public static class MethodExtensionForUnity
{
    /// <summary>
    /// 获取或创建组件
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        T t = obj.GetComponent<T>();
        if (t == null)
        {
            t = obj.AddComponent<T>();
        }
        return t;
    }
    /// <summary>
    /// 设置当前gameObject及所有子物体的层
    /// </summary>
    public static void SetLayer(this GameObject obj, string layerName)
    {
        Transform[] transArr = obj.transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transArr.Length; i++)
        {
            transArr[i].gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    /// <summary>
    /// 自动加载图片
    /// </summary>
    public static async void AutoLoadTexture(this Image img, string imgPath, bool isSetNativeSize = false)
    {
        Object asset = await GameEntry.Loader.LoadMainAssetAsync<Object>(imgPath, img.gameObject);
        if (asset == null) return;

        Sprite obj = null;
        if (asset is Sprite)
        {
            obj = (Sprite)asset;
        }
        else
        {
            Texture2D texture = (Texture2D)asset;
            obj = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        img.overrideSprite = obj;
        if (isSetNativeSize) img.SetNativeSize();
    }
    /// <summary>
    /// 自动加载图片
    /// </summary>
    public static async void AutoLoadTexture(this RawImage img, string imgPath, bool isSetNativeSize = false)
    {
        Texture2D asset = await GameEntry.Loader.LoadMainAssetAsync<Texture2D>(imgPath, img.gameObject);
        if (asset == null) return;

        img.texture = asset;
        if (isSetNativeSize) img.SetNativeSize();
    }

    /// <summary>
    /// 设置特效渲染层级
    /// </summary>
    public static void SetEffectOrder(this Transform trans, int sortingOrder)
    {
        Renderer[] renderers = trans.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++) renderers[i].sortingOrder = sortingOrder;
    }

    /// <summary>
    /// object是否为null,兼容UnityEngine.Object的特殊null
    /// </summary>
    public static bool IsNull(this object obj)
    {
        return obj == null || (obj is UnityEngine.Object unityObj && unityObj == null);
    }

    /// <summary>
    /// object是否为null,兼容UnityEngine.Object的特殊null(泛型版本)
    /// </summary>
    public static bool IsNull<T>(this T obj)
    {
        return obj == null || (obj is UnityEngine.Object unityObj && unityObj == null);
    }

}
