using UnityEngine;
using SpriteFramework;

public class TestTime : MonoBehaviour
{

    private TimeAction m_TimeAction;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A)) {
            GameEntry.Log("开始定时器");
            m_TimeAction = GameEntry.Time.CreateTimer(this, 1, 10, (int loop) => {
                GameEntry.Log("定时器的剩余循环次数:{0}", loop);
            }, () => {
                GameEntry.Log("定时器运行完毕");
            });
        }

        if (Input.GetKeyUp(KeyCode.S)) {
            m_TimeAction.Stop();
        }

        if (Input.GetKeyUp(KeyCode.P)) {
            m_TimeAction.Pause();
        }

        if (Input.GetKeyUp(KeyCode.R)) {
            m_TimeAction.Resume();
        }


        if (Input.GetKeyUp(KeyCode.D)) {
            GameEntry.Log("开始定时器");
            GameEntry.Time.CreateTimerOnce(this, 1, () => {
                GameEntry.Log("延迟1秒结束");
            });
        }

        if (Input.GetKeyUp(KeyCode.F)) {
            Attack();
        }

    }

    private async void Attack() {
        GameEntry.Log("怪物出生");

        await GameEntry.Time.Delay(this, 1);
        GameEntry.Log("怪物出生动画播完");

        await GameEntry.Time.Delay(this, 1);
        GameEntry.Log("怪物丢炸弹动画播完");

        await GameEntry.Time.Delay(this, 1);
        GameEntry.Log("怪物遁地动画播完");
        GameEntry.Log("怪物SetActive(false)");

    }

}
