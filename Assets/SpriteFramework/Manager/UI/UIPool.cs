using System.Collections.Generic;
using UnityEngine;
using SpriteMain;

namespace SpriteFramework
{
    /// <summary>
    /// UI池子
    /// </summary>
    public class UIPool
    {
        /// <summary>
        /// 对象池中存储的UI窗体集合
        /// </summary>
        private LinkedList<UIFormBase> m_UIFormList;

        private float m_NextRunTime = 0f;

        public UIPool() {
            m_UIFormList = new LinkedList<UIFormBase>();
        }

        internal void OnUpdate() {
            if(Time.time > m_NextRunTime + MainEntry.ParamsSettings.UIClearInterval) {
                m_NextRunTime = Time.time;

                //释放UI对象池
                CheckClear();
            }
        }

        /// <summary>
        /// 从池中获取窗体
        /// </summary>
        /// <param name="formId">窗体的ID</param>
        internal UIFormBase Dequeue(int formId) {
            for(var curNode = m_UIFormList.First; curNode != null; curNode = curNode.Next) {
                if(curNode.Value.UIFormEntity.Id == formId) {
                    curNode.Value.IsActive = true;
                    curNode.Value.gameObject.SetActive(true);
                    m_UIFormList.Remove(curNode.Value);
                    return curNode.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 窗体回池
        /// </summary>
        /// <param name="formBase">要回池的窗体</param>
        internal void Enqueue(UIFormBase formBase) {
            formBase.CloseTime = Time.time;
            formBase.IsActive = false;
            formBase.gameObject.SetActive(false);
            m_UIFormList.AddLast(formBase);
        }

        /// <summary>
        /// 根据窗体的ID获取对应窗体
        /// </summary>
        /// <param name="formId">窗体的ID</param>
        public UIFormBase GetUIForm(int formId) {
            for (LinkedListNode<UIFormBase> curr = m_UIFormList.First; curr != null; curr = curr.Next) {
                if (curr.Value.UIFormEntity.Id == formId) {
                    return curr.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 检查对象池释放
        /// </summary>
        internal void CheckClear() {
            for(var curNode = m_UIFormList.First; curNode != null;) {
                if(curNode.Value.UIFormEntity.IsLock != 1 && Time.time > curNode.Value.CloseTime + MainEntry.ParamsSettings.UIExpire) {
                    var nextNode = curNode.Next;
                    GameEntry.Log("从UI池的Release方法释放UI:{0}", curNode.Value.gameObject);
                    Release(curNode.Value);
                    curNode = nextNode;
                } else {
                    curNode = curNode.Next;
                }
            }
        }

        /// <summary>
        /// 通过窗体名字释放窗体
        /// </summary>
        /// <param name="formName">窗体的名字</param>
        internal void Release(string formName) {
            int formId = GameEntry.DataTable.DTSysUIFormDBModel.GetEntity(formName).Id;
            for (LinkedListNode<UIFormBase> curr = m_UIFormList.First; curr != null; curr = curr.Next) {
                if (curr.Value.UIFormEntity.Id == formId) {
                    Release(curr.Value);
                    break;
                }
            }
        }

        /// <summary>
        /// 通过窗体进行释放
        /// </summary>
        /// <param name="formBase">窗体</param>
        public void Release(UIFormBase formBase) {
            m_UIFormList.Remove(formBase);
            Object.Destroy(formBase.gameObject);
        }

        /// <summary>
        /// 立即强制清除全部窗口界面
        /// </summary>
        internal void ReleaseAll() {
            for (LinkedListNode<UIFormBase> curr = m_UIFormList.First; curr != null;) {
                LinkedListNode<UIFormBase> next = curr.Next;

                GameEntry.Log("从UI池中释放UI:{0}", curr.Value.gameObject);
                Release(curr.Value);
                curr = next;
            }
        }

    }
}
