using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteFramework;
using UnityEngine.UI;

public partial class UIRegForm : UIFormBase
{

    [SerializeField] private Button m_BtnBack;

    protected override void Awake() {
        base.Awake();
        m_BtnBack.onClick.AddListener(() => {
            GameEntry.Scene.LoadSceneAsync("Main", () => {
                GameEntry.UI.OpenUIForm<UIMainCityForm>();
            });
        });
    }

}
