using DeskSchedule.Models;

namespace DeskSchedule.Services;

/// <summary>
/// 鼠标触发服务接口
/// </summary>
public interface IMouseTriggerService
{
    /// <summary>
    /// 开始监测鼠标位置
    /// </summary>
    void StartMonitoring();

    /// <summary>
    /// 停止监测
    /// </summary>
    void StopMonitoring();

    /// <summary>
    /// 设置触发区域大小
    /// </summary>
    void SetTriggerAreaSize(int pixels);

    /// <summary>
    /// 设置触发延迟
    /// </summary>
    void SetTriggerDelay(int milliseconds);

    /// <summary>
    /// 窗口显示事件
    /// </summary>
    event EventHandler<CornerType>? WindowShowRequested;
}
