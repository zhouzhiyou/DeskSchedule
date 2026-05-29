using DeskSchedule.Data;
using DeskSchedule.Models;
using DeskSchedule.Services;

namespace DeskSchedule.ViewModels;

/// <summary>
/// 设置ViewModel
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IAutoHideService _autoHideService;
    private readonly IMouseTriggerService _mouseTriggerService;
    private readonly AppSettings _settings;

    // 自动隐藏设置
    private int _hideDelaySeconds;
    public int HideDelaySeconds
    {
        get => _hideDelaySeconds;
        set => SetProperty(ref _hideDelaySeconds, value);
    }

    // 鼠标触发设置
    private int _triggerAreaSize;
    public int TriggerAreaSize
    {
        get => _triggerAreaSize;
        set => SetProperty(ref _triggerAreaSize, value);
    }

    private int _triggerDelayMs;
    public int TriggerDelayMs
    {
        get => _triggerDelayMs;
        set => SetProperty(ref _triggerDelayMs, value);
    }

    // 窗口设置
    private double _windowOpacity;
    public double WindowOpacity
    {
        get => _windowOpacity;
        set => SetProperty(ref _windowOpacity, value);
    }

    private bool _windowAlwaysOnTop;
    public bool WindowAlwaysOnTop
    {
        get => _windowAlwaysOnTop;
        set => SetProperty(ref _windowAlwaysOnTop, value);
    }

    // 提醒设置
    private bool _reminderPopupEnabled;
    public bool ReminderPopupEnabled
    {
        get => _reminderPopupEnabled;
        set => SetProperty(ref _reminderPopupEnabled, value);
    }

    private bool _reminderSoundEnabled;
    public bool ReminderSoundEnabled
    {
        get => _reminderSoundEnabled;
        set => SetProperty(ref _reminderSoundEnabled, value);
    }

    private int _reminderVolume;
    public int ReminderVolume
    {
        get => _reminderVolume;
        set => SetProperty(ref _reminderVolume, value);
    }

    private string _reminderSoundPath = string.Empty;
    public string ReminderSoundPath
    {
        get => _reminderSoundPath;
        set => SetProperty(ref _reminderSoundPath, value);
    }

    // 弹窗模板
    private string _popupTitleTemplate = string.Empty;
    public string PopupTitleTemplate
    {
        get => _popupTitleTemplate;
        set => SetProperty(ref _popupTitleTemplate, value);
    }

    private string _popupContentTemplate = string.Empty;
    public string PopupContentTemplate
    {
        get => _popupContentTemplate;
        set => SetProperty(ref _popupContentTemplate, value);
    }

    private int _popupDurationSeconds;
    public int PopupDurationSeconds
    {
        get => _popupDurationSeconds;
        set => SetProperty(ref _popupDurationSeconds, value);
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand ResetCommand { get; }
    public RelayCommand BrowseSoundCommand { get; }

    public event EventHandler? SettingsSaved;
    public event EventHandler<string>? SoundBrowseRequested;

    public SettingsViewModel(
        ISettingsRepository settingsRepository,
        IAutoHideService autoHideService,
        IMouseTriggerService mouseTriggerService,
        AppSettings settings)
    {
        _settingsRepository = settingsRepository;
        _autoHideService = autoHideService;
        _mouseTriggerService = mouseTriggerService;
        _settings = settings;

        // 加载当前设置
        LoadSettings();

        SaveCommand = new RelayCommand(_ => SaveSettings());
        ResetCommand = new RelayCommand(_ => ResetSettings());
        BrowseSoundCommand = new RelayCommand(_ => SoundBrowseRequested?.Invoke(this, ReminderSoundPath));
    }

    private void LoadSettings()
    {
        HideDelaySeconds = _settings.HideDelaySeconds;
        TriggerAreaSize = _settings.TriggerAreaSize;
        TriggerDelayMs = _settings.TriggerDelayMs;
        WindowOpacity = _settings.WindowOpacity;
        WindowAlwaysOnTop = _settings.WindowAlwaysOnTop;
        ReminderPopupEnabled = _settings.ReminderPopupEnabled;
        ReminderSoundEnabled = _settings.ReminderSoundEnabled;
        ReminderVolume = _settings.ReminderVolume;
        ReminderSoundPath = _settings.ReminderSoundPath;
        PopupTitleTemplate = _settings.PopupTitleTemplate;
        PopupContentTemplate = _settings.PopupContentTemplate;
        PopupDurationSeconds = _settings.PopupDurationSeconds;
    }

    private async void SaveSettings()
    {
        _settings.HideDelaySeconds = HideDelaySeconds;
        _settings.TriggerAreaSize = TriggerAreaSize;
        _settings.TriggerDelayMs = TriggerDelayMs;
        _settings.WindowOpacity = WindowOpacity;
        _settings.WindowAlwaysOnTop = WindowAlwaysOnTop;
        _settings.ReminderPopupEnabled = ReminderPopupEnabled;
        _settings.ReminderSoundEnabled = ReminderSoundEnabled;
        _settings.ReminderVolume = ReminderVolume;
        _settings.ReminderSoundPath = ReminderSoundPath;
        _settings.PopupTitleTemplate = PopupTitleTemplate;
        _settings.PopupContentTemplate = PopupContentTemplate;
        _settings.PopupDurationSeconds = PopupDurationSeconds;

        await _settingsRepository.SaveAsync(_settings);

        // 更新服务配置
        _autoHideService.SetHideDelay(HideDelaySeconds);
        _mouseTriggerService.SetTriggerAreaSize(TriggerAreaSize);
        _mouseTriggerService.SetTriggerDelay(TriggerDelayMs);

        SettingsSaved?.Invoke(this, EventArgs.Empty);
    }

    private void ResetSettings()
    {
        var defaultSettings = AppSettings.GetDefault();
        HideDelaySeconds = defaultSettings.HideDelaySeconds;
        TriggerAreaSize = defaultSettings.TriggerAreaSize;
        TriggerDelayMs = defaultSettings.TriggerDelayMs;
        WindowOpacity = defaultSettings.WindowOpacity;
        WindowAlwaysOnTop = defaultSettings.WindowAlwaysOnTop;
        ReminderPopupEnabled = defaultSettings.ReminderPopupEnabled;
        ReminderSoundEnabled = defaultSettings.ReminderSoundEnabled;
        ReminderVolume = defaultSettings.ReminderVolume;
        ReminderSoundPath = defaultSettings.ReminderSoundPath;
        PopupTitleTemplate = defaultSettings.PopupTitleTemplate;
        PopupContentTemplate = defaultSettings.PopupContentTemplate;
        PopupDurationSeconds = defaultSettings.PopupDurationSeconds;
    }

    public void SetSoundPath(string path)
    {
        ReminderSoundPath = path;
    }
}
