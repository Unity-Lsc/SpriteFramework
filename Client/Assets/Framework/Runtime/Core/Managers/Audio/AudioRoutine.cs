using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteFramework;

public class AudioRoutine : MonoBehaviour
{
    public AudioSource AudioSource;
    public GameObjectDespawnHandle AutoDespawnHandle;

    public void Init()
    {
        AudioSource = GetComponent<AudioSource>();
        AutoDespawnHandle = GetComponent<GameObjectDespawnHandle>();
    }
}
