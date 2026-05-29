Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$screenWidth = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Width
$screenHeight = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Height

# 截取全屏
$bitmap = New-Object System.Drawing.Bitmap($screenWidth, $screenHeight)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.CopyFromScreen(0, 0, 0, 0, $bitmap.Size)

# 保存全屏截图
$fullPath = "C:\Users\tamli\DeskSchedule\publish\screenshot-full.png"
$bitmap.Save($fullPath, [System.Drawing.Imaging.ImageFormat]::Png)
Write-Host "Full screenshot saved"

# 裁剪窗口区域 - 主窗口
$x = $screenWidth - 420
$y = 20
$width = 400
$height = 600
$rect = New-Object System.Drawing.Rectangle($x, $y, $width, $height)
$cropped = $bitmap.Clone($rect, $bitmap.PixelFormat)
$croppedPath = "C:\Users\tamli\DeskSchedule\publish\screenshot-main.png"
$cropped.Save($croppedPath, [System.Drawing.Imaging.ImageFormat]::Png)
Write-Host "Main window screenshot saved"

$graphics.Dispose()
$bitmap.Dispose()
$cropped.Dispose()
