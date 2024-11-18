using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteFramework;

public class HttpExample : MonoBehaviour
{

    private void Start() {
        
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W)) {
            GameEntry.Http.GetArgs("https://www.baidu.com", (HttpCallBackArgs args) => {
                GameEntry.Log("Http是否有错:{0}", args.HasError);
                GameEntry.Log("Http的Json:{0}", args.Value);
                GameEntry.Log("Http的Byte:{0}", args.Data);
            });
        }
    }

}
