using NAudio.Wave;
using System.IO;
using System.Media;

namespace DeskSchedule.Services;

/// <summary>
/// 音频服务实现
/// </summary>
public class AudioService : IAudioService, IDisposable
{
    private WaveOutEvent? _waveOut;
    private AudioFileReader? _audioFile;
    private bool _disposed;

    public async Task PlaySoundAsync(string? filePath, int volume = 80)
    {
        await Task.Run(() =>
        {
            Stop();

            try
            {
                // 如果没有指定文件或文件不存在，使用系统提示音
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    // 使用系统提示音
                    SystemSounds.Exclamation.Play();
                    return;
                }

                _audioFile = new AudioFileReader(filePath);
                _waveOut = new WaveOutEvent();
                _waveOut.Volume = Math.Clamp(volume / 100f, 0f, 1f);
                _waveOut.Init(_audioFile);
                _waveOut.Play();
            }
            catch (Exception)
            {
                // 如果播放失败，使用系统提示音
                try
                {
                    SystemSounds.Exclamation.Play();
                }
                catch
                {
                    // 忽略所有错误
                }
            }
        });
    }

    public void Stop()
    {
        _waveOut?.Stop();
        _waveOut?.Dispose();
        _audioFile?.Dispose();
        _waveOut = null;
        _audioFile = null;
    }

    public string[] GetBuiltInSounds()
    {
        var soundsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sounds");
        if (!Directory.Exists(soundsPath))
            return Array.Empty<string>();

        return Directory.GetFiles(soundsPath, "*.wav")
            .Concat(Directory.GetFiles(soundsPath, "*.mp3"))
            .ToArray();
    }

    public void Dispose()
    {
        if (_disposed) return;

        Stop();
        _disposed = true;
    }
}
