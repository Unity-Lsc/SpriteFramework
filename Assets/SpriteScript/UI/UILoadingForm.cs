using UnityEngine;
using UnityEngine.UI;
using SpriteFramework;
using System;

/// <summary>
/// Loading界面
/// </summary>
public partial class UILoadingForm : UIFormBase
{

    [SerializeField] private Scrollbar _scrollBar;
    [SerializeField] private Text _txtTip;

    protected override void Awake() {
        base.Awake();
        _scrollBar = transform.Find("Scrollbar").GetComponent<Scrollbar>();
        _txtTip = transform.Find("txtTip").GetComponent<Text>();
    }

    private void OnLoadingProgressChange(float progress) {
        _txtTip.text = string.Format("正在进入场景, 加载进度 {0}%", Math.Floor(progress * 100));
        _scrollBar.size = progress;
        if(progress >= 1) {
            Close();
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        GameEntry.Scene.OnLoadingUpdateCallback += OnLoadingProgressChange;
    }

    protected override void OnDisable() {
        base.OnDisable();
        GameEntry.Scene.OnLoadingUpdateCallback -= OnLoadingProgressChange;
    }

}
