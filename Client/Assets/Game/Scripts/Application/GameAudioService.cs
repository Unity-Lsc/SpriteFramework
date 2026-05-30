using SpriteFramework;
using UnityEngine;

/// <summary>
/// 业务音频服务
/// 把音频表中的逻辑名称转换成框架音频管理器需要的资源路径和播放参数
/// </summary>
public sealed class GameAudioService
{
    /// <summary>
    /// 按业务音频名播放 BGM。
    /// </summary>
    public void PlayBGM(string audioName)
    {
        Sys_BGMEntity entity = GameEntry.DataTable.Get<Sys_BGMDBModel>().GetEntity(audioName);
        if (entity == null)
        {
            GameEntry.LogError(ELogCategory.Audio, $"BGM config not found, audioName={audioName}");
            return;
        }

        GameEntry.Audio.PlayBGM(entity.AssetFullPath, new BGMPlayOptions
        {
            Volume = entity.Volume,
            Priority = entity.Priority,
            IsLoop = entity.IsLoop == 1,
            IsFadeIn = entity.IsFadeIn == 1,
            IsFadeOut = entity.IsFadeOut == 1
        });
    }

    /// <summary>
    /// 按业务音频名播放 2D 音效。
    /// </summary>
    public void PlayAudio(string audioName)
    {
        Sys_AudioEntity entity = GameEntry.DataTable.Get<Sys_AudioDBModel>().GetEntity(audioName);
        if (entity == null)
        {
            GameEntry.LogError(ELogCategory.Audio, $"Audio config not found, audioName={audioName}");
            return;
        }

        GameEntry.Audio.PlayAudio(entity.AssetFullPath, new AudioPlayOptions
        {
            Volume = entity.Volume,
            Priority = entity.Priority
        });
    }

    /// <summary>
    /// 按业务音频名播放 3D 音效。
    /// </summary>
    public void PlayAudio(string audioName, Vector3 point)
    {
        Sys_AudioEntity entity = GameEntry.DataTable.Get<Sys_AudioDBModel>().GetEntity(audioName);
        if (entity == null)
        {
            GameEntry.LogError(ELogCategory.Audio, $"Audio config not found, audioName={audioName}");
            return;
        }

        GameEntry.Audio.PlayAudio(entity.AssetFullPath, point, new AudioPlayOptions
        {
            Volume = entity.Volume,
            Priority = entity.Priority
        });
    }
}
