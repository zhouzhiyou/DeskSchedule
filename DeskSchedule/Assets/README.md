# 图标文件说明

## 需要添加的图标文件

请在当前目录下添加 `tray_icon.ico` 文件，作为应用程序和系统托盘图标。

## 生成图标的方法

### 方法1：在线工具
访问 https://icoconvert.com/ 或 https://convertio.co/png-ico/
上传一张PNG图片，转换为ICO格式。

### 方法2：使用ImageMagick
```bash
convert icon.png -resize 256x256 -define icon:auto-resize=256,128,64,48,32,16 tray_icon.ico
```

### 方法3：使用Visual Studio
1. 在Visual Studio中打开"图像编辑器"
2. 创建新的图标文件
3. 绘制或粘贴图标内容

## 图标尺寸建议
- 16x16: 系统托盘小图标
- 32x32: 系统托盘大图标
- 48x48: 文件浏览器中等图标
- 256x256: 文件浏览器大图标

## 临时解决方案
如果暂时没有图标文件，可以注释掉 DeskSchedule.csproj 中的以下行：
```xml
<ApplicationIcon>Assets\tray_icon.ico</ApplicationIcon>
```

同时注释掉 MainWindow.xaml.cs 中的托盘图标初始化代码。
