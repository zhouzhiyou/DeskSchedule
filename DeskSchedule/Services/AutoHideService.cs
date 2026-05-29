using System.Windows.Input;
using System.Windows.Threading;

namespace DeskSchedule.Services;

/// <summary>
/// 自动隐藏服务实现
/// </summary>
public class AutoHideService : IAutoHideService, IDisposable
{
    private readonly DispatcherTimer _idleTimer;
    private DateTime _lastActivityTime;
    private int _hideDelaySeconds = 10;
    private bool _isMonitoring;
    private bool _disposed;

    public event EventHandler? WindowHidden;

    public AutoHideService()
    {
        _idleTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _idleTimer.Tick += CheckIdleTime;
        _lastActivityTime = DateTime.Now;
    }

    public void StartMonitoring()
    {
        if (_isMonitoring) return;

        _isMonitoring = true;
        _lastActivityTime = DateTime.Now;

        // 订阅全局输入事件
        InputManager.Current.PreProcessInput += OnInputEvent;

        _idleTimer.Start();
    }

    public void StopMonitoring()
    {
        if (!_isMonitoring) return;

        _isMonitoring = false;
        InputManager.Current.PreProcessInput -= OnInputEvent;
        _idleTimer.Stop();
    }

    public void ResetActivity()
    {
        _lastActivityTime = DateTime.Now;
    }

    public void SetHideDelay(int seconds)
    {
        _hideDelaySeconds = Math.Max(10, seconds);
    }

    private void OnInputEvent(object sender, PreProcessInputEventArgs e)
    {
        _lastActivityTime = DateTime.Now;
    }

    private void CheckIdleTime(object? sender, EventArgs e)
    {
        var idleTime = (DateTime.Now - _lastActivityTime).TotalSeconds;
        if (idleTime >= _hideDelaySeconds)
        {
            WindowHidden?.Invoke(this, EventArgs.Empty);
            // 重置活动时间，避免重复触发
            _lastActivityTime = DateTime.Now;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        StopMonitoring();
        _idleTimer.Tick -= CheckIdleTime;
        _disposed = true;
    }
}
