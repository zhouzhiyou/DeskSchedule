using DeskSchedule.Models;

namespace DeskSchedule.Helpers;

/// <summary>
/// 模板辅助类
/// </summary>
public static class TemplateHelper
{
    /// <summary>
    /// 替换模板变量
    /// </summary>
    public static string ReplaceVariables(string template, Schedule schedule)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        return template
            .Replace("{title}", schedule.Title ?? "")
            .Replace("{time}", schedule.RemindTime.ToString("HH:mm"))
            .Replace("{date}", schedule.RemindTime.ToString("yyyy-MM-dd"))
            .Replace("{datetime}", schedule.RemindTime.ToString("yyyy-MM-dd HH:mm"))
            .Replace("{description}", schedule.Description ?? "");
    }

    /// <summary>
    /// 获取可用变量列表
    /// </summary>
    public static string[] GetAvailableVariables()
    {
        return new[]
        {
            "{title} - 日程标题",
            "{time} - 提醒时间",
            "{date} - 提醒日期",
            "{datetime} - 提醒日期时间",
            "{description} - 日程描述"
        };
    }
}
