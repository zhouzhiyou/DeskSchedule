using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DeskSchedule.Models;
using DeskSchedule.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace DeskSchedule;

/// <summary>
/// 主窗口
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private TaskbarIcon? _taskbarIcon;
    private Views.SettingsView? _settingsView;
    private Views.ScheduleEditView? _editView;
    private CornerType _lastTriggerCorner = CornerType.Right;
    private bool _isFirstShow = true;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        // 初始化系统托盘
        InitializeTrayIcon();

        // 订阅事件
        _viewModel.OpenSettingsRequested += OnOpenSettingsRequested;
        _viewModel.EditScheduleRequested += OnEditScheduleRequested;

        // 监听窗口可见性变化
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        // 监听触发角落变化
        _viewModel.TriggerCornerChanged += OnTriggerCornerChanged;

        // 加载窗口位置
        LoadWindowPosition();
    }

    private void OnTriggerCornerChanged(object? sender, CornerType corner)
    {
        _lastTriggerCorner = corner;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.IsWindowVisible))
        {
            if (_viewModel.IsWindowVisible)
            {
                Show();
                Activate();
                if (!_isFirstShow)
                {
                    PlaySlideInAnimation();
                }
                _isFirstShow = false;
            }
            else
            {
                Hide();
            }
        }
    }

    private void PlaySlideInAnimation()
    {
        try
        {
            // 根据触发角落设置窗口位置并播放动画
            var workArea = SystemParameters.WorkArea;

            if (_lastTriggerCorner == CornerType.Right)
            {
                Left = workArea.Right - Width - 20;
                Top = workArea.Top + 20;
            }
            else
            {
                Left = workArea.Left + 20;
                Top = workArea.Top + 20;
            }

            // 创建动画
            var transform = BorderTransform;
            double startX = _lastTriggerCorner == CornerType.Right ? 400 : -400;

            var animation = new DoubleAnimation
            {
                From = startX,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }
        catch
        {
            // 动画失败时忽略
        }
    }

    private void InitializeTrayIcon()
    {
        try
        {
            _taskbarIcon = new TaskbarIcon
            {
                ToolTipText = "日程管理 - 双击显示"
            };

            // 尝试加载图标，如果失败则使用默认
            try
            {
                _taskbarIcon.IconSource = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/Hardcodet.Wpf.TaskbarNotification;component/Icons/Idle.ico"));
            }
            catch
            {
                // 使用内置图标或忽略
            }

            _taskbarIcon.TrayMouseDoubleClick += (s, e) => _viewModel.ShowWindow();
            _taskbarIcon.ContextMenu = new System.Windows.Controls.ContextMenu();

            var showItem = new System.Windows.Controls.MenuItem { Header = "显示窗口" };
            showItem.Click += (s, e) => _viewModel.ShowWindow();

            var exitItem = new System.Windows.Controls.MenuItem { Header = "退出" };
            exitItem.Click += (s, e) => Application.Current.Shutdown();

            _taskbarIcon.ContextMenu.Items.Add(showItem);
            _taskbarIcon.ContextMenu.Items.Add(new System.Windows.Controls.Separator());
            _taskbarIcon.ContextMenu.Items.Add(exitItem);
        }
        catch (Exception)
        {
            // 托盘图标初始化失败，忽略
        }
    }

    private void LoadWindowPosition()
    {
        var workArea = SystemParameters.WorkArea;
        Left = workArea.Right - Width - 20;
        Top = workArea.Top + 20;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
            _viewModel.ResetActivity();
        }
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.HideWindow();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void ScheduleItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _viewModel.ResetActivity();
    }

    private void OnOpenSettingsRequested(object? sender, EventArgs e)
    {
        if (_settingsView == null || !_settingsView.IsLoaded)
        {
            _settingsView = App.Current.Services.GetRequiredService<Views.SettingsView>();
            _settingsView.Owner = this;
            _settingsView.ShowDialog();
        }
        else
        {
            _settingsView.Activate();
        }
    }

    private void OnEditScheduleRequested(object? sender, Schedule schedule)
    {
        if (_editView == null || !_editView.IsLoaded)
        {
            _editView = App.Current.Services.GetRequiredService<Views.ScheduleEditView>();
            _editView.Owner = this;
            _editView.SetSchedule(schedule, _viewModel.IsAddingNew);
            _editView.ScheduleSaved += async (s, savedSchedule) =>
            {
                await _viewModel.SaveScheduleAsync(savedSchedule);
            };
            _editView.ShowDialog();
        }
        else
        {
            _editView.Activate();
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _taskbarIcon?.Dispose();
        base.OnClosed(e);
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        _viewModel.ResetActivity();
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseDown(e);
        _viewModel.ResetActivity();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
        _viewModel.ResetActivity();
    }
}
