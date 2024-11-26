using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpriteFramework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager
    {

        /// <summary>
        /// UI窗口的显示类型
        /// </summary>
        public enum UIFormShowMode
        {
            Normal = 0,
            /// <summary>
            /// 反切
            /// </summary>
            Reverse = 1,
        }

        /// <summary>
        /// 已经打开的UI窗体列表
        /// </summary>
        private readonly LinkedList<UIFormBase> _openUIFormList;

        /// <summary>
        /// 反切的UI窗体列表(string->窗体的名字)
        /// </summary>
        private readonly LinkedList<string> _reverseFormList;

        private readonly Dictionary<byte, UIGroup> _uiGroupDict;

        private readonly UILayer _uiLayer;

        private readonly UIPool _uiPool;

        /// <summary>
        /// 标准分辨率比值
        /// </summary>
        public float StandardScreenRate { get; private set; }

        /// <summary>
        /// 当前屏幕分辨率比值
        /// </summary>
        public float CurScreenRate { get; private set; }

        internal UIManager() {
            _openUIFormList = new LinkedList<UIFormBase>();
            _reverseFormList = new LinkedList<string>();
            _uiGroupDict = new Dictionary<byte, UIGroup>();

            _uiLayer = new UILayer();
            _uiPool = new UIPool();

            StandardScreenRate = GameEntry.Instance.UIRootCanvasScaler.referenceResolution.x / GameEntry.Instance.UIRootCanvasScaler.referenceResolution.y;
            CurScreenRate = Screen.width / (float)Screen.height;
            var groups = GameEntry.Instance.UIGroups;
            for (int i = 0; i < groups.Length; i++) {
                _uiGroupDict[groups[i].Id] = groups[i];
            }

            ChangeCanvasRanderMode(RenderMode.ScreenSpaceOverlay);
        }

        internal void OnUpdate() {
            _uiPool.OnUpdate();
        }

        /// <summary>
        /// 根据UI分组编号获取UI分组
        /// </summary>
        /// <param name="id">UI的分组编号</param>
        public UIGroup GetUIGroup(byte id) {
            _uiGroupDict.TryGetValue(id, out UIGroup group);
            return group;
        }

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="CheckReverseChange">是否检查窗体的反切</param>
        public T OpenUIForm<T>(bool CheckReverseChange = true) where T : UIFormBase {
            return OnOpenUIForm<T>(CheckReverseChange);
        }

        /// <summary>
        /// 打开UI窗体(泛型)
        /// </summary>
        /// <param name="CheckReverseChange">是否检查窗体的反切</param>
        private T OnOpenUIForm<T>(bool CheckReverseChange = true) where T : UIFormBase {
            var entity = GameEntry.DataTable.DTSysUIFormDBModel.GetEntity(typeof(T).Name);
            if (entity == null) return null;
            if (!entity.CanMulit && IsOpened(entity.Id)) {
                GameEntry.LogError("不重复打开同一个UI窗体  ID:{0} Path:{0}", entity.Id, entity.AssetFullPath);
                return null;
            }
            if (CheckReverseChange && (UIFormShowMode)entity.ShowMode == UIFormShowMode.Reverse) {
                //检查反切，在打开下一个界面前，关闭当前界面
                if (_reverseFormList.Count > 0) {
                    CloseUIForm(_reverseFormList.Last.Value, false);
                }
                //把窗体加入到反切列表里面
                _reverseFormList.AddLast(entity.UIFromName);
            }

            //先从对象池中取
            UIFormBase formBase = _uiPool.Dequeue(entity.Id);
            if (formBase != null) {
                _openUIFormList.AddLast(formBase);
                SetSortingOrder(formBase, true);
                return formBase as T;
            }

            //对象池中没有，克隆新的
            GameObject obj = UnityUtils.LoadPrefabClone(entity.AssetFullPath, GetUIGroup(entity.UIGroupId).Tran);
            formBase = obj.GetComponent<UIFormBase>();
            if (formBase == null) {
                GameEntry.Log("该UI界面:{0} 没有挂载UIFormBase脚本", obj.name);
                formBase = obj.AddComponent<T>();
            }
            formBase.Init(entity);

            _openUIFormList.AddLast(formBase);
            SetSortingOrder(formBase, true);
            return formBase as T;
        }

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="formName">UI窗体的名字</param>
        /// <param name="CheckReverseChange">是否检查窗体的反切</param>
        private UIFormBase OnOpenUIForm(string formName, bool CheckReverseChange = true) {
            var entity = GameEntry.DataTable.DTSysUIFormDBModel.GetEntity(formName);
            if (entity == null) return null;
            if(!entity.CanMulit && IsOpened(entity.Id)) {
                GameEntry.LogError("不重复打开同一个UI窗体  ID:{0} Path:{0}", entity.Id, entity.AssetFullPath);
                return null;
            }
            if(CheckReverseChange && (UIFormShowMode)entity.ShowMode == UIFormShowMode.Reverse) {
                //检查反切，在打开下一个界面前，关闭当前界面
                if(_reverseFormList.Count > 0) {
                    CloseUIForm(_reverseFormList.Last.Value, false);
                }
                //把窗体加入到反切列表里面
                _reverseFormList.AddLast(entity.UIFromName);
            }

            //先从对象池中取
            UIFormBase formBase = _uiPool.Dequeue(entity.Id);
            if(formBase != null) {
                _openUIFormList.AddLast(formBase);
                SetSortingOrder(formBase, true);
                return formBase;
            }

            //对象池中没有，克隆新的
            GameObject obj = UnityUtils.LoadPrefabClone(entity.AssetFullPath, GetUIGroup(entity.UIGroupId).Tran);
            formBase = obj.GetComponent<UIFormBase>();
            if(formBase == null) {
                GameEntry.LogError("该UI界面:{0} 没有挂载UIFormBase脚本", obj.name);
                formBase = obj.AddComponent<UIFormBase>();
            }
            formBase.Init(entity);

            _openUIFormList.AddLast(formBase);
            SetSortingOrder(formBase, true);
            return formBase;

        }

        /// <summary>
        /// 关闭UI窗体
        /// </summary>
        /// <param name="CheckReverseChange">是否检查窗体的反切</param>
        public void CloseUIForm<T>(bool CheckReverseChange = true) where T : UIFormBase {
            CloseUIForm(typeof(T).Name, CheckReverseChange);
        }

        /// <summary>
        /// 关闭UI窗体
        /// </summary>
        /// <param name="formBase">要关闭的UI窗体</param>
        /// <param name="checkReverseChange">是否检查窗体的反切</param>
        public void CloseUIForm(UIFormBase formBase, bool checkReverseChange = true) {
            if (!formBase.IsActive) return;

            if (_openUIFormList.Contains(formBase)) {
                SetSortingOrder(formBase, false);
                _openUIFormList.Remove(formBase);
                _uiPool.Enqueue(formBase);
            }

            if(checkReverseChange && (UIFormShowMode)formBase.UIFormEntity.ShowMode == UIFormShowMode.Reverse) {
                //把当前界面从反切链表中移除
                _reverseFormList.Remove(formBase.UIFormEntity.UIFromName);

                //检查反切，在关闭当前界面后，打开上一个界面
                if(_reverseFormList.Count > 0) {
                    var preForm = OnOpenUIForm(_reverseFormList.Last.Value, false);
                    if(preForm.OnBack != null) {
                        Action onBack = preForm.OnBack;
                        preForm.OnBack = null;
                        onBack();
                    }
                }
            }
        }

        /// <summary>
        /// 关闭UI窗体
        /// </summary>
        /// <param name="uiFormName">要关闭的UI窗体名字</param>
        /// <param name="checkReverseChange">是否检查窗体的反切</param>
        private void CloseUIForm(string uiFormName, bool checkReverseChange = true) {
            for(var curNode = _openUIFormList.First; curNode != null; curNode = curNode.Next) {
                if(curNode.Value.UIFormEntity.UIFromName == uiFormName) {
                    CloseUIForm(curNode.Value, checkReverseChange);
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭所有"Default"组的UI窗口
        /// </summary>
        public void CloseAllDefaultUIForm() {
            _reverseFormList.Clear();

            List<UIFormBase> lst = new();
            for (LinkedListNode<UIFormBase> curr = _openUIFormList.Last; curr != null; curr = curr.Previous) {
                lst.Add(curr.Value);
            }
            for (int i = 0; i < lst.Count; i++) {
                UIFormBase formBase = lst[i];
                if (formBase.UIFormEntity.UIGroupId != 2) continue;
                CloseUIForm(formBase);
            }
        }

        /// <summary>
        /// 强制关闭UI窗体（从UI池中移除并销毁）
        /// </summary>
        /// <param name="uiFormName">要关闭的UI窗体名字</param>
        public void ForceCloseUIForm(string uiFormName) {
            int formId = GameEntry.DataTable.DTSysUIFormDBModel.GetEntity(uiFormName).Id;
            for (var curNode = _openUIFormList.First; curNode != null; curNode = curNode.Next) {
                if(curNode.Value.UIFormEntity.Id == formId) {
                    ForceCloseUIForm(curNode.Value);
                    return;
                }
            }
            //这里是为了防止，有的打开的UI窗体，没有添加到m_OpenUIFormList中
            _uiPool.Release(uiFormName);
        }

        /// <summary>
        /// 强制关闭UI窗体（从UI池中移除并销毁）
        /// </summary>
        /// <param name="uiFormName">要关闭的UI窗体</param>
        public void ForceCloseUIForm(UIFormBase uIFormBase) {
            CloseUIForm(uIFormBase);
            _uiPool.Release(uIFormBase);
        }

        /// <summary>
        /// 强制关闭所有UI窗体（从UI池中移除并销毁）
        /// </summary>
        public void ForceCloseAllUIForm() {
            for (var curNode = _openUIFormList.First; curNode != null;) {
                var next = curNode.Next;
                _openUIFormList.Remove(curNode.Value);
                if(curNode != null) {
                    Object.Destroy(curNode.Value.gameObject);
                }
                curNode = next;
            }
            _uiPool.ReleaseAll();
            _reverseFormList.Clear();
        }

        /// <summary>
        /// 获取UI窗体
        /// </summary>
        /// <typeparam name="T">UI窗体的泛型类型</typeparam>
        public T GetUIForm<T>() where T : UIFormBase {
            return GetUIForm(typeof(T).Name) as T;
        }

        /// <summary>
        /// 获取UI窗体
        /// </summary>
        /// <param name="formName">UI窗体的名字</param>
        public UIFormBase GetUIForm(string formName) {
            int formId = GameEntry.DataTable.DTSysUIFormDBModel.GetEntity(formName).Id;
            //先检查 以打开的UI窗体列表
            for (var curNode = _openUIFormList.First; curNode != null; curNode = curNode.Next) {
                if (curNode.Value.UIFormEntity.Id == formId) {
                    return curNode.Value;
                }
            }
            //再看看对象池内有没有
            return _uiPool.GetUIForm(formId);
        }

        /// <summary>
        /// 检查UI是否已经打开
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        private bool IsOpened(int formId) {
            for (var curNode = _openUIFormList.First; curNode != null; curNode = curNode.Next){
                if(curNode.Value.UIFormEntity.Id == formId) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 设置UI窗体的层级
        /// </summary>
        /// <param name="isAdd">升高还是降低</param>
        private void SetSortingOrder(UIFormBase formBase, bool isAdd) {
            _uiLayer.SetSortingOrder(formBase, isAdd);
            if (isAdd) {
                formBase.SetSortingOrder(_uiLayer.GetCurSortingOrder(formBase));
            } else {
                var form = _openUIFormList.FindLast(formBase);
                if(form != null && form.Next != null) {
                    for (var curNode = form.Next; curNode != null; curNode = curNode.Next) {
                        if (curNode.Value.UIFormEntity.UIGroupId != formBase.UIFormEntity.UIGroupId) continue;
                        curNode.Value.SetSortingOrder(curNode.Value.sortingOrder - 10);
                    }
                }
            }
        }

        /// <summary>
        /// 切换UI渲染模式
        /// </summary>
        public void ChangeCanvasRanderMode(RenderMode renderMode) {
            GameEntry.Instance.UIRootCanvas.renderMode = renderMode;
            GameEntry.Instance.UICamera.enabled = renderMode != RenderMode.ScreenSpaceOverlay;
        }

        public void Dispose() {
            _openUIFormList.Clear();
            _reverseFormList.Clear();
            _uiGroupDict.Clear();

            _uiPool.ReleaseAll();
            _uiLayer.Dispose();
        }

    }
}
