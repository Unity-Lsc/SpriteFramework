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
                GameEntry.Log("Get Http是否有错:{0}", args.HasError);
                GameEntry.Log("Get Http的Json:{0}", args.Value);
                GameEntry.Log("Get Http的Byte:{0}", args.Data);
            });
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            string jsonData = "{\"title\": \"foo\", \"body\": \"bar\", \"userId\": 1}";
            GameEntry.Http.PostArgs("https://www.baidu.com", jsonData, (HttpCallBackArgs args) => {
                GameEntry.Log("Post Http是否有错:{0}", args.HasError);
                GameEntry.Log("Post Http的Json:{0}", args.Value);
                GameEntry.Log("Post Http的Byte:{0}", args.Data);
            });
        }
    }

}
