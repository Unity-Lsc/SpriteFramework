using UnityEngine;

public class TestScene : MonoBehaviour
{
    async void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            await GameApp.Scene.LoadSceneByGroupAsync(SceneGroupName.TestMVC);
        }
    }
}