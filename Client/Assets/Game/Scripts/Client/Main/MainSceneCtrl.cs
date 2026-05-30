using UnityEngine;

public class MainSceneCtrl : MonoBehaviour
{
    private async void Start()
    {
        await GameApp.UI.OpenFormAsync<MainForm>();
    }
}
