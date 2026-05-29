Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$screenWidth = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Width
$screenHeight = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Height

$bitmap = New-Object System.Drawing.Bitmap($screenWidth, $screenHeight)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.CopyFromScreen(0, 0, 0, 0, $bitmap.Size)

$x = $screenWidth - 400 - 20
$y = 20
$width = 400
$height = 580

$rect = New-Object System.Drawing.Rectangle($x, $y, $width, $height)
$cropped = $bitmap.Clone($rect, $bitmap.PixelFormat)

$outputPath = "C:\Users\tamli\DeskSchedule\publish\screenshot-main.png"
$cropped.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)

$graphics.Dispose()
$bitmap.Dispose()
$cropped.Dispose()

Write-Host "Screenshot saved to: $outputPath"
