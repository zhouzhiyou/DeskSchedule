namespace DeskSchedule.Services;

/// <summary>
/// 通知服务接口
/// </summary>
public interface INotifyService
{
    /// <summary>
    /// 显示通知
    /// </summary>
    Task ShowNotificationAsync(string title, string message, int durationSeconds = 10);

    /// <summary>
    /// 关闭所有通知
    /// </summary>
    void CloseAllNotifications();
}
