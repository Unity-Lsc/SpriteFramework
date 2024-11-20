using UnityEngine;
using UnityEngine.UI;
using SpriteFramework;

/// <summary>
/// 本地化(多语言)图片
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
public class ImageLocalization : MonoBehaviour
{

    [Header("本地化语言Key")]
    [SerializeField]
    private string m_Localization;

    private Image m_Image;

    private void Awake() {
        m_Image = GetComponent<Image>();
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
            m_Image.AutoLoadTexture(GameEntry.Localization.GetString(m_Localization));
        }
    }

}
