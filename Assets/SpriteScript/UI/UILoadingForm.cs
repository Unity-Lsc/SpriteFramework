using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteFramework;
using System;

/// <summary>
/// Loading界面
/// </summary>
public partial class UILoadingForm : UIFormBase
{

    [SerializeField] private Scrollbar m_ScrollBar;
    [SerializeField] private Text m_TxtTip;

    private void OnLoadingProgressChange(float progress) {
        m_TxtTip.text = string.Format("正在进入场景, 加载进度 {0}%", Math.Floor(progress * 100));
        m_ScrollBar.size = progress;
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
