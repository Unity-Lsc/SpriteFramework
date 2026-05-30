using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteFramework
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIFormBase : MonoBehaviour
    {
        /// <summary>
        /// 当前界面的运行时描述信息
        /// </summary>
        public UIFormRuntimeInfo RuntimeInfo { get; private set; }

        public Canvas CurrCanvas { get; private set; }

        public float CloseTime { get; internal set; }

        /// <summary>
        /// 当前UI状态,由UIManager/UIPool统一维护
        /// </summary>
        public UIState State { get; internal set; } = UIState.None;

        //反向切换模式下的回退回调
        public Action OnBack;

        //记录当前sortingOrder,避免外部直接改Canvas导致框架状态不同步
        internal int sortingOrder = 0;

        protected virtual void Awake()
        {
            if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();
            CurrCanvas = GetComponent<Canvas>();
        }
        protected async virtual void Start()
        {
            await UniTask.Yield();

            //这里是禁用所有按钮的导航功能，因为用不上, 还可能有意外BUG
            Button[] buttons = GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                Navigation navigation = buttons[i].navigation;
                navigation.mode = Navigation.Mode.None;
                buttons[i].navigation = navigation;
            }
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            transform.SetAsLastSibling();
#endif
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnDestroy()
        {
        }

        public void Close()
        {
            GameEntry.UI.CloseUIForm(this);
        }

        internal void Init(UIFormRuntimeInfo runtimeInfo)
        {
            RuntimeInfo = runtimeInfo;
        }

        internal void SetSortingOrder(int sortingOrder)
        {
            if (RuntimeInfo != null && RuntimeInfo.DisableLayer)
            {
                return;
            }
            this.sortingOrder = sortingOrder;
            CurrCanvas.overrideSorting = true;
            CurrCanvas.sortingOrder = sortingOrder;
        }

        /// <summary>
        /// UI首次初始化完成后调用
        /// </summary>
        public virtual void OnInit(object userData)
        {
        }

        /// <summary>
        /// UI打开时调用
        /// </summary>
        public virtual void OnOpen(object userData)
        {
        }

        /// <summary>
        /// UI关闭时调用
        /// isShutdown=true 表示框架释放或销毁时关闭
        /// </summary>
        public virtual void OnClose(bool isShutdown, object userData)
        {
        }

        /// <summary>
        /// UI进入对象池前调用
        /// 适合停止动画、取消临时监听、清理倒计时等
        /// </summary>
        public virtual void OnEnterPool()
        {
        }

        /// <summary>
        /// UI从对象池恢复时调用
        /// </summary>
        public virtual void OnResumeFromPool(object userData)
        {
        }

        /// <summary>
        /// UI被覆盖时调用
        /// </summary>
        public virtual void OnCover()
        {
        }

        /// <summary>
        /// UI重新显示时调用
        /// </summary>
        public virtual void OnReveal()
        {
        }

        /// <summary>
        /// UI暂停时调用
        /// </summary>
        public virtual void OnPause()
        {
        }

        /// <summary>
        /// UI恢复暂停时调用
        /// </summary>
        public virtual void OnResume()
        {
        }

        /// <summary>
        /// UI真正销毁前调用
        /// </summary>
        public virtual void OnRelease()
        {
        }

    }
}