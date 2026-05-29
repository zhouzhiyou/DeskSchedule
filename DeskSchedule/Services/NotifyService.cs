using System.Windows;
using System.Windows.Media;

namespace DeskSchedule.Services;

/// <summary>
/// 通知服务实现
/// </summary>
public class NotifyService : INotifyService
{
    public Task ShowNotificationAsync(string title, string message, int durationSeconds = 10)
    {
        return Application.Current.Dispatcher.InvokeAsync(() =>
        {
            try
            {
                var notification = new NotificationWindow(title, message, durationSeconds);
                notification.Show();
                notification.Activate();
            }
            catch (Exception)
            {
                // 忽略弹窗错误
            }
        }).Task;
    }

    public void CloseAllNotifications()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                var windowsToClose = new List<Window>();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is NotificationWindow notification)
                    {
                        windowsToClose.Add(notification);
                    }
                }
                foreach (var window in windowsToClose)
                {
                    window.Close();
                }
            }
            catch (Exception)
            {
                // 忽略关闭错误
            }
        });
    }
}

/// <summary>
/// 通知窗口
/// </summary>
public class NotificationWindow : Window
{
    public NotificationWindow(string title, string message, int durationSeconds)
    {
        // 设置窗口属性
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        Topmost = true;
        ShowInTaskbar = false;
        ShowActivated = true;
        Width = 320;
        Height = 90;

        // 定位到右下角
        var workArea = SystemParameters.WorkArea;
        Left = workArea.Right - Width - 20;
        Top = workArea.Bottom - Height - 20;

        // 创建内容
        var border = new System.Windows.Controls.Border
        {
            Background = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(15, 10, 15, 10)
        };

        // 添加阴影效果
        border.Effect = new System.Windows.Media.Effects.DropShadowEffect
        {
            Color = Colors.Black,
            Opacity = 0.3,
            BlurRadius = 15,
            ShadowDepth = 3
        };

        var grid = new System.Windows.Controls.Grid();
        grid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new GridLength(40) });
        grid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        // 图标
        var iconBlock = new System.Windows.Controls.TextBlock
        {
            Text = "🔔",
            FontSize = 24,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        System.Windows.Controls.Grid.SetColumn(iconBlock, 0);

        var stackPanel = new System.Windows.Controls.StackPanel();
        System.Windows.Controls.Grid.SetColumn(stackPanel, 1);

        var titleBlock = new System.Windows.Controls.TextBlock
        {
            Text = title,
            FontWeight = FontWeights.Bold,
            FontSize = 14,
            Foreground = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
            Margin = new Thickness(0, 0, 0, 5)
        };

        var messageBlock = new System.Windows.Controls.TextBlock
        {
            Text = message,
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51)),
            TextWrapping = TextWrapping.Wrap,
            MaxHeight = 40
        };

        stackPanel.Children.Add(titleBlock);
        stackPanel.Children.Add(messageBlock);

        grid.Children.Add(iconBlock);
        grid.Children.Add(stackPanel);

        border.Child = grid;
        Content = border;

        // 自动关闭
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(durationSeconds)
        };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            Close();
        };
        timer.Start();

        // 点击关闭
        MouseDown += (s, e) => Close();
    }
}
