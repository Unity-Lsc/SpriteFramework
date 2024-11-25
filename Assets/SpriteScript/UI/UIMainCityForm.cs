using UnityEngine;
using SpriteFramework;
using UnityEngine.UI;

public partial class UIMainCityForm : UIFormBase
{

    private RectTransform _trans;

    protected override void Awake() {
        base.Awake();
        _trans = transform.Find("Trans_BtnGroup").GetComponent<RectTransform>();
    }

    protected override void Start() {
        base.Start();
        foreach (Transform child in _trans) {
            Button btn = child.GetComponent<Button>();
            if (btn != null) {
                btn.onClick.AddListener(() => {
                    GameEntry.UI.CloseAllDefaultUIForm();
                    GameEntry.Scene.LoadSceneAsync(btn.GetComponentInChildren<Text>().text, () => {
                        GameEntry.UI.OpenUIForm<UIRegForm>();
                    });
                    GameEntry.Log(btn.GetComponentInChildren<Text>().text);
                });
            }
        }
    }

}
