using UnityEngine;
using SpriteFramework;


public class TestAudio : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            //播放背景音乐
            GameApp.Audio.PlayBGM("maintheme1");
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            //播放UI音效
            GameApp.Audio.PlayAudio("button_sound");
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            //播放3D音效
            GameApp.Audio.PlayAudio("button_sound", transform.position);
        }
    }
}