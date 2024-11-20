using UnityEngine;
using UnityEngine.UI;
using SpriteFramework;

/// <summary>
/// 本地化(多语言)文本
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class TextLocalization : MonoBehaviour
{
    /// <summary>
    /// 多语言的Key
    /// </summary>
    [Header("本地化语言Key")]
    [SerializeField]
    private string m_Localization;

    /// <summary>
    /// 多语言的显示文本(初始时候填上多语言的Key)
    /// </summary>
    private Text m_LocalizationText;

    private void Awake() {
        m_LocalizationText = GetComponent<Text>();
    }

    private void Start() {
        GameEntry.Localization.OnChangeLanguage += OnChangeLanguage;
        OnChangeLanguage();
    }

    private void OnDestroy() {
        GameEntry.Localization.OnChangeLanguage -= OnChangeLanguage;
    }

    private void OnChangeLanguage() {
        if (!string.IsNullOrEmpty(m_Localization)) {
            string text = GameEntry.Localization.GetString(m_Localization);
            if (!string.IsNullOrWhiteSpace(text)) {
                m_LocalizationText.text = text;
            }
        }
    }

}
