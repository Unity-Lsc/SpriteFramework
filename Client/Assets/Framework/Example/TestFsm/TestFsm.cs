using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFsm : MonoBehaviour
{
    private TestFsmMgr fsmMgr;
    private void OnDestroy()
    {
        //销毁状态机
        fsmMgr.CurrFsm.Destroy();
    }
    void Start()
    {
        //初始化状态机
        fsmMgr = new();
        fsmMgr.Init();
    }
    void Update()
    {
        fsmMgr.OnUpdate();

        if (Input.GetKeyUp(KeyCode.A))
        {
            //切换状态
            fsmMgr.ChangeState(TestFsmMgr.EState.State1);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            //切换状态
            fsmMgr.ChangeState(TestFsmMgr.EState.State2);
        }
    }
}
