Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Input

function Take-Screenshot {
    param([string]$OutputPath, [int]$X, [int]$Y, [int]$Width, [int]$Height)

    $screenWidth = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Width
    $screenHeight = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Height

    $bitmap = New-Object System.Drawing.Bitmap($screenWidth, $screenHeight)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.CopyFromScreen(0, 0, 0, 0, $bitmap.Size)

    if ($Width -gt 0 -and $Height -gt 0) {
        $rect = New-Object System.Drawing.Rectangle($X, $Y, $Width, $Height)
        $cropped = $bitmap.Clone($rect, $bitmap.PixelFormat)
        $cropped.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $cropped.Dispose()
    } else {
        $bitmap.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }

    $graphics.Dispose()
    $bitmap.Dispose()
    Write-Host "Screenshot saved to: $OutputPath"
}

function Wait-Seconds {
    param([int]$Seconds)
    Start-Sleep -Seconds $Seconds
}

function Send-Click {
    param([int]$X, [int]$Y)
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($X, $Y)
    Start-Sleep -Milliseconds 100
    # 模拟鼠标点击
    Add-Type -MemberDefinition '[DllImport("user32.dll")] public static extern void mouse_event(int flags, int dx, int dy, int cButtons, int info);' -Name U32 -Namespace W
    [W.U32]::mouse_event(0x02, 0, 0, 0, 0) # 左键按下
    [W.U32]::mouse_event(0x04, 0, 0, 0, 0) # 左键释放
    Start-Sleep -Milliseconds 500
}

# 获取屏幕分辨率
$screenWidth = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Width
$screenHeight = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Height

Write-Host "Screen resolution: ${screenWidth}x${screenHeight}"
Write-Host ""
Write-Host "请按照以下步骤手动截图："
Write-Host ""
Write-Host "1. 主界面截图："
Write-Host "   - 确保 DeskSchedule 主窗口可见"
Write-Host "   - 按 Win+Shift+S 打开截图工具"
Write-Host "   - 选择窗口区域截图"
Write-Host "   - 保存为 screenshot-main.png"
Write-Host ""
Write-Host "2. 添加日程截图："
Write-Host "   - 点击 '+ 添加日程' 按钮"
Write-Host "   - 使用 Win+Shift+S 截图"
Write-Host "   - 保存为 screenshot-add.png"
Write-Host ""
Write-Host "3. 设置界面截图："
Write-Host "   - 点击设置按钮（齿轮图标）"
Write-Host "   - 使用 Win+Shift+S 截图"
Write-Host "   - 保存为 screenshot-settings.png"
Write-Host ""
Write-Host "所有截图保存到: C:\Users\tamli\DeskSchedule\publish\"
