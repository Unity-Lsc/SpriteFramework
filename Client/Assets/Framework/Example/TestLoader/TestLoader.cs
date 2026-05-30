using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SpriteFramework;

public class TestLoader : MonoBehaviour
{
    GameObject testGameobject;

    async void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (testGameobject != null)
            {
                Destroy(testGameobject);
            }

            testGameobject = await GameUtil.LoadPrefabCloneAsync("Assets/Game/Download/Prefab/Role/cike/zhujiao_cike_animation.prefab");
            Debug.Log(testGameobject);
        }
    }
    private void OnDestroy()
    {
        if (testGameobject != null)
        {
            Destroy(testGameobject);
        }
    }
}