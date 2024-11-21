using UnityEngine;
using SpriteFramework;

public class ProcedureExample : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W)) {
            Debug.Log("当前的流程状态:" + GameEntry.Procedure.CurProcedureState);
            Debug.Log("当前流程状态的类型:" + GameEntry.Procedure.CurProcedureStateType);
        }

        if (Input.GetKeyUp(KeyCode.A)) {
            GameEntry.Procedure.ChangeState(ProcedureState.Preload);
            GameEntry.Procedure.CurProcedureState.OnUpdate();
        }

        if (Input.GetKeyUp(KeyCode.S)) {
            GameEntry.Procedure.ChangeState(ProcedureState.CheckVersion);
        }
    }

}
