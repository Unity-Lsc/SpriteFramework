using System.Collections.Generic;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 音频管理器
    /// </summary>
    public class AudioManager
    {
        /// <summary>
        /// 同时播放音效的最大数量
        /// </summary>
        private const int MAX_SOUND_NUM = 8;

        /// <summary>
        /// 音效组件的集合
        /// </summary>
        private List<AudioSource> m_AudioSourceList;

        /// <summary>
        /// 背景音乐组件
        /// </summary>
        private AudioSource m_BgmSource;

        //当前正在播放的音效AudioSource的索引
        private int m_CurIndex;

        //背景音乐是否静音
        private bool m_IsBgmMute;
        //音效是否静音
        private bool m_IsSoundMute;
        //背景音乐音量
        private float m_BgmVolume;
        //音效音量
        private float m_SoundVolume;

        //是否淡出
        private bool m_IsFadeOut = false;
        //是否淡入
        private bool m_IsFadeIn = true;
        //BGM淡入淡出的过渡音量
        private float m_InterimBgmVolume;
        //BGM的目标音量
        private float m_TargetBgmVolume;

        public AudioManager() {
            m_AudioSourceList = new List<AudioSource>(MAX_SOUND_NUM);

            var obj = new GameObject("AudioManager");
            obj.transform.SetParent(GameEntry.Instance.transform);
            for (int i = 0; i < MAX_SOUND_NUM; i++) {
                var source = obj.AddComponent<AudioSource>();
                m_AudioSourceList.Add(source);
            }
            m_BgmSource = obj.AddComponent<AudioSource>();
            m_CurIndex = 0;
        }

        /// <summary>
        /// 音频管理器的初始化
        /// </summary>
        public void Init() {
            GameEntry.PlayerPrefs.SetFloatHas(PlayerPrefsConstKey.BgmVolume, 1);
            GameEntry.PlayerPrefs.SetFloatHas(PlayerPrefsConstKey.SoundVolume, 1);
            GameEntry.PlayerPrefs.SetBoolHas(PlayerPrefsConstKey.BgmMute, false);
            GameEntry.PlayerPrefs.SetBoolHas(PlayerPrefsConstKey.SoundMute, false);

            SetBGMVolume(GameEntry.PlayerPrefs.GetFloat(PlayerPrefsConstKey.BgmVolume), false);
            SetSoundVolume(GameEntry.PlayerPrefs.GetFloat(PlayerPrefsConstKey.SoundVolume), false);
            m_IsBgmMute = GameEntry.PlayerPrefs.GetBool(PlayerPrefsConstKey.BgmMute);
            m_IsSoundMute = GameEntry.PlayerPrefs.GetBool(PlayerPrefsConstKey.SoundMute);
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="soundName">背景音乐的名字</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <param name="volume">背景音乐的音量</param>
        /// <param name="isFadeIn">是否淡入</param>
        public void PlayBgm(string soundName, bool isLoop = true, float volume = 1.0f, bool isFadeIn = true) {
            AudioClip clip = GameEntry.Resource.LoadBgm<AudioClip>(soundName);
            if(clip == null) {
                GameEntry.LogError("背景音乐:{0} 为null,请检查路径", soundName);
                return;
            }
            m_BgmSource.clip = clip;
            m_BgmSource.loop = isLoop;

            m_TargetBgmVolume = volume;
            m_IsFadeIn = isFadeIn;
            m_InterimBgmVolume = 0;

            SetBGMVolume(volume);
            if (isFadeIn) m_BgmSource.volume = 0;

            if (!m_IsBgmMute) {
                m_BgmSource.Play();
            }
        }

        /// <summary>
        /// 暂停和重新播放背景音乐
        /// </summary>
        /// <param name="isPause">是否暂停(true表示暂停,false表示重新播放)</param>
        public void PauseBgm(bool isPause = true) {
            if (isPause) {
                m_BgmSource.Pause();
            } else {
                m_BgmSource.UnPause();
            }
        }

        /// <summary>
        /// 关闭背景音乐
        /// </summary>
        /// <param name="isFadeOut">是否淡出</param>
        public void StopBgm(bool isFadeOut = true) {
            m_IsFadeOut = isFadeOut;
            if (!isFadeOut) {
                m_BgmSource.Stop();
                m_BgmSource.clip = null;
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="soundName">音效的名字</param>
        /// <param name="isLoop">是否循环播放</param>
        public int PlaySound(string soundName, bool isLoop = false, float volume = 1.0f) {
            AudioClip clip = GameEntry.Resource.LoadSound<AudioClip>(soundName);
            if (clip == null) {
                GameEntry.LogError("音效:{0} 为null,请检查路径", soundName);
                return -1;
            }
            return PlaySound(clip, isLoop, volume);
        }

        public int PlaySound(AudioClip clip, bool isLoop = false, float volume = 1.0f) {
            int soundId = m_CurIndex;
            AudioSource source = m_AudioSourceList[m_CurIndex];
            m_CurIndex++;
            m_CurIndex = m_CurIndex >= MAX_SOUND_NUM ? 0 : m_CurIndex;

            source.clip = clip;
            source.loop = isLoop;
            SetSoundVolume(volume, source);

            if (m_IsSoundMute) {
                return -1;
            }

            source.Play();
            return soundId;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="soundName">音效的名字</param>
        /// <param name="isLoop">是否循环播放</param>
        public int PlaySoundOneShot(string soundName, bool isLoop = false, float volume = 1.0f) {
            AudioClip clip = GameEntry.Resource.LoadSound<AudioClip>(soundName);
            if (clip == null) {
                GameEntry.LogError("音效:{0} 为null,请检查路径");
                return -1;
            }
            int soundId = m_CurIndex;
            AudioSource source = m_AudioSourceList[m_CurIndex];
            m_CurIndex++;
            m_CurIndex = m_CurIndex >= MAX_SOUND_NUM ? 0 : m_CurIndex;

            source.clip = clip;
            source.loop = isLoop;
            SetSoundVolume(volume, source);

            if (m_IsSoundMute) {
                return -1;
            }

            source.PlayOneShot(clip);
            return soundId;
        }

        /// <summary>
        /// 关闭某个音效组件
        /// </summary>
        /// <param name="soundId">音效组件的索引</param>
        public void StopSound(int soundId) {
            if(soundId < 0 || soundId >= MAX_SOUND_NUM) {
                return;
            }
            AudioSource source = m_AudioSourceList[soundId];
            source.Stop();
            source.clip = null;
        }

        /// <summary>
        /// 关闭全部音效组件
        /// </summary>
        public void StopAllSound() {
            for (int i = 0; i < MAX_SOUND_NUM; i++) {
                var source = m_AudioSourceList[i];
                source.Stop();
                source.clip = null;
            }
        }


        public void OnUpdate() {

            if (m_IsFadeOut) {
                if(m_InterimBgmVolume > 0) {
                    GameEntry.Log("淡出*******************");
                    m_InterimBgmVolume -= Time.unscaledDeltaTime * 2;//0.5秒淡出时间
                    SetBGMVolume(m_InterimBgmVolume, false);
                } else {
                    GameEntry.Log("淡出结束*******************");
                    m_InterimBgmVolume = 0;
                    m_BgmSource.Stop();
                    SetBGMVolume(m_InterimBgmVolume, false);
                    m_BgmSource.clip = null;
                    m_IsFadeOut = false;
                }
            }

            if (m_IsFadeIn) {
                if(m_InterimBgmVolume < m_TargetBgmVolume) {
                    GameEntry.Log("淡入*******************");
                    m_InterimBgmVolume += Time.unscaledDeltaTime * 2;//0.5秒淡入时间
                    SetBGMVolume(m_InterimBgmVolume, false);
                }else if(m_InterimBgmVolume != m_TargetBgmVolume) {
                    GameEntry.Log("淡入结束*******************");
                    m_InterimBgmVolume = m_TargetBgmVolume;
                    SetBGMVolume(m_TargetBgmVolume, false);
                    m_IsFadeIn = false;
                }
            }
        }

        /// <summary>
        /// 设置背景音乐的音量
        /// </summary>
        /// <param name="volume">背景音乐的音量</param>
        /// <param name="isSave">数据是否保存到本地</param>
        public void SetBGMVolume(float volume, bool isSave = true) {
            volume = Mathf.Clamp(volume, 0, 1);
            m_BgmSource.volume = volume;
            m_BgmVolume = volume;
            if (isSave) {
                GameEntry.PlayerPrefs.SetFloat(PlayerPrefsConstKey.BgmVolume, volume);
            }
        }

        /// <summary>
        /// 设置全部音效组件的音量(数据保存到本地)
        /// </summary>
        /// <param name="volume">音效的音量</param>
        public void SetSoundVolume(float volume, bool isSave = true) {
            volume = Mathf.Clamp(volume, 0, 1);
            m_SoundVolume = volume;
            for (int i = 0; i < m_AudioSourceList.Count; i++) {
                m_AudioSourceList[i].volume = volume;
            }
            if (isSave) {
                GameEntry.PlayerPrefs.SetFloat(PlayerPrefsConstKey.SoundVolume, volume);
            }
        }

        /// <summary>
        /// 设置某个音效组件的音量(数据不保存到本地)
        /// </summary>
        /// <param name="volume">音效的音量</param>
        /// <param name="source">要设置音量的音效组件</param>
        public void SetSoundVolume(float volume, AudioSource source) {
            volume = Mathf.Clamp(volume, 0, 1);
            source.volume = volume;
        }

        /// <summary>
        /// 设置背景音乐静音
        /// </summary>
        /// <param name="isMute">是否静音</param>
        public void SetBgmMute(bool isMute = true) {
            m_IsBgmMute = isMute;
            if (isMute) {
                m_BgmSource.volume = 0;
            } else if(m_BgmSource.clip != null) {
                m_BgmSource.volume = m_BgmVolume;
                m_BgmSource.Play();
            }
            GameEntry.PlayerPrefs.SetBool(PlayerPrefsConstKey.BgmMute, isMute);
        }

        /// <summary>
        /// 设置音效静音
        /// </summary>
        /// <param name="isMute">是否静音</param>
        public void SetSoundMute(bool isMute = true) {
            m_IsSoundMute = isMute;
            if (isMute) {
                m_SoundVolume = 0;
            } else {
                for (int i = 0; i < MAX_SOUND_NUM; i++) {
                    var source = m_AudioSourceList[i];
                    if(source.clip != null) {
                        source.volume = m_SoundVolume;
                        source.Play();
                    }
                }
            }
            GameEntry.PlayerPrefs.SetBool(PlayerPrefsConstKey.SoundMute, isMute);
        }

        public void Dispose() {
            for (int i = 0; i < m_AudioSourceList.Count; i++) {
                m_AudioSourceList[i] = null;
            }
            m_AudioSourceList.Clear();
            m_BgmSource = null;
        }

    }
}
