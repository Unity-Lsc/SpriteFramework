using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using SpriteFramework;

public class CircleCtrl
{
    public static CircleCtrl Instance { get; private set; } = new();

    private int showCount = 0;
    private readonly Dictionary<string, CancellationTokenSource> m_DicTimeToken = new();

    public async void CircleOpen(string dialogKey = null)
    {
        if (showCount < 0)
        {
            GameEntry.LogError("调用错误");
            return;
        }
        showCount++;
        if (showCount == 1)
        {
            CircleForm circleForm = await GameApp.UI.OpenFormAsync<CircleForm>();
            circleForm.SetText(string.Empty); //如果不设置要设为空

            if (!string.IsNullOrEmpty(dialogKey) && GameApp.Dialog.TryGetDialogContent(dialogKey, out string content))
            {
                circleForm.SetText(content);
            }
        }
    }

    public void CircleClose()
    {
        if (showCount == 0)
        {
            GameEntry.LogError("调用错误");
            return;
        }
        showCount--;
        if (showCount == 0)
        {
            GameApp.UI.CloseForm<CircleForm>();
        }
    }

    #region 扩展的高级用法, 使用定时器 n秒后自动调用CircleClose, 每个delayTimeKey在定时周期内只能调用一次
    
    public async void CircleOpen(string delayTimeKey, float delayTime, string dialogKey = null)
    {
        if (m_DicTimeToken.ContainsKey(delayTimeKey))
        {
            GameEntry.LogError("不允许同一个Key同时Open");
            return;
        }

        if (delayTime > 0)
        {
            CancellationTokenSource token = new();
            m_DicTimeToken.Add(delayTimeKey, token);

            CircleOpen(dialogKey);
            await UniTask.Delay(TimeSpan.FromSeconds(delayTime), true, cancellationToken: token.Token);
            CircleClose(delayTimeKey);
        }
    }
    public void CircleClose(string delayTimeKey)
    {
        if (m_DicTimeToken.TryGetValue(delayTimeKey, out var token))
        {
            CircleClose();
            m_DicTimeToken.Remove(delayTimeKey);
            token.Cancel();
        }
    }

    #endregion
}
