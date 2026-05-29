namespace DeskSchedule.Models;

/// <summary>
/// 应用配置
/// </summary>
public class AppSettings
{
    // 首次运行标记
    public bool IsFirstRun { get; set; } = true;

    // 自动隐藏设置
    public int HideDelaySeconds { get; set; } = 60;

    // 鼠标触发设置
    public int TriggerAreaSize { get; set; } = 50;
    public int TriggerDelayMs { get; set; } = 500;

    // 窗口设置
    public double WindowOpacity { get; set; } = 0.9;
    public bool WindowAlwaysOnTop { get; set; } = true;
    public double WindowWidth { get; set; } = 350;
    public double WindowHeight { get; set; } = 500;

    // 提醒设置
    public bool ReminderPopupEnabled { get; set; } = true;
    public bool ReminderSoundEnabled { get; set; } = true;
    public int ReminderVolume { get; set; } = 80;
    public string ReminderSoundPath { get; set; } = string.Empty;

    // 弹窗模板
    public string PopupTitleTemplate { get; set; } = "日程提醒";
    public string PopupContentTemplate { get; set; } = "{title} - {time}";
    public int PopupDurationSeconds { get; set; } = 10;

    // 窗口位置
    public double WindowLeft { get; set; }
    public double WindowTop { get; set; }
    public CornerType LastShowCorner { get; set; } = CornerType.Right;

    /// <summary>
    /// 获取默认设置
    /// </summary>
    public static AppSettings GetDefault() => new();
}

/// <summary>
/// 角落类型
/// </summary>
public enum CornerType
{
    Left,
    Right
}
