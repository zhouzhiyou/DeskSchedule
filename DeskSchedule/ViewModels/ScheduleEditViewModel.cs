using DeskSchedule.Models;

namespace DeskSchedule.ViewModels;

/// <summary>
/// 日程编辑ViewModel
/// </summary>
public class ScheduleEditViewModel : ViewModelBase
{
    private Schedule _schedule;
    private bool _isNew;

    public Schedule Schedule
    {
        get => _schedule;
        set => SetProperty(ref _schedule, value);
    }

    public bool IsNew
    {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }

    /// <summary>
    /// 小时选项列表
    /// </summary>
    public List<string> HourOptions { get; } = Enumerable.Range(0, 24)
        .Select(h => h.ToString("D2"))
        .ToList();

    /// <summary>
    /// 分钟选项列表
    /// </summary>
    public List<string> MinuteOptions { get; } = Enumerable.Range(0, 60)
        .Select(m => m.ToString("D2"))
        .ToList();

    // 标题
    public string Title
    {
        get => _schedule.Title;
        set
        {
            _schedule.Title = value;
            OnPropertyChanged();
        }
    }

    // 描述
    public string? Description
    {
        get => _schedule.Description;
        set
        {
            _schedule.Description = value;
            OnPropertyChanged();
        }
    }

    // 提醒时间
    public DateTime RemindDate
    {
        get => _schedule.RemindTime.Date;
        set
        {
            _schedule.RemindTime = value.Add(_schedule.RemindTime.TimeOfDay);
            OnPropertyChanged();
        }
    }

    public TimeSpan RemindTime
    {
        get => _schedule.RemindTime.TimeOfDay;
        set
        {
            _schedule.RemindTime = _schedule.RemindTime.Date.Add(value);
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 选中的小时
    /// </summary>
    public string SelectedHour
    {
        get => _schedule.RemindTime.Hour.ToString("D2");
        set
        {
            if (int.TryParse(value, out var hour))
            {
                _schedule.RemindTime = _schedule.RemindTime.Date.AddHours(hour).AddMinutes(_schedule.RemindTime.Minute);
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 选中的分钟
    /// </summary>
    public string SelectedMinute
    {
        get => _schedule.RemindTime.Minute.ToString("D2");
        set
        {
            if (int.TryParse(value, out var minute))
            {
                _schedule.RemindTime = _schedule.RemindTime.Date.AddHours(_schedule.RemindTime.Hour).AddMinutes(minute);
                OnPropertyChanged();
            }
        }
    }

    // 提醒类型
    public bool IsPopupReminder
    {
        get => _schedule.RemindType.HasFlag(RemindType.Popup);
        set
        {
            if (value)
                _schedule.RemindType |= RemindType.Popup;
            else
                _schedule.RemindType &= ~RemindType.Popup;
            OnPropertyChanged();
        }
    }

    public bool IsSoundReminder
    {
        get => _schedule.RemindType.HasFlag(RemindType.Sound);
        set
        {
            if (value)
                _schedule.RemindType |= RemindType.Sound;
            else
                _schedule.RemindType &= ~RemindType.Sound;
            OnPropertyChanged();
        }
    }

    // 自定义提醒文本
    public string? RemindText
    {
        get => _schedule.RemindText;
        set
        {
            _schedule.RemindText = value;
            OnPropertyChanged();
        }
    }

    // 自定义声音路径
    public string? RemindSoundPath
    {
        get => _schedule.RemindSoundPath;
        set
        {
            _schedule.RemindSoundPath = value;
            OnPropertyChanged();
        }
    }

    // 优先级索引
    public int PriorityIndex
    {
        get => (int)_schedule.Priority;
        set
        {
            _schedule.Priority = (Priority)value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPriorityLow));
            OnPropertyChanged(nameof(IsPriorityNormal));
            OnPropertyChanged(nameof(IsPriorityHigh));
            OnPropertyChanged(nameof(IsPriorityUrgent));
        }
    }

    // 优先级单选按钮绑定
    public bool IsPriorityLow
    {
        get => _schedule.Priority == Priority.Low;
        set { if (value) PriorityIndex = 0; }
    }

    public bool IsPriorityNormal
    {
        get => _schedule.Priority == Priority.Normal;
        set { if (value) PriorityIndex = 1; }
    }

    public bool IsPriorityHigh
    {
        get => _schedule.Priority == Priority.High;
        set { if (value) PriorityIndex = 2; }
    }

    public bool IsPriorityUrgent
    {
        get => _schedule.Priority == Priority.Urgent;
        set { if (value) PriorityIndex = 3; }
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand BrowseSoundCommand { get; }
    public RelayCommand<string> SetPriorityCommand { get; }

    public event EventHandler? SaveRequested;
    public event EventHandler? CancelRequested;
    public event EventHandler? SoundBrowseRequested;

    public ScheduleEditViewModel()
    {
        _schedule = new Schedule();
        _isNew = true;

        SaveCommand = new RelayCommand(_ => SaveRequested?.Invoke(this, EventArgs.Empty), _ => CanSave());
        CancelCommand = new RelayCommand(_ => CancelRequested?.Invoke(this, EventArgs.Empty));
        BrowseSoundCommand = new RelayCommand(_ => SoundBrowseRequested?.Invoke(this, EventArgs.Empty));
        SetPriorityCommand = new RelayCommand<string>(p =>
        {
            if (int.TryParse(p, out var index))
                PriorityIndex = index;
        });
    }

    public void LoadSchedule(Schedule schedule, bool isNew)
    {
        _schedule = schedule;
        _isNew = isNew;

        RemindDate = schedule.RemindTime.Date;
        RemindTime = schedule.RemindTime.TimeOfDay;

        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(IsPopupReminder));
        OnPropertyChanged(nameof(IsSoundReminder));
        OnPropertyChanged(nameof(RemindText));
        OnPropertyChanged(nameof(RemindSoundPath));
        OnPropertyChanged(nameof(SelectedHour));
        OnPropertyChanged(nameof(SelectedMinute));
        OnPropertyChanged(nameof(PriorityIndex));
        OnPropertyChanged(nameof(IsPriorityLow));
        OnPropertyChanged(nameof(IsPriorityNormal));
        OnPropertyChanged(nameof(IsPriorityHigh));
        OnPropertyChanged(nameof(IsPriorityUrgent));
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(Title);
    }

    public void SetSoundPath(string path)
    {
        RemindSoundPath = path;
    }

    public Schedule GetSchedule()
    {
        return _schedule;
    }
}
