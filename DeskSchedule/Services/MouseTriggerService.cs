using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using DeskSchedule.Models;

namespace DeskSchedule.Services;

/// <summary>
/// 鼠标触发服务实现
/// </summary>
public class MouseTriggerService : IMouseTriggerService, IDisposable
{
    private readonly DispatcherTimer _checkTimer;
    private int _triggerAreaSize = 50;
    private int _triggerDelayMs = 500;
    private DateTime? _enterTime;
    private bool _isMonitoring;
    private bool _disposed;

    public event EventHandler<CornerType>? WindowShowRequested;

    public MouseTriggerService()
    {
        _checkTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _checkTimer.Tick += CheckMousePosition;
    }

    public void StartMonitoring()
    {
        if (_isMonitoring) return;

        _isMonitoring = true;
        _checkTimer.Start();
    }

    public void StopMonitoring()
    {
        if (!_isMonitoring) return;

        _isMonitoring = false;
        _checkTimer.Stop();
        _enterTime = null;
    }

    public void SetTriggerAreaSize(int pixels)
    {
        _triggerAreaSize = Math.Max(20, pixels);
    }

    public void SetTriggerDelay(int milliseconds)
    {
        _triggerDelayMs = Math.Max(100, milliseconds);
    }

    private void CheckMousePosition(object? sender, EventArgs e)
    {
        var position = GetMousePosition();
        var screenSize = GetScreenSize();

        bool inTopLeft = position.X < _triggerAreaSize && position.Y < _triggerAreaSize;
        bool inTopRight = position.X > screenSize.Width - _triggerAreaSize && position.Y < _triggerAreaSize;

        if (inTopLeft || inTopRight)
        {
            if (_enterTime == null)
            {
                _enterTime = DateTime.Now;
            }
            else if ((DateTime.Now - _enterTime.Value).TotalMilliseconds >= _triggerDelayMs)
            {
                WindowShowRequested?.Invoke(this, inTopLeft ? CornerType.Left : CornerType.Right);
                _enterTime = null;
            }
        }
        else
        {
            _enterTime = null;
        }
    }

    private Point GetMousePosition()
    {
        GetCursorPos(out var point);
        return point;
    }

    private Size GetScreenSize()
    {
        return new Size(
            (int)System.Windows.SystemParameters.PrimaryScreenWidth,
            (int)System.Windows.SystemParameters.PrimaryScreenHeight);
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X;
        public int Y;
    }

    public void Dispose()
    {
        if (_disposed) return;

        StopMonitoring();
        _checkTimer.Tick -= CheckMousePosition;
        _disposed = true;
    }
}
