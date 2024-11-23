using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteFramework;
using SpriteMain;

public class SocketExample : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A)) {
            GameEntry.Socket.ConnectToMainSocket(MainEntry.ParamsSettings.ServerIp, MainEntry.ParamsSettings.Port);
        }
    }

}
