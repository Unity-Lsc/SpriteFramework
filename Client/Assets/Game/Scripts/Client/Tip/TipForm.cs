using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using SpriteFramework;


public class TipForm : UIFormBase
{
    [SerializeField] Text textContent;

    public static void ShowFormByKey(string key)
    {
        if (GameApp.Tip.TryGetTip(key, out Sys_TipEntity entity))
        {
            ShowForm(entity.Content, entity.Duration);
        }
        else
        {
            GameEntry.LogError("Tip config not found, key==" + key);
        }
    }
    public static async void ShowForm(string contenct = "", float duration = 3f)
    {
        TipForm form = await GameApp.UI.OpenFormAsync<TipForm>();
        form.SetUI(contenct, duration);
    }

    private async void SetUI(string contenct = "", float duration = 3f)
    {
        //窗口内容
        textContent.text = contenct;
        await UniTask.Delay(TimeSpan.FromSeconds(duration), true, cancellationToken: this.GetCancellationTokenOnDestroy());
        Close();
    }

}
