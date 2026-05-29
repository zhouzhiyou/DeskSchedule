using System.Windows.Threading;
using DeskSchedule.Data;
using DeskSchedule.Models;

namespace DeskSchedule.Services;

/// <summary>
/// 提醒服务实现
/// </summary>
public class ReminderService : IReminderService, IDisposable
{
    private readonly DispatcherTimer _checkTimer;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly INotifyService _notifyService;
    private readonly IAudioService _audioService;
    private readonly AppSettings _settings;
    private readonly HashSet<int> _triggeredIds = new();
    private bool _disposed;

    public event EventHandler<Schedule>? ReminderTriggered;

    public ReminderService(
        IScheduleRepository scheduleRepository,
        INotifyService notifyService,
        IAudioService audioService,
        AppSettings settings)
    {
        _scheduleRepository = scheduleRepository;
        _notifyService = notifyService;
        _audioService = audioService;
        _settings = settings;

        _checkTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _checkTimer.Tick += CheckSchedules;
    }

    public void Start()
    {
        _checkTimer.Start();
    }

    public void Stop()
    {
        _checkTimer.Stop();
    }

    public void StopCurrentReminder()
    {
        _audioService.Stop();
        _notifyService.CloseAllNotifications();
    }

    public async Task TriggerReminderAsync(Schedule schedule)
    {
        var tasks = new List<Task>();

        if (schedule.RemindType.HasFlag(RemindType.Popup) && _settings.ReminderPopupEnabled)
        {
            var title = ReplaceTemplate(_settings.PopupTitleTemplate, schedule);
            var content = ReplaceTemplate(_settings.PopupContentTemplate, schedule);
            tasks.Add(_notifyService.ShowNotificationAsync(title, content, _settings.PopupDurationSeconds));
        }

        if (schedule.RemindType.HasFlag(RemindType.Sound) && _settings.ReminderSoundEnabled)
        {
            var soundPath = schedule.RemindSoundPath ?? _settings.ReminderSoundPath;
            tasks.Add(_audioService.PlaySoundAsync(soundPath, _settings.ReminderVolume));
        }

        await Task.WhenAll(tasks);
        ReminderTriggered?.Invoke(this, schedule);
    }

    private async void CheckSchedules(object? sender, EventArgs e)
    {
        try
        {
            var now = DateTime.Now;
            // 获取所有未完成的日程
            var schedules = await _scheduleRepository.GetAllAsync();

            foreach (var schedule in schedules)
            {
                // 跳过已完成的日程
                if (schedule.IsCompleted)
                    continue;

                // 跳过已触发的日程
                if (_triggeredIds.Contains(schedule.Id))
                    continue;

                // 检查是否到达提醒时间（允许60秒的误差）
                var diff = (schedule.RemindTime - now).TotalSeconds;
                if (diff <= 0 && diff > -60)
                {
                    _triggeredIds.Add(schedule.Id);
                    // 确保在UI线程上触发提醒
                    System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _ = TriggerReminderAsync(schedule);
                    });
                }
            }
        }
        catch (Exception)
        {
            // 忽略检查错误
        }
    }

    private string ReplaceTemplate(string template, Schedule schedule)
    {
        return template
            .Replace("{title}", schedule.Title)
            .Replace("{time}", schedule.RemindTime.ToString("HH:mm"))
            .Replace("{date}", schedule.RemindTime.ToString("yyyy-MM-dd"))
            .Replace("{description}", schedule.Description ?? "");
    }

    public void Dispose()
    {
        if (_disposed) return;

        Stop();
        _checkTimer.Tick -= CheckSchedules;
        _disposed = true;
    }
}
