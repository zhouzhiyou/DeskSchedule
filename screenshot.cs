using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    static void Main(string[] args)
    {
        string outputPath = args.Length > 0 ? args[0] : "screenshot.png";

        // 获取屏幕尺寸
        int screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        // 创建位图
        using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            }

            // 计算窗口位置（右下角，380x580）
            int x = screenWidth - 400 - 20;
            int y = 20;
            int width = 400;
            int height = 580;

            // 裁剪
            using (Bitmap cropped = bitmap.Clone(new Rectangle(x, y, width, height), bitmap.PixelFormat))
            {
                cropped.Save(outputPath, ImageFormat.Png);
            }
        }

        Console.WriteLine($"Screenshot saved to: {outputPath}");
    }
}
