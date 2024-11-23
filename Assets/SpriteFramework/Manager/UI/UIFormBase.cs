using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteFramework
{
    /// <summary>
    /// UI窗体基类
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIFormBase : MonoBehaviour
    {

        public DTSysUIFormEntity UIFormEntity { get; private set; }

        public Canvas CurCanvas { get; private set; }

        public float CloseTime { get; internal set; }

        /// <summary>
        /// 反切时的回调
        /// </summary>
        public Action OnBack;

        /// <summary>
        /// 是否活跃（防止有人自己改了gameObject.SetAcitve() 所以这里做了数值备份）
        /// </summary>
        internal bool IsActive = true;

        /// <summary>
        /// 当前canvas的sortingOrder（防止有人自己改了canvas.sortingOrder 所以这里做了数值备份）
        /// </summary>
        internal int sortingOrder = 0;

        protected virtual void Awake() {
            if(GetComponent<GraphicRaycaster>() == null) {
                gameObject.AddComponent<GraphicRaycaster>();
            }
            CurCanvas = GetComponent<Canvas>();
        }

        protected virtual void Start() {
            GameEntry.Time.Yield(() => {
                //这里是禁用所有按钮的导航功能，因为用不上, 还可能有意外BUG
                Button[] btns = GetComponentsInChildren<Button>(true);
                for (int i = 0; i < btns.Length; i++) {
                    var navigation = btns[i].navigation;
                    navigation.mode = Navigation.Mode.None;
                    btns[i].navigation = navigation;
                }
            });
        }

        /// <summary>
        /// 窗体的初始化
        /// </summary>
        /// <param name="entity"></param>
        internal void Init(DTSysUIFormEntity entity) {
            UIFormEntity = entity;
        }

        protected virtual void OnEnable() {
            //编辑器模式下，默认将窗体移动到父节点的最顶层
#if UNITY_EDITOR
            transform.SetAsLastSibling();
#endif
        }

        /// <summary>
        /// 关闭自身
        /// </summary>
        public void Close() {
            GameEntry.UI.CloseUIForm(this);
        }

        internal void SetSortingOrder(int sortingOrder) {
            if (UIFormEntity.IsDisableUILayer == 1) return;
            this.sortingOrder = sortingOrder;
            CurCanvas.overrideSorting = true;
            CurCanvas.sortingOrder = sortingOrder;
        }

        protected virtual void OnDisable() {
            
        }

        protected virtual void OnDestroy() {
            
        }

    }
}
