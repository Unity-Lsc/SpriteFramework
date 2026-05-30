using System.Collections.Generic;
using UnityEngine;

namespace SpriteFramework
{
    public class UIPool
    {
        /// <summary>
        /// 对象池中的列表
        /// </summary>
        private readonly LinkedList<UIFormBase> m_UIFormList = new();

        /// <summary>
        /// 下次运行时间
        /// </summary>
        private float m_NextRunTime = 0f;

        internal void OnUpdate()
        {
            if (Time.time > m_NextRunTime + GameEntry.ParamsSettings.UIClearInterval)
            {
                m_NextRunTime = Time.time;

                //释放UI对象池
                CheckClear();
            }
        }

        /// <summary>
        /// 从池中按资源路径取回界面实例
        /// </summary>
        internal UIFormBase Dequeue(string assetFullPath, object userData)
        {
            for (LinkedListNode<UIFormBase> curr = m_UIFormList.First; curr != null; curr = curr.Next)
            {
                UIFormBase form = curr.Value;
                if (form.RuntimeInfo.AssetFullPath != assetFullPath) continue;

                m_UIFormList.Remove(form);
                form.State = UIState.Opened;
                form.gameObject.SetActive(true);
                form.OnResumeFromPool(userData);
                return form;
            }

            return null;
        }

        /// <summary>
        /// 界面实例入池(记录关闭时间,并禁用对象)
        /// </summary>
        internal void Enqueue(UIFormBase formBase)
        {
            formBase.CloseTime = Time.time;
            formBase.State = UIState.Cached;
            formBase.OnEnterPool();
            formBase.gameObject.SetActive(false);
            m_UIFormList.AddLast(formBase);
        }

        /// <summary>
        /// 检查对象池释放
        /// </summary>
        internal void CheckClear()
        {
            for (LinkedListNode<UIFormBase> curr = m_UIFormList.First; curr != null;)
            {
                LinkedListNode<UIFormBase> next = curr.Next;
                UIFormBase form = curr.Value;

                if (!form.RuntimeInfo.IsResident && Time.time > form.CloseTime + GameEntry.ParamsSettings.UIExpire)
                {
                    Release(form);
                }

                curr = next;
            }
        }

        /// <summary>
        /// 按资源路径释放池中的某个界面
        /// </summary>
        internal void Release(string assetFullPath)
        {
            for (LinkedListNode<UIFormBase> curr = m_UIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.RuntimeInfo.AssetFullPath == assetFullPath)
                {
                    Release(curr.Value);
                    break;
                }
            }
        }

        /// <summary>
        /// 按资源实例释放池中的某个界面
        /// </summary>
        public void Release(UIFormBase form)
        {
            if (form == null) return;

            m_UIFormList.Remove(form);

            if (form.State != UIState.Destroyed)
            {
                form.State = UIState.Destroyed;
                form.OnRelease();
                Object.Destroy(form.gameObject);
            }
        }

        /// <summary>
        /// 立即强制清除全部窗口界面
        /// </summary>
        internal void ReleaseAll()
        {
            for (LinkedListNode<UIFormBase> curr = m_UIFormList.First; curr != null;)
            {
                LinkedListNode<UIFormBase> next = curr.Next;
                Release(curr.Value);
                curr = next;
            }

            m_UIFormList.Clear();
        }

        /// <summary>
        /// 按资源路径查找缓存中的界面实例
        /// </summary>
        public UIFormBase GetUIForm(string assetFullPath)
        {
            for (LinkedListNode<UIFormBase> curr = m_UIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.RuntimeInfo.AssetFullPath == assetFullPath)
                {
                    return curr.Value;
                }
            }
            return null;
        }
    }
}