using System;
using UnityEngine;
using UnityEngine.UI;
using SpriteFramework;

/// <summary>
/// 提示窗口
/// </summary>
public partial class DialogForm : UIFormBase
{
    /// <summary>
    /// 提示窗口,按钮显示方式
    /// </summary>
    public enum DialogFormType
    {
        /// <summary>
        /// 确定按钮
        /// </summary>
        Affirm,
        /// <summary>
        /// 确定,取消按钮
        /// </summary>
        AffirmAndCancel
    }

    [SerializeField] Text textTitle;
    [SerializeField] Text textContent;
    [SerializeField] Button btnOK;
    [SerializeField] Button btnCancel;
    [SerializeField] Text textOK;
    [SerializeField] Text textCancel;

    private Action actionOK;
    private Action actionCancel;


    protected override void Awake()
    {
        base.Awake();
        btnOK.onClick.AddListener(() =>
        {
            actionOK?.Invoke();
            this.Close();
        });
        btnCancel.onClick.AddListener(() =>
        {
            actionCancel?.Invoke();
            Close();
        });
    }

    /// <summary>
    /// 按业务 key 展示对话框。
    /// 调用点不再直接访问 DataTable，而是统一走业务服务。
    /// </summary>
    public static void ShowFormByKey(string key, DialogFormType type = DialogFormType.Affirm, Action okAction = null, Action cancelAction = null)
    {
        if (GameApp.Dialog.TryGetDialog(key, out Sys_DialogEntity entity))
        {
            ShowForm(entity.Content, entity.Title, entity.BtnText1, entity.BtnText2, type, okAction, cancelAction);
        }
        else
        {
            GameEntry.LogError("Dialog config not found, key==" + key);
        }
    }

    public static async void ShowForm(string contenct = "", string title = "提示", string textBtn1 = "确定", string textBtn2 = "取消", DialogFormType type = DialogFormType.AffirmAndCancel, Action okAction = null, Action cancelAction = null)
    {
        // 通过业务UI服务打开
        DialogForm formDialog = await GameApp.UI.OpenFormAsync<DialogForm>();
        formDialog.SetUI(contenct, title, textBtn1, textBtn2, type, okAction, cancelAction);
    }

    private void SetUI(string contenct = "", string title = "提示", string textBtn1 = "确定", string textBtn2 = "取消", DialogFormType type = DialogFormType.AffirmAndCancel, Action okAction = null, Action cancelAction = null)
    {
        //窗口内容
        textTitle.text = title;
        textContent.text = contenct;
        textOK.text = textBtn1;
        textCancel.text = textBtn2;

        //点击按钮的类型
        switch (type)
        {
            case DialogFormType.Affirm:
                btnOK.gameObject.SetActive(true);
                btnCancel.gameObject.SetActive(false);
                break;
            case DialogFormType.AffirmAndCancel:
                btnCancel.gameObject.SetActive(true);
                btnOK.gameObject.SetActive(true);
                break;
        }

        actionOK = okAction;
        actionCancel = cancelAction;
    }
}
