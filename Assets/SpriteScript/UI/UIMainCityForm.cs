using UnityEngine;
using SpriteFramework;
using UnityEngine.UI;

public partial class UIMainCityForm : UIFormBase
{

    [SerializeField] private RectTransform m_Trans;

    protected override void Awake() {
        base.Awake();

        foreach (Transform child in m_Trans) {
            Button btn = child.GetComponent<Button>();
            if(btn != null) {
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
