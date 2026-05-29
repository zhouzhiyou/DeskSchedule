using DeskSchedule.Models;

namespace DeskSchedule.Data;

/// <summary>
/// 配置仓储接口
/// </summary>
public interface ISettingsRepository
{
    Task<AppSettings> LoadAsync();
    Task SaveAsync(AppSettings settings);
    void ResetToDefault();
}
