using Avalonia;
using System;
using System.Reflection;
using System.Threading;

namespace Chernobyl_Relay_Chat
{
    static class Program
    {
        public const float FontSize = 10.25f;

        public static string Version =>
            Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";

        // Prevent multiple instances
        private static readonly Mutex mutex = new Mutex(false, "64eb5dda-2131-47fc-a32c-fbc64f440d8a");

        [STAThread]
        public static void Main(string[] args)
        {
#if !DEBUG
            if (!mutex.WaitOne(TimeSpan.FromSeconds(0), false))
                return;
#endif
            CRCStrings.Load();
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            CRCOptions.Save();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
