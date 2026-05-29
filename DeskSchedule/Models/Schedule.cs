using System.ComponentModel.DataAnnotations;

namespace DeskSchedule.Models;

/// <summary>
/// 提醒类型枚举
/// </summary>
[Flags]
public enum RemindType
{
    None = 0,
    Popup = 1,
    Sound = 2,
    Both = Popup | Sound
}

/// <summary>
/// 日程优先级
/// </summary>
public enum Priority
{
    Low = 0,      // 低
    Normal = 1,   // 普通
    High = 2,     // 高
    Urgent = 3    // 紧急
}

/// <summary>
/// 日程实体
/// </summary>
public class Schedule
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime RemindTime { get; set; }

    public RemindType RemindType { get; set; } = RemindType.Popup;

    public string? RemindSoundPath { get; set; }

    public string? RemindText { get; set; }

    public bool IsCompleted { get; set; }

    public Priority Priority { get; set; } = Priority.Normal;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 是否已过期
    /// </summary>
    public bool IsExpired => RemindTime < DateTime.Now && !IsCompleted;

    /// <summary>
    /// 是否今天
    /// </summary>
    public bool IsToday => RemindTime.Date == DateTime.Today;

    /// <summary>
    /// 是否明天
    /// </summary>
    public bool IsTomorrow => RemindTime.Date == DateTime.Today.AddDays(1);

    /// <summary>
    /// 是否启用弹窗提醒
    /// </summary>
    public bool IsPopupReminder => RemindType.HasFlag(RemindType.Popup);

    /// <summary>
    /// 是否启用声音提醒
    /// </summary>
    public bool IsSoundReminder => RemindType.HasFlag(RemindType.Sound);
}
