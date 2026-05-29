using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DeskSchedule.Views;

/// <summary>
/// 首次使用引导窗口
/// </summary>
public partial class GuideView : Window
{
    private int _currentPage = 0;
    private readonly List<GuidePage> _pages;

    public bool DontShowAgain { get; private set; }

    public GuideView()
    {
        InitializeComponent();

        _pages = new List<GuidePage>
        {
            new GuidePage
            {
                Title = "欢迎使用 DeskSchedule",
                Subtitle = "轻量级桌面日程管理工具",
                Description = "帮您高效管理日程，自动隐藏不占空间，鼠标角落快速唤出。",
                Icon = "📅",
                IconColor = "#2196F3"
            },
            new GuidePage
            {
                Title = "添加日程",
                Subtitle = "快速创建日程",
                Description = "点击「+ 添加日程」按钮，输入标题、设置时间和优先级。\n支持弹窗提醒和声音提醒两种方式。",
                Icon = "➕",
                IconColor = "#4CAF50"
            },
            new GuidePage
            {
                Title = "优先级管理",
                Subtitle = "四级优先级设置",
                Description = "🟢 低  🔵 普通  🟠 高  🔴 紧急\n\n日程按优先级从高到低排列，重要事项一目了然。",
                Icon = "📊",
                IconColor = "#FF9800"
            },
            new GuidePage
            {
                Title = "自动隐藏",
                Subtitle = "不占用桌面空间",
                Description = "窗口会在空闲一段时间后自动隐藏到系统托盘。\n\n默认空闲 60 秒后隐藏，可在设置中调整时间。",
                Icon = "👁️",
                IconColor = "#9C27B0"
            },
            new GuidePage
            {
                Title = "鼠标触发",
                Subtitle = "快速唤出窗口",
                Description = "将鼠标移到屏幕角落即可唤出窗口：\n\n• 左上角：窗口从左侧滑出\n• 右上角：窗口从右侧滑出\n\n在设置中可调整触发延迟。",
                Icon = "🖱️",
                IconColor = "#00BCD4"
            },
            new GuidePage
            {
                Title = "提醒功能",
                Subtitle = "弹窗 + 声音提醒",
                Description = "日程时间到达时自动提醒：\n\n• 弹窗提醒：右下角显示通知窗口\n• 声音提醒：播放系统提示音\n\n可在设置中自定义提醒模板和声音。",
                Icon = "🔔",
                IconColor = "#F44336"
            },
            new GuidePage
            {
                Title = "开始使用",
                Subtitle = "准备就绪",
                Description = "您已了解所有基本功能！\n\n点击「开始使用」按钮立即体验。\n如需帮助，可随时通过设置按钮查看。",
                Icon = "🚀",
                IconColor = "#2196F3"
            }
        };

        ShowPage(0);
        UpdateNavigationButtons();
    }

    private void ShowPage(int index)
    {
        if (index < 0 || index >= _pages.Count) return;

        var page = _pages[index];
        _currentPage = index;

        // 更新内容
        IconText.Text = page.Icon;
        TitleText.Text = page.Title;
        SubtitleText.Text = page.Subtitle;
        DescriptionText.Text = page.Description;

        // 更新图标背景色
        var color = (Color)ColorConverter.ConvertFromString(page.IconColor);
        IconBorder.Background = new SolidColorBrush(color);

        // 更新进度指示器
        UpdateProgressIndicators();

        // 播放淡入动画
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
        ContentPanel.BeginAnimation(OpacityProperty, fadeIn);

        UpdateNavigationButtons();
    }

    private void UpdateProgressIndicators()
    {
        ProgressPanel.Children.Clear();
        for (int i = 0; i < _pages.Count; i++)
        {
            var dot = new Border
            {
                Width = i == _currentPage ? 20 : 8,
                Height = 8,
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(3, 0, 3, 0),
                Background = i == _currentPage
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"))
                    : new SolidColorBrush(Color.FromRgb(200, 200, 200))
            };
            ProgressPanel.Children.Add(dot);
        }
    }

    private void UpdateNavigationButtons()
    {
        PrevButton.Visibility = _currentPage > 0 ? Visibility.Visible : Visibility.Collapsed;

        if (_currentPage == _pages.Count - 1)
        {
            NextButton.Content = "开始使用";
            NextButton.Width = 120;
        }
        else
        {
            NextButton.Content = "下一步";
            NextButton.Width = 80;
        }
    }

    private void PrevButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage > 0)
        {
            ContentPanel.Opacity = 0;
            ShowPage(_currentPage - 1);
        }
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage < _pages.Count - 1)
        {
            ContentPanel.Opacity = 0;
            ShowPage(_currentPage + 1);
        }
        else
        {
            DontShowAgain = DontShowCheckBox.IsChecked ?? false;
            DialogResult = true;
            Close();
        }
    }

    private void SkipButton_Click(object sender, RoutedEventArgs e)
    {
        DontShowAgain = DontShowCheckBox.IsChecked ?? false;
        DialogResult = true;
        Close();
    }
}

/// <summary>
/// 引导页面数据
/// </summary>
public class GuidePage
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "";
    public string IconColor { get; set; } = "#2196F3";
}
