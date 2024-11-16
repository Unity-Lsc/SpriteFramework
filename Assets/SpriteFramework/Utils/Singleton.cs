using UnityEngine;

/// <summary>
/// 普通单例模版(where限制模版类型 new()指的是这个类型必须要能被实例化)
/// </summary>
public abstract class Singleton<T> where T : new()
{

    private static T mInstance;

    //为了线程安全 互斥变量
    private static readonly object mutex = new();

    public static T Instance {
        get {
            if (mInstance == null) {
                //保证单例是线程安全的
                lock (mutex) {
                    if (mInstance == null)
                        mInstance = new T();
                }
            }
            return mInstance;
        }
    }

}

/// <summary>
/// Unity单例模版(不用考虑多线程)
/// </summary>
public class UnitySingleton<T> : MonoBehaviour where T : Component
{

    private static T mInstance;

    public static T Instance {
        get {
            if (mInstance == null) {
                mInstance = FindFirstObjectByType<T>();
                if (mInstance == null) {
                    GameObject obj = new();
                    mInstance = obj.AddComponent(typeof(T)) as T;
                    obj.hideFlags = HideFlags.DontSave;
                    obj.name = typeof(T).Name;
                }
            }

            return mInstance;

        }
    }

    public virtual void Awake() {
        DontDestroyOnLoad(gameObject);
        if (mInstance == null) {
            mInstance = this as T;
        } else {
            Destroy(gameObject);
        }
    }

}
