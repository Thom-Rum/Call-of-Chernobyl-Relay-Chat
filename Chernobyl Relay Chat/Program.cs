using Avalonia;
using System;
using System.Diagnostics;
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
#if DEBUG
            Console.WriteLine("[CRC] Main() entered");
            Console.Out.Flush();
#endif
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
                var ex = e.ExceptionObject as Exception;
                string msg = ex?.ToString() ?? e.ExceptionObject?.ToString() ?? "Unknown error";
                App.LogCrash(ex ?? new Exception(msg));
            };

#if DEBUG
            // Make Avalonia's LogToTrace() output visible in the terminal.
            Trace.Listeners.Add(new ConsoleTraceListener());
#endif

            CRCStrings.Load();
#if DEBUG
            Console.WriteLine("[CRC] CRCStrings.Load() complete, starting Avalonia...");
            Console.Out.Flush();
#endif
            int avaloniaExitCode = BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
#if DEBUG
            Console.WriteLine($"[CRC] Avalonia lifetime ended (exit code {avaloniaExitCode}), saving options...");
            Console.Out.Flush();
#endif
            CRCOptions.Save();
            if (avaloniaExitCode != 0)
                Environment.Exit(avaloniaExitCode);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
