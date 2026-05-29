using DeskSchedule.Models;
using Newtonsoft.Json;
using System.IO;

namespace DeskSchedule.Data;

/// <summary>
/// 配置仓储实现
/// </summary>
public class SettingsRepository : ISettingsRepository
{
    private readonly string _settingsPath;
    private AppSettings? _cachedSettings;

    public SettingsRepository()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "DeskSchedule");
        Directory.CreateDirectory(appFolder);
        _settingsPath = Path.Combine(appFolder, "settings.json");
    }

    public async Task<AppSettings> LoadAsync()
    {
        if (_cachedSettings != null)
            return _cachedSettings;

        if (!File.Exists(_settingsPath))
        {
            _cachedSettings = AppSettings.GetDefault();
            return _cachedSettings;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_settingsPath);
            _cachedSettings = JsonConvert.DeserializeObject<AppSettings>(json)
                ?? AppSettings.GetDefault();
            return _cachedSettings;
        }
        catch
        {
            _cachedSettings = AppSettings.GetDefault();
            return _cachedSettings;
        }
    }

    public async Task SaveAsync(AppSettings settings)
    {
        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        await File.WriteAllTextAsync(_settingsPath, json);
        _cachedSettings = settings;
    }

    public void ResetToDefault()
    {
        _cachedSettings = AppSettings.GetDefault();
        if (File.Exists(_settingsPath))
        {
            File.Delete(_settingsPath);
        }
    }
}
