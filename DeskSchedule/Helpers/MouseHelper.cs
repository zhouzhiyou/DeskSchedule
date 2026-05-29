using System.Drawing;
using System.Runtime.InteropServices;

namespace DeskSchedule.Helpers;

/// <summary>
/// 鼠标辅助类
/// </summary>
public static class MouseHelper
{
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// 获取鼠标位置
    /// </summary>
    public static Point GetPosition()
    {
        GetCursorPos(out var point);
        return new Point(point.X, point.Y);
    }

    /// <summary>
    /// 设置鼠标位置
    /// </summary>
    public static void SetPosition(int x, int y)
    {
        SetCursorPos(x, y);
    }
}
