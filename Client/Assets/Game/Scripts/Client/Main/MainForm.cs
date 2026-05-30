using UnityEngine;
using UnityEngine.UI;
using SpriteFramework;
using Cysharp.Threading.Tasks;

public partial class MainForm : UIFormBase
{
    [SerializeField] private RectTransform m_Trans_BtnGroup;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEntry.Model.GetModel<MainModel>().TestAction -= OnTest;
    }
    protected override void Awake()
    {
        base.Awake();
        foreach (Transform child in m_Trans_BtnGroup)
        {
            if (child.TryGetComponent<Button>(out var button))
            {
                button.onClick.AddListener(() =>
                {
                    GameApp.UI.CloseDefaultForms();
                    GameApp.UI.OpenFormAsync<LoadingForm>().Forget();
                    GameApp.Scene.LoadSceneByGroup(button.GetComponentInChildren<Text>().text);
                });
            }
        }

        GameEntry.Model.GetModel<MainModel>().TestAction += OnTest;
        MainCtrl.Instance.SendTest();
    }

    private void OnTest(int obj)
    {
        //假装这是后端给的数据 可以贴到UI上
        Debug.Log("这个Log文本可以随便修改 测试热更新222");
    }

}
