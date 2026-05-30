using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteFramework;

public class TestTime : MonoBehaviour
{
    async void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            for (int i = 0; i < 5; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: this.GetCancellationTokenOnDestroy());
                Debug.Log($"第 {i + 1} 次执行（绑定生命周期）");
            }
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: this.GetCancellationTokenOnDestroy());
            GameEntry.Log(ELogCategory.Normal, "延迟1秒等待结束时");
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            Attack();
        }
    }

    private async void Attack()
    {
        GameEntry.Log(ELogCategory.Normal, "怪物SetActive(true)");

        await UniTask.Delay(1000);
        GameEntry.Log(ELogCategory.Normal, "怪物出生动画播完");

        await UniTask.Delay(1000);
        GameEntry.Log(ELogCategory.Normal, "怪物丢炸弹动画播完");

        await UniTask.Delay(1000);
        GameEntry.Log(ELogCategory.Normal, "怪物遁地动画播完");
        GameEntry.Log(ELogCategory.Normal, "怪物SetActive(false)");
    }
}