using UnityEngine;
using Cysharp.Threading.Tasks;

public class TestUI : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            // 示例调用改为走业务 UI 服务。
            GameApp.UI.OpenFormAsync<DialogForm>().Forget();
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            GameApp.Dialog.ShowByKey("LoadingOK");
        }
    }
}