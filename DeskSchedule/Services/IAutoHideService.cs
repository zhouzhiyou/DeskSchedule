namespace DeskSchedule.Services;

/// <summary>
/// 自动隐藏服务接口
/// </summary>
public interface IAutoHideService
{
    /// <summary>
    /// 开始监测用户活动
    /// </summary>
    void StartMonitoring();

    /// <summary>
    /// 停止监测
    /// </summary>
    void StopMonitoring();

    /// <summary>
    /// 重置活动计时器
    /// </summary>
    void ResetActivity();

    /// <summary>
    /// 设置隐藏延迟时间
    /// </summary>
    void SetHideDelay(int seconds);

    /// <summary>
    /// 窗口隐藏事件
    /// </summary>
    event EventHandler? WindowHidden;
}
