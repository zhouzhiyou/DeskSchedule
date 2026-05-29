using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using DeskSchedule.Data;
using DeskSchedule.Models;
using DeskSchedule.Services;

namespace DeskSchedule.ViewModels;

/// <summary>
/// 日程筛选类型
/// </summary>
public enum ScheduleFilter
{
    All,        // 全部
    Pending,    // 待完成
    Completed   // 已完成
}

/// <summary>
/// 日期分组
/// </summary>
public class DateGroup
{
    public DateTime Date { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public ObservableCollection<Schedule> Schedules { get; } = new();
}

/// <summary>
/// 反向布尔转可见性转换器
/// </summary>
public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && b)
            return Visibility.Collapsed;
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 优先级转颜色转换器
/// </summary>
public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Priority priority)
        {
            return priority switch
            {
                Priority.Low => new SolidColorBrush(Color.FromRgb(76, 175, 80)),      // 绿色
                Priority.Normal => new SolidColorBrush(Color.FromRgb(33, 150, 243)), // 蓝色
                Priority.High => new SolidColorBrush(Color.FromRgb(255, 152, 0)),    // 橙色
                Priority.Urgent => new SolidColorBrush(Color.FromRgb(244, 67, 54)),  // 红色
                _ => new SolidColorBrush(Color.FromRgb(158, 158, 158))
            };
        }
        return new SolidColorBrush(Color.FromRgb(158, 158, 158));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 优先级转文本转换器
/// </summary>
public class PriorityToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Priority priority)
        {
            return priority switch
            {
                Priority.Low => "低",
                Priority.Normal => "普",
                Priority.High => "高",
                Priority.Urgent => "急",
                _ => ""
            };
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 主窗口ViewModel
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IAutoHideService _autoHideService;
    private readonly IMouseTriggerService _mouseTriggerService;
    private readonly IReminderService _reminderService;
    private readonly ISettingsRepository _settingsRepository;
    private readonly AppSettings _settings;

    private bool _isWindowVisible = true;
    private double _windowOpacity = 0.9;
    private string _searchText = string.Empty;
    private Schedule? _selectedSchedule;
    private bool _isAddingNew;
    private ScheduleFilter _currentFilter = ScheduleFilter.All;

    public ObservableCollection<Schedule> Schedules { get; } = new();
    public ObservableCollection<DateGroup> GroupedSchedules { get; } = new();

    public ScheduleFilter CurrentFilter
    {
        get => _currentFilter;
        set
        {
            if (SetProperty(ref _currentFilter, value))
            {
                FilterSchedules();
            }
        }
    }

    public bool IsWindowVisible
    {
        get => _isWindowVisible;
        set => SetProperty(ref _isWindowVisible, value);
    }

    public double WindowOpacity
    {
        get => _windowOpacity;
        set
        {
            if (SetProperty(ref _windowOpacity, value))
            {
                _settings.WindowOpacity = value;
                _ = SaveSettingsAsync();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                FilterSchedules();
            }
        }
    }

    public Schedule? SelectedSchedule
    {
        get => _selectedSchedule;
        set => SetProperty(ref _selectedSchedule, value);
    }

    public bool IsAddingNew
    {
        get => _isAddingNew;
        set => SetProperty(ref _isAddingNew, value);
    }

    public RelayCommand AddScheduleCommand { get; }
    public RelayCommand<Schedule> EditScheduleCommand { get; }
    public RelayCommand<Schedule> DeleteScheduleCommand { get; }
    public RelayCommand<Schedule> CompleteScheduleCommand { get; }
    public RelayCommand<Schedule> UncompleteScheduleCommand { get; }
    public RelayCommand<Schedule> ToggleCompleteCommand { get; }
    public RelayCommand OpenSettingsCommand { get; }
    public RelayCommand RefreshCommand { get; }
    public RelayCommand SetFilterAllCommand { get; }
    public RelayCommand SetFilterPendingCommand { get; }
    public RelayCommand SetFilterCompletedCommand { get; }

    public event EventHandler? OpenSettingsRequested;
    public event EventHandler<Schedule>? EditScheduleRequested;
    public event EventHandler<CornerType>? TriggerCornerChanged;

    public MainViewModel(
        IScheduleService scheduleService,
        IAutoHideService autoHideService,
        IMouseTriggerService mouseTriggerService,
        IReminderService reminderService,
        ISettingsRepository settingsRepository,
        AppSettings settings)
    {
        _scheduleService = scheduleService;
        _autoHideService = autoHideService;
        _mouseTriggerService = mouseTriggerService;
        _reminderService = reminderService;
        _settingsRepository = settingsRepository;
        _settings = settings;

        _windowOpacity = settings.WindowOpacity;

        // 初始化命令
        AddScheduleCommand = new RelayCommand(_ => OpenAddSchedule());
        EditScheduleCommand = new RelayCommand<Schedule>(s => EditSchedule(s), s => s != null);
        DeleteScheduleCommand = new RelayCommand<Schedule>(s => DeleteSchedule(s), s => s != null);
        CompleteScheduleCommand = new RelayCommand<Schedule>(s => ToggleScheduleComplete(s, true), s => s != null);
        UncompleteScheduleCommand = new RelayCommand<Schedule>(s => ToggleScheduleComplete(s, false), s => s != null);
        ToggleCompleteCommand = new RelayCommand<Schedule>(s => ToggleScheduleComplete(s, !s.IsCompleted), s => s != null);
        OpenSettingsCommand = new RelayCommand(_ => OpenSettingsRequested?.Invoke(this, EventArgs.Empty));
        RefreshCommand = new RelayCommand(_ => _ = LoadSchedulesAsync());
        SetFilterAllCommand = new RelayCommand(_ => CurrentFilter = ScheduleFilter.All);
        SetFilterPendingCommand = new RelayCommand(_ => CurrentFilter = ScheduleFilter.Pending);
        SetFilterCompletedCommand = new RelayCommand(_ => CurrentFilter = ScheduleFilter.Completed);

        // 订阅服务事件
        _autoHideService.WindowHidden += OnWindowHidden;
        _mouseTriggerService.WindowShowRequested += OnWindowShowRequested;

        // 启动服务
        _autoHideService.StartMonitoring();
        _mouseTriggerService.StartMonitoring();

        // 加载数据
        _ = LoadSchedulesAsync();
    }

    public async Task LoadSchedulesAsync()
    {
        Schedules.Clear();
        var schedules = await _scheduleService.GetAllAsync();
        foreach (var schedule in schedules)
        {
            Schedules.Add(schedule);
        }
        FilterSchedules();
    }

    private void FilterSchedules()
    {
        GroupedSchedules.Clear();

        // 先按筛选条件过滤
        var filtered = CurrentFilter switch
        {
            ScheduleFilter.Pending => Schedules.Where(s => !s.IsCompleted),
            ScheduleFilter.Completed => Schedules.Where(s => s.IsCompleted),
            _ => Schedules
        };

        // 再按搜索文本过滤
        if (!string.IsNullOrEmpty(SearchText))
        {
            filtered = filtered.Where(s =>
                s.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (s.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        // 按日期分组
        var grouped = filtered
            .OrderBy(s => s.RemindTime)
            .GroupBy(s => s.RemindTime.Date)
            .OrderBy(g => g.Key);

        foreach (var group in grouped)
        {
            var dateGroup = new DateGroup
            {
                Date = group.Key,
                DisplayText = GetDateDisplayText(group.Key)
            };

            // 按优先级降序（紧急优先），然后按时间排序
            foreach (var schedule in group.OrderByDescending(s => s.Priority).ThenBy(s => s.RemindTime))
            {
                dateGroup.Schedules.Add(schedule);
            }

            GroupedSchedules.Add(dateGroup);
        }
    }

    private string GetDateDisplayText(DateTime date)
    {
        if (date == DateTime.Today)
            return "今天";
        if (date == DateTime.Today.AddDays(1))
            return "明天";
        if (date == DateTime.Today.AddDays(-1))
            return "昨天";
        if (date.Year == DateTime.Today.Year)
            return date.ToString("MM月dd日 dddd");
        return date.ToString("yyyy年MM月dd日 dddd");
    }

    private void OpenAddSchedule()
    {
        IsAddingNew = true;
        SelectedSchedule = new Schedule
        {
            RemindTime = DateTime.Now.AddHours(1),
            RemindType = RemindType.Popup
        };
        EditScheduleRequested?.Invoke(this, SelectedSchedule);
    }

    private void EditSchedule(Schedule? schedule)
    {
        if (schedule == null) return;
        IsAddingNew = false;
        SelectedSchedule = schedule;
        EditScheduleRequested?.Invoke(this, schedule);
    }

    private async void DeleteSchedule(Schedule? schedule)
    {
        if (schedule == null) return;

        var result = MessageBox.Show(
            $"确定要删除日程「{schedule.Title}」吗？",
            "确认删除",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            await _scheduleService.DeleteAsync(schedule.Id);
            Schedules.Remove(schedule);
            FilterSchedules();
        }
    }

    private async void ToggleScheduleComplete(Schedule? schedule, bool completed)
    {
        if (schedule == null) return;

        // 停止当前提醒
        _reminderService.StopCurrentReminder();

        if (completed)
        {
            await _scheduleService.MarkCompletedAsync(schedule.Id);
            schedule.IsCompleted = true;
        }
        else
        {
            await _scheduleService.MarkUncompletedAsync(schedule.Id);
            schedule.IsCompleted = false;
        }
        FilterSchedules();
    }

    private void OnWindowHidden(object? sender, EventArgs e)
    {
        IsWindowVisible = false;
    }

    private void OnWindowShowRequested(object? sender, CornerType corner)
    {
        IsWindowVisible = true;
        _autoHideService.ResetActivity();
        TriggerCornerChanged?.Invoke(this, corner);
    }

    public async Task SaveScheduleAsync(Schedule schedule)
    {
        if (IsAddingNew)
        {
            var added = await _scheduleService.AddAsync(schedule);
            Schedules.Add(added);
        }
        else if (schedule.Id > 0)
        {
            await _scheduleService.UpdateAsync(schedule);
        }
        FilterSchedules();
    }

    private async Task SaveSettingsAsync()
    {
        await _settingsRepository.SaveAsync(_settings);
    }

    public void ResetActivity()
    {
        _autoHideService.ResetActivity();
    }

    public void ShowWindow()
    {
        IsWindowVisible = true;
        _autoHideService.ResetActivity();
    }

    public void HideWindow()
    {
        IsWindowVisible = false;
    }
}
