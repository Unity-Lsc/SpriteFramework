using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using Cysharp.Threading.Tasks;

namespace SpriteFramework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager
    {
        /// <summary>
        /// 反切页面的数据快照
        /// 用于关闭当前页面后恢复上一个页面
        /// </summary>
        private sealed class ReverseUIFormInfo
        {
            public int SerialId;
            public string AssetFullPath;
            public int Order;
            public UIFormOpenOptions Options;
            public object UserData;
        }

        /// <summary>
        /// 正在加载中的UI打开请求信息
        /// </summary>
        private sealed class OpeningUIFormInfo
        {
            public int SerialId;
            public string AssetFullPath;
            public UIFormOpenOptions Options;
            public object UserData;
        }

        /// <summary>
        /// 单次打开回调
        /// 该回调由UIManager内部自动移除,业务层不需要手动 += 或 -=
        /// </summary>
        private readonly struct UIOpenCallback
        {
            public readonly Action<UIFormBase> OnSuccess;
            public readonly Action<string> OnFailed;

            public UIOpenCallback(Action<UIFormBase> onSuccess, Action<string> onFailed)
            {
                OnSuccess = onSuccess;
                OnFailed = onFailed;
            }
        }

        public event Action<OpenUIFormSuccessEventArgs> OpenUIFormSuccess;//打开页面成功的回调
        public event Action<OpenUIFormFailureEventArgs> OpenUIFormFailure;//打开页面失败的回调
        public event Action<CloseUIFormCompleteEventArgs> CloseUIFormComplete;//关闭页面完成的回调

        private int m_SerialId = 0;//序列Id自增计数器
        private readonly LinkedList<UIFormBase> m_OpenUIFormList = new();//当前已打开并显示的UI列表
        private readonly LinkedList<ReverseUIFormInfo> m_ReverseChangeUIList = new();//反切页面栈
        private readonly Dictionary<byte, UIGroup> m_UIGroupDict = new();//UI分组表(key->UIGroupId value->场景中配置的UIGroup)
        private readonly Dictionary<int, OpeningUIFormInfo> m_UIFormsBeingLoaded = new();//正在加载的UI表(key->SerialId value->打开的请求信息)
        private readonly Dictionary<string, int> m_SingleLoadingAssetToSerialId = new();//非多开UI的加载中路径映射(key->AssetFullPath value->正在加载的SerialId) 作用:当同一路径正在加载时,后续打开请求直接返回已有SerialId
        private readonly Dictionary<int, UIOpenCallback> m_OpenCallbacks = new();//单次打开回调表(key->SerialId value->本次打开对应的业务回调)
        //正在加载的UI中,需要在加载完成后立即释放的UI表(key->SerialId)
        //当UIForm资源异步加载中,又调用了CloseUIForm(m_SerialId),那么等UIForm加载结束后,m_SerialId如果在该集合中,就会直接销毁实例
        private readonly HashSet<int> m_UIFormsToReleaseOnLoad = new();
        private readonly UILayer m_UILayer = new();//UI层级管理器,负责维护每个UIGroup当前可用sortingOrder
        private readonly UIPool m_UIPool = new();//UI对象池,负责缓存关闭后的UI实例,并定时清理非驻留实例

        /// <summary>
        /// 标准分辨率比值
        /// </summary>
        public float StandardScreen { get; private set; }

        /// <summary>
        /// 当前分辨率比值
        /// </summary>
        public float CurrScreen { get; private set; }

        internal UIManager()
        {
            StandardScreen = GameEntry.Instance.UIRootCanvasScaler.referenceResolution.x / GameEntry.Instance.UIRootCanvasScaler.referenceResolution.y;
            for (int i = 0; i < GameEntry.Instance.UIGroups.Length; i++)
            {
                UIGroup group = GameEntry.Instance.UIGroups[i];
                m_UIGroupDict[group.Id] = group;
            }

            //设置默认渲染模式
            ChangeCanvasRanderMode(RenderMode.ScreenSpaceCamera);
        }

        internal void OnUpdate()
        {
            m_UIPool.OnUpdate();
        }

        /// <summary>
        /// 根据UI分组编号获取UI分组
        /// </summary>
        public UIGroup GetUIGroup(byte id)
        {
            m_UIGroupDict.TryGetValue(id, out UIGroup group);
            return group;
        }

        /// <summary>
        /// 打开UI界面
        /// </summary>
        public int OpenUIForm(string assetFullPath, UIFormOpenOptions options, object userData = null)
        {
            return OpenUIFormInternal(assetFullPath, options, userData, null);
        }

        /// <summary>
        /// 打开UI界面(直接传入回调)
        /// </summary>
        public int OpenUIForm(string assetFullPath, UIFormOpenOptions options, Action<UIFormBase> onSuccess, Action<string> onFailed = null, object userData = null)
        {
            return OpenUIFormInternal(assetFullPath, options, userData, new UIOpenCallback(onSuccess, onFailed));
        }

        /// <summary>
        /// 打开UI界面(泛型)
        /// </summary>
        public int OpenUIForm<T>(string assetFullPath, UIFormOpenOptions options, Action<T> onSuccess, Action<string> onFailed = null, object userData = null) where T : UIFormBase
        {
            return OpenUIForm(assetFullPath, options, form => onSuccess?.Invoke(form as T), onFailed, userData);
        }

        /// <summary>
        /// 打开UI界面(UniTask)
        /// </summary>
        public UniTask<UIFormBase> OpenUIFormAsync(string assetFullPath, UIFormOpenOptions options, object userData = null)
        {
            var taskCompleteSource = new UniTaskCompletionSource<UIFormBase>();
            OpenUIForm(assetFullPath, options, form => taskCompleteSource.TrySetResult(form), error => taskCompleteSource.TrySetException(new Exception(error)), userData);
            return taskCompleteSource.Task;
        }

        /// <summary>
        /// 打开UI界面(UniTask泛型)
        /// </summary>
        public async UniTask<T> OpenUIFormAsync<T>(string assetFullPath, UIFormOpenOptions options, object userData = null) where T : UIFormBase
        {
            return await OpenUIFormAsync(assetFullPath, options, userData) as T;
        }

        /// <summary>
        /// 打开UI的内部主流程
        /// </summary>
        private int OpenUIFormInternal(string assetFullPath, UIFormOpenOptions options, object userData, UIOpenCallback? callback)
        {
            if (!ValidateOpenArgs(assetFullPath, options))
            {
                callback?.OnFailed?.Invoke("Invalid open UI arguments.");
                return 0;
            }

            UIFormOpenOptions optionSnapshot = options.Clone();

            if (!optionSnapshot.AllowMultiInstance)
            {
                UIFormBase opened = GetOpenedUIForm(assetFullPath);
                if (opened != null)
                {
                    int openedSerialId = opened.RuntimeInfo.SerialId;
                    RegisterOpenCallback(openedSerialId, callback);
                    FireOpenUIFormSuccess(openedSerialId, assetFullPath, opened, userData);
                    return openedSerialId;
                }

                if (m_SingleLoadingAssetToSerialId.TryGetValue(assetFullPath, out int loadingSerialId))
                {
                    RegisterOpenCallback(loadingSerialId, callback);
                    return loadingSerialId;
                }
            }

            int serialId = GenerateSerialId();

            OpeningUIFormInfo info = new OpeningUIFormInfo
            {
                SerialId = serialId,
                AssetFullPath = assetFullPath,
                Options = optionSnapshot,
                UserData = userData
            };

            RegisterOpenCallback(serialId, callback);
            m_UIFormsBeingLoaded.Add(serialId, info);

            if (!optionSnapshot.AllowMultiInstance)
            {
                m_SingleLoadingAssetToSerialId[assetFullPath] = serialId;
            }

            LoadUIForm(info, true);
            return serialId;
        }

        /// <summary>
        /// 注册单次打开回调
        /// </summary>
        private void RegisterOpenCallback(int serialId, UIOpenCallback? callback)
        {
            if (!callback.HasValue) return;
            m_OpenCallbacks[serialId] = callback.Value;
        }

        /// <summary>
        /// 执行UI加载
        /// </summary>
        private void LoadUIForm(OpeningUIFormInfo info, bool checkReverseChange)
        {
            UIFormRuntimeInfo runtimeInfo = BuildRuntimeInfo(info);

            if (checkReverseChange)
            {
                HandleBeforeOpen(runtimeInfo, info.Options, info.UserData);
            }

            UIFormBase formBase = m_UIPool.Dequeue(info.AssetFullPath, info.UserData);
            if (formBase != null)
            {
                OnLoadUIFormSuccess(info, formBase, false);
                return;
            }

            UIGroup group = GetUIGroup(info.Options.UIGroupId);

            GameUtil.LoadPrefabClone(
                info.AssetFullPath,
                group.Group,
                uiObj => OnLoadUIFormAssetSuccess(info.SerialId, uiObj),
                error => OnLoadUIFormAssetFailure(info.SerialId, error));
        }

        /// <summary>
        /// UI资源加载成功回调
        /// </summary>
        private void OnLoadUIFormAssetSuccess(int serialId, GameObject uiObj)
        {
            if (!m_UIFormsBeingLoaded.TryGetValue(serialId, out OpeningUIFormInfo info))
            {
                if (uiObj != null)
                {
                    Object.Destroy(uiObj);
                }
                return;
            }

            if (m_UIFormsToReleaseOnLoad.Remove(serialId))
            {
                ClearLoadingInfo(info);
                RemoveOpenCallback(serialId);

                if (uiObj != null)
                {
                    Object.Destroy(uiObj);
                }

                return;
            }

            if (uiObj == null)
            {
                OnLoadUIFormAssetFailure(serialId, $"UI prefab load failed: {info.AssetFullPath}");
                return;
            }

            UIFormBase formBase = uiObj.GetComponent<UIFormBase>();
            if (formBase == null)
            {
                Object.Destroy(uiObj);
                OnLoadUIFormAssetFailure(serialId, $"UI prefab missing UIFormBase: {info.AssetFullPath}");
                return;
            }

            formBase.OnInit(info.UserData);
            OnLoadUIFormSuccess(info, formBase, true);
        }

        /// <summary>
        /// UI资源加载失败回调
        /// </summary>
        private void OnLoadUIFormAssetFailure(int serialId, string error)
        {
            if (!m_UIFormsBeingLoaded.TryGetValue(serialId, out OpeningUIFormInfo info))
            {
                return;
            }

            ClearLoadingInfo(info);
            FireOpenUIFormFailure(serialId, info.AssetFullPath, error, info.UserData);
        }

        /// <summary>
        /// UI实例准备完成后执行真正打开流程
        /// </summary>
        private void OnLoadUIFormSuccess(OpeningUIFormInfo info, UIFormBase formBase, bool isNewInstance)
        {
            ClearLoadingInfo(info);

            UIFormRuntimeInfo runtimeInfo = BuildRuntimeInfo(info);

            formBase.Init(runtimeInfo);
            formBase.State = UIState.Opened;
            formBase.transform.SetParent(GetUIGroup(info.Options.UIGroupId).Group, false);
            formBase.SetSortingOrder(m_UILayer.GetCurrSortingOrder(runtimeInfo));
            formBase.OnOpen(info.UserData);

            m_OpenUIFormList.AddLast(formBase);

            FireOpenUIFormSuccess(info.SerialId, info.AssetFullPath, formBase, info.UserData);
        }

        /// <summary>
        /// 按序列号关闭UI
        /// </summary>
        public void CloseUIForm(int serialId)
        {
            if (m_UIFormsBeingLoaded.ContainsKey(serialId))
            {
                m_UIFormsToReleaseOnLoad.Add(serialId);
                RemoveOpenCallback(serialId);
                return;
            }

            UIFormBase formBase = GetOpenedUIForm(serialId);
            if (formBase != null)
            {
                CloseUIForm(formBase);
            }
        }

        /// <summary>
        /// 按资源路径关闭UI
        /// </summary>
        public void CloseUIForm(string assetFullPath)
        {
            UIFormBase formBase = GetOpenedUIForm(assetFullPath);
            if (formBase != null)
            {
                CloseUIForm(formBase);
            }
        }

        /// <summary>
        /// 按实例关闭 UI。
        /// </summary>
        public void CloseUIForm(UIFormBase formBase)
        {
            if (formBase == null)
            {
                GameEntry.LogError(ELogCategory.UI, "CloseUIForm failed: formBase is null.");
                return;
            }

            if (formBase.State != UIState.Opened)
            {
                GameEntry.LogError(ELogCategory.UI, $"CloseUIForm failed: invalid state={formBase.State}");
                return;
            }

            HideUIForm(formBase, true);
        }

        /// <summary>
        /// 执行关闭流程
        /// 该函数保持同步,不使用 async void
        /// 关闭后进入对象池,而不是立即销毁
        /// </summary>
        private void HideUIForm(UIFormBase formBase, bool checkReverseChange)
        {
            LinkedListNode<UIFormBase> node = m_OpenUIFormList.FindLast(formBase);
            if (node == null)
            {
                GameEntry.LogError(ELogCategory.UI, $"{formBase} not found in open list.");
                return;
            }

            formBase.State = UIState.Closing;
            formBase.OnClose(false, formBase.RuntimeInfo.UserData);

            bool shouldRestoreReverse = false;
            ReverseUIFormInfo restoreEntity = null;

            if (checkReverseChange)
            {
                if (formBase.RuntimeInfo.ShowMode == EUIFormShowMode.ReverseChange)
                {
                    if (m_ReverseChangeUIList.Count > 0 &&
                        m_ReverseChangeUIList.Last.Value.SerialId == formBase.RuntimeInfo.SerialId)
                    {
                        m_ReverseChangeUIList.RemoveLast();
                    }

                    if (m_ReverseChangeUIList.Count > 0)
                    {
                        restoreEntity = m_ReverseChangeUIList.Last.Value;
                        shouldRestoreReverse = true;
                    }
                }
                else
                {
                    RecoverLayerAfterClose(node, formBase);
                }
            }

            m_OpenUIFormList.Remove(formBase);
            m_UIPool.Enqueue(formBase);

            FireCloseUIFormComplete(formBase.RuntimeInfo.SerialId, formBase.RuntimeInfo.AssetFullPath, formBase, formBase.RuntimeInfo.UserData);

            if (shouldRestoreReverse && restoreEntity != null)
            {
                OpenUIForm(
                    restoreEntity.AssetFullPath,
                    restoreEntity.Options.Clone(),
                    restored => {
                        restored.SetSortingOrder(restoreEntity.Order);
                        InvokeAndClearOnBack(restored);
                    },
                    error => GameEntry.LogError(ELogCategory.UI, error),
                    restoreEntity.UserData);
            }
        }

        /// <summary>
        /// 关闭普通UI后,恢复同组后续UI的层级
        /// </summary>
        private void RecoverLayerAfterClose(LinkedListNode<UIFormBase> node, UIFormBase formBase)
        {
            if (node.Next != null && !formBase.RuntimeInfo.DisableLayer)
            {
                for (LinkedListNode<UIFormBase> curr = node.Next; curr != null; curr = curr.Next)
                {
                    if (curr.Value.RuntimeInfo.UIGroupId != formBase.RuntimeInfo.UIGroupId) continue;
                    if (curr.Value.RuntimeInfo.DisableLayer) continue;

                    curr.Value.SetSortingOrder(curr.Value.sortingOrder - 10);
                }
            }

            m_UILayer.SetSortingOrder(formBase.RuntimeInfo, false);
        }

        /// <summary>
        /// 打开前处理反切和层级游标
        /// </summary>
        private void HandleBeforeOpen(UIFormRuntimeInfo runtimeInfo, UIFormOpenOptions options, object userData)
        {
            if (runtimeInfo.ShowMode == EUIFormShowMode.ReverseChange && m_ReverseChangeUIList.Count > 0)
            {
                ReverseUIFormInfo last = m_ReverseChangeUIList.Last.Value;
                UIFormBase currentTop = GetOpenedUIForm(last.SerialId);

                if (currentTop != null)
                {
                    HideUIForm(currentTop, false);
                }
            }
            else
            {
                m_UILayer.SetSortingOrder(runtimeInfo, true);
            }

            if (runtimeInfo.ShowMode == EUIFormShowMode.ReverseChange)
            {
                m_ReverseChangeUIList.AddLast(new ReverseUIFormInfo
                {
                    SerialId = runtimeInfo.SerialId,
                    AssetFullPath = runtimeInfo.AssetFullPath,
                    Order = m_UILayer.GetCurrSortingOrder(runtimeInfo),
                    Options = options.Clone(),
                    UserData = userData
                });
            }
        }

        /// <summary>
        /// 执行并清空反切返回回调
        /// </summary>
        private void InvokeAndClearOnBack(UIFormBase formBase)
        {
            if (formBase.OnBack == null) return;

            Action onBack = formBase.OnBack;
            formBase.OnBack = null;
            onBack();
        }

        /// <summary>
        /// 生成新的 SerialId
        /// </summary>
        private int GenerateSerialId()
        {
            m_SerialId++;
            if (m_SerialId <= 0)
            {
                m_SerialId = 1;
            }

            return m_SerialId;
        }

        /// <summary>
        /// 构建运行时信息
        /// </summary>
        private UIFormRuntimeInfo BuildRuntimeInfo(OpeningUIFormInfo info)
        {
            return new UIFormRuntimeInfo
            {
                SerialId = info.SerialId,
                AssetFullPath = info.AssetFullPath,
                UIGroupId = info.Options.UIGroupId,
                AllowMultiInstance = info.Options.AllowMultiInstance,
                DisableLayer = info.Options.DisableLayer,
                IsResident = info.Options.IsResident,
                ShowMode = info.Options.ShowMode,
                UserData = info.UserData
            };
        }

        /// <summary>
        /// 校验打开参数
        /// </summary>
        private bool ValidateOpenArgs(string assetFullPath, UIFormOpenOptions options)
        {
            if (string.IsNullOrEmpty(assetFullPath))
            {
                GameEntry.LogError(ELogCategory.UI, "assetFullPath is null or empty.");
                return false;
            }

            if (options == null)
            {
                GameEntry.LogError(ELogCategory.UI, $"OpenUIForm options is null, assetFullPath={assetFullPath}");
                return false;
            }

            if (!m_UIGroupDict.ContainsKey(options.UIGroupId))
            {
                GameEntry.LogError(ELogCategory.UI, $"UI group not found, groupId={options.UIGroupId}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 清理加载中信息
        /// </summary>
        private void ClearLoadingInfo(OpeningUIFormInfo info)
        {
            m_UIFormsBeingLoaded.Remove(info.SerialId);

            if (!info.Options.AllowMultiInstance &&
                m_SingleLoadingAssetToSerialId.TryGetValue(info.AssetFullPath, out int serialId) &&
                serialId == info.SerialId)
            {
                m_SingleLoadingAssetToSerialId.Remove(info.AssetFullPath);
            }
        }

        /// <summary>
        /// 移除单次打开回调
        /// </summary>
        private void RemoveOpenCallback(int serialId)
        {
            m_OpenCallbacks.Remove(serialId);
        }

        /// <summary>
        /// 派发打开成功事件,并触发单次打开成功回调
        /// </summary>
        private void FireOpenUIFormSuccess(int serialId, string assetFullPath, UIFormBase formBase, object userData)
        {
            OpenUIFormSuccess?.Invoke(new OpenUIFormSuccessEventArgs
            {
                SerialId = serialId,
                AssetFullPath = assetFullPath,
                UIForm = formBase,
                UserData = userData
            });

            if (m_OpenCallbacks.TryGetValue(serialId, out UIOpenCallback callback))
            {
                m_OpenCallbacks.Remove(serialId);
                callback.OnSuccess?.Invoke(formBase);
            }
        }

        /// <summary>
        /// 派发打开失败事件,并触发单次打开失败回调
        /// </summary>
        private void FireOpenUIFormFailure(int serialId, string assetFullPath, string error, object userData)
        {
            OpenUIFormFailure?.Invoke(new OpenUIFormFailureEventArgs
            {
                SerialId = serialId,
                AssetFullPath = assetFullPath,
                ErrorMessage = error,
                UserData = userData
            });

            if (m_OpenCallbacks.TryGetValue(serialId, out UIOpenCallback callback))
            {
                m_OpenCallbacks.Remove(serialId);
                callback.OnFailed?.Invoke(error);
            }
        }

        /// <summary>
        /// 派发关闭完成事件
        /// </summary>
        private void FireCloseUIFormComplete(int serialId, string assetFullPath, UIFormBase formBase, object userData)
        {
            CloseUIFormComplete?.Invoke(new CloseUIFormCompleteEventArgs
            {
                SerialId = serialId,
                AssetFullPath = assetFullPath,
                UIForm = formBase,
                UserData = userData
            });
        }

        /// <summary>
        /// 关闭所有已打开UI
        /// </summary>
        public void CloseAllUIForms()
        {
            List<UIFormBase> forms = new();

            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.Last; curr != null; curr = curr.Previous)
            {
                forms.Add(curr.Value);
            }

            for (int i = 0; i < forms.Count; i++)
            {
                CloseUIForm(forms[i]);
            }
        }

        /// <summary>
        /// 关闭指定分组下的全部UI
        /// </summary>
        public void CloseUIFormsByGroup(byte groupId)
        {
            List<UIFormBase> forms = new();

            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.Last; curr != null; curr = curr.Previous)
            {
                if (curr.Value.RuntimeInfo.UIGroupId == groupId)
                {
                    forms.Add(curr.Value);
                }
            }

            for (int i = 0; i < forms.Count; i++)
            {
                CloseUIForm(forms[i]);
            }
        }

        /// <summary>
        /// 按路径释放UI
        /// </summary>
        public void Release(string assetFullPath)
        {
            UIFormBase opened = GetOpenedUIForm(assetFullPath);
            if (opened != null)
            {
                Release(opened);
                return;
            }

            m_UIPool.Release(assetFullPath);
        }

        /// <summary>
        /// 按实例释放UI
        /// </summary>
        public void Release(UIFormBase formBase)
        {
            if (formBase == null) return;

            if (formBase.State == UIState.Opened)
            {
                HideUIForm(formBase, true);
            }

            m_UIPool.Release(formBase);
        }

        /// <summary>
        /// 释放全部 UI。
        /// </summary>
        public void ReleaseAll()
        {
            foreach (int serialId in m_UIFormsBeingLoaded.Keys)
            {
                m_UIFormsToReleaseOnLoad.Add(serialId);
            }

            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.Last; curr != null;)
            {
                LinkedListNode<UIFormBase> prev = curr.Previous;
                UIFormBase form = curr.Value;

                if (form != null && form.State != UIState.Destroyed)
                {
                    form.State = UIState.Destroyed;
                    form.OnClose(true, form.RuntimeInfo.UserData);
                    form.OnRelease();
                    Object.Destroy(form.gameObject);
                }

                curr = prev;
            }

            m_OpenUIFormList.Clear();
            m_ReverseChangeUIList.Clear();
            m_UIFormsBeingLoaded.Clear();
            m_UIFormsToReleaseOnLoad.Clear();
            m_SingleLoadingAssetToSerialId.Clear();
            m_OpenCallbacks.Clear();
            m_UIPool.ReleaseAll();
        }

        /// <summary>
        /// 按序列号查询已打开UI
        /// </summary>
        public UIFormBase GetOpenedUIForm(int serialId)
        {
            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.Last; curr != null; curr = curr.Previous)
            {
                if (curr.Value.RuntimeInfo.SerialId == serialId)
                {
                    return curr.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// 按资源路径查询已打开UI
        /// </summary>
        public UIFormBase GetOpenedUIForm(string assetFullPath)
        {
            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.Last; curr != null; curr = curr.Previous)
            {
                if (curr.Value.RuntimeInfo.AssetFullPath == assetFullPath)
                {
                    return curr.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// 按路径查询显示中或缓存中的UI
        /// </summary>
        public T GetUIForm<T>(string assetFullPath) where T : UIFormBase
        {
            UIFormBase opened = GetOpenedUIForm(assetFullPath);
            if (opened != null)
            {
                return opened as T;
            }

            return m_UIPool.GetUIForm(assetFullPath) as T;
        }

        /// <summary>
        /// 判断指定索引号的UI是否正在加载
        /// </summary>
        public bool IsLoading(int serialId)
        {
            return m_UIFormsBeingLoaded.ContainsKey(serialId);
        }

        /// <summary>
        /// 判断指定路径是否存在非多开加载请求
        /// </summary>
        public bool IsLoading(string assetFullPath)
        {
            return m_SingleLoadingAssetToSerialId.ContainsKey(assetFullPath);
        }

        /// <summary>
        /// 切换UI根Canvas渲染模式
        /// </summary>
        public void ChangeCanvasRanderMode(RenderMode renderMode)
        {
            GameEntry.Instance.UIRootCanvas.renderMode = renderMode;
            GameEntry.Instance.UICamera.enabled = renderMode != RenderMode.ScreenSpaceOverlay;
        }

        /// <summary>
        /// 释放UIManager
        /// </summary>
        internal void Dispose()
        {
            ReleaseAll();
            m_UIGroupDict.Clear();
            m_UILayer.Dispose();
        }

    }
}