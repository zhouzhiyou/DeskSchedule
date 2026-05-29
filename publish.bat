@echo off
chcp 65001 >nul
echo ========================================
echo   DeskSchedule 发布脚本
echo ========================================
echo.

set PROJECT_DIR=DeskSchedule
set OUTPUT_DIR=publish
set VERSION=1.0.0

:: 清理旧的发布文件
if exist %OUTPUT_DIR% rd /s /q %OUTPUT_DIR%
mkdir %OUTPUT_DIR%

echo [1/3] 正在编译项目...
dotnet publish %PROJECT_DIR%\DeskSchedule.csproj ^
    --configuration Release ^
    --runtime win-x64 ^
    --self-contained true ^
    --output %OUTPUT_DIR%\DeskSchedule-v%VERSION% ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true ^
    /p:Version=%VERSION%

if %ERRORLEVEL% neq 0 (
    echo 编译失败！
    pause
    exit /b 1
)

echo.
echo [2/3] 创建便携版ZIP压缩包...
cd %OUTPUT_DIR%
powershell -Command "Compress-Archive -Path 'DeskSchedule-v%VERSION%\*' -DestinationPath 'DeskSchedule-v%VERSION%-Portable.zip' -Force"

echo.
echo [3/3] 创建发布说明...
echo DeskSchedule v%VERSION% > README.txt
echo =============================== >> README.txt
echo. >> README.txt
echo 系统要求： >> README.txt
echo - Windows 10 (1809+) / Windows 11 >> README.txt
echo - 64位系统 >> README.txt
echo. >> README.txt
echo 安装方法： >> README.txt
echo 1. 解压 ZIP 文件到任意目录 >> README.txt
echo 2. 运行 DeskSchedule.exe >> README.txt
echo. >> README.txt
echo 功能特性： >> README.txt
echo - 自动隐藏：空闲后自动隐藏到系统托盘 >> README.txt
echo - 鼠标触发：移到屏幕角落自动显示 >> README.txt
echo - 日程管理：添加、编辑、删除日程 >> README.txt
echo - 提醒功能：弹窗和声音提醒 >> README.txt
echo - 优先级：支持4级优先级设置 >> README.txt
echo - 透明度：可调节窗口透明度 >> README.txt

echo.
echo ========================================
echo   发布完成！
echo ========================================
echo.
echo 输出目录: %OUTPUT_DIR%
echo - DeskSchedule-v%VERSION%\        (发布文件夹)
echo - DeskSchedule-v%VERSION%-Portable.zip (便携版)
echo.
pause
