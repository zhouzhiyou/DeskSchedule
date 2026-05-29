using DeskSchedule.Models;

namespace DeskSchedule.Services;

/// <summary>
/// 提醒服务接口
/// </summary>
public interface IReminderService
{
    /// <summary>
    /// 启动提醒调度
    /// </summary>
    void Start();

    /// <summary>
    /// 停止提醒调度
    /// </summary>
    void Stop();

    /// <summary>
    /// 立即触发提醒
    /// </summary>
    Task TriggerReminderAsync(Schedule schedule);

    /// <summary>
    /// 停止当前提醒（关闭弹窗和声音）
    /// </summary>
    void StopCurrentReminder();

    /// <summary>
    /// 提醒触发事件
    /// </summary>
    event EventHandler<Schedule>? ReminderTriggered;
}
