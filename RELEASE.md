# DeskSchedule 发布指南

## 发布方式

### 方式一：便携版（推荐用于分享）

便携版是最简单的发布方式，用户下载后解压即可运行，无需安装。

**生成方法：**
```bash
# 运行发布脚本
publish.bat

# 或手动执行
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

**输出文件：**
- `publish/DeskSchedule-v1.0.0/` - 发布文件夹
- `publish/DeskSchedule-v1.0.0-Portable.zip` - ZIP压缩包

**优点：**
- 用户无需安装，解压即用
- 可以放在U盘随身携带
- 不需要管理员权限

**缺点：**
- 文件较大（约60-80MB，包含.NET运行时）
- 不会创建开始菜单快捷方式

---

### 方式二：MSIX 打包（上架Windows应用商店）

MSIX是Windows推荐的打包格式，适合上架Microsoft Store。

**前置要求：**
1. 安装 Visual Studio Installer Projects 扩展
2. 准备应用签名证书
3. 注册 Microsoft 开发者账户（上架Store需要）

**创建MSIX步骤：**
1. 在Visual Studio中右键解决方案 → 添加 → 新建项目
2. 选择 "Windows Application Packaging Project"
3. 将 DeskSchedule 项目添加为引用
4. 配置包标识、版本、发布者信息
5. 构建生成 .msix 文件

---

### 方式三：Inno Setup 安装程序

适合分发独立的安装程序(.exe)。

**安装 Inno Setup：**
下载地址：https://jrsoftware.org/isdownload.php

**创建安装脚本 `setup.iss`：**
```ini
[Setup]
AppName=DeskSchedule
AppVersion=1.0.0
DefaultDirName={commonpf}\DeskSchedule
DefaultGroupName=DeskSchedule
OutputDir=publish
OutputBaseFilename=DeskSchedule-Setup
Compression=lzma2
SolidCompression=yes

[Files]
Source: "publish\DeskSchedule-v1.0.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{commondesktop}\DeskSchedule"; Filename: "{app}\DeskSchedule.exe"
Name: "{group}\DeskSchedule"; Filename: "{app}\DeskSchedule.exe"

[Run]
Filename: "{app}\DeskSchedule.exe"; Description: "启动程序"; Flags: postinstall nowait skipifsilent
```

**编译安装程序：**
```bash
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" setup.iss
```

---

## 发布前检查清单

- [ ] 更新版本号 (`DeskSchedule.csproj` 中的 `<Version>`)
- [ ] 更新版权年份
- [ ] 确认图标文件存在 (`Assets/tray_icon.ico`)
- [ ] 测试所有功能正常工作
- [ ] 测试提醒功能
- [ ] 测试自动隐藏和鼠标触发
- [ ] 检查数据库迁移兼容性

---

## 文件大小参考

| 发布方式 | 大小 |
|---------|------|
| 单文件自包含 (win-x64) | ~70MB |
| 单文件自包含 (win-x86) | ~65MB |
| 框架依赖 (需.NET运行时) | ~5MB |
| ZIP压缩后 | ~35MB |

---

## 上架平台指南

### Microsoft Store

1. 注册 [Windows 开发者中心](https://developer.microsoft.com/windows)
2. 支付开发者账户费用（$19一次性）
3. 使用 MSIX 打包
4. 提交应用并通过审核

### GitHub Releases

1. 在 GitHub 创建 Release
2. 上传 ZIP 或安装程序
3. 编写 Release Notes

### 其他分发平台

- **蓝奏云**：免费，不限速，适合个人开发者
- **腾讯软件管家**：提交审核
- **360软件管家**：提交审核

---

## 签名证书

如果需要代码签名以避免Windows SmartScreen警告：

1. 购买代码签名证书（约$100-300/年）
   - DigiCert
   - Sectigo
   - GlobalSign

2. 使用 SignTool 签名：
```bash
signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com DeskSchedule.exe
```

---

## 联系方式

如有问题，请提交 Issue 或联系开发者。
