using System.Runtime.InteropServices;

namespace HanamikojiConsoleVersion;

public partial class Program
{
    const int MAXIMIZE = 3;

    static void Main(string[] args)
    {
        ShowWindow(ThisConsole, MAXIMIZE);
        HanamikojiGame.RunGame();
    }

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();
    private static IntPtr ThisConsole = GetConsoleWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
