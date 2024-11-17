using UnityEngine;
using SpriteFramework;

public class AudioExample : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A)) {
            //播放背景音乐
            GameEntry.Audio.PlayBgm("Audio_Bg_ChangAn");
        }

        if (Input.GetKeyUp(KeyCode.S)) {
            //停止播放背景音乐
            GameEntry.Audio.StopBgm();
        }

        if (Input.GetKeyUp(KeyCode.D)) {
            GameEntry.Audio.PlaySound("jumpLandingGround");


        }
    }

}
