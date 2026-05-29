# 📅 DeskSchedule

一款轻量级的 Windows 桌面日程管理工具，支持自动隐藏、鼠标触发、优先级管理、智能提醒等功能。

## ✨ 功能特性

### 核心功能
- 🗓️ **日程管理** - 添加、编辑、删除日程，支持标题、描述、提醒时间设置
- 📊 **优先级管理** - 四级优先级（低/普通/高/紧急），按优先级排序显示
- 📅 **日期分组** - 按日期分组显示日程，支持今天、明天等智能分组
- 🏷️ **分类筛选** - 全部/待完成/已完成三种视图切换

### 便捷操作
- 👁️ **自动隐藏** - 空闲后自动隐藏到系统托盘，不占用桌面空间
- 🖱️ **鼠标触发** - 移动鼠标到屏幕角落即可唤出窗口
- 🎨 **透明度调节** - 窗口透明度可调节 10%-100%
- 🎯 **首次引导** - 首次使用时显示功能引导，快速上手

### 提醒功能
- 🔔 **弹窗提醒** - 右下角显示通知窗口，支持自定义模板
- 🔊 **声音提醒** - 播放系统提示音或自定义声音
- ⏰ **精确提醒** - 时间精确到分钟，准时提醒

## 💻 系统要求

- **操作系统**: Windows 10 (1809+) / Windows 11
- **架构**: 64位 (x64)
- **运行时**: 无需安装 .NET（已内置）

## 📥 下载安装

### 方式一：便携版（推荐）
1. 下载最新版本 [Releases](https://github.com/zhouzhiyou/DeskSchedule/release)
2. 解压到任意目录
3. 双击运行 `DeskSchedule.exe`

### 方式二：从源码编译
```bash
# 克隆仓库
git clone https://github.com/zhouzhiyou/DeskSchedule.git
cd deskschedule

# 编译运行
dotnet restore
dotnet run --project DeskSchedule
```

## 🚀 快速开始

1. **添加日程**: 点击「+ 添加日程」按钮
2. **设置时间**: 选择日期和具体时间（精确到分钟）
3. **选择优先级**: 低/普通/高/紧急
4. **开启提醒**: 勾选弹窗提醒或声音提醒
5. **保存**: 点击保存，等待提醒

## ⚙️ 配置说明

### 自动隐藏
- 默认空闲 60 秒后自动隐藏
- 可在设置中调整隐藏延迟时间

### 鼠标触发
- 左上角：窗口从左侧滑出
- 右上角：窗口从右侧滑出
- 可调整触发延迟和区域大小

### 提醒设置
- 弹窗标题模板：支持 `{title}`、`{time}`、`{date}` 变量
- 弹窗内容模板：同上
- 弹窗持续时间：默认 10 秒

## 🛠️ 技术栈

- **框架**: .NET 8.0 + WPF
- **数据库**: SQLite + Entity Framework Core
- **架构**: MVVM 模式
- **依赖注入**: Microsoft.Extensions.DependencyInjection
- **系统托盘**: Hardcodet.NotifyIcon.Wpf
- **音频播放**: NAudio

## 📁 项目结构

```
DeskSchedule/
├── DeskSchedule/              # 主项目
│   ├── Models/               # 数据模型
│   ├── ViewModels/           # 视图模型
│   ├── Views/                # 视图窗口
│   ├── Services/             # 业务服务
│   ├── Data/                 # 数据访问
│   └── Resources/            # 资源文件
├── DeskSchedule.Tests/       # 单元测试
└── docs/                     # 文档和截图
```

## 🗓️ 更新日志

### v1.1.0 (2025-05-28)
- ✨ 新增首次使用引导功能
- ✨ 新增日程优先级管理
- ✨ 新增日程分类筛选
- ✨ 新增按日期分组显示
- ✨ 已完成日程添加删除线样式
- 🔧 优化提醒弹窗样式
- 🔧 修复复选框交互问题

### v1.0.0 (2025-05-27)
- 🎉 首次发布
- ✅ 基础日程管理功能
- ✅ 自动隐藏和鼠标触发
- ✅ 弹窗和声音提醒

## 🤝 贡献指南

欢迎提交 Issue 和 Pull Request！

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 提交 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

## 📧 联系方式

- 问题反馈: [GitHub Issues](https://github.com/zhouzhiyou/DeskSchedule/issues)

---

⭐ 如果这个项目对你有帮助，请给一个 Star！
