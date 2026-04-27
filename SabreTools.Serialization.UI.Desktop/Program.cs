using System.Text;
using Avalonia;

namespace SabreTools.Serialization.UI.Desktop
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
