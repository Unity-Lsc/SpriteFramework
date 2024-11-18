using SpriteFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> m_AudioClips = new List<AudioClip>();

    private Button m_Button;

    void Start()
    {
        int count = m_AudioClips.Count;
        if (count == 0) {
            GameEntry.LogError("没有绑定AudioClip");
            return;
        }

        AudioClip clip = m_AudioClips[Random.Range(0, count)];
        if(clip == null) {
            GameEntry.LogWarning("该组件的AudioClip为空");
            return;
        }

        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(() => {
            GameEntry.Audio.PlaySound(clip);
        });

    }

}
