namespace DeskSchedule.Services;

/// <summary>
/// 音频服务接口
/// </summary>
public interface IAudioService
{
    /// <summary>
    /// 播放声音
    /// </summary>
    Task PlaySoundAsync(string? filePath, int volume = 80);

    /// <summary>
    /// 停止播放
    /// </summary>
    void Stop();

    /// <summary>
    /// 获取内置声音列表
    /// </summary>
    string[] GetBuiltInSounds();
}
