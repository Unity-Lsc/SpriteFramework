using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using SpriteFramework;
using System;

/// <summary>
/// Unity的一些扩展方法
/// </summary>
public static class MethodExtensionForUnity
{
    /// <summary>
    /// 添加并获取组件
    /// </summary>
    public static T GetOrAddComponent<T>(this Transform origin) where T : Component {
        if (!origin.TryGetComponent<T>(out var component)) {
            component = origin.gameObject.AddComponent<T>();
        }
        return component;
    }
    /// <summary>
    /// 添加并获取组件
    /// </summary>
    public static T GetOrAddCompponent<T>(this GameObject origin) where T : Component {
        if (!origin.TryGetComponent<T>(out var component)) {
            component = origin.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// 设置当前gameObject及所有子物体的层
    /// </summary>
    public static void SetLayer(this GameObject obj, string layerName) {
        Transform[] transArr = obj.transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transArr.Length; i++) {
            transArr[i].gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    /// <summary>
    /// 自动加载图片(Image)
    /// </summary>
    /// <param name="imgPath">图片资源的路径</param>
    /// <param name="isSetNativeSize">是否设置为原尺寸大小</param>
    public static async void AutoLoadTexture(this Image img, string imgPath, bool isSetNativeSize = false) {
        var handler = GameEntry.Resource.LoadAssetAsync<Object>(imgPath);
        await handler.Task;
        var asset = handler.AssetObject;
        Sprite obj;
        if (asset is Sprite sprite) {
            obj = sprite;
        } else {
            Texture2D texture = (Texture2D)asset;
            obj = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        img.sprite = obj;
        if (isSetNativeSize) {
            img.SetNativeSize();
        }
    }

    /// <summary>
    /// 自动加载图片(RawImage)
    /// </summary>
    /// <param name="imgPath">图片资源的路径</param>
    /// <param name="isSetNativeSize">是否设置为原尺寸大小</param>
    public static async void AutoLoadTexture(this RawImage img, string imgPath, bool isSetNativeSize = false) {
        var handler = GameEntry.Resource.LoadAssetAsync<Texture2D>(imgPath);
        await handler.Task;
        Texture2D asset = handler.AssetObject as Texture2D;
        img.texture = asset;
        if (isSetNativeSize) {
            img.SetNativeSize();
        }
    }

    /// <summary>
    /// 设置特效渲染层级
    /// </summary>
    public static void SetEffectOrder(this Transform trans, int sortingOrder) {
        Renderer[] renderers = trans.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].sortingOrder = sortingOrder;
        }
    }

}
