using UnityEngine;
using UnityEngine.UI;
using SpriteFramework;
using Cysharp.Threading.Tasks;

public class BtnBack : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            GameEntry.Log("回到上一级场景");
            GameApp.UI.OpenFormAsync<LoadingForm>().Forget();
            GameApp.Scene.LoadSceneByGroup(SceneGroupName.Main);
        });
    }
}
