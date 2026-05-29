Add-Type -AssemblyName System.Drawing

$inputPath = $args[0]
$outputPath = $args[1]
$x = [int]$args[2]
$y = [int]$args[3]
$width = [int]$args[4]
$height = [int]$args[5]

$source = [System.Drawing.Image]::FromFile($inputPath)
$bitmap = New-Object System.Drawing.Bitmap($width, $height)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.DrawImage($source, New-Object System.Drawing.Rectangle(0, 0, $width, $height), New-Object System.Drawing.Rectangle($x, $y, $width, $height), [System.Drawing.GraphicsUnit]::Pixel)

$bitmap.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose()
$bitmap.Dispose()
$source.Dispose()

Write-Host "Cropped image saved to: $outputPath"
