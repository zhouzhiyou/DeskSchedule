using System.Windows;
using DeskSchedule.ViewModels;
using Microsoft.Win32;

namespace DeskSchedule.Views;

/// <summary>
/// 设置窗口
/// </summary>
public partial class SettingsView : Window
{
    private readonly SettingsViewModel _viewModel;

    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        _viewModel.SettingsSaved += OnSettingsSaved;
        _viewModel.SoundBrowseRequested += OnSoundBrowseRequested;
    }

    private void OnSettingsSaved(object? sender, EventArgs e)
    {
        MessageBox.Show("设置已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OnSoundBrowseRequested(object? sender, string currentPath)
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择提示音文件",
            Filter = "音频文件|*.wav;*.mp3|所有文件|*.*",
            FileName = currentPath
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.SetSoundPath(dialog.FileName);
        }
    }
}
