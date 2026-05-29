using System.Windows;
using DeskSchedule.Data;
using DeskSchedule.Models;
using DeskSchedule.Services;
using DeskSchedule.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DeskSchedule;

/// <summary>
/// 应用入口
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    private AppSettings? _settings;
    private ISettingsRepository? _settingsRepository;

    /// <summary>
    /// 服务提供者，用于获取依赖注入的服务
    /// </summary>
    public static new App Current => (App)Application.Current;

    public IServiceProvider Services => _serviceProvider ?? throw new InvalidOperationException("Services not initialized");

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();
        await ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // 初始化数据库
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
            dbContext.Database.EnsureCreated();
        }

        // 启动提醒服务
        var reminderService = _serviceProvider.GetRequiredService<IReminderService>();
        reminderService.Start();

        // 检查是否首次运行，显示引导窗口
        if (_settings!.IsFirstRun)
        {
            var guideView = new Views.GuideView();
            if (guideView.ShowDialog() == true)
            {
                // 如果用户勾选"不再显示"，更新设置
                if (guideView.DontShowAgain || true) // 首次运行后标记为非首次
                {
                    _settings.IsFirstRun = false;
                    await SaveSettingsAsync();
                }
            }
        }

        // 显示主窗口
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private async Task ConfigureServices(IServiceCollection services)
    {
        // 加载配置
        _settingsRepository = new SettingsRepository();
        _settings = await _settingsRepository.LoadAsync();
        services.AddSingleton(_settings);
        services.AddSingleton<Data.ISettingsRepository>(_settingsRepository);

        // 数据库
        services.AddSingleton<Data.AppDbContext>();

        // 仓储
        services.AddSingleton<Data.IScheduleRepository, Data.ScheduleRepository>();

        // 服务
        services.AddSingleton<IScheduleService, ScheduleService>();
        services.AddSingleton<IAutoHideService, AutoHideService>();
        services.AddSingleton<IMouseTriggerService, MouseTriggerService>();
        services.AddSingleton<INotifyService, NotifyService>();
        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<IReminderService, ReminderService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ScheduleEditViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
        services.AddTransient<Views.SettingsView>();
        services.AddTransient<Views.ScheduleEditView>();
    }

    private async Task SaveSettingsAsync()
    {
        if (_settingsRepository != null && _settings != null)
        {
            await _settingsRepository.SaveAsync(_settings);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
