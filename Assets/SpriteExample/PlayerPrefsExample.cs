using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteFramework;

public class PlayerPrefsExample : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A)) {
            //性能好, 存入本地存档
            GameEntry.PlayerPrefs.SetFloat(PlayerPrefsConstKey.MasterVolume, 1);
            GameEntry.PlayerPrefs.SetFloat(PlayerPrefsConstKey.AudioVolume, 1);
            GameEntry.PlayerPrefs.SetFloat(PlayerPrefsConstKey.BGMVolume, 1);
        }
        if (Input.GetKeyUp(KeyCode.D)) {
            //获取本地存档
            GameEntry.Log(GameEntry.PlayerPrefs.GetFloat(PlayerPrefsConstKey.MasterVolume));
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            //存储
            GameEntry.PlayerPrefs.SaveAllData();
        }
    }

}
