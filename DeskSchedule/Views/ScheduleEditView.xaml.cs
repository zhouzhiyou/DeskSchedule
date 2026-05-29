using System.Windows;
using DeskSchedule.Models;
using DeskSchedule.ViewModels;
using Microsoft.Win32;

namespace DeskSchedule.Views;

/// <summary>
/// 日程编辑窗口
/// </summary>
public partial class ScheduleEditView : Window
{
    private readonly ScheduleEditViewModel _viewModel;

    public event EventHandler<Schedule>? ScheduleSaved;

    public ScheduleEditView(ScheduleEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        _viewModel.SaveRequested += OnSaveRequested;
        _viewModel.CancelRequested += OnCancelRequested;
        _viewModel.SoundBrowseRequested += OnSoundBrowseRequested;

        // 生成时间选项
        GenerateTimeOptions();
    }

    private void GenerateTimeOptions()
    {
        var times = new List<TimeSpan>();
        for (int h = 0; h < 24; h++)
        {
            times.Add(new TimeSpan(h, 0, 0));
            times.Add(new TimeSpan(h, 30, 0));
        }

        var comboBox = FindChild<System.Windows.Controls.ComboBox>(this, "");
        if (comboBox != null)
        {
            comboBox.ItemsSource = times.Select(t => t.ToString(@"hh\:mm"));
        }
    }

    private static T? FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
    {
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild)
            {
                return typedChild;
            }
            var result = FindChild<T>(child, childName);
            if (result != null) return result;
        }
        return null;
    }

    public void SetSchedule(Schedule schedule, bool isNew)
    {
        _viewModel.LoadSchedule(schedule, isNew);
        Title = isNew ? "添加日程" : "编辑日程";
    }

    private void OnSaveRequested(object? sender, EventArgs e)
    {
        var schedule = _viewModel.GetSchedule();
        if (string.IsNullOrWhiteSpace(schedule.Title))
        {
            MessageBox.Show("请输入日程标题", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ScheduleSaved?.Invoke(this, schedule);
        Close();
    }

    private void OnCancelRequested(object? sender, EventArgs e)
    {
        Close();
    }

    private void OnSoundBrowseRequested(object? sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择提示音文件",
            Filter = "音频文件|*.wav;*.mp3|所有文件|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.SetSoundPath(dialog.FileName);
        }
    }
}
