using Avalonia;
using System;
using System.IO;
using System.Reflection;
using System.Text;
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
            // Register Windows-1251 and other non-Unicode encodings for CRCGame file I/O.
            // Required on Linux/.NET 8 where only ASCII/UTF-8 are available by default.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Log any unhandled exceptions to crash.log so Linux crashes aren't silent.
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                string msg = (e.ExceptionObject as Exception)?.ToString()
                             ?? e.ExceptionObject?.ToString()
                             ?? "Unknown error";
                Console.Error.WriteLine(msg);
                try { File.WriteAllText("crash.log", msg); } catch { }
            };

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
