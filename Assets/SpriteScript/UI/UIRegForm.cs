using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteFramework;
using UnityEngine.UI;

public partial class UIRegForm : UIFormBase
{

    [SerializeField] private Button _btnBack;

    protected override void Awake() {
        base.Awake();
        _btnBack = transform.Find("btnBack").GetComponent<Button>();
    }

    protected override void Start() {
        base.Start();
        _btnBack.onClick.AddListener(() => {
            GameEntry.Scene.LoadSceneAsync("Main", () => {
                GameEntry.UI.OpenUIForm<UIMainCityForm>();
            });
        });
    }

}
