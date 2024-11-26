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
        private List<AudioSource> _audioSourceList;

        /// <summary>
        /// 背景音乐组件
        /// </summary>
        private AudioSource _bgmSource;

        //当前正在播放的音效AudioSource的索引
        private int _curIndex;

        //背景音乐是否静音
        private bool _isBgmMute;
        //音效是否静音
        private bool _isSoundMute;
        //背景音乐音量
        private float _bgmVolume;
        //音效音量
        private float _soundVolume;

        //是否淡出
        private bool _isFadeOut = false;
        //是否淡入
        private bool _isFadeIn = true;
        //BGM淡入淡出的过渡音量
        private float _interimBgmVolume;
        //BGM的目标音量
        private float _targetBgmVolume;

        public AudioManager() {
            _audioSourceList = new List<AudioSource>(MAX_SOUND_NUM);

            var obj = new GameObject("AudioManager");
            obj.transform.SetParent(GameEntry.Instance.transform);
            for (int i = 0; i < MAX_SOUND_NUM; i++) {
                var source = obj.AddComponent<AudioSource>();
                _audioSourceList.Add(source);
            }
            _bgmSource = obj.AddComponent<AudioSource>();
            _curIndex = 0;
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
            _isBgmMute = GameEntry.PlayerPrefs.GetBool(PlayerPrefsConstKey.BgmMute);
            _isSoundMute = GameEntry.PlayerPrefs.GetBool(PlayerPrefsConstKey.SoundMute);
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="soundName">背景音乐的名字</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <param name="volume">背景音乐的音量</param>
        /// <param name="isFadeIn">是否淡入</param>
        public void PlayBgm(string soundName, bool isLoop = true, float volume = 1.0f, bool isFadeIn = true) {
            AudioClip clip = GameEntry.Resource.LoadBgm(soundName);
            if(clip == null) {
                GameEntry.LogError("背景音乐:{0} 为null,请检查路径", soundName);
                return;
            }
            _bgmSource.clip = clip;
            _bgmSource.loop = isLoop;

            _targetBgmVolume = volume;
            this._isFadeIn = isFadeIn;
            _interimBgmVolume = 0;

            SetBGMVolume(volume);
            if (isFadeIn) _bgmSource.volume = 0;

            if (!_isBgmMute) {
                _bgmSource.Play();
            }
        }

        /// <summary>
        /// 暂停和重新播放背景音乐
        /// </summary>
        /// <param name="isPause">是否暂停(true表示暂停,false表示重新播放)</param>
        public void PauseBgm(bool isPause = true) {
            if (isPause) {
                _bgmSource.Pause();
            } else {
                _bgmSource.UnPause();
            }
        }

        /// <summary>
        /// 关闭背景音乐
        /// </summary>
        /// <param name="isFadeOut">是否淡出</param>
        public void StopBgm(bool isFadeOut = true) {
            this._isFadeOut = isFadeOut;
            if (!isFadeOut) {
                _bgmSource.Stop();
                _bgmSource.clip = null;
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="soundName">音效的名字</param>
        /// <param name="isLoop">是否循环播放</param>
        public int PlaySound(string soundName, bool isLoop = false, float volume = 1.0f) {
            AudioClip clip = GameEntry.Resource.LoadSound(soundName);
            if (clip == null) {
                GameEntry.LogError("音效:{0} 为null,请检查路径", soundName);
                return -1;
            }
            return PlaySound(clip, isLoop, volume);
        }

        public int PlaySound(AudioClip clip, bool isLoop = false, float volume = 1.0f) {
            int soundId = _curIndex;
            AudioSource source = _audioSourceList[_curIndex];
            _curIndex++;
            _curIndex = _curIndex >= MAX_SOUND_NUM ? 0 : _curIndex;

            source.clip = clip;
            source.loop = isLoop;
            SetSoundVolume(volume, source);

            if (_isSoundMute) {
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
            AudioClip clip = GameEntry.Resource.LoadSound(soundName);
            if (clip == null) {
                GameEntry.LogError("音效:{0} 为null,请检查路径");
                return -1;
            }
            int soundId = _curIndex;
            AudioSource source = _audioSourceList[_curIndex];
            _curIndex++;
            _curIndex = _curIndex >= MAX_SOUND_NUM ? 0 : _curIndex;

            source.clip = clip;
            source.loop = isLoop;
            SetSoundVolume(volume, source);

            if (_isSoundMute) {
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
            AudioSource source = _audioSourceList[soundId];
            source.Stop();
            source.clip = null;
        }

        /// <summary>
        /// 关闭全部音效组件
        /// </summary>
        public void StopAllSound() {
            for (int i = 0; i < MAX_SOUND_NUM; i++) {
                var source = _audioSourceList[i];
                source.Stop();
                source.clip = null;
            }
        }


        public void OnUpdate() {

            if (_isFadeOut) {
                if(_interimBgmVolume > 0) {
                    GameEntry.Log("淡出*******************");
                    _interimBgmVolume -= Time.unscaledDeltaTime * 2;//0.5秒淡出时间
                    SetBGMVolume(_interimBgmVolume, false);
                } else {
                    GameEntry.Log("淡出结束*******************");
                    _interimBgmVolume = 0;
                    _bgmSource.Stop();
                    SetBGMVolume(_interimBgmVolume, false);
                    _bgmSource.clip = null;
                    _isFadeOut = false;
                }
            }

            if (_isFadeIn) {
                if(_interimBgmVolume < _targetBgmVolume) {
                    GameEntry.Log("淡入*******************");
                    _interimBgmVolume += Time.unscaledDeltaTime * 2;//0.5秒淡入时间
                    SetBGMVolume(_interimBgmVolume, false);
                }else if(_interimBgmVolume != _targetBgmVolume) {
                    GameEntry.Log("淡入结束*******************");
                    _interimBgmVolume = _targetBgmVolume;
                    SetBGMVolume(_targetBgmVolume, false);
                    _isFadeIn = false;
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
            _bgmSource.volume = volume;
            _bgmVolume = volume;
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
            _soundVolume = volume;
            for (int i = 0; i < _audioSourceList.Count; i++) {
                _audioSourceList[i].volume = volume;
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
            _isBgmMute = isMute;
            if (isMute) {
                _bgmSource.volume = 0;
            } else if(_bgmSource.clip != null) {
                _bgmSource.volume = _bgmVolume;
                _bgmSource.Play();
            }
            GameEntry.PlayerPrefs.SetBool(PlayerPrefsConstKey.BgmMute, isMute);
        }

        /// <summary>
        /// 设置音效静音
        /// </summary>
        /// <param name="isMute">是否静音</param>
        public void SetSoundMute(bool isMute = true) {
            _isSoundMute = isMute;
            if (isMute) {
                _soundVolume = 0;
            } else {
                for (int i = 0; i < MAX_SOUND_NUM; i++) {
                    var source = _audioSourceList[i];
                    if(source.clip != null) {
                        source.volume = _soundVolume;
                        source.Play();
                    }
                }
            }
            GameEntry.PlayerPrefs.SetBool(PlayerPrefsConstKey.SoundMute, isMute);
        }

        public void Dispose() {
            for (int i = 0; i < _audioSourceList.Count; i++) {
                _audioSourceList[i] = null;
            }
            _audioSourceList.Clear();
            _bgmSource = null;
        }

    }
}
